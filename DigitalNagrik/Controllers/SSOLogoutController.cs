using NICServiceAdaptor;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using DigitalNagrik.Models;
using System.Web.Mvc;
using System;
using System.Text;

namespace DigitalNagrik.Controllers
{
    public class SSOLogoutController : Controller
    {
        // GET: SSOLogout
        public ActionResult Index()
        {
            try
            {
                if (UserSession.isSSOLogin.ToString().Equals("N"))
                {
                    if (Session["LogoutID"] != null)
                    {
                        UpdateLogoutTime();

                    }
                }
                else
                {
                    var qs = "userName=" + UserSession.UserName.ToString() + "&service=" + UserSession.service.ToString() + "&sessionId=" + UserSession.sessionId.ToString() + "&browserId=" + UserSession.browserId.ToString() + "&localTokenId=" + UserSession.localTokenId.ToString();
                    if (Session["LogoutID"] != null)
                    {
                        UpdateLogoutTime();
                    }
                    HttpWebRequest myWebRequest = (HttpWebRequest)System.Net.WebRequest.Create("http://" + ConfigurationManager.AppSettings["VMIP"].ToString() + ":" + ConfigurationManager.AppSettings["SSOAPIPort"].ToString() + "/logoutAll?" + qs);
                    myWebRequest.KeepAlive = false;
                    myWebRequest.Method = "GET";
                    myWebRequest.ProtocolVersion = HttpVersion.Version10;
                    myWebRequest.AllowAutoRedirect = true;
                    myWebRequest.MaximumAutomaticRedirections = 10;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    WebResponse response1 = myWebRequest.GetResponse();
                    string str = string.Empty;
                    using (StreamReader sr = new StreamReader(response1.GetResponseStream()))
                    {
                        int intC;
                        while ((intC = sr.Read()) != -1)
                        {
                            char c = (char)intC;
                            StringBuilder sb = new StringBuilder();
                            if (c == '\n')
                            {
                                break;
                            }
                            if (sb.Length >= 5000)
                            {
                                throw new Exception("input too long");
                            }
                            sb.Append(c);
                            str = sb.ToString();
                        }

                    }
                    response1.Close();
                    if (str.ToLower().Contains("success"))
                    {
                        return RedirectToAction("index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("index", "Home");
                    }

                }
                Session.Abandon();
                Session.Clear();
                return RedirectToAction("index", "Home");
                //var qs = String.Empty;

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                Session.Abandon();
                Session.Clear();
                return RedirectToAction("index", "Home");
            }
        }
        public ActionResult IntermediaryLogout()
        {
            try
            {

                if (Session["LogoutID"] != null)
                {
                    UpdateLogoutTime();
                }
                Session.Abandon();
                Session.Clear();
                return RedirectToAction("Intermediary", "Home");
                //var qs = String.Empty;

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                Session.Abandon();
                Session.Clear();
                return RedirectToAction("Intermediary", "Home");
            }
        }
        public ActionResult GACLogout()
        {
            try
            {
                if (UserSession.isSSOLogin.ToString().Equals("N"))
                {
                    if (Session["LogoutID"] != null)
                    {
                        UpdateLogoutTime();

                    }
                }
                else
                {
                    var qs = "userName=" + UserSession.UserName.ToString() + "&service=" + UserSession.service.ToString() + "&sessionId=" + UserSession.sessionId.ToString() + "&browserId=" + UserSession.browserId.ToString() + "&localTokenId=" + UserSession.localTokenId.ToString();
                    if (Session["LogoutID"] != null)
                    {
                        UpdateLogoutTime();
                    }
                    HttpWebRequest myWebRequest = (HttpWebRequest)System.Net.WebRequest.Create("http://" + ConfigurationManager.AppSettings["VMIP"].ToString() + ":" + ConfigurationManager.AppSettings["SSOAPIPort"].ToString() + "/logoutAll?" + qs);
                    myWebRequest.KeepAlive = false;
                    myWebRequest.Method = "GET";
                    myWebRequest.ProtocolVersion = HttpVersion.Version10;
                    myWebRequest.AllowAutoRedirect = true;
                    myWebRequest.MaximumAutomaticRedirections = 10;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    WebResponse response1 = myWebRequest.GetResponse();
                    string str = string.Empty;
                    using (StreamReader sr = new StreamReader(response1.GetResponseStream()))
                    {
                        int intC;
                        while ((intC = sr.Read()) != -1)
                        {
                            char c = (char)intC;
                            StringBuilder sb = new StringBuilder();
                            if (c == '\n')
                            {
                                break;
                            }
                            if (sb.Length >= 5000)
                            {
                                throw new Exception("input too long");
                            }
                            sb.Append(c);
                            str = sb.ToString();
                        }

                    }
                    response1.Close();
                    if (str.ToLower().Contains("success"))
                    {
                        return RedirectToAction("GAC", "Home");
                    }

                    else
                    {
                        return RedirectToAction("GAC", "Home");
                    }

                }
                Session.Abandon();
                Session.Clear();
                return RedirectToAction("GAC", "Home");
                //var qs = String.Empty;

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                Session.Abandon();
                Session.Clear();
                return RedirectToAction("GAC", "Home");
            }
        }
        public string LogoutOTP()
        {
            if (Session["LogoutID"] != null)
            {
                UpdateLogoutTime();
            }
            return "";
        }
        private string UpdateLogoutTime()
        {
            try
            {
                string msg = "0";
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@ID", Session["LogoutID"].ToString()));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "UpdateLoginStatus", methodparameter);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    msg = ds.Tables[0].Rows[0]["msg"].ToString();
                }

                return msg;
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return "0";
            }

        }

    }
}