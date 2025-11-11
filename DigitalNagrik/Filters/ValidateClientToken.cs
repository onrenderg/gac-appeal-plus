using DigitalNagrik.Controllers;
using DigitalNagrik.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalNagrik.Filters
{
    public class ValidateClientTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {

                if (HttpContext.Current.Session["_RandomToken"].Equals(Convert.ToString(HttpContext.Current.Request.Form[DigitalNagrik.Models.RandomToken.CreateMD5( Convert.ToString(filterContext.RouteData.Values["Controller"]))])))
                {
                    base.OnActionExecuting(filterContext);
                }
               
                else
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    GoToLoginPage(filterContext);
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

       
    }


}