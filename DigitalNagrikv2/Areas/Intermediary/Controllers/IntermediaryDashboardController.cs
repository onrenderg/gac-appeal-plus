using ClosedXML.Excel;
using DigitalNagrik.Areas.Intermediary.Data;
using DigitalNagrik.Areas.Public.Models;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using DigitalNagrik.Validators;
using NICServiceAdaptor;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon;

namespace DigitalNagrik.Areas.Intermediary.Controllers
{
    public class IntermediaryDashboardController : Controller
    {
        // GET: Dashboard/AdminDashboard
        [CheckSessions]
        public ActionResult Index()
        {
            if (UserSession.RoleID.Equals("98"))
            //
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
                List<SelectListItem> Notifications = DigitalNagrik.Areas.GridView.Data.GACGridViewInbox.GetNotifications(UserSession.UserID, UserSession.GACID);
                ViewBag.Notifications = Notifications;
                return View();
            }
            else
            {
                //RedirectToAction("Error", "Home", new { Area = "" });
                return RedirectToAction("Error", "Home", new { Area = "" });
            }
        }
        [CheckSessions]
        public ActionResult Statistics_PV()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            IntermediaryDash_Stats Data = IntermediaryDash_Stats.GetData(UserSession.IntermediaryId);
            return View("PartialViews/_Stats", Data);
        }
        [CheckSessions]
        public ActionResult ListofAppeals(string ListType = "PENDING")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            Intermediary_AppealList Data = Intermediary_AppealList.GetData(UserSession.IntermediaryId, ListType, (Dictionary<string, string>)ViewData["LabelDictionary"]);
            if (ListType == "QUERY" || ListType == "QUERY_Y" || ListType == "QUERY_N")
            {
                return View("PartialViews/_AdditionalInputSought", Data);
            }
            else if (ListType == "COMPLIANCE")
            {
                return View("PartialViews/_Compliance", Data);
            }
            else
            {
                return View("PartialViews/_ListofAppeals", Data);
            }

        }

        //------------------------------Compliance---------------------------------------//
        [CheckSessions]
        public ActionResult OpenComplianceDetails(string GrievanceId, string RegistrationYear, string HeaderText = "")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            AddCompliance obj = new AddCompliance
            {
                GrievanceId = GrievanceId,
                RegistrationYear = RegistrationYear,
                HeaderText = HeaderText
            };
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", RegistrationYear),
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GetComplianceDetails]", SP_Parameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.ComplianceDate = ds.Tables[0].Rows[0]["ComplianceDate"].ToString();
                        obj.URL = ds.Tables[0].Rows[0]["URL"].ToString();
                        obj.FilePath = ds.Tables[0].Rows[0]["FilePath"].ToString();
                        obj.GrievanceDesc = ds.Tables[0].Rows[0]["GrievanceDesc"].ToString();
                        obj.GrievanceId = ds.Tables[0].Rows[0]["GrievanceId"].ToString();
                        obj.GroundTitle = ds.Tables[0].Rows[0]["GroundTitle"].ToString();
                        obj.isSupportingDocument = ds.Tables[0].Rows[0]["isSupportingDocument"].ToString();
                        obj.ReceiptDate = ds.Tables[0].Rows[0]["ReceiptDate"].ToString();
                        obj.RegistrationYear = ds.Tables[0].Rows[0]["RegistrationYear"].ToString();
                        obj.ReliefTitle = ds.Tables[0].Rows[0]["ReliefTitle"].ToString();
                        obj.RelieftSoughtSpecification = ds.Tables[0].Rows[0]["RelieftSoughtSpecification"].ToString();
                        obj.Remarks = ds.Tables[0].Rows[0]["Remarks"].ToString();
                        obj.BriefofComplaint = ds.Tables[0].Rows[0]["BriefofComplaint"].ToString();
                    }

                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return PartialView("PartialViews/_AddCompliance", obj);
        }
        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCompliance(AddCompliance obj)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    TempData["Status"] = "F";
                    string DirPath = string.Empty;
                    string FileNm = string.Empty;
                    string FinalPath = string.Empty;
                    if (obj.SupportingDocument != null)
                    {
                        if (!ValidateFileAttribute.IsFileTypeValid(obj.SupportingDocument))
                        {
                            TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Error, "Failed", "The file you have uploaded is invalid. Please upload a valid PDF File.");
                            return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                        }
                        else
                        {
                            DirPath = obj.RegistrationYear + obj.GrievanceId + "/" + GridView.Data.GACGridViewInbox.Dir_IntermediaryCompliance; FileNm = GridView.Data.GACGridViewInbox.Dir_IntermediaryCompliance + ".pdf";
                            FinalPath = DirPath + "/" + FileNm;
                        }
                    }

                    List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
                {
                     new KeyValuePair<string, string>("@GrievanceID", obj.GrievanceId),
                     new KeyValuePair<string, string>("@RegistrationYear", obj.RegistrationYear),
                     new KeyValuePair<string, string>("@ComplianceDate", obj.ComplianceDate),
                     new KeyValuePair<string, string>("@Remarks", obj.Remarks),
                     new KeyValuePair<string, string>("@URL", obj.URL),
                     new KeyValuePair<string, string>("@SupportingDocument", FinalPath),
                        new KeyValuePair<string, string>("@CreatedBy", UserSession.UserID),
                        new KeyValuePair<string, string>("@CreatedByIp", BALCommon.GetIPAddress()),

                };
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_SaveCompliance]", SP_Parameter);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                            if (!string.IsNullOrWhiteSpace(res))
                            {
                                switch (res.Trim().ToUpper())
                                {
                                    case "S":
                                        if (obj.SupportingDocument != null)
                                        {
                                            byte[] fileBytes = new byte[obj.SupportingDocument.ContentLength]; obj.SupportingDocument.InputStream.Read(fileBytes, 0, obj.SupportingDocument.ContentLength);
                                            FTPHelper Ftp = new FTPHelper(); Ftp.CreateFTPDir(DirPath); Ftp.saveFTPfile(fileBytes, DirPath + "/" + FileNm);
                                        }
                                        TempData["Status"] = null;
                                        TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Respondent Intermediary Compliance saved successfully.", "Compliance added successfully");

                                        break;
                                    default:
                                        TempData["Status"] = null;
                                        break;
                                }
                            }
                        }
                    }
                    //return RedirectToAction("IntermediaryReply", "Intermediary", new { Area = "Intermediary", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + obj.RegistrationYear + "&GrievanceId=" + obj.GrievanceId + "&InputId=" + obj.InputID) });
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
        }
        public ActionResult CompliancePDF(string qs)
        {
            try
            {
                if (qs != "")
                {
                    byte[] DocumentView = new byte[] { };
                    string RegistrationYear = string.Empty; string GrievanceId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
                    List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", RegistrationYear)
            };
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_Compliance_Doc]", SP_Parameter);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DocumentView = ds.Tables[0].Rows[0]["InputReplyFile"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["InputReplyFile"];
                            }
                        }
                    }
                    //return View(obj);
                    return File(DocumentView, "application/pdf");
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return new EmptyResult();
        }
        //------------------------------Compliance---------------------------------------//
        //------------------------------Add New Intermediary-----------------------------//
        [CheckSessions]
        public ActionResult AddNewIntermediary()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterIntermediary, UserSession.LangCulture);
            Intermediarys obj = new Intermediarys();
            obj.List = new List<IntermediaryList>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    //methodparameter.Add(new KeyValuePair<string, string>("@GrievanceId", GrievanceId));
                    //methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_GetIntermediaryList]", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.List.Add(new IntermediaryList
                            {
                                Address = Convert.ToString(dr["Address"]),
                                GOEmail = Convert.ToString(dr["GOEmail"]),
                                GOName = Convert.ToString(dr["GOName"]),
                                HelpLink = Convert.ToString(dr["HelpLink"]),
                                IntermediaryId = Convert.ToString(dr["IntermediaryId"]),
                                IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                                IntermediaryType = Convert.ToString(dr["IntermediaryType"]),
                                IsActive = Convert.ToString(dr["IsActive"]),
                                URL = Convert.ToString(dr["URL"]),
                                LoginEmail = Convert.ToString(dr["LoginEmail"]),
                            });
                        }

                    }
                }
            }

            catch (Exception ex)
            {

            }
            return View("AddNewIntermediary", obj);
        }
        [CheckSessions]
        public ActionResult AddUpdateIntermediaryShowPopUp(string qs)
        {
            Dictionary<string, string> LabelDictionary = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterIntermediary, UserSession.LangCulture);
            IntermediaryList obj = new IntermediaryList();
            string IntermediaryId = string.Empty; string Type = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { IntermediaryId = Params["IntermediaryId"]; Type = Params["Type"]; } catch { }
            int count = IntermediaryId.Split(',').Length;
            if (count > 1)
            {
                obj = AddUpdateIntermediaryShowPopUpData(IntermediaryId, "Y");
            }
            else
            {
                obj = AddUpdateIntermediaryShowPopUpData(IntermediaryId);
            }

            obj.Type = Type;
            if (obj.Type == "V")
            {
                obj.HeaderText = LabelDictionary["VerifyRespondentIntermediaryDetails"];
            }
            else if (obj.Type == "E")
            {
                obj.HeaderText = LabelDictionary["UpdateRespondentIntermediaryDetails"];
            }
            else
            {
                obj.HeaderText = LabelDictionary["Add/UpdateRespondentIntermediary"];
            }
            return PartialView("PartialViews/_AddUpdateIntermediary", obj);
        }
        [CheckSessions]
        public JsonResult AddUpdateIntermediaryShowPopUpDataJson(string IntermediaryID)
        {
            return Json(new { Data = AddUpdateIntermediaryShowPopUpData(IntermediaryID, "Y") }, JsonRequestBehavior.AllowGet);
        }
        [CheckSessions]
        public IntermediaryList AddUpdateIntermediaryShowPopUpData(string IntermediaryID, string MultipleIntemediaries = "")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterIntermediary, UserSession.LangCulture);
            IntermediaryList obj = new IntermediaryList();
            if (MultipleIntemediaries == "Y")
            {
                obj.MultipleIntemediaries = "Y";
            }
            obj.IntermediariesList = new List<SelectListItem>();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@IntermediaryID", IntermediaryID));

                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_GetIntermediaryList", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.IntermediaryId = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryId"]);
                        obj.IntermediaryTitle = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryTitle"]);
                        obj.Address = Convert.ToString(ds.Tables[0].Rows[0]["Address"]);
                        obj.GOEmail = Convert.ToString(ds.Tables[0].Rows[0]["GOEmail"]);
                        obj.GOName = Convert.ToString(ds.Tables[0].Rows[0]["GOName"]);
                        obj.HelpLink = Convert.ToString(ds.Tables[0].Rows[0]["HelpLink"]);
                        obj.IntermediaryType = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryType"]);
                        obj.IsActive = Convert.ToString(ds.Tables[0].Rows[0]["IsActive"]);
                        obj.URL = Convert.ToString(ds.Tables[0].Rows[0]["URL"]);
                        obj.LoginEmail = Convert.ToString(ds.Tables[0].Rows[0]["LoginEmail"]);
                    }
                }
                if (ds.Tables[1].Rows.Count > 1)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        obj.IntermediariesList.Add(new SelectListItem { Value = Convert.ToString(dr["IntermediaryId"]), Text = Convert.ToString(dr["IntermediaryTitle"]), });
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        [CheckSessions]
        public JsonResult ChangeStatusisActive(string IntermediaryID, string Command)
        {
            try
            {
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@IntermediaryID", IntermediaryID),
                    new KeyValuePair<string, string>("@Command", Command)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_Commands", methodparameter);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AntiFogeryToken]
        public ActionResult SaveIntermediary(IntermediaryList obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    var methodparameter = new List<KeyValuePair<string, string>>();
                    methodparameter.Add(new KeyValuePair<string, string>("@IntermediaryTitle", obj.IntermediaryTitle));
                    if (obj.IsActive != null && obj.IsActive == "on")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "Y"));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "N"));
                    }
                    if (obj.Type != null && obj.Type != "")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@IsVerified", obj.Type));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@URL", obj.URL));
                    methodparameter.Add(new KeyValuePair<string, string>("@GOName", obj.GOName));
                    methodparameter.Add(new KeyValuePair<string, string>("@GOEmail", obj.GOEmail));
                    methodparameter.Add(new KeyValuePair<string, string>("@LoginEmail", obj.LoginEmail));
                    methodparameter.Add(new KeyValuePair<string, string>("@HelpLink", obj.HelpLink));
                    methodparameter.Add(new KeyValuePair<string, string>("@Address", obj.Address));
                    if (obj.IntermediaryId != null && obj.IntermediaryId != "" && obj.IntermediaryId != "0")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "U"));
                        methodparameter.Add(new KeyValuePair<string, string>("@IntermediaryId", obj.IntermediaryId));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "I"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@UserID", UserSession.UserID));
                    methodparameter.Add(new KeyValuePair<string, string>("@IpAddress", BALCommon.GetIPAddress()));
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_SaveIntermediary]", methodparameter);
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
                                case "E":
                                    TempData["Status"] = "O";
                                    TempData["Status_Type"] = BALCommon.CustomClientAlerts.AlertTypeFormat.Error;
                                    TempData["Status_Msg"] = ds.Tables[0].Rows[0]["msg"];
                                    break;
                                default:
                                    TempData["Status"] = "F";
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    TempData["Status"] = "O";
                    TempData["Status_Type"] = BALCommon.CustomClientAlerts.AlertTypeFormat.Error;
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["Status_Msg"] = message.ToString();
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            if (obj.Type == "" || obj.Type == null)
            {
                return RedirectToAction("AddNewIntermediary");
            }
            else
            {
                return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
            }

        }
        //------------------------------Add New Intermediary-----------------------------//

        //------------------------------Add Add Relief Sought-----------------------------//
        [CheckSessions]
        public ActionResult AddNewReliefSought()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterReliefSought, UserSession.LangCulture);
            ReliefSoughts obj = new ReliefSoughts();
            obj.List = new List<ReliefSoughtList>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    //methodparameter.Add(new KeyValuePair<string, string>("@GrievanceId", GrievanceId));
                    //methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_GetReliefSoughtList", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.List.Add(new ReliefSoughtList
                            {
                                ReliefId = Convert.ToString(dr["ReliefId"]),
                                ReliefTitle = Convert.ToString(dr["ReliefTitle"]),
                                SpecificationLabel = Convert.ToString(dr["SpecificationLabel"]),
                                IsActive = Convert.ToString(dr["IsActive"])
                            });
                        }

                    }
                }
            }

            catch (Exception ex)
            {

            }
            return View("AddNewReliefSought", obj);
        }
        [CheckSessions]
        public ActionResult AddUpdateReliefSoughtShowPopUp(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterReliefSought, UserSession.LangCulture);
            ReliefSoughtList obj = new ReliefSoughtList();
            string ReliefId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { ReliefId = Params["ReliefId"]; } catch { }
            obj = AddUpdateReliefSoughtShowPopUpData(ReliefId);
            return PartialView("PartialViews/_AddUpdateReliefSought", obj);
        }
        [CheckSessions]
        public ReliefSoughtList AddUpdateReliefSoughtShowPopUpData(string ReliefId)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterReliefSought, UserSession.LangCulture);
            ReliefSoughtList obj = new ReliefSoughtList();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@ReliefId", ReliefId));
                //if (UserSession.MemberID != null || UserSession.MemberID != "")
                //{
                //    methodparameter.Add(new KeyValuePair<string, string>("@MemberID", UserSession.UserID));
                //}
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_GetReliefSoughtList", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.ReliefId = Convert.ToString(ds.Tables[0].Rows[0]["ReliefId"]);
                        obj.ReliefTitle = Convert.ToString(ds.Tables[0].Rows[0]["ReliefTitle"]);
                        obj.SpecificationLabel = Convert.ToString(ds.Tables[0].Rows[0]["SpecificationLabel"]);
                        obj.IsActive = Convert.ToString(ds.Tables[0].Rows[0]["IsActive"]);
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        [CheckSessions]
        public JsonResult ChangeStatusisActiveRS(string ReliefID, string Command)
        {
            try
            {
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@ReliefID", ReliefID),
                    new KeyValuePair<string, string>("@Command", Command)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "ReliefSought_Commands", methodparameter);
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
        [HttpPost]
        public ActionResult SaveReliefSought(ReliefSoughtList obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    var methodparameter = new List<KeyValuePair<string, string>>();
                    methodparameter.Add(new KeyValuePair<string, string>("@ReliefTitle", obj.ReliefTitle));
                    if (obj.IsActive != null && obj.IsActive == "on")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "Y"));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "N"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@SpecificationLabel", obj.SpecificationLabel));
                    if (obj.ReliefId != null && obj.ReliefId != "" && obj.ReliefId != "0")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "U"));
                        methodparameter.Add(new KeyValuePair<string, string>("@ReliefId", obj.ReliefId));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "I"));
                    }

                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[ReliefSought_SaveReliefSought]", methodparameter);
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
                                case "E":
                                    TempData["Status"] = "O";
                                    TempData["Status_Type"] = BALCommon.CustomClientAlerts.AlertTypeFormat.Error;
                                    TempData["Status_Msg"] = ds.Tables[0].Rows[0]["msg"];
                                    break;
                                default:
                                    TempData["Status"] = "F";
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("AddNewReliefSought");
        }
        //------------------------------Add Relief Sought-----------------------------//
        //------------------------------Add Ground of Appeal-----------------------------//
        [CheckSessions]
        public ActionResult AddNewGroundforAppeal()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterGroundforAppeal, UserSession.LangCulture);
            GroundforAppeals obj = new GroundforAppeals();
            obj.List = new List<GroundforAppealsList>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    //methodparameter.Add(new KeyValuePair<string, string>("@GrievanceId", GrievanceId));
                    //methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_GetGroundforAppealList]", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.List.Add(new GroundforAppealsList
                            {
                                GroundId = Convert.ToString(dr["GroundId"]),
                                CorrespondingITRule = Convert.ToString(dr["CorrespondingITRule"]),
                                EntryFieldLabel = Convert.ToString(dr["EntryFieldLabel"]),
                                EntryRequired = Convert.ToString(dr["EntryRequired"]),
                                GACId = Convert.ToString(dr["GACId"]),
                                GACName = Convert.ToString(dr["GACName"]),
                                IsActive = Convert.ToString(dr["IsActive"]),
                                GroundTitle = Convert.ToString(dr["GroundTitle"]),
                                IntermediaryResponseNeeded = Convert.ToString(dr["IntermediaryResponseNeeded"]),
                                ITRuleExtract = Convert.ToString(dr["ITRuleExtract"]),
                                ListOrder = Convert.ToString(dr["ListOrder"]),
                                Grounds = Convert.ToString(dr["Grounds"]),
                            });
                        }

                    }
                }
            }

            catch (Exception ex)
            {

            }
            return View("AddNewGroundforAppeal", obj);
        }
        [CheckSessions]
        public ActionResult AddUpdateGroundforAppealShowPopUp(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterGroundforAppeal, UserSession.LangCulture);
            GroundforAppealsList obj = new GroundforAppealsList();
            string GroundId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { GroundId = Params["GroundId"]; } catch { }
            obj = AddUpdateGroundforAppealShowPopUpData(GroundId);
            return PartialView("PartialViews/_AddUpdateGroundforAppeal", obj);
        }
        [CheckSessions]
        public GroundforAppealsList AddUpdateGroundforAppealShowPopUpData(string GroundId)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterGroundforAppeal, UserSession.LangCulture);
            GroundforAppealsList obj = new GroundforAppealsList();
            obj.GACList = new List<SelectListItem>();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@GroundId", GroundId));
                //if (UserSession.MemberID != null || UserSession.MemberID != "")
                //{
                //    methodparameter.Add(new KeyValuePair<string, string>("@MemberID", UserSession.UserID));
                //}
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_GetGroundforAppealList", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.ListOrder = Convert.ToString(ds.Tables[0].Rows[0]["ListOrder"]);
                        obj.CorrespondingITRule = Convert.ToString(ds.Tables[0].Rows[0]["CorrespondingITRule"]);
                        obj.EntryRequired = Convert.ToString(ds.Tables[0].Rows[0]["EntryRequired"]);
                        obj.EntryFieldLabel = Convert.ToString(ds.Tables[0].Rows[0]["EntryFieldLabel"]);
                        obj.GACId = Convert.ToString(ds.Tables[0].Rows[0]["GACId"]);
                        obj.GACName = Convert.ToString(ds.Tables[0].Rows[0]["GACName"]);
                        obj.GroundId = Convert.ToString(ds.Tables[0].Rows[0]["GroundId"]);
                        obj.GroundTitle = Convert.ToString(ds.Tables[0].Rows[0]["GroundTitle"]);
                        obj.IntermediaryResponseNeeded = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryResponseNeeded"]);
                        obj.IsActive = Convert.ToString(ds.Tables[0].Rows[0]["IsActive"]);
                        obj.ITRuleExtract = Convert.ToString(ds.Tables[0].Rows[0]["ITRuleExtract"]);
                        obj.Grounds = Convert.ToString(ds.Tables[0].Rows[0]["Grounds"]);
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            obj.GACList.Add(new SelectListItem { Value = Convert.ToString(dr["GACId"]), Text = Convert.ToString(dr["GACTitle"]), });
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        [CheckSessions]
        public JsonResult ChangeStatusisActiveGA(string ColumnName, int GroundID, string Command)
        {
            try
            {
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                     new KeyValuePair<string, string>("@ColumnName", ColumnName),
                    new KeyValuePair<string, string>("@GroundID", GroundID.ToString()),
                    new KeyValuePair<string, string>("@Command", Command)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GroundforAppeal_Commands]", methodparameter);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveGroundforAppeal(GroundforAppealsList obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    var methodparameter = new List<KeyValuePair<string, string>>();
                    methodparameter.Add(new KeyValuePair<string, string>("@GroundTitle", obj.GroundTitle));
                    if (obj.IsActive != null && obj.IsActive == "on")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "Y"));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "N"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@CorrespondingITRule", obj.CorrespondingITRule));
                    methodparameter.Add(new KeyValuePair<string, string>("@EntryFieldLabel", obj.EntryFieldLabel));
                    if (obj.EntryRequired != null && obj.EntryRequired == "on")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@EntryRequired", "Y"));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@EntryRequired", "N"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@GACId", obj.GACId));
                    if (obj.IntermediaryResponseNeeded != null && obj.IntermediaryResponseNeeded == "on")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@IntermediaryResponseNeeded", "Y"));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@IntermediaryResponseNeeded", "N"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@ITRuleExtract", obj.ITRuleExtract));
                    methodparameter.Add(new KeyValuePair<string, string>("@Grounds", obj.Grounds));
                    methodparameter.Add(new KeyValuePair<string, string>("@ListOrder", obj.ListOrder));
                    if (obj.GroundId != null && obj.GroundId != "" && obj.GroundId != "0")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "U"));
                        methodparameter.Add(new KeyValuePair<string, string>("@GroundId", obj.GroundId));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "I"));
                    }

                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GroundForAppeal_SaveGroundforAppeal]", methodparameter);
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
                                case "E":
                                    TempData["Status"] = "O";
                                    TempData["Status_Type"] = BALCommon.CustomClientAlerts.AlertTypeFormat.Error;
                                    TempData["Status_Msg"] = ds.Tables[0].Rows[0]["msg"];
                                    break;
                                default:
                                    TempData["Status"] = "F";
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("AddNewGroundforAppeal");
        }
        //------------------------------Add Ground of Appeal-----------------------------//

        //------------------------------Download Intermediary PDF-------------------------------------//
        public ActionResult IntermediaryDownloadPDF()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterIntermediary, UserSession.LangCulture);
            Intermediarys obj = new Intermediarys();
            obj.List = new List<IntermediaryList>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_GetIntermediaryList]", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.List.Add(new IntermediaryList
                            {
                                Address = Convert.ToString(dr["Address"]),
                                GOEmail = Convert.ToString(dr["GOEmail"]),
                                GOName = Convert.ToString(dr["GOName"]),
                                HelpLink = Convert.ToString(dr["HelpLink"]),
                                IntermediaryId = Convert.ToString(dr["IntermediaryId"]),
                                IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                                IntermediaryType = Convert.ToString(dr["IntermediaryType"]),
                                IsActive = Convert.ToString(dr["IsActive"]),
                                URL = Convert.ToString(dr["URL"]),
                                LoginEmail = Convert.ToString(dr["LoginEmail"]),
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            ViewAsPdf PDF = new ViewAsPdf("Download/_DownloadPDFIntermediary", obj)
            {
                FileName = "DownloadPDFIntermediary.pdf",
                PageMargins = { Left = 8, Right = 8 },
                CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"9\" --footer-left \"GAC\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \"",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
            byte[] PDFBytes = PDF.BuildPdf(ControllerContext);
            return File(PDFBytes, "application/pdf");
        }
        //------------------------------Download Intermediary PDF-------------------------------------//
        //------------------------------Download Intermediary Excel-------------------------------------//
        public ActionResult IntermediaryDownloadExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                }
                ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_GetIntermediaryList]", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string[] DBColumnNames = new[] { "IntermediaryTitle", "LoginEmail", "URL", "HelpLink", "Address", "IntermediaryType", "GOName", "GOEmail", "IsActive" };
                    string[] NewColumnNames = new[] { "Intermediary Title", "Login Email", "URL", "HelpLink", "Address", "Intermediary Type", "Grievance Officer Name", "Grievance Officer Email", "IsActive" };
                    string[] ColumnsToRemoveNew = new[] { "IntermediaryId" };

                    // Dim ColumnsToRemoveNew As String() = {}
                    System.Data.DataTable dtDataTable1 = BALCommon.UpdateExcelDataTable(ds.Tables[0], DBColumnNames, NewColumnNames, ColumnsToRemoveNew);

                    using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("IntermediaryList");
                        ws.Cell(1, 1).Value = "Intermediary List";
                        ws.Range(1, 1, 1, 8).Merge().AddToNamed("Titles");
                        ws.Protect("123456");
                        var tableWithData = ws.Cell(2, 1).InsertTable(dtDataTable1.AsEnumerable());

                        var titlesStyle = wb.Style;
                        titlesStyle.Font.Bold = true;
                        titlesStyle.Font.FontSize = 14;
                        titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        titlesStyle.Fill.BackgroundColor = XLColor.Navy;
                        titlesStyle.Font.FontColor = XLColor.White;
                        wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;


                        ws.Columns().AdjustToContents();
                        wb.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        wb.Style.Font.Bold = true;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=IntermediaryList.xlsx");

                        using (System.IO.MemoryStream MyMemoryStream = new System.IO.MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            // ltMessage.Text = "<div class='col-lg-12 alert alert-success'><b>Excel file has been generated for " + Records + " records.</b></div>"
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("../ErrorPage.aspx");
            }
            return new EmptyResult();
        }

        //------------------------------Download Intermediary Excel-------------------------------------//
        //------------------------------Download Grounds PDF-------------------------------------//
        public ActionResult GroundsDownloadPDF()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterGroundforAppeal, UserSession.LangCulture);
            GroundforAppeals obj = new GroundforAppeals();
            obj.List = new List<GroundforAppealsList>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_GetGroundforAppealList]", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.List.Add(new GroundforAppealsList
                            {
                                GroundId = Convert.ToString(dr["GroundId"]),
                                CorrespondingITRule = Convert.ToString(dr["CorrespondingITRule"]),
                                EntryFieldLabel = Convert.ToString(dr["EntryFieldLabel"]),
                                EntryRequired = Convert.ToString(dr["EntryRequired"]),
                                GACId = Convert.ToString(dr["GACId"]),
                                GACName = Convert.ToString(dr["GACName"]),
                                IsActive = Convert.ToString(dr["IsActive"]),
                                GroundTitle = Convert.ToString(dr["GroundTitle"]),
                                IntermediaryResponseNeeded = Convert.ToString(dr["IntermediaryResponseNeeded"]),
                                ITRuleExtract = Convert.ToString(dr["ITRuleExtract"]),
                                ListOrder = Convert.ToString(dr["ListOrder"]),
                                Grounds = Convert.ToString(dr["Grounds"]),
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            ViewAsPdf PDF = new ViewAsPdf("Download/_DownloadPDFGrounds", obj)
            {
                FileName = "DownloadPDFGrounds.pdf",
                PageMargins = { Left = 8, Right = 8 },
                CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"9\" --footer-left \"GAC\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \"",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
            byte[] PDFBytes = PDF.BuildPdf(ControllerContext);
            return File(PDFBytes, "application/pdf");
        }
        //------------------------------Download Grounds PDF-------------------------------------//
        //------------------------------Download Grounds Excel-------------------------------------//
        public ActionResult GroundsDownloadExcel()
        {
            DataSet ds = new DataSet();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                }
                ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_GetGroundforAppealList]", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string[] DBColumnNames = new[] { "GACName", "GroundTitle", "Grounds", "ITRuleExtract", "CorrespondingITRule" };
                    string[] NewColumnNames = new[] { "Default GAC", "Ground for appeal, which will be displayed on the portal for user selection", "Keywords", "Extract of the rule to which the ground is related to", "Related rule provision" };
                    string[] ColumnsToRemoveNew = new[] { "GroundId", "GACId", "IsActive", "EntryRequired", "EntryFieldLabel", "IntermediaryResponseNeeded" };

                    // Dim ColumnsToRemoveNew As String() = {}
                    System.Data.DataTable dtDataTable1 = BALCommon.UpdateExcelDataTable(ds.Tables[0], DBColumnNames, NewColumnNames, ColumnsToRemoveNew);

                    using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("GroundsList");
                        ws.Cell(1, 1).Value = "Grounds List";
                        ws.Range(1, 1, 1, 6).Merge().AddToNamed("Titles");
                        var tableWithData = ws.Cell(2, 1).InsertTable(dtDataTable1.AsEnumerable());

                        var titlesStyle = wb.Style;
                        titlesStyle.Font.Bold = true;
                        titlesStyle.Font.FontSize = 14;
                        titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        titlesStyle.Fill.BackgroundColor = XLColor.Navy;
                        titlesStyle.Font.FontColor = XLColor.White;
                        wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;


                        ws.Columns().AdjustToContents();
                        wb.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        wb.Style.Font.Bold = true;
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=GroundsList.xlsx");

                        using (System.IO.MemoryStream MyMemoryStream = new System.IO.MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            // ltMessage.Text = "<div class='col-lg-12 alert alert-success'><b>Excel file has been generated for " + Records + " records.</b></div>"
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return new EmptyResult();
        }
        //------------------------------Download Grounds Excel-------------------------------------//
        //------------------------------Extend Response Time-------------------------------------//
        [CheckSessions]
        public ActionResult ExtendResponseTime(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            ExtendResponseTimeforAppeal obj = new ExtendResponseTimeforAppeal();
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            obj = ExtendResponseTimePopUpData(RegistrationYear, GrievanceId);
            return PartialView("PartialViews/_ExtendResponseTime", obj);
        }
        [CheckSessions]
        public ExtendResponseTimeforAppeal ExtendResponseTimePopUpData(string RegistrationYear, string GrievanceId)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            ExtendResponseTimeforAppeal obj = new ExtendResponseTimeforAppeal();
            obj.ResponseTimeList = new List<SelectListItem>();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                methodparameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceId));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_GetExtendedTimeDetails", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.RegistrationYear = Convert.ToString(ds.Tables[0].Rows[0]["RegistrationYear"]);
                        obj.ApprovalStatus = Convert.ToString(ds.Tables[0].Rows[0]["ApprovalStatus"]);
                        obj.GrievanceId = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceId"]);
                        obj.Remarks = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]);
                        obj.ResponseTimeID = Convert.ToString(ds.Tables[0].Rows[0]["ResponseTimeID"]);
                        obj.ReceiptDate = Convert.ToString(ds.Tables[0].Rows[0]["ReceiptDate"]);
                        obj.ResponseTimeDesc = Convert.ToString(ds.Tables[0].Rows[0]["ResponseTimeDesc"]);
                        obj.ApprovalStatusDesc = Convert.ToString(ds.Tables[0].Rows[0]["ApprovalStatusDesc"]);
                        obj.ApprovalDateTime = Convert.ToString(ds.Tables[0].Rows[0]["ApprovalDateTime"]);
                        obj.ApprovalRemarks = Convert.ToString(ds.Tables[0].Rows[0]["ApprovalRemarks"]);
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            obj.ResponseTimeList.Add(new SelectListItem { Value = Convert.ToString(dr["ResponseTime"]), Text = Convert.ToString(dr["ResponseTimeDesc"]), });
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveExtendedTimeRequest(ExtendResponseTimeforAppeal obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    var methodparameter = new List<KeyValuePair<string, string>>();
                    methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", obj.RegistrationYear));
                    methodparameter.Add(new KeyValuePair<string, string>("@GrievanceID", obj.GrievanceId));
                    methodparameter.Add(new KeyValuePair<string, string>("@ResponseTimeID", obj.ResponseTimeID));
                    methodparameter.Add(new KeyValuePair<string, string>("@Remarks", obj.Remarks));
                    methodparameter.Add(new KeyValuePair<string, string>("@UserID", UserSession.UserID));
                    methodparameter.Add(new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()));
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_SaveExtendedTimeRequest]", methodparameter);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            switch (res.Trim().ToUpper())
                            {
                                case "S":
                                    TempData["Status"] = null;
                                    TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Request submitted successfully", "Response Time Extention Request submitted successfully.");
                                    break;
                                default:
                                    TempData["Status"] = null;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
        }
        //------------------------------Extend Response Time-------------------------------------//
        //------------------------------Approve Response Time-------------------------------------//
        [CheckSessions]
        public ActionResult IntermediaryExtensionTime_List()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            ExtendResponseTimeforAppeal_List List = ExtendResponseTimeforAppeal_List.GetData();
            return View("IntermediaryExtensionTime_List", List);
        }
        [CheckSessions]
        public ActionResult OpenApproveRejectPopup(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            ExtendResponseTimeforAppeal_List obj = new ExtendResponseTimeforAppeal_List();
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string OpenFrom = string.Empty;
            Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; OpenFrom = Params["OpenFrom"]; } catch { }
            obj = ExtendResponseTimeAcceptRejectPopUpData(RegistrationYear, GrievanceId, OpenFrom);
            return PartialView("PartialViews/_ExtendResponseTimeApproveReject", obj);
        }
        [CheckSessions]
        public ExtendResponseTimeforAppeal_List ExtendResponseTimeAcceptRejectPopUpData(string RegistrationYear, string GrievanceId, string OpenFrom)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            ExtendResponseTimeforAppeal_List obj = new ExtendResponseTimeforAppeal_List();
            obj.ResponseTimeList = new List<SelectListItem>();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                methodparameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceId));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_GetExtendedTimeDetails", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.RegistrationYear = Convert.ToString(ds.Tables[0].Rows[0]["RegistrationYear"]);
                        obj.GrievanceId = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceId"]);
                        obj.ResponseTimeID = Convert.ToString(ds.Tables[0].Rows[0]["ResponseTimeID"]);
                        obj.OpenFrom = OpenFrom;
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            obj.ResponseTimeList.Add(new SelectListItem { Value = Convert.ToString(dr["ResponseTime"]), Text = Convert.ToString(dr["ResponseTimeDesc"]), });
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        [CheckSessions]
        public ActionResult ApproveRejectTimeExtensionRequest(ExtendResponseTimeforAppeal_List obj)
        {
            try
            {
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@RegistrationYear", obj.RegistrationYear),
                    new KeyValuePair<string, string>("@GrievanceId", obj.GrievanceId),
                    new KeyValuePair<string, string>("@ApproveReject", obj.ApproveReject),
                    new KeyValuePair<string, string>("@ResponseTimeID", obj.ResponseTimeID),
                    new KeyValuePair<string, string>("@Remarks", obj.Remarks),
                    new KeyValuePair<string, string>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, string>("@IpAddress", BALCommon.GetIPAddress()),
                    new KeyValuePair<string, string>("@Mode", "Action"),
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_ExtendTimeRequestApprove]", methodparameter);
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
            if (obj.OpenFrom == "GV")
            {
                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
            }
            else
            {
                return RedirectToAction("IntermediaryExtensionTime_List", "IntermediaryDashboard", new { Area = "Intermediary" });
            }

        }
        //------------------------------Approve Response Time-------------------------------------//
        //------------------------------Change Password--------------------------------------------//
        [CheckSessions]
        public ActionResult ChangePassword()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            try
            {
                ViewBag.IsPasswordSet = "N";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserID", UserSession.UserID),
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[CheckPasswordIntermediary]", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.IsPasswordSet = Convert.ToString(ds.Tables[0].Rows[0]["IsPasswordSet"]);
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return View();
        }
        [CheckSessions]
        public JsonResult VerifyOTPforPasswordChange(string OTP)
        {
            string Result = "OtpMisMatched";
            try
            {
                var methodParameter = new List<KeyValuePair<string, string>>();
                //methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.InterEmailID.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", UserSession.UserID));
                methodParameter.Add(new KeyValuePair<string, string>("@OTPID", Convert.ToString(Session["OTPID"])));
                methodParameter.Add(new KeyValuePair<string, string>("@OTP", OTP));
                methodParameter.Add(new KeyValuePair<string, string>("@func", "Inter"));

                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CheckEmailMobileOTPAll", methodParameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["UserVerfied"].ToString() == "Y")
                    {
                        Result = "OtpMatched";
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return Json(new { Result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CheckSessions]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(ChangePasswordIntermediary obj)
        {
            try
            {
                ModelState.Remove("CurrentPassword");
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmNewPassword");
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserID", UserSession.UserID),
                    new KeyValuePair<string, string>("@CurrentPassword", obj.HashCurrentPassword),
                    new KeyValuePair<string, string>("@NewPassword", obj.HashNewPassword),
                    new KeyValuePair<string, string>("@IpAddress", BALCommon.GetIPAddress()),
                    new KeyValuePair<string, string>("@IsPasswordSet", obj.IsPasswordSet),
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "UpdatePasswordIntermediary", methodparameter);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    if (dr["Status"].ToString() == "200")
                    {
                        TempData["Status"] = null;
                        TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Successfully Updated", "Password updated successfully.");
                    }
                    else if (dr["Status"].ToString() == "400")
                    {
                        TempData["Status"] = null;
                        TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Error, "Successfully Updated", dr["Msg"].ToString());
                    }
                    else
                    {
                        TempData["Status"] = null;
                        TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Error, "Something went wrong...", dr["Msg"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return RedirectToAction("ChangePassword", "IntermediaryDashboard", new { Area = "Intermediary" });
        }
        [CheckSessions]
        public ActionResult SendOTPtoIntermediary()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            try
            {
                string Result = string.Empty;
                var methodParameter = new List<KeyValuePair<string, string>>();
                methodParameter.Clear();
                methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", UserSession.UserID));
                methodParameter.Add(new KeyValuePair<string, string>("@EmailorMobile", "E"));
                methodParameter.Add(new KeyValuePair<string, string>("@Func", "InterLogin"));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetResendOTP_new", methodParameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["OTPID"].ToString() == "0" && ds.Tables[0].Rows[0]["OTP"].ToString() == "0")
                    {
                        Result = "F";                      
                    }
                    else
                    {
                        Session["OTPID"] = ds.Tables[0].Rows[0]["OTPID"].ToString();
                        SendSMS smsObj = new SendSMS();
                        string emailSubject = "Grievance Appellate Committee : Password Change Verification OTP";
                        string emailMsg = ds.Tables[0].Rows[0]["Msg"].ToString();
                        smsObj.SendMailToUser(emailMsg, emailSubject, UserSession.UserID, "Grievance Appellate Committee ", UserSession.UserID);
                        Result = "S";
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return PartialView("PartialViews/_OTPGrid");
        }
        [CheckSessions]
        public JsonResult ResendOTP()
        {
            string Result = string.Empty;
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", UserSession.UserID));
            methodParameter.Add(new KeyValuePair<string, string>("@EmailorMobile", "E"));
            methodParameter.Add(new KeyValuePair<string, string>("@Func", "InterLogin"));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetResendOTP_new", methodParameter);
            if (ds != null && ds.Tables.Count>0)
            {
                if (ds.Tables[0].Rows[0]["OTPID"].ToString() == "0" && ds.Tables[0].Rows[0]["OTP"].ToString() == "0")
                {
                    //ModelState.AddModelError("AppellantErrorMessage", ds.Tables[0].Rows[0]["Msg"].ToString());
                    Result = "F";
                }
                else
                {
                    Result = "S";
                    Session["OTPID"] = ds.Tables[0].Rows[0]["OTPID"].ToString();
                    SendSMS smsObj = new SendSMS();
                    string emailSubject = "Grievance Appellate Committee : Password Change Verification OTP";
                    string emailMsg = ds.Tables[0].Rows[0]["Msg"].ToString();
                    smsObj.SendMailToUser(emailMsg, emailSubject, UserSession.UserID, "Grievance Appellate Committee ", UserSession.UserID);
                }
            }
            return Json(new { Result }, JsonRequestBehavior.AllowGet);
        }
        //------------------------------Change Password--------------------------------------------//

    }
}