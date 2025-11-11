using System.Web.Mvc;

namespace DigitalNagrik.Areas.Intermediary
{
    public class IntermediaryAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Intermediary";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Intermediary_default",
                "Intermediary/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}