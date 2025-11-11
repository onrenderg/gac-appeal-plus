using System.Web.Mvc;

namespace DigitalNagrik.Areas.SubjectExpert
{
    public class SubjectExpertAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SubjectExpert";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SubjectExpert_default",
                "SubjectExpert/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}