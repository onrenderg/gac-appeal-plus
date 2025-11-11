using DigitalNagrik.Areas.GridView.Data;
using DigitalNagrik.Areas.Intermediary.Data;
using DigitalNagrik.Areas.Public.Models;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon;

namespace DigitalNagrik.Areas.GridView.Controllers
{
    public class GACGridViewInboxController : Controller
    {
        // GET: GridView/GACGridViewInbox

        //[CheckSessions]
        public ActionResult SwitchGAC(string GACId)
        {
            UserSession.GACID = GACId;
            return RedirectToAction("Index");
        }

        [CheckSessions]
        public ActionResult Index(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            try
            {
                if (Session["GVMenuStatusCd"] == null) { Session["GVMenuStatusCd"] = GACGridViewInbox.MenuStatus.Inbox; }
                List<SelectListItem> GACList = GACGridViewInbox.GetMappedGACList(UserSession.UserID);
                if (string.IsNullOrWhiteSpace(UserSession.GACID)) { UserSession.GACID = GACList.FirstOrDefault().Value; }
                UserSession.GACName = GACList.Where(x => x.Value == UserSession.GACID).FirstOrDefault().Text;
                GACList.RemoveAll(x => x.Value == UserSession.GACID);
                ViewBag.GACList = GACList; ViewBag.qs = qs;
                //New
                List<SelectListItem> Notifications = GACGridViewInbox.GetNotifications(UserSession.UserID, UserSession.GACID);
                ViewBag.Notifications = Notifications;
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return View();
        }

        //[CheckSessions]
        public ActionResult GetInboxMenu_PV()
        {
            GACGridViewInbox Menu = GACGridViewInbox.GetMenuList(UserSession.GACID, UserSession.UserID);
            return View("_Menu", Menu);
        }

        [CheckSessions]
        public ActionResult GetResponseDelayStats_PV()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            GACGridViewInbox_DelayResponses Data = GACGridViewInbox_DelayResponses.GetDelayResponsesData(UserSession.GACID, UserSession.UserID);
            return View("PartialViews/_DelayResponseStats", Data);
        }

        //[CheckSessions]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult GetAppealList_SearchPV(GACGridViewInbox_Appeal Data)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string MenuStatusCd = string.Empty; string IsDateGroup = string.Empty; var Params = QueryString.GetDecryptedParameters(Data.qs); try { MenuStatusCd = Params["MenuStatusCd"]; IsDateGroup = Params["IsDateGroup"]; } catch { }
            if (!string.IsNullOrWhiteSpace(MenuStatusCd) && !string.IsNullOrWhiteSpace(IsDateGroup) && !string.IsNullOrWhiteSpace(Data.SearchText))
            {
                GACGridViewInbox_Appeal Appeal = GACGridViewInbox_Appeal.GetAppealList(MenuStatusCd, UserSession.GACID, UserSession.UserID, Data.SearchText, (Data.Days == "undefined" ? "5" : Data.Days)); Appeal.IsDateGroup = IsDateGroup; Appeal.IsDateGroup = IsDateGroup; Appeal.SearchText = Data.SearchText; Appeal.Days = Data.Days;
                return PartialView("AppealList/_AppealList", Appeal);
            }
            return new EmptyResult();
        }


        [CheckSessions]
        public ActionResult GetAppealList_PV(string qs, string SearchText = "", string Days = "5")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string MenuStatusCd = string.Empty; string IsDateGroup = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { MenuStatusCd = Params["MenuStatusCd"]; IsDateGroup = Params["IsDateGroup"]; } catch { }
            if (!string.IsNullOrWhiteSpace(MenuStatusCd) && !string.IsNullOrWhiteSpace(IsDateGroup))
            {
                Session["GVMenuStatusCd"] = MenuStatusCd;
                GACGridViewInbox_Appeal Appeal = GACGridViewInbox_Appeal.GetAppealList(MenuStatusCd, UserSession.GACID, UserSession.UserID, SearchText, (Days == "undefined" ? "5" : Days)); Appeal.IsDateGroup = IsDateGroup; Appeal.IsDateGroup = IsDateGroup; Appeal.SearchText = SearchText; Appeal.Days = Days;
                return PartialView("AppealList/_AppealList", Appeal);
            }
            return new EmptyResult();
        }

        [CheckSessions]
        public ActionResult GetAppealDetails_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACGridViewInbox_AppealDetails Appeal = GACGridViewInbox_AppealDetails.GetAppealDetails(RegistrationYear, GrievanceId);
            return View("_AppealDetails", Appeal);
        }

        [CheckSessions]
        public ActionResult GetAppealDetailsGrid_PV(string qs, string IsDocGrid = "Y")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACGridViewInbox_AppealDetails Appeal = GACGridViewInbox_AppealDetails.GetAppealDetails(RegistrationYear, GrievanceId); ViewBag.IsDocGrid = IsDocGrid;
            return PartialView("PartialViews/_AppealDetails", Appeal);
        }

        //----------------------------------------------------Appeal Actions-------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------//

        #region Appeal Actions

        [CheckSessions]
        public ActionResult GetAssignAppealForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACAppealAssign Action = new GACAppealAssign { GACList = GACAppealAssign.GetGACList(RegistrationYear, GrievanceId), RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            return PartialView("Forms/_Reassign", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACAssignAppeal(GACAppealAssign PostData)
        {
            TempData["Status"] = "F";
            if (ModelState.IsValid)
            {
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@SuggestedGACId", PostData.GACId),
                    new KeyValuePair<string, string>("@Remarks", PostData.Remarks.Trim()),
                    new KeyValuePair<string, string>("@GACID", UserSession.GACID),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "ACTION_REASSIGN")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "AppealTransferrequestsubmittedtotheManager") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        //-------------------------------------------------------------------------------------------------------------------------------//

        [CheckSessions]
        public ActionResult GetTransferApproveForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string TransferRequestId = string.Empty; string GACId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs);
            try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; TransferRequestId = Params["TransferRequestId"]; GACId = Params["GACId"]; } catch { }
            GACAppealTransferApprove Action = new GACAppealTransferApprove { GACList = GACAppealAssign.GetGACList(RegistrationYear, GrievanceId), RegistrationYear = RegistrationYear, GrievanceId = GrievanceId, TransferRequestId = TransferRequestId, GACId = GACId };
            return PartialView("Forms/_TransferApprove", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACTransferApprove(GACAppealTransferApprove PostData)
        {
            TempData["Status"] = "F";
            if (PostData.TransferRequestStatus == GACGridViewInbox.TransferRequestStatus.Rejected) { ModelState.Remove("GACId"); }
            if (ModelState.IsValid)
            {
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@TransferRequestId", PostData.TransferRequestId),
                    new KeyValuePair<string, string>("@TransferRequestStatus", PostData.TransferRequestStatus),
                    new KeyValuePair<string, string>("@SuggestedGACId", (PostData.TransferRequestStatus == GACGridViewInbox.TransferRequestStatus.Rejected ? DBNull.Value.ToString() : PostData.GACId )),
                    new KeyValuePair<string, string>("@Remarks", PostData.Remarks.Trim()),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "ACTION_TRANSFER")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, "Successfully " + (PostData.TransferRequestStatus == GACGridViewInbox.TransferRequestStatus.Approved ? "Approved" : "Rejected"), "Appeal Transfer request has been successfully " + (PostData.TransferRequestStatus == GACGridViewInbox.TransferRequestStatus.Approved ? "Approved" : "Rejected") + " (Appeal No. : <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                break;
                        }
                    }
                }
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        //-------------------------------------------------------------------------------------------------------------------------------//

        //[CheckSessions]
        //public ActionResult GACRejectAppeal(string qs)
        //{
        //    TempData["Status"] = "F";
        //    string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
        //    if (!string.IsNullOrWhiteSpace(RegistrationYear) && !string.IsNullOrWhiteSpace(GrievanceId))
        //    {
        //        var methodparameter = new List<KeyValuePair<string, string>>
        //        {
        //            new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
        //            new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
        //            new KeyValuePair<string, string>("@UserId", UserSession.UserID),
        //            new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
        //            new KeyValuePair<string, string>("@Mode", "ACTION_REJECT")
        //        };
        //        DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", methodparameter);
        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
        //            if (!string.IsNullOrWhiteSpace(res))
        //            {
        //                switch (res.Trim().ToUpper())
        //                {
        //                    case "S":
        //                        string EmailMsg = Convert.ToString(ds.Tables[0].Rows[0]["EmailMsg"]);
        //                        string EmailSubject = "Grievance Appellate Committee - Appeal number " + Convert.ToString(ds.Tables[0].Rows[0]["AppealID"]) + " not admitted.";
        //                        string EmailId = Convert.ToString(ds.Tables[0].Rows[0]["AppellanEmail"]);
        //                        new SendSMS().SendMailToUser(EmailMsg, EmailSubject, EmailId, "Grievance Appellate Committee", UserSession.UserID);
        //                        Session["GVMenuStatusCd"] = GACGridViewInbox.MenuStatus.Rejected;
        //                        TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "Appealrejected") + ": <span class='fw-600'>" + GrievanceId + "/" + RegistrationYear + "</span>).");
        //                        break;
        //                }
        //            }
        //        }
        //        return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + RegistrationYear + "&GrievanceId=" + GrievanceId) });
        //    }
        //    return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        //}

        //-------------------------------------------------------------------------------------------------------------------------------//

        [CheckSessions]
        public ActionResult GetMatterSummaryForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACAppealMatterSummary Action = GACAppealMatterSummary.GetMatterSummaryDetails(RegistrationYear, GrievanceId);
            Action.RecTypeList = GACAppealMatterSummary.GetRecTypeList(); Action.NoAdmitTypeList = GACAppealMatterSummary.GetNoAdmitTypeList();
            return PartialView("Forms/_MatterSummary", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACMatterSummary(GACAppealMatterSummary PostData)
        {
            TempData["Status"] = "F";
            if (PostData.IsAdmit == "Y") { ModelState.Remove("NoAdmitTypeId"); ModelState.Remove("RejectSummary"); PostData.RejectSummary = string.Empty; }
            bool SubmitType = PostData.SubmitType == "D" || (PostData.SubmitType == "S" && PostData.IsAdmit == "Y") || (PostData.SubmitType == "R" && PostData.IsAdmit == "N");
            if (ModelState.IsValid && SubmitType)
            {
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@HtmlContent", PostData.Summary.Trim()),
                    new KeyValuePair<string, string>("@ObservationText", PostData.ObservationText.Trim()),
                    new KeyValuePair<string, string>("@RecommendationText", PostData.RecommendationText),
                    new KeyValuePair<string, string>("@IsAdmit", PostData.IsAdmit),
                    new KeyValuePair<string, string>("@NoAdmitTypeId", PostData.IsAdmit == "Y" ? DBNull.Value.ToString() : PostData.NoAdmitTypeId),
                    new KeyValuePair<string, string>("@RecOtherText", PostData.RecTypeId != "3" ? DBNull.Value.ToString() : PostData.RecOtherText.Trim()),
                    new KeyValuePair<string, string>("@RejectSummary", Trim(PostData.RejectSummary)),
                    new KeyValuePair<string, string>("@SubmitType", Trim(PostData.SubmitType)),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "ACTION_SUMMARY_DRAFT")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = null;
                                if (PostData.SubmitType == "R")
                                {
                                    string EmailMsg = Convert.ToString(ds.Tables[0].Rows[0]["EmailMsg"]);
                                    string EmailMsg_Interm = Convert.ToString(ds.Tables[0].Rows[0]["EmailMsg_Interm"]);
                                    string EmailSubject = "Grievance Appellate Committee - Appeal number " + Convert.ToString(ds.Tables[0].Rows[0]["AppealID"]) + " not admitted.";
                                    string EmailId = Convert.ToString(ds.Tables[0].Rows[0]["AppellanEmail"]);
                                    string EmailId_Interm = Convert.ToString(ds.Tables[0].Rows[0]["EmailId_Interm"]);
                                    SendSMS SendEmail = new SendSMS(); SendEmail.SendMailToUser(EmailMsg, EmailSubject, EmailId, "Grievance Appellate Committee", UserSession.UserID);
                                    SendEmail.SendMailToUser(EmailMsg_Interm, EmailSubject, EmailId_Interm, "Grievance Appellate Committee", UserSession.UserID);
                                    Session["GVMenuStatusCd"] = GACGridViewInbox.MenuStatus.Rejected;
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "Appealrejected") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                }
                                else if (PostData.SubmitType == "S")
                                {
                                    Session["GVMenuStatusCd"] = GACGridViewInbox.MenuStatus.InProcess;
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "TheMatterSummarysubmittedtoCommittee") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                }
                                else
                                {
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "MatterSummarysavedasdraft") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                }
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        //[CheckSessions]
        //public ActionResult FinalizeGACMatterSummary(string qs)
        //{
        //    TempData["Status"] = "F";
        //    string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
        //    if (!string.IsNullOrWhiteSpace(RegistrationYear) && !string.IsNullOrWhiteSpace(GrievanceId))
        //    {
        //        GACAppealMatterSummary Action = GACAppealMatterSummary.GetMatterSummaryDetails(RegistrationYear, GrievanceId); ViewBag.IsDraft = "F";
        //        string Summary = RenderRazorViewToString("Prints/_MatterSummary", ControllerContext, ViewData, TempData, Action);
        //        var methodparameter = new List<KeyValuePair<string, string>>
        //        {
        //            new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
        //            new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
        //            new KeyValuePair<string, string>("@HtmlContent", Summary),
        //            new KeyValuePair<string, string>("@UserId", UserSession.UserID),
        //            new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
        //            new KeyValuePair<string, string>("@Mode", "ACTION_SUMMARY_FINALIZE")
        //        };
        //        DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", methodparameter);
        //        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
        //            if (!string.IsNullOrWhiteSpace(res))
        //            {
        //                switch (res.Trim().ToUpper())
        //                {
        //                    case "S":
        //                        Session["GVMenuStatusCd"] = GACGridViewInbox.MenuStatus.InProcess;
        //                        TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "TheMatterSummarysubmittedtoCommittee") + ": <span class='fw-600'>" + GrievanceId + "/" + RegistrationYear + "</span>).");
        //                        break;
        //                }
        //            }
        //        }
        //        return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + RegistrationYear + "&GrievanceId=" + GrievanceId) });
        //    }
        //    return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        //}

        public ActionResult PrintMatterSummary(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string IsDraft = "N"; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; IsDraft = Params["IsDraft"]; } catch { }
            GACAppealMatterSummary Action = GACAppealMatterSummary.GetMatterSummaryDetails(RegistrationYear, GrievanceId);
            ViewBag.IsDraft = IsDraft;
            ViewAsPdf PDF = new ViewAsPdf("Prints/_MatterSummary", Action)
            {
                FileName = "DownloadLetter.pdf",
                PageMargins = { Left = 8, Right = 8 },
                CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"9\" --footer-left \"GAC (Appeal No: " + GrievanceId + "/" + RegistrationYear + ")\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \"",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait
            };
            byte[] PDFBytes = PDF.BuildPdf(ControllerContext);
            return File(PDFBytes, "application/pdf");
        }

        //-------------------------------------------------------------------------------------------------------------------------------//

        [CheckSessions]
        public ActionResult GetIntermediaryVerificationForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACIntermediaryVerification Action = GACIntermediaryVerification.GetIntermediaryVerification(RegistrationYear, GrievanceId);
            Action.IntermediaryList = GACIntermediaryVerification.GetIntermediaryList();
            return PartialView("Forms/_IntermediaryVerification", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACIntermedVerification(GACIntermediaryVerification PostData)
        {
            TempData["Status"] = "F";
            if (PostData.IntermediaryActionId == GACGridViewInbox.IntermediaryAction.MapWithExisting)
            {
                ModelState.Remove("IntermediaryDetails.IntermediaryTitle"); ModelState.Remove("IntermediaryDetails.IntermediaryUrl"); ModelState.Remove("IntermediaryDetails.GOName");
                ModelState.Remove("IntermediaryDetails.GOEmail"); ModelState.Remove("IntermediaryDetails.IntermediaryAddress"); ModelState.Remove("IntermediaryDetails.Helplink");
            }
            else { ModelState.Remove("IntermediaryId"); }
            if (ModelState.IsValid)
            {
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@IntermediaryActionId", PostData.IntermediaryActionId),
                    new KeyValuePair<string, string>("@IntermediaryId",  PostData.IntermediaryId == GACGridViewInbox.IntermediaryAction.NewIntermediary ? DBNull.Value.ToString() : PostData.IntermediaryId),
                    new KeyValuePair<string, string>("@IntermediaryTitle", PostData.IntermediaryActionId == GACGridViewInbox.IntermediaryAction.MapWithExisting ? DBNull.Value.ToString() : PostData.IntermediaryDetails.IntermediaryTitle.Trim().ToUpper()),
                    new KeyValuePair<string, string>("@IntermediaryUrl", PostData.IntermediaryActionId == GACGridViewInbox.IntermediaryAction.MapWithExisting ? DBNull.Value.ToString() : PostData.IntermediaryDetails.IntermediaryUrl.Trim()),
                    new KeyValuePair<string, string>("@GOName", PostData.IntermediaryActionId == GACGridViewInbox.IntermediaryAction.MapWithExisting ? DBNull.Value.ToString() : PostData.IntermediaryDetails.GOName.Trim().ToUpper()),
                    new KeyValuePair<string, string>("@GOEmail", PostData.IntermediaryActionId == GACGridViewInbox.IntermediaryAction.MapWithExisting ? DBNull.Value.ToString() : PostData.IntermediaryDetails.GOEmail.Trim()),
                    new KeyValuePair<string, string>("@IntermediaryAddress", PostData.IntermediaryActionId == GACGridViewInbox.IntermediaryAction.MapWithExisting ? DBNull.Value.ToString() : Trim(PostData.IntermediaryDetails.IntermediaryAddress)),
                    new KeyValuePair<string, string>("@Helplink", PostData.IntermediaryActionId == GACGridViewInbox.IntermediaryAction.MapWithExisting ? DBNull.Value.ToString() : Trim(PostData.IntermediaryDetails.Helplink)),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "ACTION_INTERM_VERIFY")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_VerifyIntermediary", methodparameter);
                if (ds != null && ds.Tables.Count > 1)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                if (ds.Tables[1].Rows.Count > 0)
                                {
                                    string IntermediaryLetterText = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryLetterFormat"]);
                                    string AppellantLetterText = Convert.ToString(ds.Tables[0].Rows[0]["AppealSubmitLetterFormat"]);

                                    foreach (DataRow dr in ds.Tables[1].Rows)
                                    {
                                        string EmailMsg = IntermediaryLetterText;
                                        string EmailSubject = "Grievance Appellate Committee - Appeal number " + Convert.ToString(dr["AppealID"]) + " from " + Convert.ToString(dr["AppellantName"]) + " against " + Convert.ToString(dr["IntermediaryTitle"]);
                                        string EmailId = Convert.ToString(dr["IntermediaryGROEmail"]);
                                        Dictionary<string, string> keyword = new Dictionary<string, string>
                                        {
                                            { "@IntermediaryTitle", dr["IntermediaryTitle"] == DBNull.Value ? "" : Convert.ToString(dr["IntermediaryTitle"]) },
                                            { "@IntermediaryUrl", dr["IntermediaryURL"] == DBNull.Value ? "" : Convert.ToString(dr["IntermediaryURL"]) },
                                            { "@AppealID", dr["AppealID"] == DBNull.Value ? "" : Convert.ToString(dr["AppealID"]) },
                                            { "@TargetDate", dr["TargetDate"] == DBNull.Value ? "" : Convert.ToString(dr["TargetDate"]) },
                                        };
                                        foreach (KeyValuePair<string, string> entry in keyword)
                                        {
                                            EmailMsg = EmailMsg.Replace(entry.Key.ToString(), entry.Value.ToString());
                                        }
                                        new SendSMS().SendMailToUser(EmailMsg, EmailSubject, EmailId, "Grievance Appellate Committee", UserSession.UserID);
                                    }

                                    foreach (DataRow dr in ds.Tables[1].Rows)
                                    {
                                        string EmailMsg = AppellantLetterText;
                                        string EmailSubject = "Grievance Appellate Committee - Appeal number " + Convert.ToString(dr["AppealID"]) + " is Submitted.";
                                        string EmailId = Convert.ToString(dr["UserEmail"]);
                                        Dictionary<string, string> keyword = new Dictionary<string, string>
                                        {
                                            { "@IntermediaryTitle", dr["IntermediaryTitle"] == DBNull.Value ? "" : Convert.ToString(dr["IntermediaryTitle"]) },
                                            { "@AppellantName", dr["AppellantName"] == DBNull.Value ? "" : Convert.ToString(dr["AppellantName"]) },
                                            { "@AppealID", dr["AppealID"] == DBNull.Value ? "" : Convert.ToString(dr["AppealID"]) },
                                            { "@AppealDate", dr["ReceiptDate"] == DBNull.Value ? "" : Convert.ToString(dr["ReceiptDate"]) },
                                        };
                                        foreach (KeyValuePair<string, string> entry in keyword)
                                        {
                                            EmailMsg = EmailMsg.Replace(entry.Key.ToString(), entry.Value.ToString());
                                        }
                                        new SendSMS().SendMailToUser(EmailMsg, EmailSubject, EmailId, "Grievance Appellate Committee", UserSession.UserID);
                                    }
                                }
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "RespondentIntermediarydetailsVerifiedAppealavailableinInbox") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        //-------------------------------------Recommendations----------------------------------------//

        [CheckSessions]
        public ActionResult GetRecommendationForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACRecomendations Action = GACRecomendations.GetRecomendations(RegistrationYear, GrievanceId);
            return PartialView("Forms/_Recomendations", Action);
        }
        [CheckSessions]
        public JsonResult UpdateWindowOpenedStatus(string qs)
        {
            string result = "";
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId",GrievanceId),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "Update_Status")
                };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Recomendations", methodparameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = Convert.ToString(ds.Tables[0].Rows[0]["Status"]);

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACRecomendation(GACRecomendations PostData)
        {
            TempData["Status"] = "F";
            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, dynamic>> methodparameter2 = new List<KeyValuePair<string, dynamic>>
                {
                    new KeyValuePair<string, dynamic>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, dynamic>("@GrievanceID", PostData.GrievanceId),
                    new KeyValuePair<string, dynamic>("@AgreeDisagree", PostData.AgreeDisagree),
                    new KeyValuePair<string, dynamic>("@DraftDecision", PostData.DraftDecision),
                    new KeyValuePair<string, dynamic>("@ParaId", PostData.ParaId),
                    new KeyValuePair<string, dynamic>("@Recomendations", PostData.Recomendations),
                    //new KeyValuePair<string, dynamic>("@Observations", dtObservations),
                    new KeyValuePair<string, dynamic>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, dynamic>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, dynamic>("@Mode", "INSERT_SUMMARY")
                };
                DataTable ds = new DBAccess().INSERTUpdateData(methodparameter2, "[GAC_GridViewInbox_Recomendations]");
                if (ds != null && ds.Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                Session["GVMenuStatusCd"] = GACGridViewInbox.MenuStatus.InProcess;
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "MemberOpinionsubmitted") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        //---------------------------------Recommendations----------------------------------------------------//

        //------------------------------Decision by Chairperson---------------------------------------//

        [CheckSessions]
        public ActionResult GetDecisionByChairpersonForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACDecisionbyChairperson Action = GACDecisionbyChairperson.GetDecisionbyChairperson(RegistrationYear, GrievanceId);
            return PartialView("Forms/_DecisionbyChairperson", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACDecisionByChairperson(GACDecisionbyChairperson PostData)
        {
            TempData["Status"] = "F";
            if (PostData.DecisionTypeId == "1" || PostData.DecisionTypeId == "2" || PostData.DecisionTypeId == "3")
            {
                ModelState.Remove("DecisionType");
            }
            if (ModelState.IsValid)
            {
                string SubmitType = string.Empty;
                if (PostData.SubmitType == "Draft")
                {
                    SubmitType = "INSERT_DECISION_DRAFT";
                }
                else
                {
                    SubmitType = "INSERT_DECISION";
                }

                List<KeyValuePair<string, dynamic>> methodparameter2 = new List<KeyValuePair<string, dynamic>>
                {
                    new KeyValuePair<string, dynamic>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, dynamic>("@GrievanceID", PostData.GrievanceId),
                    new KeyValuePair<string, dynamic>("@ParaID", PostData.ParaId),
                    new KeyValuePair<string, dynamic>("@AgreeDisagree", PostData.AgreeDisagree),
                    new KeyValuePair<string, dynamic>("@Recommendations", PostData.Recomendations),
                    new KeyValuePair<string, dynamic>("@DecisionTypeId", PostData.DecisionTypeId),
                    new KeyValuePair<string, dynamic>("@Remarks", PostData.Remarks),
                    new KeyValuePair<string, dynamic>("@DraftDecision", PostData.DraftDecision),
                    //new KeyValuePair<string, dynamic>("@Observations", dtObservations),
                    new KeyValuePair<string, dynamic>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, dynamic>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, dynamic>("@Mode", SubmitType),
            };
                DataTable ds = new DBAccess().INSERTUpdateData(methodparameter2, "[GAC_GridViewInbox_DecisionbyChairperson]");
                if (ds != null && ds.Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                Session["GVMenuStatusCd"] = GACGridViewInbox.MenuStatus.InProcess;
                                TempData["Status"] = null;
                                if (PostData.SubmitType == "Draft")
                                {
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "ChairpersonDecisonsubmittedDraft") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");

                                }
                                else
                                {
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "ChairpersonDecisonsubmitted") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");

                                }
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        //------------------------------Decision by Chairperson---------------------------------------//

        [CheckSessions]
        public ActionResult GetDecisionAppealForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACAppealDecision Action = GACAppealDecision.GetOrderSummaryDetails(RegistrationYear, GrievanceId);
            return PartialView("Forms/_Decision", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACDecisionAppeal(GACAppealDecision PostData)
        {
            TempData["Status"] = "F";
            if (ModelState.IsValid)
            {
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@HtmlContent", PostData.DraftDecision.Trim()),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "ACTION_ORDER_DRAFT")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "DraftDecisionsuccessfullyprepared") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        public ActionResult PrintDraftDecision(string qs)
        {
            byte[] PDFBytes = new byte[15000];
            try
            {
                string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
                GACAppealDecision Action = GACAppealDecision.GetOrderSummaryDetails(RegistrationYear, GrievanceId);
                ViewAsPdf PDF = new ViewAsPdf("Prints/_DraftDecision", Action)
                {
                    FileName = "DownloadLetter.pdf",
                    PageMargins = { Left = 8, Right = 8, Top = 8, Bottom = 8 },
                    CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"9\" --footer-left \"GAC (https://gac.gov.in)\" --footer-center \"Page [page] of [topage]\"  --footer-right \" (Appeal No: " + GrievanceId + "/" + RegistrationYear + ") \"",
                    PageSize = Rotativa.Options.Size.A4,
                    PageOrientation = Rotativa.Options.Orientation.Portrait
                };
                PDFBytes = PDF.BuildPdf(ControllerContext);
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return File(PDFBytes, "application/pdf");
        }

        //-------------------------------------------------------------------------------------------------------------------------------//

        [CheckSessions]
        public ActionResult GetAdditionalInputForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACAppealAdditionalInput Action = new GACAppealAdditionalInput { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            return PartialView("Forms/_AdditionalInput", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACAdditionalInput(GACAppealAdditionalInput PostData)
        {
            TempData["Status"] = "F";
            if (ModelState.IsValid)
            {
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@Remarks", PostData.SeekQuery.Trim()),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "ACTION_ADD_INPUT")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", methodparameter);
                if (ds != null && ds.Tables.Count > 1)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                string InputRequestNo = Convert.ToString(ds.Tables[0].Rows[0]["InputRequestNo"]);
                                string EmailSubject = "Grievance Appellate Committee - " + Convert.ToString(ds.Tables[0].Rows[0]["EmailSubject"]);
                                string EmailId = Convert.ToString(ds.Tables[1].Rows[0]["IntermediaryGROEmail"]);
                                string LetterText = Convert.ToString(ds.Tables[0].Rows[0]["LetterText"]);
                                Dictionary<string, string> keyword = new Dictionary<string, string>
                                {
                                    { "@IntermediaryTitle", ds.Tables[1].Rows[0]["IntermediaryTitle"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["IntermediaryTitle"]) },
                                    { "@IntermediaryUrl", ds.Tables[1].Rows[0]["IntermediaryURL"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["IntermediaryURL"]) },
                                    { "@AppealID", ds.Tables[1].Rows[0]["AppealID"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["AppealID"]) },
                                    { "@AppealDate", ds.Tables[1].Rows[0]["ReceiptDate"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["ReceiptDate"]) },
                                    { "@AppellantName", ds.Tables[1].Rows[0]["AppellantName"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["AppellantName"]) },
                                    { "@Ground", ds.Tables[1].Rows[0]["GroundTitle"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["GroundTitle"]) },
                                    { "@Justification", ds.Tables[1].Rows[0]["Justification"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["Justification"]) },
                                    { "@EmailId", EmailId },
                                    { "@Query", PostData.SeekQuery.Trim() },
                                };
                                foreach (KeyValuePair<string, string> entry in keyword)
                                {
                                    LetterText = LetterText.Replace(entry.Key.ToString(), entry.Value.ToString());
                                }
                                new SendSMS().SendMailToUser(LetterText, EmailSubject, EmailId, "Grievance Appellate Committee", UserSession.UserID);
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), "Additional input request sent to intermediary" + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        //-------------------------------------------------------------------------------------------------------------------------------//

        [CheckSessions]
        public ActionResult GetExpertOpinonForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACAppealExpertOpinion Action = new GACAppealExpertOpinion { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId, ExpertUserList = GACAppealExpertOpinion.GACExpertUser.GetExpertUserList(UserSession.GACID) };
            return PartialView("Forms/_ExpertOpinon", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACExpertOpinonInput(GACAppealExpertOpinion PostData)
        {
            TempData["Status"] = "F";
            if (!string.IsNullOrWhiteSpace(PostData.ExpertUserId)) { ModelState.Remove("ExpertName"); ModelState.Remove("ExpertEmail"); ModelState.Remove("ExpertMobile"); ModelState.Remove("AreaOfExpertise"); } else { ModelState.Remove("ExpertUserId"); }
            if (ModelState.IsValid)
            {
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@ExpertUserId", PostData.ExpertUserId),
                    new KeyValuePair<string, string>("@ExpertName", Trim(PostData.ExpertName)),
                    new KeyValuePair<string, string>("@ExpertEmail", Trim(PostData.ExpertEmail)),
                    new KeyValuePair<string, string>("@ExpertMobile", Trim(PostData.ExpertMobile)),
                    new KeyValuePair<string, string>("@AreaOfExpertise", Trim(PostData.AreaOfExpertise)),
                    new KeyValuePair<string, string>("@Remarks", PostData.SeekRemarks.Trim()),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "ACTION_EXPERT")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", methodparameter);
                if (ds != null && ds.Tables.Count > 1)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                string InputRequestNo = Convert.ToString(ds.Tables[0].Rows[0]["InputRequestNo"]);
                                string EmailSubject = "Grievance Appellate Committee - " + Convert.ToString(ds.Tables[0].Rows[0]["EmailSubject"]);
                                string LetterText = Convert.ToString(ds.Tables[0].Rows[0]["LetterText"]);
                                string ExpertEmail = Convert.ToString(ds.Tables[0].Rows[0]["ExpertEmail"]);
                                Dictionary<string, string> keyword = new Dictionary<string, string>
                                {
                                    { "@AppealID", ds.Tables[1].Rows[0]["AppealID"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["AppealID"]) },
                                    { "@AppealDate", ds.Tables[1].Rows[0]["ReceiptDate"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["ReceiptDate"]) },
                                    { "@AppellantName", ds.Tables[1].Rows[0]["AppellantName"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["AppellantName"]) },
                                    { "@Ground", ds.Tables[1].Rows[0]["GroundTitle"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["GroundTitle"]) },
                                    { "@Justification", ds.Tables[1].Rows[0]["Justification"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["Justification"]) },
                                    { "@TargetDate", ds.Tables[1].Rows[0]["TargetDate"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["TargetDate"]) }
                                };
                                foreach (KeyValuePair<string, string> entry in keyword)
                                {
                                    LetterText = LetterText.Replace(entry.Key.ToString(), entry.Value.ToString());
                                }
                                new SendSMS().SendMailToUser(LetterText, EmailSubject, ExpertEmail, "Grievance Appellate Committee", UserSession.UserID);
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), "Opinion request has been sent to the Subject Expert (Request No. : <span class='fw-semibold'>" + InputRequestNo + "</span>) on Email ID : <span class='fw-semibold'>" + ExpertEmail + "</span>.");
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        //-------------------------------------------------------------------------------------------------------------------------------//

        [CheckSessions]
        public ActionResult GetDisposeAppealForm_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACAppealDispose Action = new GACAppealDispose { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            return PartialView("Forms/_Disposal", Action);
        }

        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GACDisposeAppeal(GACAppealDispose PostData)
        {
            TempData["Status"] = "F";
            if (ModelState.IsValid)
            {
                string DirPath = PostData.RegistrationYear + PostData.GrievanceId + "/" + GACGridViewInbox.Dir_FinalDecision; string FileNm = GACGridViewInbox.Dir_FinalDecision + ".pdf";
                var methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", PostData.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", PostData.GrievanceId),
                    new KeyValuePair<string, string>("@UploadPDF", DirPath + "/" + FileNm),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IPAddress", CommonRepository.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "INSERT_DISPOSAL_DET")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Disposal", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                byte[] fileBytes = new byte[PostData.UploadPDF.ContentLength]; PostData.UploadPDF.InputStream.Read(fileBytes, 0, PostData.UploadPDF.ContentLength);
                                FTPHelper Ftp = new FTPHelper(); Ftp.CreateFTPDir(DirPath); Ftp.saveFTPfile(fileBytes, DirPath + "/" + FileNm);
                                string EmailSubject = "Grievance Appellate Committee - " + Convert.ToString(ds.Tables[0].Rows[0]["EmailSubject"]);
                                string EmailId = Convert.ToString(ds.Tables[1].Rows[0]["IntermediaryGROEmail"]);
                                string AppellantEmailId = Convert.ToString(ds.Tables[1].Rows[0]["AppellantEmailId"]);
                                string IntermediaryText = Convert.ToString(ds.Tables[0].Rows[0]["LetterText"]);
                                SendSMS SMS = new SendSMS();
                                Dictionary<string, string> keyword_intermediary = new Dictionary<string, string>
                                {
                                    { "@IntermediaryTitle", ds.Tables[1].Rows[0]["IntermediaryTitle"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["IntermediaryTitle"]) },
                                    { "@IntermediaryUrl", ds.Tables[1].Rows[0]["IntermediaryURL"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["IntermediaryURL"]) },
                                    { "@AppealID", ds.Tables[1].Rows[0]["AppealID"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["AppealID"]) },
                                    { "@AppealDate", ds.Tables[1].Rows[0]["ReceiptDate"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["ReceiptDate"]) },
                                    { "@DisposeDate", ds.Tables[1].Rows[0]["DisposeDate"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["DisposeDate"])  },
                                };
                                foreach (KeyValuePair<string, string> entry in keyword_intermediary)
                                {
                                    IntermediaryText = IntermediaryText.Replace(entry.Key.ToString(), entry.Value.ToString());
                                }
                                SMS.SendAttachmentMailToUser(IntermediaryText, EmailSubject, EmailId, fileBytes, "DecisionCopy", "Grievance Appellate Committee", UserSession.UserID);
                                string AppellantText = Convert.ToString(ds.Tables[0].Rows[0]["AppellantLetterText"]); ;
                                Dictionary<string, string> keyword_appellant = new Dictionary<string, string>
                                {
                                    { "@AppellantName", ds.Tables[1].Rows[0]["AppellantName"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["AppellantName"]) },
                                    { "@AppealID", ds.Tables[1].Rows[0]["AppealID"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["AppealID"]) },
                                    { "@AppealDate", ds.Tables[1].Rows[0]["ReceiptDate"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["ReceiptDate"]) },
                                    { "@DisposeDate", ds.Tables[1].Rows[0]["DisposeDate"] == DBNull.Value ? "" : Convert.ToString(ds.Tables[1].Rows[0]["DisposeDate"])  },
                                };
                                foreach (KeyValuePair<string, string> entry in keyword_appellant)
                                {
                                    AppellantText = AppellantText.Replace(entry.Key.ToString(), entry.Value.ToString());
                                }
                                SMS.SendAttachmentMailToActualUser(AppellantText, EmailSubject, AppellantEmailId, fileBytes, "DecisionCopy", "Grievance Appellate Committee", UserSession.UserID);

                                Session["GVMenuStatusCd"] = GACGridViewInbox.MenuStatus.Disposed;
                                TempData["Status"] = null; TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Success, FormLabels.GetSingleLabels("0", "1", UserSession.LangCulture, "ActionSuccessful"), FormLabels.GetSingleLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture, "DecisionCopyuploadedandsamehasbeencommunicatedtotheconcernedparties") + ": <span class='fw-600'>" + PostData.GrievanceId + "/" + PostData.RegistrationYear + "</span>).");
                                break;
                        }
                    }
                }
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + PostData.RegistrationYear + "&GrievanceId=" + PostData.GrievanceId) });
            }
            return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
        }

        [CheckSessions]
        public ActionResult GetMemberChairpersonOpinion_PV(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACDecisionbyChairperson Action = GACDecisionbyChairperson.GetDecisionbyChairperson(RegistrationYear, GrievanceId);
            return PartialView("PartialViews/_MemberChairpersonOpinon", Action);
        }


        //-------------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public JsonResult DocFileToBase64String()
        {
            string Base64DocString = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var File = System.Web.HttpContext.Current.Request.Files["UploadPDF"];
                if (File != null && File.ContentLength > 0)
                {
                    HttpPostedFileBase UploadedFile = new HttpPostedFileWrapper(File);
                    using (var binaryReader = new BinaryReader(UploadedFile.InputStream))
                    {
                        byte[] FileBytes = new byte[] { };
                        FileBytes = binaryReader.ReadBytes(UploadedFile.ContentLength);
                        Base64DocString = Convert.ToBase64String(FileBytes);
                    }
                }
            }
            var JsonResult = Json(Base64DocString, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------//

        public ActionResult IntermediaryResponseView(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            Dictionary<string, string> Params = CommonRepository.GetQSDecryptedParameters(qs);
            string RegistrationYear = Params["RegistrationYear"];
            string GrievanceId = Params["GrievanceId"];

            IntermediaryResponseDownloadPDF obj = new IntermediaryResponseDownloadPDF
            {
                IntermediaryResponseDownloadPDFResponse = new List<IntermediaryResponseDownloadPDFResponse_List>(),
                IntermediaryResponseDownloadPDFsubResponse = new List<IntermediaryResponseDownloadPDFsubResponse_List>(),
                IntermediaryResponseDownloadPDFResponseFiles = new List<IntermediaryResponseDownloadPDFResponseFiles_List>(),
                IntermediaryResponseDownloadPDFsubResponseFiles = new List<IntermediaryResponseDownloadPDFsubResponseFiles_List>()
            };

            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    methodparameter.Add(new KeyValuePair<string, string>("@GrievanceId", GrievanceId));
                    methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[IntermediaryResponse_getData]", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.IntermediaryResponseDownloadPDFResponse.Add(new IntermediaryResponseDownloadPDFResponse_List
                            {
                                GrievanceID = Convert.ToString(dr["GrievanceID"]),
                                RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                                ResponseDetails = Convert.ToString(dr["ResponseDetails"]),
                                ResponseID = Convert.ToString(dr["ResponseID"]),
                                ResponseText = Convert.ToString(dr["ResponseText"]),
                                ResponseValue = Convert.ToString(dr["ResponseValue"]),
                                ResponseUrls = Convert.ToString(dr["ResponseUrls"])

                            });
                        }

                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            obj.IntermediaryResponseDownloadPDFsubResponse.Add(new IntermediaryResponseDownloadPDFsubResponse_List
                            {
                                GrievanceID = Convert.ToString(dr["GrievanceID"]),
                                RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                                SubResponseDetails = Convert.ToString(dr["SubResponseDetails"]),
                                ResponseID = Convert.ToString(dr["ResponseID"]),
                                SubResponseID = Convert.ToString(dr["SubResponseID"]),
                                SubResponseText = Convert.ToString(dr["SubResponseText"])

                            });
                        }

                    }
                    //if (ds.Tables[2].Rows.Count > 0)
                    //{
                    //    foreach (DataRow dr in ds.Tables[2].Rows)
                    //    {
                    //        obj.IntermediaryResponseDownloadPDFResponseFiles.Add(new IntermediaryResponseDownloadPDFResponseFiles_List
                    //        {
                    //            GrievanceID = Convert.ToString(dr["GrievanceID"]),
                    //            RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                    //            File = Convert.ToByte(dr["File"]),
                    //            ResponseID = Convert.ToString(dr["ResponseID"]),
                    //            FileID = Convert.ToString(dr["FileID"])
                    //        });
                    //    }

                    //}
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[2].Rows)
                        {
                            obj.IntermediaryResponseDownloadPDFsubResponseFiles.Add(new IntermediaryResponseDownloadPDFsubResponseFiles_List
                            {
                                GrievanceID = Convert.ToString(dr["GrievanceID"]),
                                RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                                FileID = Convert.ToString(dr["FileID"]),
                                ResponseID = Convert.ToString(dr["ResponseID"]),
                                File = Convert.ToString(dr["File"]),
                                //File = dr["File"] == DBNull.Value ? null : (byte[])dr["File"],
                                SubResponseID = Convert.ToString(dr["SubResponseID"])

                            });
                        }

                    }
                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        obj.GrievanceId = Convert.ToString(ds.Tables[3].Rows[0]["GrievanceID"]);
                        obj.RegistrationYear = Convert.ToString(ds.Tables[3].Rows[0]["RegistrationYear"]);
                    }
                }

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return PartialView("PartialViews/_IntermediaryReponseView", obj);
        }

        [AllowAnonymous]
        public ActionResult ViewAppealDocument(string qs)
        {
            if (Request.UrlReferrer != null)
            {
                string FilePath = string.Empty; string FileType = string.Empty; var Params = QueryString.GetDecryptedParameters(qs); try { FilePath = Params["FilePath"]; FileType = Params["FileType"]; } catch { }
                string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
                string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
                string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];
                FilePath = AESCryptography.DecryptAES(FilePath);
                FileType = AESCryptography.DecryptAES(FileType);
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + FilePath);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.EnableSsl = false;
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Stream ftpStream = response.GetResponseStream();
                    string ft = FileType.Replace(".", "").ToLower().ToString();
                    string contentType = string.Empty;
                    if (ft == "pdf")
                    {
                        contentType = "application/" + ft;
                    }
                    else if (ft == "jpg" || ft == "jpeg" || ft == "png")
                    {
                        contentType = "image/" + ft;
                    }
                    string fileNameDisplayedToUser = FilePath.Split('/')[2];
                    byte[] bytes = new byte[] { };
                    using (MemoryStream stream = new MemoryStream())
                    {
                        response.GetResponseStream().CopyTo(stream);
                        bytes = stream.ToArray();
                    }
                    ftpStream.Close();
                    return File(bytes, contentType);
                }
                catch (WebException ex) { }

            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            return new EmptyResult();

        }

        [CheckSessions]
        public ActionResult DecisionLog_List()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            DecisionLog List = DecisionLog.GetDelayResponsesData();
            return View("DecisionLog_List", List);
        }
        [CheckSessions]
        public JsonResult DeleteDraftDecisionLog(string qs)
        {
            try
            {
                string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string UserID = string.Empty;
                var Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; UserID = Params["UserID"]; } catch { }
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                    new KeyValuePair<string, string>("@UserID", UserID),
                    new KeyValuePair<string, string>("@Mode", "Action"),
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GAC_GridViewInbox_DecisionLogList]", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = "S";
                                break;
                            default:
                                TempData["Status"] = "F";
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return Json(new { Status = TempData["Status"] }, JsonRequestBehavior.AllowGet);
        }
        [CheckSessions]
        public JsonResult DeleteNotification(string qs)
        {
            try
            {
                string NotificationId = string.Empty; string IsSingle = string.Empty;
                var Params = QueryString.GetDecryptedParameters(qs); try { NotificationId = Params["NotificationId"]; IsSingle = Params["IsSingle"]; } catch { }
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@NotificationId", NotificationId),
                    new KeyValuePair<string, string>("@IsSingle", IsSingle),
                    new KeyValuePair<string, string>("@UserID", UserSession.UserID),
                    new KeyValuePair<string, string>("@Mode", "DELETE"),
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[DeleteNotification]", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = "S";
                                break;
                            case "A":
                                TempData["Status"] = "A";
                                break;
                            default:
                                TempData["Status"] = "F";
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return Json(new { Status = TempData["Status"] }, JsonRequestBehavior.AllowGet);
        }

    }
}