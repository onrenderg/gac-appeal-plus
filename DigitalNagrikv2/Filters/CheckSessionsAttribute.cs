using DigitalNagrik.Controllers;
using DigitalNagrik.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalNagrik.Filters
{
    public class CheckSessionsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
                filterContext.HttpContext.Response.Cache.SetNoStore();
                var expectedHost = ConfigurationManager.AppSettings["HostName"];
                var headers = filterContext.HttpContext.Request.Headers;
                //if (!String.IsNullOrEmpty(headers["Referer"]) && new Uri(headers["Referer"]).Host == expectedHost)
                if (!string.IsNullOrEmpty(headers["host"]) && headers["host"].ToString() == expectedHost)
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    GoToLoginPage(filterContext);
                }
               
                if (string.IsNullOrWhiteSpace(UserSession.UserID))
                {
                    ClearAllSessions();
                    string requestAcceptType = filterContext.HttpContext.Request.AcceptTypes[0];
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                        ReturnAjaxMessage(filterContext, requestAcceptType);
                    else
                        GoToLoginPage(filterContext);
                }
                else
                {
                    if (UserSession.isSSOLogin == "Y" && !ValidateSSOToken(filterContext))
                    {
                        ClearAllSessions();
                        string requestAcceptType = filterContext.HttpContext.Request.AcceptTypes[0];
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                            ReturnAjaxMessage(filterContext, requestAcceptType);
                        else
                            GoToLoginPage(filterContext);
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.HandleException(ex); GoToLoginPage(filterContext); }
        }

        public void ReturnAjaxMessage(ActionExecutingContext filterContext, string acceptType)
        {
            switch (acceptType)
            {
                case "application/json":
                    break;
                case "text/html":
                default:
                    PartialViewResult result = new PartialViewResult { ViewName = "~/Views/Shared/SessionExpire.cshtml" };
                    filterContext.Result = result;
                    break;
            }
        }

        public static void ClearAllSessions()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.User = null;
            System.Web.Security.FormsAuthentication.SignOut();
        }

        public static void GoToLoginPage(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" }, { "Area", "" } });
            }
            catch { }
        }

        public bool ValidateSSOToken(ActionExecutingContext filterContext)
        {
            string qs = string.Empty;
            HttpWebRequest myWebRequest;
            try
            {
                if (UserSession.isSSOLogin.Equals("Y"))
                {
                    qs = "userName=" + UserSession.UserName + "&service=GACePortal" + "&sessionId=" + UserSession.sessionId + "&browserId=" + UserSession.browserId + "&localTokenId=" + UserSession.localTokenId;
                    myWebRequest = (HttpWebRequest)System.Net.WebRequest.Create("http://" + ConfigurationManager.AppSettings["VMIP"].ToString() + ":" + ConfigurationManager.AppSettings["SSOAPIPort"].ToString() + "/isTokenValid?" + qs);
                    myWebRequest.KeepAlive = false;
                    myWebRequest.Method = "GET";
                    myWebRequest.ProtocolVersion = HttpVersion.Version10;
                    myWebRequest.AllowAutoRedirect = true;
                    myWebRequest.MaximumAutomaticRedirections = 10;
                    WebResponse response1 = myWebRequest.GetResponse();
                    string str = string.Empty;
                    using (var streamReader = new StreamReader(response1.GetResponseStream(), Encoding.UTF8))
                    {
                        int usrSleepTime = Int32.Parse("60");
                        if (usrSleepTime >= 60 && usrSleepTime <= 120)
                        {
                            Thread.Sleep(usrSleepTime);
                        }
                        else
                        {
                            throw new Exception("Invalid sleep duration");
                        }
                        int intC;
                        StringBuilder sb = new StringBuilder();
                        while ((intC = streamReader.Read()) != -1)
                        {
                            char c = (char)intC;
                            if (c == '\n')
                            {
                                break;
                            }
                            if (sb.Length >= 4000)
                            {
                                throw new Exception("input too long");
                            }
                            sb.Append(c);
                            str = sb.ToString();
                        }                      
                    }
                    response1.Close();
                    response1.Close();
                    if (str.ToLower().Contains("success"))
                        return true;
                    else return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return true;
            }
        }
    }
}