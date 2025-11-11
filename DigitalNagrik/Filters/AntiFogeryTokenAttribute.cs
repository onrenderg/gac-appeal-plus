using DigitalNagrik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalNagrik.Filters
{
    public class AntiFogeryTokenAttribute: ActionFilterAttribute

    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
                filterContext.HttpContext.Response.Cache.SetNoStore();
                if (filterContext.HttpContext.Request.Form.AllKeys.Any(k => k == "CustomCSRFToken"))
                {
                    var CheckValue = filterContext.HttpContext.Request.Form["CustomCSRFToken"];
                    if (CheckValue == Convert.ToString(HttpContext.Current.Session["_CustomCSRFToken"]))
                    {
                        HttpContext.Current.Session["_CustomCSRFToken"] = "";
                        base.OnActionExecuting(filterContext);                
                    }
                    else
                    {
                        filterContext.Result = new HttpUnauthorizedResult();
                        HttpContext.Current.Session["_CustomCSRFToken"] = "";
                        GoToLoginPage(filterContext);
                    }
                }
               
           
            }
            catch (Exception ex) { MyExceptionHandler.HandleException(ex); GoToLoginPage(filterContext); }
        }
        public static void GoToLoginPage(ActionExecutingContext filterContext)
        {
            try
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Error" }, { "Area", "" } });
            }
            catch { }
        }
    }
}