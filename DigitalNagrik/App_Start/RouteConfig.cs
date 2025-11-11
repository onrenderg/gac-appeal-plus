using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalNagrik
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(name: "SSOLogin", url: "SSOLogin", defaults: new { controller = "SSOLogin", action = "Index" });
            routes.MapRoute(name: "Intermediary", url: "Intermediary", defaults: new { controller = "Home", action = "Intermediary" });
            routes.MapRoute(name: "GAC", url: "GAC", defaults: new { controller = "Home", action = "GAC" });

            routes.MapRoute(
                name: "Index",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
