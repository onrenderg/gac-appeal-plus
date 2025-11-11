using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon;
using DigitalNagrik.Filters;

namespace DigitalNagrik.Controllers
{
    public class GridViewInboxController : Controller
    {
        // GET: GridViewInbox
        //[CheckSessions]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInboxMenu_PV()
        {
            GridViewInbox Menu = GridViewInbox.GetMenuList();
            return View("_Menu", Menu);
        }

        public ActionResult GetAppealList_PV(string qs)
        {
            string MenuStatusCd = string.Empty; string IsDateGroup = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { MenuStatusCd = Params["MenuStatusCd"]; IsDateGroup = Params["IsDateGroup"]; } catch { }
            if (!string.IsNullOrWhiteSpace(MenuStatusCd) && !string.IsNullOrWhiteSpace(IsDateGroup))
            {
                GridViewInbox_Appeal Appeal = GridViewInbox_Appeal.GetAppealList(MenuStatusCd); Appeal.IsDateGroup = IsDateGroup;
                return PartialView("AppealList/_AppealList", Appeal);
            }
            return new EmptyResult();
        }

        public ActionResult GetAppealDetails_PV(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GridViewInbox_AppealDetails Appeal = GridViewInbox_AppealDetails.GetAppealDetails(RegistrationYear, GrievanceId);
            ViewBag.ActionList = ActionList(RegistrationYear, GrievanceId);
            return View("_AppealDetails", Appeal);
        }

        public List<SelectListItem> ActionList(string RegistrationYear, string GrievanceId)
        {
            List<SelectListItem> ActionList = new List<SelectListItem>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", RegistrationYear)
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AppealActionList", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow Dr in ds.Tables[1].Rows)
                    {
                        ActionList.Add(new SelectListItem { Value = Dr["ActionId"].ToString(), Text = Dr["ActionTitle"].ToString() });
                    }
                }
            }
            return ActionList;
        }

        public ActionResult GetAppealActionForm_PV(string qs)
        {
            string ActionID = string.Empty; string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { ActionID = RegistrationYear = Params["ActionID"]; RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            AppealAction Action = new AppealAction { ActionID = ActionID, RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            return PartialView("Forms/_GACActionDetails", Action);
        }

        public ActionResult GetAppealAssignForm_PV(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            AppealAction Action = new AppealAction { GACUsersForAssignment = new List<SelectListItem>(), RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", RegistrationYear)
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AppealAssignmentUsers", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Action.GACId = Convert.ToString(ds.Tables[0].Rows[0]["GACId"]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow Dr in ds.Tables[1].Rows)
                    {
                        Action.GACUsersForAssignment.Add(new SelectListItem { Value = Dr["GACId"].ToString(), Text = Dr["GACTitle"].ToString() });
                    }
                }
            }
            return PartialView("Forms/_GACUserAssignment", Action);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACAppealActionApproval(AppealAction obj)
        {
            TempData["Status"] = "F";
            var methodparameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", obj.GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", obj.RegistrationYear),
                new KeyValuePair<string, string>("@actionID", obj.ActionID),
                new KeyValuePair<string, string>("@remarks", obj.Remarks),
                new KeyValuePair<string, string>("@actionBy", UserSession.UserID),
                new KeyValuePair<string, string>("@actionIP", CommonRepository.GetIPAddress())
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AppealActionInsert", methodparameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    switch (res.Trim().ToUpper())
                    {
                        case "S":
                            TempData["Status"] = null;
                            TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, "Action Successful", "Action has been successfully taken on the appeal (Appeal No. : <span class='fw-600'>" + obj.RegistrationYear + "/" + obj.GrievanceId + "</span>).");
                            break;
                    }
                }
            }
            return RedirectToAction("Index", "GridViewInbox", new { Area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignGACUserToGrievance(AppealAction obj)
        {
            TempData["Status"] = "F";
            var methodparameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", obj.GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", obj.RegistrationYear),
                new KeyValuePair<string, string>("@GACUserID", obj.GACId),
                new KeyValuePair<string, string>("@remarks", obj.Remarks),
                new KeyValuePair<string, string>("@actionBy", UserSession.UserID),
                new KeyValuePair<string, string>("@actionIP", CommonRepository.GetIPAddress())
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AssignGACUserToGrievance", methodparameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    switch (res.Trim().ToUpper())
                    {
                        case "S":
                            TempData["Status"] = null;
                            TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, "Appeal Assigned", "Appeal (Appeal No. : <span class='fw-600'>" + obj.RegistrationYear + "/" + obj.GrievanceId + "</span>) has been successfully assigned to GAC.");
                            break;
                    }
                }
            }
            return RedirectToAction("Index", "GridViewInbox", new { Area = "" });
        }
    }
}