using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalNagrik
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Session_End(object sennder, EventArgs e)
        {
            try
            {

                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@SessionID", Session.SessionID),
                    new KeyValuePair<string, string>("@VMName",System.Environment.MachineName),
                    new KeyValuePair<string, string>("@Mode", "UpdateStatus_SessionID")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Recomendations", methodparameter);
            }
            catch (Exception ex)
            { }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            MyExceptionHandler.LogError(exception);
            try
            {
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                int _ErrorCode = (exception is HttpException httpException) ? httpException.GetHttpCode() : 500;
                bool _IsAjaxRequest = HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                Response.Clear();
                Server.ClearError();
                Server.TransferRequest(urlHelper.Action("Error", "Home", new { Area = "" }));
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
        }

        protected void Application_PreSendRequestHeaders()
        {
            if (Response.Cookies.Count > 0)
            {
                foreach (string cookieName in Response.Cookies.AllKeys)
                {
                    //Response.Cookies[cookieName].Secure = true;
                    //Response.Cookies[cookieName].SameSite = SameSiteMode.Strict;
                    //Response.Cookies[cookieName].Path = "/DigitalNagrik";
                }
            }
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-Powerered-By");
        }
    }
}
