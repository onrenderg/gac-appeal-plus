using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon;

namespace DigitalNagrik.Controllers
{
    public class GACGridViewInboxController : Controller
    {
        // GET: GACGridViewInbox
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetInboxMenu_PV()
        {
            GACGridViewInbox Menu = GACGridViewInbox.GetMenuList(UserSession.GACID);
            return View("_Menu", Menu);
        }

        public ActionResult GetAppealList_PV(string qs)
        {
            string MenuStatusCd = string.Empty; string IsDateGroup = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { MenuStatusCd = Params["MenuStatusCd"]; IsDateGroup = Params["IsDateGroup"]; } catch { }
            if (!string.IsNullOrWhiteSpace(MenuStatusCd) && !string.IsNullOrWhiteSpace(IsDateGroup))
            {
                GACGridViewInbox_Appeal Appeal = GACGridViewInbox_Appeal.GetAppealList(MenuStatusCd, UserSession.GACID); Appeal.IsDateGroup = IsDateGroup;
                return PartialView("AppealList/_AppealList", Appeal);
            }
            return new EmptyResult();
        }

        public ActionResult GetAppealDetails_PV(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACGridViewInbox_AppealDetails Appeal = GACGridViewInbox_AppealDetails.GetAppealDetails(RegistrationYear, GrievanceId);
            return View("_AppealDetails", Appeal);
        }

        public ActionResult GetAppealAssignForm_PV(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACAppealAssignedAction obj = new GACAppealAssignedAction { GAC_AsssignedForwardList = new List<SelectListItem>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GACID", UserSession.GACID)
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AppealAssignedActionList", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    obj.GrievanceId = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceId"]);
                    obj.GrievanceDesc = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceDesc"]);
                    obj.RegistrationYear = Convert.ToString(ds.Tables[0].Rows[0]["RegistrationYear"]);
                    obj.ContentClassification = Convert.ToString(ds.Tables[0].Rows[0]["ContentClassificationTitle"]);
                    obj.SubContentClassification = Convert.ToString(ds.Tables[0].Rows[0]["SubContentClassificationTitle"]);
                    obj.IntermediaryTitle = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryTitle"]);
                    obj.IntermediaryType = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryType"]);
                    obj.RemarksMeity = Convert.ToString(ds.Tables[0].Rows[0]["RemarksMeity"]);
                    obj.ReceiptDate = Convert.ToString(ds.Tables[0].Rows[0]["ReceiptDate"]);
                    obj.GACTitle = Convert.ToString(ds.Tables[0].Rows[0]["GACTitle"]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow Dr in ds.Tables[1].Rows)
                    {
                        obj.GAC_AsssignedForwardList.Add(new SelectListItem { Value = Dr["RoleID"].ToString(), Text = Dr["RoleName"].ToString() });
                    }
                }
            }
            return PartialView("Forms/_ForwardAction", obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACAssignAppeal(GACAppealAssignedAction obj)
        {
            TempData["Status"] = "F";
            ModelState.Remove("ActionID");
            ModelState.Remove("ForwardedToID");
            ModelState.Remove("EMName");
            ModelState.Remove("EMEmailID");
            ModelState.Remove("EMAreaofExpertise");
            ModelState.Remove("MailContent");
            if (ModelState.IsValid)
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@grievanceId", obj.GrievanceId));
                methodparameter.Add(new KeyValuePair<string, string>("@registrationYear", obj.RegistrationYear));
                methodparameter.Add(new KeyValuePair<string, string>("@ForwardedToID", obj.ForwardedToID));
                methodparameter.Add(new KeyValuePair<string, string>("@actionID", "5"));
                methodparameter.Add(new KeyValuePair<string, string>("@GACID", UserSession.GACID));
                methodparameter.Add(new KeyValuePair<string, string>("@remarks", obj.Remarks));
                methodparameter.Add(new KeyValuePair<string, string>("@actionBy", UserSession.UserID));
                methodparameter.Add(new KeyValuePair<string, string>("@actionIP", CommonRepository.GetIPAddress()));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AppealAssignedAction", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = null;
                                TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, "Appeal Assigned", "Appeal (Appeal No. : <span class='fw-600'>" + obj.RegistrationYear + "/" + obj.GrievanceId + "</span>) has been successfully assigned to " + obj.ForwardedToID);
                                break;
                        }
                    }
                }
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "" });
        }
    }
}