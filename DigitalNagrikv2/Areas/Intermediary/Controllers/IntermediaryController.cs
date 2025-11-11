using DigitalNagrik.Areas.Intermediary.Data;
using DigitalNagrik.Areas.Public.Models;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using DigitalNagrik.Validators;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NICServiceAdaptor;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon;

namespace DigitalNagrik.Areas.Intermediary.Controllers
{
    public class IntermediaryController : Controller
    {
        // GET: Intermediary/Intemediary
        public ActionResult Index()
        {
            return View();
        }
        //-----------------------------------------Intermediary Response-------------------------------------------------------//
        public JsonResult IntermediaryVerifyOTP(string OTPData, string UserID)
        {
            string Result = "";
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("@userID", UserID),
                            new KeyValuePair<string, string>("@OTP", OTPData),
                            new KeyValuePair<string, string>("@func", "VERIFY")
                        };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_VerifyOTP", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["status"].ToString() == "200")
                    {
                        Result = "Y";
                    }
                    else if (ds.Tables[0].Rows[0]["status"].ToString() == "300")
                    {
                        Result = "N";
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
        public ActionResult IntermediaryResponse(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
            GACApplicationAssigned obj = new GACApplicationAssigned
            {
                GACAppealAssigned = new List<GACAppealAssignedList>(),
                GrievanceId = GrievanceId,
                RegistrationYear = RegistrationYear,
                LinkExpired = "N"

            };
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear)
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_SendOTP]", SP_Parameter);
                {
                    string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = "S";
                                break;
                            case "X":
                                obj.LinkExpired = "Y";
                                TempData["Status"] = "X";
                                break;
                            default:
                                TempData["Status"] = "F";
                                break;
                        }
                        if (res.Trim().ToUpper() == "S" && ds.Tables.Count > 0)
                        {
                            obj.UserID = ds.Tables[0].Rows[0]["EmailID"].ToString();
                            string Message = "OTP has been sent to user : " + ds.Tables[0].Rows[0]["EmailID"].ToString();
                            SendSMS smsObj = new SendSMS();
                            string emailSubject = "Grievance Appellate Committee (AppealID - " + GrievanceId + "/" + RegistrationYear + ") - OTP";
                            string emailMsg = "Dear User,<br/> <br/> Grievance Appellate Committee: " + ds.Tables[0].Rows[0]["OTP"].ToString() + " is your OTP to submit response against the filled appeal.<br/> Keep it safe for next 10 minutes. OTP generated on " + ds.Tables[0].Rows[0]["GenerationDatetime"].ToString() + "<br/><br/>Thanks,<br/>Grievance Appellate Committee <br/>Government of India";
                            smsObj.SendMailToUser(emailMsg, emailSubject, ds.Tables[0].Rows[0]["EmailID"].ToString(), "Grievance Appellate Committee", ds.Tables[0].Rows[0]["EmailID"].ToString());
                            TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "OTP has been sent successfully.", Message);
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View("IntermediaryResponse", obj);
        }
        [CheckSessions]
        public ActionResult OpenIntermediaryResponseDetails(string GrievanceId, string RegistrationYear, string OpenInModal = "", string IsPartialView = "", string HeaderText = "", string FormName = "")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
            GACApplicationAssigned obj = new GACApplicationAssigned
            {
                GACAppealAssigned = new List<GACAppealAssignedList>(),
                GrievanceId = GrievanceId,
                RegistrationYear = RegistrationYear,
                OpenInModal = OpenInModal,
                IsPartialView = IsPartialView,
                HeaderText = HeaderText,
                FormName = FormName,

            };
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear)
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GetAppealDetails]", SP_Parameter);
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                obj.GACAppealAssigned.Add(new GACAppealAssignedList
                                {
                                    GrievanceId = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceId"]),
                                    GrievanceDesc = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceDesc"]),
                                    RegistrationYear = Convert.ToString(ds.Tables[0].Rows[0]["RegistrationYear"]),
                                    //ContentClassification = Convert.ToString(ds.Tables[0].Rows[0]["ContentClassificationTitle"]),
                                    //SubContentClassification = Convert.ToString(ds.Tables[0].Rows[0]["SubContentClassificationTitle"]),
                                    IntermediaryTitle = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryTitle"]),
                                    IntermediaryType = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryType"]),
                                    ReceiptDate = Convert.ToString(ds.Tables[0].Rows[0]["ReceiptDate"]),
                                    GACTitle = Convert.ToString(ds.Tables[0].Rows[0]["GACTitle"]),
                                    isResponse = Convert.ToString(ds.Tables[0].Rows[0]["isResponse"]),
                                    GroundTitle = Convert.ToString(ds.Tables[0].Rows[0]["GroundTitle"]),
                                    ReliefTitle = Convert.ToString(ds.Tables[0].Rows[0]["ReliefTitle"]),
                                    RelieftSoughtSpecification = Convert.ToString(ds.Tables[0].Rows[0]["RelieftSoughtSpecification"]),
                                    BriefofComplaint = Convert.ToString(ds.Tables[0].Rows[0]["BriefofComplaint"]),
                                });
                            }

                        }

                    }
                    obj.IntermediaryResponseMaster = new List<IntermediaryResponseMasterList>();
                    List<KeyValuePair<string, string>> SP_Parameter1 = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Func", "IM")
                //new KeyValuePair<string, string>("@GACID", UserSession.GACID)
            };
                    DataSet ds1 = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[IntermediaryResponse_GetMasterData]", SP_Parameter1);
                    if (ds1 != null && ds1.Tables.Count > 0)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds1.Tables[0].Rows)
                            {
                                obj.IntermediaryResponseMaster.Add(new IntermediaryResponseMasterList
                                {
                                    ResponseID = Convert.ToString(dr["ResponseID"]),
                                    ResponseText = Convert.ToString(dr["ResponseText"]),
                                    ChangeOn = Convert.ToString(dr["ChangeOn"]),
                                    HasFiles = Convert.ToString(dr["HasFiles"]),
                                    IsActive = Convert.ToString(dr["IsActive"]),
                                    IsMultipleSubResponse = Convert.ToString(dr["IsMultipleSubResponse"]),
                                    IsSubResponse = Convert.ToString(dr["IsSubResponse"]),
                                    MaxFileSizeMb = Convert.ToString(dr["MaxFileSizeMb"]),
                                    MaxNoOfFiles = Convert.ToString(dr["MaxNoOfFiles"]),
                                    MinNoOfFiles = Convert.ToString(dr["MinNoOfFiles"])
                                });
                            }

                        }

                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return PartialView("_IntermediaryResponseDetails", obj);
        }
        [CheckSessions]
        public ActionResult OpenIntermediarySubResponse(string responseid, string changeon, string value)
        {
            return PartialView("PartialViews/_IntermediarySubResponse", OpenIntermediarySubResponseDetails(responseid, changeon, value));
        }
        [CheckSessions]
        private GACApplicationAssigned OpenIntermediarySubResponseDetails(string responseid, string changeon, string value)
        {
            GACApplicationAssigned obj = new GACApplicationAssigned
            {
                IntermediarySubResponseMaster = new List<IntermediarySubResponseMasterList>()
            };
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter1 = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@responseid", responseid),
                new KeyValuePair<string, string>("@changeon", changeon),
                new KeyValuePair<string, string>("@value", value),
                new KeyValuePair<string, string>("@Func", "SIM")
                //new KeyValuePair<string, string>("@GACID", UserSession.GACID)
            };
                DataSet ds1 = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "IntermediaryResponse_GetMasterData", SP_Parameter1);
                if (ds1 != null && ds1.Tables.Count > 0)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds1.Tables[0].Rows)
                        {
                            obj.IntermediarySubResponseMaster.Add(new IntermediarySubResponseMasterList
                            {
                                ResponseID = Convert.ToString(dr["ResponseID"]),
                                SubResponseID = Convert.ToString(dr["SubResponseID"]),
                                SubResponseText = Convert.ToString(dr["SubResponseText"]),
                                HasFiles = Convert.ToString(dr["HasFiles"]),
                                IsActive = Convert.ToString(dr["IsActive"]),
                                MaxFileSizeMb = Convert.ToString(dr["MaxFileSizeMb"]),
                                MaxNoOfFiles = Convert.ToString(dr["MaxNoOfFiles"]),
                                MinNoOfFiles = Convert.ToString(dr["MinNoOfFiles"])
                            });
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
        public ActionResult SaveIntermediaryResponse(FormCollection frmcollection)
        {
            string GrievanceID = frmcollection["GrievanceID"];
            string RegistrationYear = frmcollection["RegistrationYear"];
            try
            {

                DataTable dtResponse = new DataTable();
                _ = dtResponse.Columns.Add("GrievanceID");
                _ = dtResponse.Columns.Add("RegistrationYear");
                _ = dtResponse.Columns.Add("ResponseID");
                _ = dtResponse.Columns.Add("ResponseValue");
                _ = dtResponse.Columns.Add("ResponseDetails");
                _ = dtResponse.Columns.Add("ResponseURLs");

                DataTable dtSubResponse = new DataTable();
                _ = dtSubResponse.Columns.Add("GrievanceID");
                _ = dtSubResponse.Columns.Add("RegistrationYear");
                _ = dtSubResponse.Columns.Add("ResponseID");
                _ = dtSubResponse.Columns.Add("SubResponseID");
                _ = dtSubResponse.Columns.Add("SubResponseDetails");

                DataTable dtResponseFile = new DataTable();
                _ = dtResponseFile.Columns.Add("GrievanceID");
                _ = dtResponseFile.Columns.Add("RegistrationYear");
                _ = dtResponseFile.Columns.Add("ResponseID");
                _ = dtResponseFile.Columns.Add("FileID");
                _ = dtResponseFile.Columns.Add("File");

                DataTable dtSubResponseFile = new DataTable();
                _ = dtSubResponseFile.Columns.Add("GrievanceID");
                _ = dtSubResponseFile.Columns.Add("RegistrationYear");
                _ = dtSubResponseFile.Columns.Add("ResponseID");
                _ = dtSubResponseFile.Columns.Add("SubResponseID");
                _ = dtSubResponseFile.Columns.Add("FileID");
                _ = dtSubResponseFile.Columns.Add("File");

                List<KeyValuePair<string, string>> responseKeys = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> subResponseKeys = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> responseFilesKeys = new List<KeyValuePair<string, string>>();
                List<KeyValuePair<string, string>> subResponseFilesKeys = new List<KeyValuePair<string, string>>();



                foreach (string key in frmcollection.AllKeys)
                {
                    if (key.Contains("rdlResponse_"))
                    {
                        responseKeys.Add(new KeyValuePair<string, string>(key.Substring(key.LastIndexOf('_') + 1), frmcollection[key]));
                    }
                    if (key.Contains("chksubresponse_"))
                    {
                        subResponseKeys.Add(new KeyValuePair<string, string>(key.Substring(key.LastIndexOf('_') + 1), frmcollection[key]));
                    }

                }
                foreach (KeyValuePair<string, string> entry in responseKeys)
                {
                    DataRow dr = dtResponse.NewRow();
                    dr["GrievanceID"] = frmcollection["GrievanceID"];
                    dr["RegistrationYear"] = frmcollection["RegistrationYear"];
                    dr["ResponseID"] = entry.Key;
                    dr["ResponseValue"] = entry.Value;
                    dr["ResponseDetails"] = frmcollection["ResponseText_" + entry.Key];
                    dr["ResponseURLs"] = frmcollection["ResponseTextURL_" + entry.Key];
                    dtResponse.Rows.Add(dr);
                }
                foreach (KeyValuePair<string, string> entry1 in subResponseKeys)
                {
                    DataRow dr1 = dtSubResponse.NewRow();
                    //string[] ResSubRes = entry1.Key.Split("-");
                    List<string> ResSubRes = new List<string>(entry1.Key.Split(new string[] { "-" }, StringSplitOptions.None));

                    dr1["GrievanceID"] = frmcollection["GrievanceID"];
                    dr1["RegistrationYear"] = frmcollection["RegistrationYear"];
                    dr1["ResponseID"] = ResSubRes[0];
                    dr1["SubResponseID"] = ResSubRes[1];
                    dr1["SubResponseDetails"] = frmcollection["subResponseText_" + entry1.Key];
                    dtSubResponse.Rows.Add(dr1);
                }
                string DirPath = string.Empty;
                string FileNm = string.Empty;
                string FinalPath = string.Empty;
                if (Request.Files.Count > 0)
                {
                    foreach (string file in Request.Files)
                    {

                        if (file.Contains("fuDocumenttoUpload_"))
                        {
                            HttpPostedFileBase file1 = Request.Files[file];
                            if (file1.ContentLength != 0)
                            {
                                if (ValidateFileAttribute.CheckMimeType("pdf", file1))
                                {
                                    if (!ValidateFileAttribute.IsFileTypeValid(file1))
                                    {
                                        TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Error, "Failed", "The file you have uploaded is invalid. Please upload a valid PDF File.");
                                        return frmcollection["FormName"] == "GV" ? (ActionResult)RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" }) : (ActionResult)RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                                    }
                                    else
                                    {
                                        DirPath = RegistrationYear + GrievanceID + "/" + GridView.Data.GACGridViewInbox.Dir_IntermediaryResponse; FileNm = GridView.Data.GACGridViewInbox.Dir_IntermediaryResponse + ".pdf";
                                        FinalPath = DirPath + "/" + FileNm;
                                        byte[] fileBytes = new byte[file1.ContentLength]; file1.InputStream.Read(fileBytes, 0, file1.ContentLength);
                                        FTPHelper Ftp = new FTPHelper(); Ftp.CreateFTPDir(DirPath); Ftp.saveFTPfile(fileBytes, DirPath + "/" + FileNm);
                                        responseFilesKeys.Add(new KeyValuePair<string, string>(file.Substring(file.LastIndexOf('_') + 1), FinalPath));
                                    }                     
                                }
                                else
                                {
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Error, "Failed", "The file you have uploaded is invalid. Please upload a valid PDF File.");
                                    return frmcollection["FormName"] == "GV" ? (ActionResult)RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" }) : (ActionResult)RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                                    //return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                                }

                            }

                        }
                        if (file.Contains("fusubDocumenttoUpload_"))
                        {
                            HttpPostedFileBase file1 = Request.Files[file];
                            if (file1.ContentLength != 0 && ValidateFileAttribute.CheckMimeType("pdf", file1))
                            {
                                if (!ValidateFileAttribute.IsFileTypeValid(file1))
                                {
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Error, "Failed", "The file you have uploaded is invalid. Please upload a valid PDF File.");
                                    return frmcollection["FormName"] == "GV" ? (ActionResult)RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" }) : (ActionResult)RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                                }
                                else
                                {
                                    subResponseFilesKeys.Add(new KeyValuePair<string, string>(file.Substring(file.LastIndexOf('_') + 1), ConverttoBinarytext(file)));
                                }                               
                            }
                        }
                    }

                    foreach (KeyValuePair<string, string> entry in responseFilesKeys)
                    {
                        List<string> ResSubRes = new List<string>(entry.Key.Split(new string[] { "-" }, StringSplitOptions.None));
                        DataRow dr = dtResponseFile.NewRow();
                        dr["GrievanceID"] = frmcollection["GrievanceID"];
                        dr["RegistrationYear"] = frmcollection["RegistrationYear"];
                        dr["ResponseID"] = ResSubRes[0];
                        dr["FileID"] = ResSubRes[1];
                        dr["File"] = entry.Value;
                        dtResponseFile.Rows.Add(dr);
                    }
                    foreach (KeyValuePair<string, string> entry1 in subResponseFilesKeys)
                    {
                        DataRow dr1 = dtSubResponseFile.NewRow();
                        //string[] ResSubRes = entry1.Key.Split("-");
                        //List<string> ResSubRes = new List<string>(entry1.Key.Split(new string[] { "-" }, StringSplitOptions.None));
                        string[] ResSubRes = entry1.Key.Split(new char[] { '-' });
                        dr1["GrievanceID"] = frmcollection["GrievanceID"];
                        dr1["RegistrationYear"] = frmcollection["RegistrationYear"];
                        dr1["ResponseID"] = ResSubRes[0];
                        dr1["SubResponseID"] = ResSubRes[1];
                        dr1["FileID"] = ResSubRes[2];
                        dr1["File"] = entry1.Value;
                        dtSubResponseFile.Rows.Add(dr1);
                    }
                }
                List<KeyValuePair<string, dynamic>> methodparameter2 = new List<KeyValuePair<string, dynamic>>
                {
                     //new KeyValuePair<string, DataTable>("@GrievanceID", GrievanceID),
                    new KeyValuePair<string, dynamic>("@UserId", UserSession.UserID),
                    new KeyValuePair<string, dynamic>("@dtResponse", dtResponse),
                     new KeyValuePair<string, dynamic>("@dtSubResponse", dtSubResponse),
                      new KeyValuePair<string, dynamic>("@dtResponseFile", dtResponseFile),
                       new KeyValuePair<string, dynamic>("@dtSubResponseFile", dtSubResponseFile),
                       new KeyValuePair<string, dynamic>("IpAddress", BALCommon.GetIPAddress())
                };
                DataTable ds1 = new DBAccess().INSERTUpdateData(methodparameter2, "[IntermediaryResponse_InsertData]");
                if (ds1 != null && ds1.Rows.Count > 0)
                {
                    string res = Convert.ToString(ds1.Rows[0]["Result"]);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        switch (res.Trim().ToUpper())
                        {
                            case "S":
                                TempData["Status"] = null;
                                TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Successfully Saved", "Respondent Intermediary Response has been successfully saved.");
                                break;
                            default:
                                TempData["Status"] = null;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            //TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Response saved Successfully.", "Response sent to MietY");
            return frmcollection["FormName"] == "GV"
                ? (ActionResult)RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" })
                : (ActionResult)RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
        }
        //public ActionResult IntermediaryResponsePDFView(string qs)
        //{
        //    try
        //    {
        //        if (qs != "")
        //        {
        //            byte[] DocumentView = new byte[] { };
        //            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string ResponseId = string.Empty; string SubResponseId = string.Empty; string FileId = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; ; ResponseId = Params["ResponseId"]; SubResponseId = Params["SubResponseId"]; FileId = Params["FileId"]; } catch { }
        //            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
        //                {
        //                    new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
        //                    new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
        //                    new KeyValuePair<string, string>("@ResponseId", ResponseId),
        //                    new KeyValuePair<string, string>("@SubResponseId", SubResponseId),
        //                    new KeyValuePair<string, string>("@FileId", FileId)
        //                };
        //            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_IntermediaryResponsePDF]", SP_Parameter);
        //            if (ds != null)
        //            {
        //                if (ds.Tables.Count > 0)
        //                {
        //                    if (ds.Tables[0].Rows.Count > 0)
        //                    {
        //                        DocumentView = ds.Tables[0].Rows[0]["File"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["File"];
        //                    }
        //                }
        //            }
        //            //return View(obj);
        //            return File(DocumentView, "application/pdf");
        //        }
        //    }
        //    catch (Exception ex) { MyExceptionHandler.LogError(ex); }
        //    return new EmptyResult();
        //}
        public string ConverttoBinarytext(string file)
        {
            HttpPostedFileBase file1 = Request.Files[file];
            string BytesDocument = "";
            try
            {
                if (file1.ContentLength != 0)
                {
                    byte[] EXE_Document = { 77, 90 };
                    byte[] fileBytes = new byte[file1.ContentLength];
                    _ = file1.InputStream.Read(fileBytes, 0, file1.ContentLength);
                    if (file1.ContentLength > 1000000)
                    {
                        TempData["Status"] = "F"; //"File size should be less than 1024 kb.";
                                                  //return RedirectToAction("Index");
                    }
                    if (fileBytes[0] == EXE_Document[0] && fileBytes[1] == EXE_Document[1])
                    {
                        TempData["Status"] = "N";//"File type EXE not supported.";
                                                 //return RedirectToAction("Index");
                    }
                    BytesDocument = System.Text.UnicodeEncoding.Default.GetString(fileBytes);
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return BytesDocument;
        }
        //public ActionResult IntermediaryDownloadPDF(string GrievanceId, string RegistrationYear)
        //{
        public ActionResult IntermediaryResponseView(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
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
                                ResponseUrls = Convert.ToString(dr["ResponseUrls"]),
                                IntermediaryReplyDateTime = Convert.ToString(dr["IntermediaryReplyDateTime"]),

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
        [CheckSessions]
        public ActionResult IntermediaryDownloadPDF(string qs1)
        {
            if (Request.UrlReferrer != null)
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Intermediary, "1", UserSession.LangCulture);
                Dictionary<string, string> Params = CommonRepository.GetQSDecryptedParameters(qs1);
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
                            //obj.ContentClassification = ds.Tables[3].Rows[0]["ContentClassificationTitle"].ToString();
                            //obj.SubContentClassification = ds.Tables[3].Rows[0]["SubContentClassificationTitle"].ToString();
                            obj.ReceiptDate = ds.Tables[3].Rows[0]["ReceiptDate"].ToString();
                            obj.GrievanceDesc = ds.Tables[3].Rows[0]["GrievanceDesc"].ToString();
                            obj.IntermediaryTitle = ds.Tables[3].Rows[0]["IntermediaryTitle"].ToString();
                            obj.IntermediaryType = ds.Tables[3].Rows[0]["IntermediaryType"].ToString();
                            obj.GACTitle = ds.Tables[3].Rows[0]["GACTitle"].ToString();
                            obj.GroundTitle = ds.Tables[3].Rows[0]["GroundTitle"].ToString();
                            obj.ReliefTitle = ds.Tables[3].Rows[0]["ReliefTitle"].ToString();
                            obj.RelieftSoughtSpecification = ds.Tables[3].Rows[0]["RelieftSoughtSpecification"].ToString();
                            obj.BriefofComplaint = ds.Tables[3].Rows[0]["BriefofComplaint"].ToString();
                        }
                    }

                }
                catch (Exception ex)
                {
                    MyExceptionHandler.LogError(ex);
                }
                ViewAsPdf ApplicationPDF = new ViewAsPdf("DownloadLetter", obj)
                {

                    FileName = "DownloadLetter.pdf",
                    PageMargins = { Left = 8, Right = 8 },
                    CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"9\" --footer-left \"GAC\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \"",
                    PageSize = Rotativa.Options.Size.A4,
                    PageOrientation = Rotativa.Options.Orientation.Portrait

                };
                List<byte[]> PDFBytesList = new List<byte[]>();
                byte[] MergedPDFBytes = null;
                try
                {
                    PDFBytesList.Add(ApplicationPDF.BuildPdf(ControllerContext));
                    if (obj.IntermediaryResponseDownloadPDFsubResponseFiles.Count > 0)
                    {
                        foreach (IntermediaryResponseDownloadPDFsubResponseFiles_List doc in obj.IntermediaryResponseDownloadPDFsubResponseFiles.Where(x => x.File != null))
                        {
                            PDFBytesList.Add(BALCommon.GetFTPFileBytes(doc.File, "pdf"));
                        }   //x => x.DocBytes.Length > 0
                    }
                    if (PDFBytesList.Count > 0 && PDFBytesList != null)
                    {
                        MergedPDFBytes = MergeDocs(PDFBytesList);
                    }
                }
                catch (Exception)
                {
                }

                return MergedPDFBytes != null
                    ? File(MergedPDFBytes, "application/pdf")
                    : (ActionResult)File(MergedPDFBytes, "application/pdf");

            }
            else
            {
                return RedirectToAction("Error", "Home", new { Area = "" });
            }
            //return new ViewAsPdf("DownloadLetter", obj)
            //{
            //    FileName = "DownloadLetter.pdf",
            //    PageMargins = { Left = 8, Right = 8 },
            //    CustomSwitches = "--print-media-type --footer-line  --footer-font-size \"9\" --footer-left \"MeitY (GAc Portal)\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \""
            //    //CustomSwitches = "--print-media-type  --footer-html \"{<hr/>}\"  --footer-left \"NIC-HP\" --footer-center \"Page [page] of [topage]\"  --footer-right \" Printed on " + DateTime.Now + " \""
            //    //string cusomtSwitches = string.Format("--print-media-type --allow {0} --footer-html {0}", Url.Action("Footer", "Home", new { area = "" }, "http")); //--footer-spacing -15
            //};
            //return View("DownloadLetter", obj);
        }
        public static byte[] MergeDocs(List<byte[]> PDFBytesList)
        {
            PdfReader.unethicalreading = true;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document doc = new Document())
                {
                    using (PdfSmartCopy copy = new PdfSmartCopy(doc, ms))
                    {
                        doc.Open();
                        foreach (byte[] PDFBytes in PDFBytesList) //Loop through each byte array
                        {
                            using (PdfReader reader = new PdfReader(PDFBytes)) //Create a PdfReader bound to that byte array
                            {
                                copy.AddDocument(reader); //Add the entire document instead of page-by-page
                            }
                        }
                        doc.Close();
                    }
                }
                return ms.ToArray();
            }
        }
        //-----------------------------------------Intermediary Response-------------------------------------------------------//
        //-----------------------------------------Intermediary Reply-------------------------------------------------------//
        public ActionResult IntermediaryReply(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string InputID = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; InputID = Params["InputId"]; } catch { }

            IntermediaryReply obj = new IntermediaryReply
            {
                GrievanceId = GrievanceId,
                RegistrationYear = RegistrationYear,
                InputID = InputID
            };
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear)
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_SendOTP]", SP_Parameter);
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
                        if (res.Trim().ToUpper() == "S" && ds.Tables.Count > 0)
                        {
                            obj.UserID = ds.Tables[0].Rows[0]["EmailID"].ToString();
                            string Message = "OTP has been sent to user : " + ds.Tables[0].Rows[0]["EmailID"].ToString();
                            SendSMS smsObj = new SendSMS();
                            string emailSubject = "Grievance Appellate Committee (AppealID - " + GrievanceId + "/" + RegistrationYear + ") - OTP";
                            string emailMsg = "Dear User,<br/> <br/> Grievance Appellate Committee: " + ds.Tables[0].Rows[0]["OTP"].ToString() + " is your OTP to submit response against the filled appeal.<br/> Keep it safe for next 10 minutes. OTP generated on " + ds.Tables[0].Rows[0]["GenerationDatetime"].ToString() + "<br/><br/>Thanks,<br/>Grievance Appellate Committee <br/>Government of India";
                            smsObj.SendMailToUser(emailMsg, emailSubject, ds.Tables[0].Rows[0]["EmailID"].ToString(), "Grievance Appellate Committee", ds.Tables[0].Rows[0]["EmailID"].ToString());
                            TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "OTP has been sent successfully.", Message);
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View("IntermediaryReply", obj);

        }
        [CheckSessions]
        public ActionResult OpenIntermediaryReplyDetails(string GrievanceId, string RegistrationYear, string InputID, string IsPartialView = "", string HeaderText = "")
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.SubjectExpert, "1", UserSession.LangCulture);
            IntermediaryReply obj = new IntermediaryReply
            {
                GrievanceId = GrievanceId,
                RegistrationYear = RegistrationYear,
                InputID = InputID,
                IsPartialView = IsPartialView,
                HeaderText = HeaderText
            };
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@InputID", InputID)
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GetIntermediaryReply]", SP_Parameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.IntermediaryReplyText = ds.Tables[0].Rows[0]["InputReplyRemarks"].ToString();
                        obj.InputSeekRemarks = ds.Tables[0].Rows[0]["InputSeekRemarks"].ToString();
                        obj.InputSeekDateTime = ds.Tables[0].Rows[0]["InputSeekDateTime"].ToString();
                        obj.IntermediaryDateTime = ds.Tables[0].Rows[0]["InputReplyDateTime"].ToString();
                        obj.isSupportingDocument = ds.Tables[0].Rows[0]["isSupportingDocument"].ToString();
                        obj.FilePath = ds.Tables[0].Rows[0]["FilePath"].ToString();
                        //obj.ContentClassification = ds.Tables[0].Rows[0]["ContentClassificationTitle"].ToString();
                        //obj.SubContentClassification = ds.Tables[0].Rows[0]["SubContentClassificationTitle"].ToString();
                        obj.ReceiptDate = ds.Tables[0].Rows[0]["ReceiptDate"].ToString();
                        obj.RegistrationYear = ds.Tables[0].Rows[0]["RegistrationYear"].ToString();
                        obj.GrievanceId = ds.Tables[0].Rows[0]["GrievanceId"].ToString();
                        obj.GrievanceDesc = ds.Tables[0].Rows[0]["GrievanceDesc"].ToString();
                        obj.IntermediaryTitle = ds.Tables[0].Rows[0]["IntermediaryTitle"].ToString();
                        obj.IntermediaryType = ds.Tables[0].Rows[0]["IntermediaryType"].ToString();
                        obj.GACTitle = ds.Tables[0].Rows[0]["GACTitle"].ToString();
                        obj.GroundTitle = Convert.ToString(ds.Tables[0].Rows[0]["GroundTitle"]);
                        obj.ReliefTitle = Convert.ToString(ds.Tables[0].Rows[0]["ReliefTitle"]);
                        obj.RelieftSoughtSpecification = Convert.ToString(ds.Tables[0].Rows[0]["RelieftSoughtSpecification"]);
                        obj.BriefofComplaint = Convert.ToString(ds.Tables[0].Rows[0]["BriefofComplaint"]);

                    }

                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return PartialView("_IntermediaryReplyDetails", obj);
        }
        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveIntermediaryReply(IntermediaryReply obj)
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
                        if (obj.SupportingDocument.ContentLength != 0)
                        {
                            if (ValidateFileAttribute.CheckMimeType("pdf", obj.SupportingDocument))
                            {
                                if (!ValidateFileAttribute.IsFileTypeValid(obj.SupportingDocument))
                                {
                                    TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Error, "Failed", "The file you have uploaded is invalid. Please upload a valid PDF File.");
                                    return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });                  
                                }
                                else
                                {
                                    DirPath = obj.RegistrationYear + obj.GrievanceId + "/" + GridView.Data.GACGridViewInbox.Dir_IntermediaryAdditionalInput; FileNm = obj.InputID + ".pdf";
                                    FinalPath = DirPath + "/" + FileNm;
                                }

                            }
                            else
                            {
                                TempData["AlertStatus"] = CustomClientAlerts.GenerateAlert(CustomClientAlerts.AlertTypeFormat.Error, "Failed", "The file you have uploaded is invalid. Please upload a valid PDF File.");
                                return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                                //return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                            }
                        }
                    }

                    //if (obj.SupportingDocument != null)
                    //    {
                    //        DirPath = obj.RegistrationYear + obj.GrievanceId + "/" + GridView.Data.GACGridViewInbox.Dir_IntermediaryAdditionalInput; FileNm = obj.InputID + ".pdf";
                    //        FinalPath = DirPath + "/" + FileNm;
                    //    }

                    List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
                {
                     new KeyValuePair<string, string>("@GrievanceID", obj.GrievanceId),
                     new KeyValuePair<string, string>("@RegistrationYear", obj.RegistrationYear),
                     new KeyValuePair<string, string>("@InputID", obj.InputID),
                     new KeyValuePair<string, string>("@IntermediaryReplyText", obj.IntermediaryReplyText),
                     new KeyValuePair<string, string>("@SupportingDocument", FinalPath)
                };
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_SaveIntermediaryReply]", SP_Parameter);
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
                                        TempData["Status"] = "S";
                                        break;
                                    default:
                                        TempData["Status"] = "F";
                                        break;
                                }
                            }
                        }
                    }
                    TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Respondent Intermediary Reply saved successfully.", "Reply saved successfully");
                    //return RedirectToAction("IntermediaryReply", "Intermediary", new { Area = "Intermediary", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + obj.RegistrationYear + "&GrievanceId=" + obj.GrievanceId + "&InputId=" + obj.InputID) });
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
            //return RedirectToAction("IntermediaryReply", "Intermediary", new { Area = "Intermediary", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + obj.RegistrationYear + "&GrievanceId=" + obj.GrievanceId + "&InputId=" + obj.InputID) });
            //return RedirectToAction("OpenIntermediaryReplyDetails", "Intermediary", new { Area = "Intermediary", GrievanceId = obj.GrievanceId, RegistrationYear = obj.RegistrationYear, InputID = obj.InputID });
        }
        //public ActionResult IntermediaryReplyPDF(string qs)
        //{
        //    try
        //    {
        //        if (qs != "")
        //        {
        //            byte[] DocumentView = new byte[] { };
        //            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string InputID = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; InputID = Params["InputId"]; } catch { }
        //            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("@grievanceId", GrievanceId),
        //        new KeyValuePair<string, string>("@registrationYear", RegistrationYear),
        //        new KeyValuePair<string, string>("@InputID", InputID)
        //    };
        //            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_IntermediaryReply_Doc]", SP_Parameter);
        //            if (ds != null)
        //            {
        //                if (ds.Tables.Count > 0)
        //                {
        //                    if (ds.Tables[0].Rows.Count > 0)
        //                    {
        //                        DocumentView = ds.Tables[0].Rows[0]["InputReplyFile"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["InputReplyFile"];
        //                    }
        //                }
        //            }
        //            //return View(obj);
        //            return File(DocumentView, "application/pdf");
        //        }
        //    }
        //    catch (Exception ex) { MyExceptionHandler.LogError(ex); }
        //    return new EmptyResult();
        //}
        //-----------------------------------------Intermediary Reply-------------------------------------------------------//

        //-----------------------------------------Subject Expert-------------------------------------------------------//
        public ActionResult SubjectExpertReply(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string InputID = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; InputID = Params["InputId"]; } catch { }

            IntermediaryReply obj = new IntermediaryReply
            {
                GrievanceId = GrievanceId,
                RegistrationYear = RegistrationYear,
                InputID = InputID
            };
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear)
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[SubjectExpert_SendOTP]", SP_Parameter);
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
                        if (res.Trim().ToUpper() == "S" && ds.Tables.Count > 0)
                        {
                            obj.UserID = ds.Tables[0].Rows[0]["EmailID"].ToString();
                            string Message = "OTP has been sent to user : " + ds.Tables[0].Rows[0]["EmailID"].ToString();
                            SendSMS smsObj = new SendSMS();
                            string emailSubject = "Grievance Appellate Committee (AppealID - " + GrievanceId + "/" + RegistrationYear + ") - OTP";
                            string emailMsg = "Dear User,<br/> <br/> Grievance Appellate Committee: " + ds.Tables[0].Rows[0]["OTP"].ToString() + " is your OTP to submit response against the filled appeal.<br/> Keep it safe for next 10 minutes. OTP generated on " + ds.Tables[0].Rows[0]["GenerationDatetime"].ToString() + "<br/><br/>Thanks,<br/>Grievance Appellate Committee <br/>Government of India";
                            smsObj.SendMailToUser(emailMsg, emailSubject, ds.Tables[0].Rows[0]["EmailID"].ToString(), "Grievance Appellate Committee", ds.Tables[0].Rows[0]["EmailID"].ToString());
                            TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "OTP has been sent successfully.", Message);
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View("SubjectExpertReply", obj);

        }
        [CheckSessions]
        //public ActionResult SubjectExpertReplyDetails(string GrievanceId, string RegistrationYear, string InputID, string IsPartialView = "", string HeaderText = "")
        public ActionResult SubjectExpertReplyDetails(string qs)
        {
            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string InputID = string.Empty; string IsPartialView = ""; string HeaderText = "";
            Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; InputID = Params["InputID"]; HeaderText = Params["HeaderText"]; } catch { }

            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.SubjectExpert, "1", UserSession.LangCulture);
            IntermediaryReply obj = new IntermediaryReply
            {
                GrievanceId = GrievanceId,
                RegistrationYear = RegistrationYear,
                InputID = InputID,
                IsPartialView = IsPartialView,
                HeaderText = HeaderText
            };
            try
            {
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@grievanceId", GrievanceId),
                new KeyValuePair<string, string>("@registrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@InputID", InputID)
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[GetSubjectExpertReplyDetails]", SP_Parameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.IntermediaryReplyText = ds.Tables[0].Rows[0]["InputReplyRemarks"].ToString();
                        obj.InputSeekRemarks = ds.Tables[0].Rows[0]["InputSeekRemarks"].ToString();
                        obj.InputSeekDateTime = ds.Tables[0].Rows[0]["InputSeekDateTime"].ToString();
                        obj.IntermediaryDateTime = ds.Tables[0].Rows[0]["InputReplyDateTime"].ToString();
                        obj.isSupportingDocument = ds.Tables[0].Rows[0]["isSupportingDocument"].ToString();
                        obj.FilePath = ds.Tables[0].Rows[0]["FilePath"].ToString();
                        //obj.ContentClassification = ds.Tables[0].Rows[0]["ContentClassificationTitle"].ToString();
                        //obj.SubContentClassification = ds.Tables[0].Rows[0]["SubContentClassificationTitle"].ToString();
                        obj.ReceiptDate = ds.Tables[0].Rows[0]["ReceiptDate"].ToString();
                        obj.RegistrationYear = ds.Tables[0].Rows[0]["RegistrationYear"].ToString();
                        obj.GrievanceId = ds.Tables[0].Rows[0]["GrievanceId"].ToString();
                        obj.GrievanceDesc = ds.Tables[0].Rows[0]["GrievanceDesc"].ToString();
                        obj.IntermediaryTitle = ds.Tables[0].Rows[0]["IntermediaryTitle"].ToString();
                        obj.IntermediaryType = ds.Tables[0].Rows[0]["IntermediaryType"].ToString();
                        obj.GACTitle = ds.Tables[0].Rows[0]["GACTitle"].ToString();
                        obj.GroundTitle = Convert.ToString(ds.Tables[0].Rows[0]["GroundTitle"]);
                        obj.ReliefTitle = Convert.ToString(ds.Tables[0].Rows[0]["ReliefTitle"]);
                        obj.RelieftSoughtSpecification = Convert.ToString(ds.Tables[0].Rows[0]["RelieftSoughtSpecification"]);
                        obj.BriefofComplaint = Convert.ToString(ds.Tables[0].Rows[0]["BriefofComplaint"]);

                    }

                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return PartialView("_SubjectExpertReplyDetails", obj);
        }
        [CheckSessions]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSubjectExpertReply(IntermediaryReply obj)
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
                        DirPath = obj.RegistrationYear + obj.GrievanceId + "/" + GridView.Data.GACGridViewInbox.Dir_ExpertInput; FileNm = obj.InputID + ".pdf";
                        FinalPath = DirPath + "/" + FileNm;
                    }
                    //string BytesDocument = "";
                    //if (obj.SupportingDocument != null)
                    //{
                    //    byte[] EXE_Document = { 77, 90 };
                    //    byte[] fileBytes = new byte[obj.SupportingDocument.ContentLength];
                    //    _ = obj.SupportingDocument.InputStream.Read(fileBytes, 0, obj.SupportingDocument.ContentLength);
                    //    if (obj.SupportingDocument.ContentLength > 1000000)
                    //    {
                    //        TempData["Status"] = "F"; //"File size should be less than 1024 kb.";
                    //                                  //return RedirectToAction("Index");
                    //    }
                    //    if (fileBytes[0] == EXE_Document[0] && fileBytes[1] == EXE_Document[1])
                    //    {
                    //        TempData["Status"] = "N";//"File type EXE not supported.";
                    //                                 //return RedirectToAction("Index");
                    //    }
                    //    BytesDocument = System.Text.UnicodeEncoding.Default.GetString(fileBytes);
                    //}

                    List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
                {
                     new KeyValuePair<string, string>("@GrievanceID", obj.GrievanceId),
                     new KeyValuePair<string, string>("@RegistrationYear", obj.RegistrationYear),
                     new KeyValuePair<string, string>("@InputID", obj.InputID),
                     new KeyValuePair<string, string>("@SubjectExpertReplyText", obj.IntermediaryReplyText),
                     new KeyValuePair<string, string>("@SupportingDocument", FinalPath)

                };
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_SaveSubjectExpertReply]", SP_Parameter);
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
                                        TempData["Status"] = "S";
                                        break;
                                    default:
                                        TempData["Status"] = "F";
                                        break;
                                }
                            }
                        }
                    }
                    TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Subject Expert Input saved successfully.", "Input saved successfully");
                    //return RedirectToAction("IntermediaryReply", "Intermediary", new { Area = "Intermediary", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + obj.RegistrationYear + "&GrievanceId=" + obj.GrievanceId + "&InputId=" + obj.InputID) });
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return RedirectToAction("Index", "SubjectExpert", new { Area = "SubjectExpert" });
            //return RedirectToAction("SubjectExpertReply", "Intermediary", new { Area = "Intermediary", qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + obj.RegistrationYear + "&GrievanceId=" + obj.GrievanceId + "&InputId=" + obj.InputID) });
            //return RedirectToAction("SubjectExpertReplyDetails", "Intermediary", new { Area = "Intermediary", GrievanceId = obj.GrievanceId, RegistrationYear = obj.RegistrationYear, InputID = obj.InputID, IsPartialView = "N" });
        }
        //public ActionResult SubjectExpertReplyPDF(string qs)
        //{
        //    try
        //    {
        //        if (qs != "")
        //        {
        //            byte[] DocumentView = new byte[] { };
        //            string RegistrationYear = string.Empty; string GrievanceId = string.Empty; string InputID = string.Empty; Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs); try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; InputID = Params["InputId"]; } catch { }
        //            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("@grievanceId", GrievanceId),
        //        new KeyValuePair<string, string>("@registrationYear", RegistrationYear),
        //        new KeyValuePair<string, string>("@InputID", InputID)
        //    };
        //            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_SubjectExpertReply_Doc]", SP_Parameter);
        //            if (ds != null)
        //            {
        //                if (ds.Tables.Count > 0)
        //                {
        //                    if (ds.Tables[0].Rows.Count > 0)
        //                    {
        //                        DocumentView = ds.Tables[0].Rows[0]["InputReplyFile"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["InputReplyFile"];
        //                    }
        //                }
        //            }
        //            //return View(obj);
        //            return File(DocumentView, "application/pdf");
        //        }
        //    }
        //    catch (Exception ex) { MyExceptionHandler.LogError(ex); }
        //    return new EmptyResult();
        //}
        //-----------------------------------------Subject Expert-------------------------------------------------------//
    }
}