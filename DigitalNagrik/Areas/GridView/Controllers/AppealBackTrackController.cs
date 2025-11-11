using DigitalNagrik.Areas.GridView.Data;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon;

namespace DigitalNagrik.Areas.GridView.Controllers
{
    public class AppealBackTrackController : Controller
    {
        // GET: GridView/AppealBackTrack
        [CheckSessions]
        public ActionResult Index()
        {
            try
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return View();
        }
        [CheckSessions]
        public ActionResult GetAppealDetails(string AppealID)
        {
            AppealBackTracking PostData = new AppealBackTracking();
            try
            {
                string AppealNumber = AppealID;
                string GrievanceID = AppealNumber.Split('/')[0];
                string RegistrationYear = AppealNumber.Split('/')[1];

                PostData = AppealBackTracking.GetAppealDetails(RegistrationYear, GrievanceID);
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                TempData["Status"] = "F";
            }
            return PartialView("PartialViews/_AppealDetails", PostData);
        }
        [CheckSessions]
        [ValidateAntiForgeryToken]
        [HttpPost]

        public ActionResult SaveAppealBackTracking(AppealBackTracking PostData)
        {
            try
            {
                TempData["Status"] = "F";
                //if (PostData.IsAdmit == "Y") { ModelState.Remove("NoAdmitTypeId"); ModelState.Remove("RejectSummary"); PostData.RejectSummary = string.Empty; }
                //bool SubmitType = PostData.SubmitType == "D" || (PostData.SubmitType == "S" && PostData.IsAdmit == "Y") || (PostData.SubmitType == "R" && PostData.IsAdmit == "N");
                if (ModelState.IsValid)
                {
                    var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@ActionString", PostData.ActionString.Trim()),
                    new KeyValuePair<string, string>("@ActionBy", UserSession.UserID),
                    new KeyValuePair<string, string>("@ActionIP", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "UpdateData")
                };
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AppealBackTracking_Action", methodparameter);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            switch (res.Trim().ToUpper())
                            {
                                case "S":
                                    TempData["Status"] = null;
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), "Data Saved Successfully " + ": (<span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                    break;
                                default:
                                    TempData["Status"] = null;
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Error, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), "Some Error Occured while saving.." + ": (<span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                    break;

                            }
                        }
                    }
                    return RedirectToAction("Index", "AppealBackTrack", new { Area = "GridView" });
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return RedirectToAction("Index", "AppealBackTrack", new { Area = "GridView" });
        }
    }
}