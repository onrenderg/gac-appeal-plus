using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalNagrik.Models;

namespace DigitalNagrik.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult MeityDashboard()
        {
            return View();
        }

        public ActionResult MeityStatistics_PV()
        {
            Dashboard_MeityStats Data = Dashboard_MeityStats.GetData();
            return View("PartialViews/_MeityStats", Data);
        }

        public ActionResult MeityStatisticsAverageDays_PV()
        {
            Dashboard_MeityStats_AverageDays Data = new Dashboard_MeityStats_AverageDays { TotalNoOfDays = "5" };
            Data.HtmlString = BALCommon.RenderRazorViewToString("PartialViews/_MeityStats_AverageDays", ControllerContext, ViewData, TempData, new Dashboard_MeityStats_AverageDays());
            return Json(Data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult MeityAvgDays_PV()
        {
            Dashboard_MeityAvgDays Data = Dashboard_MeityAvgDays.GetData();
            return View("PartialViews/_MeityAvgDays", Data);
        }

        public ActionResult Meity6Months_PV()
        {
            Dashboard_Meity6Months Data = Dashboard_Meity6Months.GetData();
            return View("PartialViews/_Meity6Months", Data);
        }

        public ActionResult MeityGACStats_PV()
        {
            Dashboard_MeityGACStats Data = Dashboard_MeityGACStats.GetData();
            return View("PartialViews/_MeityGACStats", Data);
        }

        public ActionResult MeityContentClassification_PV()
        {
            Dashboard_MeityContentClassification Data = Dashboard_MeityContentClassification.GetData();
            return View("PartialViews/_MeityContentClassification", Data);
        }

        //---------------------------------------------------------------------------------------//
        //---------------------------------------------------------------------------------------//

        public ActionResult GACDashboard()
        {
            return View();
        }

        public ActionResult GACStatistics_PV()
        {
            Dashboard_GACStats Data = Dashboard_GACStats.GetData(UserSession.GACID);
            return View("PartialViews/_GACStats", Data);
        }

        public ActionResult GAC6Months_PV()
        {
            Dashboard_GAC6Months Data = Dashboard_GAC6Months.GetData(UserSession.GACID);
            return View("PartialViews/_GAC6Months", Data);
        }

        public ActionResult GACMemberStats_PV()
        {
            Dashboard_GACMemberStats Data = Dashboard_GACMemberStats.GetData(UserSession.GACID);
            return View("PartialViews/_GACMemberStats", Data);
        }

        public ActionResult GACContentClassification_PV()
        {
            Dashboard_GACContentClassification Data = Dashboard_GACContentClassification.GetData();
            return View("PartialViews/_GACContentClassification", Data);
        }
    }
}