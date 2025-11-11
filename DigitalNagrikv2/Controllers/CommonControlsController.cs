using DigitalNagrik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Controllers
{
    public class CommonControlsController : Controller
    {
        // GET: CommonControls
        public ActionResult ChangeLanguage(string LangCulture)
        {
            //LangCulture = (string.IsNullOrWhiteSpace(LangCulture) ? "en-IN" : LangCulture);
            //UserSession.LangCulture = LangCulture;
            ////return RedirectToAction("Index", "Home", new { Area = "" });
            //return Redirect(Request.UrlReferrer.ToString());
            string ServiceURL = Request.UrlReferrer.ToString();
            if (CommonRepository.IsValidUrl(ServiceURL))
            {
                LangCulture = (string.IsNullOrWhiteSpace(LangCulture) ? "en-IN" : LangCulture);
                UserSession.LangCulture = LangCulture;
                return Redirect(ServiceURL);
            }
            else
            {
                return RedirectToAction("GAC", "Home");
            }
        }
    }
}