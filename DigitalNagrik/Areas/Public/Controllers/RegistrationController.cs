using iTextSharp.text;
using iTextSharp.text.pdf;
using DigitalNagrik.Areas.Public.Models;
using NICServiceAdaptor;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.html.simpleparser;
using System.Web.Script.Serialization;
using DigitalNagrik.Models;
using System.Drawing;
using Newtonsoft.Json;
using DigitalNagrik.Filters;
using System.Web.Security;
using DigitalNagrik.Validators;

namespace DigitalNagrik.Areas.Public.Controllers
{
    [CheckSessions]
    public class RegistrationController : MainController
    {
        CommonFunctions objcm = new CommonFunctions();

        public ActionResult CheckIntermediaryEntry(string IntermediaryNew, string urlofintermediaryNew, string GROIntermediaryEmailNew)
        {
            DataTable dt = new DataTable();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@title", IntermediaryNew));
            methodParameter.Add(new KeyValuePair<string, string>("@url", urlofintermediaryNew));
            methodParameter.Add(new KeyValuePair<string, string>("@email", GROIntermediaryEmailNew));
            //methodParameter.Add(new KeyValuePair<string, string>("@msg", GROIntermediaryEmailNew));
            //methodParameter.Add(new KeyValuePair<string, string>("@errorcode", GROIntermediaryEmailNew));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", "DigitalNagrikConnStr", "SelectMSSql", "CheckIntermediaryEntry", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);

        }



        public ActionResult Logout()
        {

            if (Session["LogoutID"] != null)
            {
                UpdateLogoutTime();
            }

            Session.Abandon();
            Session.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new { Area = "" });
        }
        private string UpdateLogoutTime()
        {
            try
            {
                string msg = "0";
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@ID", Session["LogoutID"].ToString()));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "UpdateLoginStatus", methodparameter);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    msg = ds.Tables[0].Rows[0]["msg"].ToString();
                }

                return msg;
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return "0";
            }

        }
        public ActionResult DownloadDocument(string qs)
        {
            string FilePath = string.Empty; string FileType = string.Empty;
            //var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);

            string RegistrationYear = string.Empty; string GrievanceID = string.Empty; string FileID = string.Empty;
            try
            {
                if (qs != null)
                {
                    var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
                    FilePath = Params["FilePath"];
                    FileType = Params["FileType"];
                    RegistrationYear = Params["RegistrationYear"];
                    GrievanceID = Params["GrievanceId"];
                    FileID = Params["FileID"];
                }

                string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
                string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
                string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + FilePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                var tm = FilePath.Split('0');
                string contentType = "application/" + FileType.ToLower().ToString();
                string fileNameDisplayedToUser = FilePath.Split('/')[2];
                // return File(ftpStream, contentType, fileNameDisplayedToUser);
                using (MemoryStream stream = new MemoryStream())
                {
                    //Download the File.
                    response.GetResponseStream().CopyTo(stream);
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileNameDisplayedToUser);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();

                }
                ftpStream.Close();
                response.Close();
                return View();

            }
            catch (WebException ex)
            {
                //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
                TempData["ErrorMessage"] = "No file available, Please upload file again!";
                objcm.deleteAppealDocument(RegistrationYear, GrievanceID, FileID);
                return Redirect(Request.UrlReferrer.ToString());
            }

        }
        public ActionResult InterComplianceDOC(string qs)
        {
            try
            {
                if (qs != "")
                {
                    byte[] DocumentView = new byte[] { };
                    string RegistrationYear = string.Empty;
                    string GrievanceId = string.Empty;
                    Dictionary<string, string> Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
                    try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
                    var methodParameter = new List<KeyValuePair<string, string>>();
                    methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                    methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceId));
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "getAppealComplianceDetail", methodParameter);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DocumentView = ds.Tables[0].Rows[0]["SupportingDocument"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["SupportingDocument"];
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
        public ActionResult CopyofDecissionGAC(string qs)
        {
            try
            {
                if (qs != "")
                {
                    byte[] DocumentView = new byte[] { };
                    string RegistrationYear = string.Empty;
                    string GrievanceId = string.Empty;
                    Dictionary<string, string> Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
                    try { RegistrationYear = Params["RegistrationYear"]; GrievanceId = Params["GrievanceId"]; } catch { }
                    var methodParameter = new List<KeyValuePair<string, string>>();
                    methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                    methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceId));
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", "DigitalNagrikConnStr", "SelectMSSql", "getAppealDisposalDetail", methodParameter);
                    if (ds != null)
                    {

                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DocumentView = ds.Tables[0].Rows[0]["LetterCopy"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["LetterCopy"];
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

        public ActionResult NewIntermediaryDetail(string qs, string Message, string from)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "2", UserSession.LangCulture);
            Dictionary<string, string> LabelDictionary = ViewData["LabelDictionary"] as Dictionary<string, string>;
            if (!(from == "CheckList" || from == "NewAppeal" || from == "dashboard" || from == "Existing"))
            {
                return RedirectToAction("CitizenDashBoard");
            }
            if (Session["CitizenEmailMobile"] == null) { return RedirectToAction("Index", "Home", new { Area = "" }); }
            string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
            if (qs != null)
            {
                var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
                RegistrationYear = Params["RegistrationYear"];
                GrievanceID = Params["GrievanceId"];
            }
            var objIntermediaryDetailPart = new mIntermediaryDetailPart();


            if (Message != null)
            {
                checkMessage(Message, "");
            }
            // ViewData["CurrentStep"] = checkCurrentStep(RegistrationYear, GrievanceID);

            RegistrationYear = (RegistrationYear == null ? "" : RegistrationYear);
            GrievanceID = (GrievanceID == null ? "" : GrievanceID);
            if (!string.IsNullOrEmpty(RegistrationYear) & !string.IsNullOrEmpty(GrievanceID))
            {
                objIntermediaryDetailPart = objcm.fillIntermediaryDetailPart(Session["CitizenEmailMobile"].ToString(), RegistrationYear, GrievanceID);
                objIntermediaryDetailPart.HeaderText = LabelDictionary["EditAppeal"];
            }
            else
            {
                if (from == "CheckList")
                {
                    List<mIntermediaryDetailPart> li = objcm.fillUserAppealList(Session["CitizenEmailMobile"].ToString(), "1");
                    ViewData["listIntermediaryDetails"] = li;
                    if (li.Count == 0)
                    {
                        from = "NewAppeal";
                    }
                }
                if (from == "NewAppeal")
                {
                    objIntermediaryDetailPart = objcm.GenerateAppealID_new(Session["CitizenEmailMobile"].ToString());
                    objIntermediaryDetailPart = objcm.fillIntermediaryDetailPart(Session["CitizenEmailMobile"].ToString(), objIntermediaryDetailPart.RegistrationYear, objIntermediaryDetailPart.GrievanceID);
                    objcm.createFTPPath(objIntermediaryDetailPart.RegistrationYear, objIntermediaryDetailPart.GrievanceID);

                }
                objIntermediaryDetailPart.HeaderText = LabelDictionary["FileNewAppeal"];

            }
            objIntermediaryDetailPart.MobileNo = Session["CitizenEmailMobile"].ToString();
            ViewData["DocumentFileType"] = objcm.fillDocumentFileTypeMaster();
            ViewData["KeyWords"] = objcm.GetGroundKeywords();
            List<mAppealDocument> list = new List<mAppealDocument>();
            list = objcm.fillAppealDocuments(objIntermediaryDetailPart.RegistrationYear, objIntermediaryDetailPart.GrievanceID);
            ViewData["AppealDocList"] = list;
            ViewData["AppealGroundMaster"] = objcm.getAppealGroundMaster().OrderBy(t => t.listorder).ToList();
            ViewData["AppealReliefMaster"] = objcm.getAppealReliefMaster().OrderBy(t => t.ReliefId).ToList();
            var IntermediaryList = objcm.fillIntermediaryMaster();
            ViewData["IntermediaryMaster"] = IntermediaryList;
            ViewData["IntermediaryList"] = JsonConvert.SerializeObject(IntermediaryList);
            //var json = new JavaScriptSerializer().Serialize(IntermediaryList);
            //ViewData["IntermediaryJson"] = json.ToString();
            return View(objIntermediaryDetailPart);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NewIntermediaryDetail(mIntermediaryDetailPart objIntermediaryDetail)
        {
            List<IntermediaryMaster> IntermediaryList = new List<IntermediaryMaster>();
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "2", UserSession.LangCulture);
            Dictionary<string, string> LabelDictionary = ViewData["LabelDictionary"] as Dictionary<string, string>;
            if (Session["CitizenEmailMobile"] == null) { return RedirectToAction("Index", "Home", new { Area = "" }); }
            List<mAppealDocument> list = new List<mAppealDocument>();

            //if (ModelState.IsValid)
            //{
            bool haserror = false;
            List<mGroundMaster> listground = objcm.getAppealGroundMaster();
            string errormessage = "";

            list = objcm.fillAppealDocuments(objIntermediaryDetail.RegistrationYear, objIntermediaryDetail.GrievanceID);

            if (objIntermediaryDetail.actionDraftProceed == "Proceed")
            {

                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.txtFirstName))
                {
                    haserror = true;
                    errormessage = LabelDictionary["Pleaseentername"];
                    ModelState.AddModelError("txtFirstName", errormessage);
                }
                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.txtEmail))
                {
                    haserror = true;
                    errormessage = LabelDictionary["Pleaseenteremail"];
                    ModelState.AddModelError("txtEmail", errormessage);
                }
                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.txtIntermediary))
                {
                    haserror = true;
                    errormessage = LabelDictionary["Pleaseselectrespondentintermediarydetail"];
                    ModelState.AddModelError("txtIntermediary", errormessage);
                }
                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.txturlofintermediary))
                {
                    haserror = true;
                    errormessage = LabelDictionary["PleaseIntermediaryURL"];
                    ModelState.AddModelError("txturlofintermediary", errormessage);
                }
                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.GROIntermediaryEmail))
                {
                    haserror = true;
                    errormessage = LabelDictionary["Pleaseenteremailintermediary"];
                    ModelState.AddModelError("GROIntermediaryEmail", errormessage);
                }
                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.Keyword))
                {
                    haserror = true;
                    errormessage = LabelDictionary["PleaseenterSubject"];
                    ModelState.AddModelError("Keyword", errormessage);
                }

                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.BriefofComplaint))
                {
                    haserror = true;
                    errormessage = LabelDictionary["PleaseenterBriefofComplaint"];
                    ModelState.AddModelError("BriefofComplaint", errormessage);
                }
                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.txtJustification))
                {
                    haserror = true;
                    errormessage = LabelDictionary["PleaseenterJustification"];
                    ModelState.AddModelError("txtJustification", errormessage);
                }
                if (string.IsNullOrWhiteSpace(objIntermediaryDetail.ReliefSoughtSpecification))
                {
                    haserror = true;
                    errormessage = LabelDictionary["PleaseenterReliefSought"];
                    ModelState.AddModelError("ReliefSoughtSpecification", errormessage);
                }
                if (list != null)
                {
                    var tmplist = list.Where(x => x.DocumentType == "CC" && x.EvidenceType == "F").ToList();
                    if (tmplist.Count == 0)
                    {
                        haserror = true;
                        errormessage = LabelDictionary["PleaseuploadCopyofComplaint"];
                        ModelState.AddModelError("CopyofComplaint", errormessage);
                    }
                }
                //if (list != null)
                //{
                //    var tmplist = list.Where(x => x.DocumentType == "CD" && x.EvidenceType == "F").ToList();
                //    if (tmplist.Count == 0)
                //    {
                //        haserror = true;
                //        errormessage = "Please upload Copy of Decision";
                //        ModelState.AddModelError("CopyofDecision", errormessage);
                //    }
                //}


            }
            if (!string.IsNullOrEmpty(objIntermediaryDetail.txtJustification))
            {
                if (objIntermediaryDetail.txtJustification.Trim().Split(' ').Length > 250)
                {
                    haserror = true;
                    errormessage = LabelDictionary["Max250JustificationValidation"];
                    ModelState.AddModelError("txtJustification", errormessage);
                }
            }
            if (objIntermediaryDetail.IntermediaryId == "0")
            {
                DataTable dt = new DataTable();
                var methodParameter1 = new List<KeyValuePair<string, string>>();
                methodParameter1.Add(new KeyValuePair<string, string>("@title", objIntermediaryDetail.txtIntermediary));
                methodParameter1.Add(new KeyValuePair<string, string>("@url", objIntermediaryDetail.txturlofintermediary));
                methodParameter1.Add(new KeyValuePair<string, string>("@email", objIntermediaryDetail.GROIntermediaryEmail));
                DataSet ds1 = ServiceAdaptor.GetDataSetFromService("HPPSC", "DigitalNagrikConnStr", "SelectMSSql", "CheckIntermediaryEntry", methodParameter1);
                if (ds1 != null)
                {
                    if (ds1.Tables.Count > 0)
                    {
                        DataTable dt1 = ds1.Tables[0];
                        if (Int16.Parse(dt1.Rows[0]["errorcode"].ToString()) >= 1 && Int16.Parse(dt1.Rows[0]["errorcode"].ToString()) <= 9)
                        {
                            haserror = true;
                            errormessage = ds1.Tables[0].Rows[0]["msg"].ToString();
                            ModelState.AddModelError("", errormessage);

                        }
                    }
                }
            }
            if (haserror == true)
            {
                //ModelState.AddModelError("", errormessage);
                checkMessage("ErrorFinalSubmit", errormessage);
                //if (!string.IsNullOrEmpty(objIntermediaryDetail.RegistrationYear) & !string.IsNullOrEmpty(objIntermediaryDetail.GrievanceID))
                //{
                //    objIntermediaryDetail = objcm.fillIntermediaryDetailPart(Session["CitizenEmailMobile"].ToString(), objIntermediaryDetail.RegistrationYear, objIntermediaryDetail.GrievanceID);
                //}
                ViewData["DocumentFileType"] = objcm.fillDocumentFileTypeMaster();
                ViewData["KeyWords"] = objcm.GetGroundKeywords();

                ViewData["AppealDocList"] = list;
                ViewData["AppealGroundMaster"] = objcm.getAppealGroundMaster().OrderBy(t => t.listorder).ToList();
                ViewData["AppealReliefMaster"] = objcm.getAppealReliefMaster().OrderBy(t => t.ReliefId).ToList();
                 IntermediaryList = objcm.fillIntermediaryMaster();
                ViewData["IntermediaryMaster"] = IntermediaryList;
                ViewData["IntermediaryList"] = JsonConvert.SerializeObject(IntermediaryList);
                return View("NewIntermediaryDetail", objIntermediaryDetail);
            }

            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserMobile", objIntermediaryDetail.MobileNo.ToString()));
            methodParameter.Add(new KeyValuePair<string, string>("@UserEmail", (objIntermediaryDetail.txtEmail == null ? "" : objIntermediaryDetail.txtEmail.ToString().ToUpper())));
            methodParameter.Add(new KeyValuePair<string, string>("@FirstName", objIntermediaryDetail.txtFirstName.ToString().ToUpper()));
            methodParameter.Add(new KeyValuePair<string, string>("@MiddleName", (objIntermediaryDetail.txtMiddleName == null ? "" : objIntermediaryDetail.txtMiddleName.ToString().ToUpper())));
            methodParameter.Add(new KeyValuePair<string, string>("@LastName", (objIntermediaryDetail.txtLastName == null ? "" : objIntermediaryDetail.txtLastName.ToString().ToUpper())));
            methodParameter.Add(new KeyValuePair<string, string>("@IntermediaryTitle", (objIntermediaryDetail.txtIntermediary == null) ? "" : objIntermediaryDetail.txtIntermediary.ToString().ToUpper()));
            methodParameter.Add(new KeyValuePair<string, string>("@URL", (objIntermediaryDetail.txturlofintermediary == null ? "" : objIntermediaryDetail.txturlofintermediary.ToString())));
            methodParameter.Add(new KeyValuePair<string, string>("@IntermediaryId", (objIntermediaryDetail.IntermediaryId == null) ? "0" : objIntermediaryDetail.IntermediaryId));

            methodParameter.Add(new KeyValuePair<string, string>("@IntermediaryGROEmail", objIntermediaryDetail.GROIntermediaryEmail == null ? "" : objIntermediaryDetail.GROIntermediaryEmail.ToString().ToUpper()));
            methodParameter.Add(new KeyValuePair<string, string>("@IntermediaryAddress", (objIntermediaryDetail.txtCorrespondenceAddress == null ? "" : objIntermediaryDetail.txtCorrespondenceAddress.ToString().ToUpper())));
            //methodParameter.Add(new KeyValuePair<string, string>("@GroundAppealID", objIntermediaryDetail.ddlGroundAppeal.ToString()));
            methodParameter.Add(new KeyValuePair<string, string>("@GroundTitleText", (objIntermediaryDetail.GroundTitleText == null ? "" : objIntermediaryDetail.GroundTitleText.ToString().ToUpper())));
            methodParameter.Add(new KeyValuePair<string, string>("@Justification", (objIntermediaryDetail.txtJustification == null ? "" : objIntermediaryDetail.txtJustification.ToString().Trim().ToUpper())));


            methodParameter.Add(new KeyValuePair<string, string>("@ReliefSpecification", objIntermediaryDetail.ReliefSoughtSpecification == null ? "" : objIntermediaryDetail.ReliefSoughtSpecification.ToString().Trim().ToUpper()));

            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", (objIntermediaryDetail.RegistrationYear == null) ? "" : objIntermediaryDetail.RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", (objIntermediaryDetail.GrievanceID == null) ? "" : objIntermediaryDetail.GrievanceID));
            methodParameter.Add(new KeyValuePair<string, string>("@UserID", Session["UserID"].ToString()));
            methodParameter.Add(new KeyValuePair<string, string>("@dateofComplaint", (objIntermediaryDetail.txtDateofComplaint == null) ? "" : objIntermediaryDetail.txtDateofComplaint));
            methodParameter.Add(new KeyValuePair<string, string>("@dateofDecision", (objIntermediaryDetail.txtdateofDecision == null) ? "" : objIntermediaryDetail.txtdateofDecision));
            methodParameter.Add(new KeyValuePair<string, string>("@TicketDocketNumber", (objIntermediaryDetail.TicketDocketNumber == null) ? "" : objIntermediaryDetail.TicketDocketNumber));
            methodParameter.Add(new KeyValuePair<string, string>("@BriefofComplaint", (objIntermediaryDetail.BriefofComplaint == null) ? "" : objIntermediaryDetail.BriefofComplaint.Trim().ToUpper()));
            methodParameter.Add(new KeyValuePair<string, string>("@Keyword", (objIntermediaryDetail.Keyword == null) ? "" : objIntermediaryDetail.Keyword.Trim()));
            methodParameter.Add(new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "insertUpdateAppealChange_new", methodParameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows[0]["statuscode"].ToString() == "200")
                {
                    objIntermediaryDetail.RegistrationYear = ds.Tables[0].Rows[0]["RegistrationYear"].ToString();
                    Session["RegistrationYear"] = objIntermediaryDetail.RegistrationYear;
                    objIntermediaryDetail.GrievanceID = ds.Tables[0].Rows[0]["GrievanceID"].ToString();
                    Session["GrievanceId"] = objIntermediaryDetail.GrievanceID;
                    objcm.createFTPPath(objIntermediaryDetail.RegistrationYear, objIntermediaryDetail.GrievanceID);

                    if (objIntermediaryDetail.actionDraftProceed == "Draft")
                    {
                        return RedirectToAction("NewIntermediaryDetail", "Registration", new { qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + objIntermediaryDetail.RegistrationYear + "&GrievanceId=" + objIntermediaryDetail.GrievanceID), Message = "DraftSuccess", from = "dashboard" });
                    }
                    else
                    {
                        return RedirectToAction("IntermediaryDetailPreview", "Registration", new { qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + objIntermediaryDetail.RegistrationYear + "&GrievanceId=" + objIntermediaryDetail.GrievanceID), Message = "IntermediarySuccess" });
                    }
                }
                else
                {
                    ViewData["ErrorMessage"] = ds.Tables[0].Rows[0]["statusMessage"].ToString();
                    ModelState.AddModelError("", ds.Tables[0].Rows[0]["statusMessage"].ToString());
                    ViewData["KeyWords"] = objcm.GetGroundKeywords();
                    
                    ViewData["DocumentFileType"] = objcm.fillDocumentFileTypeMaster();
                    list = new List<mAppealDocument>();
                    list = objcm.fillAppealDocuments(objIntermediaryDetail.RegistrationYear, objIntermediaryDetail.GrievanceID);
                    ViewData["AppealDocList"] = list;
                    ViewData["AppealGroundMaster"] = objcm.getAppealGroundMaster().OrderBy(t => t.listorder).ToList();
                    ViewData["AppealReliefMaster"] = objcm.getAppealReliefMaster().OrderBy(t => t.ReliefId).ToList();
                     IntermediaryList = objcm.fillIntermediaryMaster();
                    ViewData["IntermediaryMaster"] = IntermediaryList;
                    ViewData["IntermediaryList"] = JsonConvert.SerializeObject(IntermediaryList);
                    return View("NewIntermediaryDetail", objIntermediaryDetail);
                }
            }


            // }


            ModelState.AddModelError("", LabelDictionary["ErroronSavingAppeal"]);

            if (!string.IsNullOrEmpty(objIntermediaryDetail.RegistrationYear) & !string.IsNullOrEmpty(objIntermediaryDetail.GrievanceID))
            {
                objIntermediaryDetail = objcm.fillIntermediaryDetailPart(Session["CitizenEmailMobile"].ToString(), objIntermediaryDetail.RegistrationYear, objIntermediaryDetail.GrievanceID);
            }
            ViewData["KeyWords"] = objcm.GetGroundKeywords();
            ViewData["DocumentFileType"] = objcm.fillDocumentFileTypeMaster();
            list = new List<mAppealDocument>();
            list = objcm.fillAppealDocuments(objIntermediaryDetail.RegistrationYear, objIntermediaryDetail.GrievanceID);
            ViewData["AppealDocList"] = list;
            ViewData["AppealGroundMaster"] = objcm.getAppealGroundMaster().OrderBy(t => t.listorder).ToList();
            ViewData["AppealReliefMaster"] = objcm.getAppealReliefMaster().OrderBy(t => t.ReliefId).ToList();
             IntermediaryList = objcm.fillIntermediaryMaster();
            ViewData["IntermediaryMaster"] = IntermediaryList;
            ViewData["IntermediaryList"] = JsonConvert.SerializeObject(IntermediaryList);
            return View("NewIntermediaryDetail", objIntermediaryDetail);
        }



        public ActionResult ViewAppealDocument(string RegistrationYear, string GrievanceID)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "2", UserSession.LangCulture);
            List<mAppealDocument> list = new List<mAppealDocument>();
            ViewData["DocumentFileType"] = objcm.fillDocumentFileTypeMaster();
            list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
            ViewData["AppealDocList"] = list;
            mAppealDocument obj = new mAppealDocument();
            obj.RegistrationYear = RegistrationYear;
            obj.GrievanceID = GrievanceID;
            return PartialView("~/Areas/Public/Views/Registration/_ViewAppealDocument.cshtml", obj);
        }
        [HttpPost]
        public JsonResult ViewAnnexureC(int GoundID)
        {
            List<mAnnexureC> objAnnexureC = new List<mAnnexureC>();
            objAnnexureC = objcm.fillAnnexureC(GoundID);
            return Json(objAnnexureC);
        }
        //public ActionResult ViewUploadDocs(string qs, string Message)
        //{
        //    mAppealDocument objAppealDocument = new mAppealDocument();
        //    string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
        //    if (qs != null)
        //    {
        //        var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
        //        RegistrationYear = Params["RegistrationYear"];
        //        GrievanceID = Params["GrievanceId"];
        //    }
        //    RegistrationYear = (RegistrationYear == null ? "" : RegistrationYear);
        //    GrievanceID = (GrievanceID == null ? "" : GrievanceID);
        //    objAppealDocument.RegistrationYear = RegistrationYear;
        //    objAppealDocument.GrievanceID = GrievanceID;

        //    if (Message != null)
        //    {
        //        checkMessage(Message, "");
        //    }
        //    ViewData["DocumentFileType"] = objcm.fillDocumentFileTypeMaster();
        //    List<mAppealDocument> list = new List<mAppealDocument>();
        //    list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
        //    ViewData["AppealDocList"] = list;
        //    return View(objAppealDocument);
        //}
        public ActionResult UploadDoc(string qs, string DocumentType)
        {

            if (Session["CitizenEmailMobile"] == null) { return RedirectToAction("Index", "Home", new { Area = "" }); }
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "2", UserSession.LangCulture);
            UploadDocument objUploadDocument = new UploadDocument();
            string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
            if (qs != null)
            {
                var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
                RegistrationYear = Params["RegistrationYear"];
                GrievanceID = Params["GrievanceId"];
            }
            objUploadDocument.RegistrationYear = RegistrationYear;
            objUploadDocument.GrievanceID = GrievanceID;
            objUploadDocument.DocumentType = DocumentType;
            objUploadDocument.UserEmailMobile = Session["CitizenEmailMobile"].ToString();
            ViewData["DocumentFileType"] = objcm.fillDocumentFileTypeMaster();
            ViewData["AppealDocumentType"] = DocumentType;
            return PartialView("~/Areas/Public/Views/Registration/_UploadAppealDoc.cshtml", objUploadDocument);
        }
        public static bool IsPasswordProtected(byte[] fileBytes)
        {
            try
            {
                PdfReader pdfReader = new PdfReader(fileBytes);
                return false;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return true;
            }
        }
        [HttpPost]
        public JsonResult UploadAppealDocs(UploadDocument uploadDocument)
        {
            string returnMessage = "ErrorFileUpload";
            string FileType = "", FileName = "";
            string FileTypeID = "0";
            var file = "";
            if (uploadDocument.fileUpload != null)
            {        
                byte[] fileBytes = null;
                List<DocumentFileTypeMaster> fileExtList = objcm.fillDocumentFileTypeMaster();
                FileType = Path.GetExtension(uploadDocument.fileUpload.FileName);

                FileName = uploadDocument.fileUpload.FileName;
                if (ValidateFileAttribute.CheckMimeType(FileType.Replace(".",""), uploadDocument.fileUpload))
                {
                    if (!ValidateFileAttribute.IsFileTypeValid(uploadDocument.fileUpload))
                    {
                        file = "NotValidExtension";
                    }
                    else
                    {
                        if (objcm.checkExtension(fileExtList, FileType.Replace(".", "").ToLower(), uploadDocument.DocumentType))
                        {
                            Int32 filesize = uploadDocument.fileUpload.ContentLength;
                            if (objcm.checkFileSize(fileExtList, uploadDocument.DocumentType, filesize))
                            {

                                using (var binaryReaderPDF = new System.IO.BinaryReader(uploadDocument.fileUpload.InputStream))
                                {
                                    fileBytes = binaryReaderPDF.ReadBytes(uploadDocument.fileUpload.ContentLength);
                                }
                                if (FileType.Replace(".", "").ToLower() == "pdf")
                                {
                                    if (IsPasswordProtected(fileBytes))
                                    {
                                        return Json("InvalidFile");
                                    }
                                }
                                if (FileType.ToLower().Contains("pdf")) { FileTypeID = "4"; } else { FileTypeID = "1"; }
                                List<mAppealDocument> list = new List<mAppealDocument>();
                                list = objcm.fillAppealDocuments(uploadDocument.RegistrationYear, uploadDocument.GrievanceID);
                                if (fileBytes != null)
                                {

                                    if (list.Count == 0)
                                    {
                                        FileName = uploadDocument.DocumentType + "1" + FileType;
                                    }
                                    else
                                    {
                                        var cnt = Convert.ToInt32(list.LastOrDefault().FileId) + 1;
                                        FileName = uploadDocument.DocumentType + cnt.ToString() + FileType;
                                    }



                                    file = objcm.makeFTPPath(uploadDocument.RegistrationYear, uploadDocument.GrievanceID, uploadDocument.DocumentType) + FileName;
                                    file = replaceCharacter(file);
                                    var tmp = list.Where(x => x.DocumentType == uploadDocument.DocumentType && x.EvidenceType == "F").ToList();
                                    FTPHelper objFTPHerlper = new FTPHelper();
                                    if (tmp.Count > 0)
                                    {
                                        if (uploadDocument.DocumentType == "CC")
                                        {
                                            if (tmp.Count >= Int32.Parse(fileExtList.Where(x => x.AppealDocumentType == "CC").Select(x => x.UploadLimit).FirstOrDefault()))
                                            {
                                                objFTPHerlper.DeleteFile(tmp.FirstOrDefault().FilePath);
                                                objcm.deleteAppealDocument(uploadDocument.RegistrationYear, uploadDocument.GrievanceID, tmp.FirstOrDefault().FileId);
                                            }
                                        }
                                        else
                                        {
                                            objFTPHerlper.DeleteFile(tmp.FirstOrDefault().FilePath);
                                            objcm.deleteAppealDocument(uploadDocument.RegistrationYear, uploadDocument.GrievanceID, tmp.FirstOrDefault().FileId);
                                        }
                                    }
                                    if (objFTPHerlper.FileExists(file))
                                    {
                                        file = "AlreadyFileExists";
                                    }
                                    else
                                    {
                                        Boolean res = objFTPHerlper.saveFTPfile(fileBytes, "/" + file);

                                        if (res == true)
                                        {
                                            objcm.saveAppealDocument(uploadDocument.RegistrationYear, uploadDocument.GrievanceID, "F", file, FileTypeID, FileType, uploadDocument.DocumentType);
                                            file = "SuccessFileUpload";
                                        }
                                        else
                                        {
                                            file = "InvalidFile";
                                        }
                                    }
                                    //Boolean res = objFTPHerlper.saveFTPfile(fileBytes, "/" + file);

                                    //if (res == true)
                                    //{
                                    //    objcm.saveAppealDocument(uploadDocument.RegistrationYear, uploadDocument.GrievanceID, "F", file, FileTypeID, FileType, uploadDocument.DocumentType);
                                    //    file = "SuccessFileUpload";
                                    //}
                                    //else
                                    //{
                                    //    file = "InvalidFile";
                                    //}

                                }

                            }
                            else { file = "MaxFilesize"; }

                        }
                        else { file = "NotValidExtension"; }
                    }
                
                }
                else { file = "NotValidExtension"; }
            }


            return Json(file);
        }
        [HttpPost]
        public ActionResult SaveURL(UploadDocument uploadDocument)
        {
            string returnMessage = "ErrorURLUpload";
            if (uploadDocument.IURL != null)
            {
                objcm.saveAppealDocument(uploadDocument.RegistrationYear, uploadDocument.GrievanceID, "U", uploadDocument.IURL.ToUpper(), "0", "", "IURL");
                returnMessage = "URLSavedSuccess";
            }
            return Json(returnMessage);
        }
        [HttpPost]
        public JsonResult DeleteFile(string qs, string DocumentType, string FileID)
        {
            string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
            string returnMessage = "SuccessFileDelete";
            if (qs != null)
            {
                var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
                RegistrationYear = Params["RegistrationYear"];
                GrievanceID = Params["GrievanceId"];
            }
            if (DocumentType == "IURL")
            {
                objcm.deleteAppealDocument(RegistrationYear, GrievanceID, FileID);
                returnMessage = "SuccessURLDelete";
            }
            else
            {
                try
                {

                    List<mAppealDocument> list = new List<mAppealDocument>();
                    list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
                    FTPHelper objFTPHelper = new FTPHelper();
                    var tmp = list.Where(x => x.FileId == FileID && x.EvidenceType == "F").ToList();
                    objFTPHelper.DeleteFile(tmp.FirstOrDefault().FilePath);
                    objcm.deleteAppealDocument(RegistrationYear, GrievanceID, tmp.FirstOrDefault().FileId);
                }
                catch (Exception)
                {
                    returnMessage = "ErrorFileDelete";
                }
            }
            return Json(returnMessage);
        }
        //public ActionResult ProceedUploadDoc(mAppealDocument objmAppealDocument)
        //{
        //    string returnMessage = "FileMandatory";
        //    bool haserror = false;


        //    List<mAppealDocument> list = new List<mAppealDocument>();
        //    list = objcm.fillAppealDocuments(objmAppealDocument.RegistrationYear, objmAppealDocument.GrievanceID);
        //    var tmplist = list.Where(x => x.DocumentType == "CC" && x.EvidenceType == "F").ToList();
        //    if (tmplist.Count <= 0)
        //    {
        //        haserror = true;
        //    }
        //    tmplist = list.Where(x => x.DocumentType == "CD" && x.EvidenceType == "F").ToList();
        //    if (tmplist.Count <= 0)
        //    {
        //        haserror = true;
        //    }
        //    if (haserror)
        //    {
        //        return RedirectToAction("ViewUploadDocs", "Registration", new { qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + objmAppealDocument.RegistrationYear + "&GrievanceId=" + objmAppealDocument.GrievanceID), Message = returnMessage });
        //    }
        //    else
        //    {
        //        returnMessage = "DocumentSuccess";
        //        return RedirectToAction("EvidenceDetail", "Registration", new { qs = SecureQueryString.SecureQueryString.Encrypt("RegistrationYear=" + objmAppealDocument.RegistrationYear + "&GrievanceId=" + objmAppealDocument.GrievanceID), Message = returnMessage });
        //    }
        //}

        public ActionResult IntermediaryDetailPreview(string qs, string Message)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "3", UserSession.LangCulture);
            if (Message != null)
            {
                checkMessage(Message, "");
            }
            string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
            var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
            RegistrationYear = Params["RegistrationYear"];
            GrievanceID = Params["GrievanceId"];
            var objIntermediaryDetailPreview = new mIntermediaryDetailPart();
            if (Session["CitizenEmailMobile"] == null)
                return RedirectToAction("Index", "Home", new { Area = "" });

            objIntermediaryDetailPreview.MobileNo = Session["CitizenEmailMobile"].ToString();
            RegistrationYear = (RegistrationYear == null ? "" : RegistrationYear);
            GrievanceID = (GrievanceID == null ? "" : GrievanceID);
            objIntermediaryDetailPreview.RegistrationYear = RegistrationYear;
            objIntermediaryDetailPreview.GrievanceID = GrievanceID;
            if (!string.IsNullOrEmpty(RegistrationYear) & !string.IsNullOrEmpty(GrievanceID))
            {
                objIntermediaryDetailPreview = objcm.fillIntermediaryDetailPart(Session["CitizenEmailMobile"].ToString(), RegistrationYear, GrievanceID);
            }

            List<mAppealDocument> list = new List<mAppealDocument>();
            list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
            ViewData["AppealDocList"] = list;
            return View(objIntermediaryDetailPreview);

        }
        [HttpPost]
        public ActionResult IntermediaryFinalSubmit(mIntermediaryDetailPart objDetailPreview)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "3", UserSession.LangCulture);
            Dictionary<string, string> LabelDictionary = ViewData["LabelDictionary"] as Dictionary<string, string>;
            bool haserror = false;
            string errormessage = "";
            var mindate = DateTime.Now.AddDays(-29).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(objDetailPreview.RegistrationYear) & !string.IsNullOrEmpty(objDetailPreview.GrievanceID))
            {
                objDetailPreview = objcm.fillIntermediaryDetailPart(Session["CitizenEmailMobile"].ToString(), objDetailPreview.RegistrationYear, objDetailPreview.GrievanceID);
            }
            List<mGroundMaster> listground = objcm.getAppealGroundMaster();
            List<mAppealDocument> list = objcm.fillAppealDocuments(objDetailPreview.RegistrationYear, objDetailPreview.GrievanceID);
            ViewData["AppealDocList"] = list;
            if (string.IsNullOrWhiteSpace(objDetailPreview.txtFirstName))
            {
                haserror = true;
                errormessage = LabelDictionary["Pleaseentername"];
                ModelState.AddModelError("txtFirstName", errormessage);
            }
            if (string.IsNullOrWhiteSpace(objDetailPreview.txtEmail))
            {
                haserror = true;
                errormessage = LabelDictionary["Pleaseenteremail"];
                ModelState.AddModelError("txtEmail", errormessage);
            }
            if (string.IsNullOrWhiteSpace(objDetailPreview.txtIntermediary))
            {
                haserror = true;
                errormessage = LabelDictionary["Pleaseselectrespondentintermediarydetail"];
                ModelState.AddModelError("txtIntermediary", errormessage);
            }
            if (string.IsNullOrWhiteSpace(objDetailPreview.txturlofintermediary))
            {
                haserror = true;
                errormessage = LabelDictionary["PleaseIntermediaryURL"];
                ModelState.AddModelError("txturlofintermediary", errormessage);
            }
            if (string.IsNullOrWhiteSpace(objDetailPreview.GROIntermediaryEmail))
            {
                haserror = true;
                errormessage = LabelDictionary["Pleaseenteremailintermediary"];
                ModelState.AddModelError("GROIntermediaryEmail", errormessage);
            }
            if (string.IsNullOrWhiteSpace(objDetailPreview.Keyword))
            {
                haserror = true;
                errormessage = LabelDictionary["PleaseenterSubject"];
                ModelState.AddModelError("Keyword", errormessage);
            }

            if (string.IsNullOrWhiteSpace(objDetailPreview.BriefofComplaint))
            {
                haserror = true;
                errormessage = LabelDictionary["PleaseenterBriefofComplaint"];
                ModelState.AddModelError("BriefofComplaint", errormessage);
            }
            if (string.IsNullOrWhiteSpace(objDetailPreview.txtJustification))
            {
                haserror = true;
                errormessage = LabelDictionary["PleaseenterJustification"];
                ModelState.AddModelError("txtJustification", errormessage);
            }
            if (string.IsNullOrWhiteSpace(objDetailPreview.ReliefSoughtSpecification))
            {
                haserror = true;
                errormessage = LabelDictionary["PleaseenterReliefSought"];
                ModelState.AddModelError("ReliefSoughtSpecification", errormessage);
            }
            if (list != null)
            {
                var tmplist = list.Where(x => x.DocumentType == "CC" && x.EvidenceType == "F").ToList();
                if (tmplist.Count == 0)
                {
                    haserror = true;
                    errormessage = LabelDictionary["PleaseuploadCopyofComplaint"];
                    ModelState.AddModelError("CopyofComplaint", errormessage);
                }
            }
            //if (list != null)
            //{
            //    var tmplist = list.Where(x => x.DocumentType == "CD" && x.EvidenceType == "F").ToList();
            //    if (tmplist.Count == 0)
            //    {
            //        haserror = true;
            //        errormessage = "Please upload Copy of Decision";
            //        ModelState.AddModelError("CopyofDecision", errormessage);
            //    }
            //}

            if (haserror == true)
            {
                ModelState.AddModelError("", errormessage);
                string Message = "ErrorFinalSubmit";
                checkMessage(Message, errormessage);
                return View("IntermediaryDetailPreview", objDetailPreview);
            }
            if (objDetailPreview.IntermediaryId == "0")
            {
                DataTable dt = new DataTable();
                var methodParameter1 = new List<KeyValuePair<string, string>>();
                methodParameter1.Add(new KeyValuePair<string, string>("@title", objDetailPreview.txtIntermediary));
                methodParameter1.Add(new KeyValuePair<string, string>("@url", objDetailPreview.txturlofintermediary));
                methodParameter1.Add(new KeyValuePair<string, string>("@email", objDetailPreview.GROIntermediaryEmail));
                DataSet ds1 = ServiceAdaptor.GetDataSetFromService("HPPSC", "DigitalNagrikConnStr", "SelectMSSql", "CheckIntermediaryEntry", methodParameter1);
                if (ds1 != null)
                {
                    if (ds1.Tables.Count > 0)
                    {
                        DataTable dt1 = ds1.Tables[0];
                        if (Int16.Parse(dt1.Rows[0]["errorcode"].ToString()) >= 1 && Int16.Parse(dt1.Rows[0]["errorcode"].ToString()) <= 9)
                        {
                            errormessage = ds1.Tables[0].Rows[0]["msg"].ToString();
                            ModelState.AddModelError("", errormessage);
                            string Message = "ErrorFinalSubmit";
                            checkMessage(Message, errormessage);
                            return View("IntermediaryDetailPreview", objDetailPreview);
                        }
                    }
                }
            }
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", objDetailPreview.RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", objDetailPreview.GrievanceID));
            methodParameter.Add(new KeyValuePair<string, string>("@UserMobile", objDetailPreview.MobileNo));
            methodParameter.Add(new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "FinalSubmitAppeal", methodParameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows[0]["statusMessage"].ToString() == "Saved")
                {
                    Session["FinalSubmitSuccess"] = "Appeal No: " + objDetailPreview.GrievanceID + "/" + objDetailPreview.RegistrationYear + " against intermediary " + objDetailPreview.txtIntermediary + " is submitted successfully";
                    string citizenEmail = "", IntermediaryGROEmail = "";
                    citizenEmail = ds.Tables[0].Rows[0]["emailCitizen"].ToString();
                    IntermediaryGROEmail = ds.Tables[0].Rows[0]["IntermediaryGROEmail"].ToString();
                    objDetailPreview.SubmitDate = ds.Tables[0].Rows[0]["SubmitDate"].ToString();
                    objDetailPreview.ReceiptDate = ds.Tables[0].Rows[0]["ReceiptDate"].ToString();
                    objDetailPreview.ReceiptDateTime = ds.Tables[0].Rows[0]["ReceiptDateTime"].ToString();
                    DigitalNagrik.Models.SendSMS smsObj = new DigitalNagrik.Models.SendSMS();
                    string emailMsg = "";
                    string emailSubject = "";
                    //Send SMS to Appellant
                    SendSMS objmSendSMS = new SendSMS();

                    objmSendSMS.send_SMS(objDetailPreview.MobileNo, ds.Tables[0].Rows[0]["MobileMessage"].ToString(), ds.Tables[0].Rows[0]["TemplateID"].ToString(), "S");
                    // send Mail to complainant 
                    if (!string.IsNullOrEmpty(citizenEmail))
                    {
                        objDetailPreview.RegistrationYear = objDetailPreview.RegistrationYear;
                        objDetailPreview.GrievanceID = objDetailPreview.GrievanceID;

                        emailMsg = ConvertViewToString("~/Areas/Public/Views/Message/_SendMailFinalContent.cshtml", objDetailPreview);
                        emailSubject = "Grievance Appellate Committee: Appeal number " + objDetailPreview.GrievanceID + "/" + objDetailPreview.RegistrationYear + "  is Submitted. ";


                        smsObj.SendMailToActualUser(emailMsg, emailSubject, citizenEmail, "Grievance Appellate Committee", citizenEmail);
                    }

                    // send mail to intermediary
                    if (!string.IsNullOrEmpty(IntermediaryGROEmail))
                    {
                        emailMsg = ConvertViewToString("~/Areas/Public/Views/Message/_SendMailtoIntermediary.cshtml", objDetailPreview);
                        emailSubject = "Grievance Appellate Committee: Appeal number " + objDetailPreview.GrievanceID + "/" + objDetailPreview.RegistrationYear + " from " + objDetailPreview.CitizenName + " against " + objDetailPreview.txtIntermediary;
                        smsObj.SendMailToUser(emailMsg, emailSubject, IntermediaryGROEmail, "Grievance Appellate Committee", IntermediaryGROEmail);
                    }
                    return RedirectToAction("CitizenDashboard", "Registration", new { Message = "FinalSubmitSuccess" });

                }
                else
                {
                    if (!string.IsNullOrEmpty(objDetailPreview.RegistrationYear) & !string.IsNullOrEmpty(objDetailPreview.GrievanceID))
                    {
                        objDetailPreview = objcm.fillIntermediaryDetailPart(Session["CitizenEmailMobile"].ToString(), objDetailPreview.RegistrationYear, objDetailPreview.GrievanceID);
                    }
                    ModelState.AddModelError("", "An error occured while submitting..");
                    string Message = "ErrorFinalSubmit";
                    checkMessage(Message, "An error occured while submitting..");
                    return View("IntermediaryDetailPreview", objDetailPreview);
                }

            }
            else
            {

                ModelState.AddModelError("", "An error occured while submitting..");
                return View("IntermediaryDetailPreview", objDetailPreview);
            }

        }
        public ActionResult AppealPDF(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "4", UserSession.LangCulture);
            byte[] MergedPDFBytes = null;
            var objIntermediaryDetailPreview = new mIntermediaryDetailPart();
            try
            {
                string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
                var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
                RegistrationYear = Params["RegistrationYear"];
                GrievanceID = Params["GrievanceId"];


                objIntermediaryDetailPreview.MobileNo = Convert.ToString(Session["CitizenEmailMobile"]);
                RegistrationYear = (RegistrationYear == null ? "" : RegistrationYear);
                GrievanceID = (GrievanceID == null ? "" : GrievanceID);
                objIntermediaryDetailPreview.RegistrationYear = RegistrationYear;
                objIntermediaryDetailPreview.GrievanceID = GrievanceID;
                if (!string.IsNullOrEmpty(RegistrationYear) & !string.IsNullOrEmpty(GrievanceID))
                {
                    objIntermediaryDetailPreview = objcm.fillIntermediaryDetailPart(Convert.ToString(Session["CitizenEmailMobile"]), RegistrationYear, GrievanceID);
                }

                List<mAppealDocument> list = new List<mAppealDocument>();
                list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
                ViewData["AppealDocList"] = list;
                ViewAsPdf ApplicationPDF = new ViewAsPdf("AppealDetailasPDF", objIntermediaryDetailPreview)
                {
                    FileName = "AppealPDF.pdf",
                    PageMargins = { Left = 8, Right = 8 },
                    CustomSwitches = "--print-media-type --footer-right \"Appeal details as on - [date]\" --footer-left \"Grievance Appellate Committee\" --footer-center \"Page [page] of [topage] \" --footer-font-size 8",
                    PageSize = Rotativa.Options.Size.A4,
                    PageOrientation = Rotativa.Options.Orientation.Portrait
                };
                List<byte[]> PDFBytesList = new List<byte[]>();

                try
                {
                    PDFBytesList.Add(ApplicationPDF.BuildPdf(ControllerContext));
                    if (list.Count > 0)
                    {
                        string oldfiletype = "";
                        foreach (mAppealDocument m in list.OrderBy(x => x.FileId))
                        {
                            if (m.DocumentType == "CC")
                            {

                                if (oldfiletype != m.DocumentType)
                                {
                                    oldfiletype = m.DocumentType;
                                    PDFBytesList.Add(texttoPDFByte1("<br/><br/><br/><br/><br/> <br/><br/> <br/><br/> <div style='font-size:30px;text-align:center;height:600px;margin-top: 160px;'>Copy of Complaint</div>"));
                                }

                                if (m.FileType.Contains("pdf"))
                                {
                                    byte[] filebyte = ViewDocumentasbyte(m.FilePath, m.FileType);
                                    if (filebyte.Length > 100)
                                    {
                                        PDFBytesList.Add(filebyte);
                                    }
                                    //PDFBytesList.Add(ViewDocumentasbyte(m.FilePath, m.FileType));
                                }
                                else
                                {
                                    byte[] filebyte = ImagesToPdf(ResizeImage(ViewDocumentasbyte(m.FilePath, m.FileType), 500));
                                    if (filebyte.Length > 100)
                                    {
                                        PDFBytesList.Add(filebyte);
                                    }
                                    //PDFBytesList.Add(ImagesToPdf(ResizeImage(ViewDocumentasbyte(m.FilePath, m.FileType), 500)));
                                }
                            }
                            else if (m.DocumentType == "CD")
                            {
                                PDFBytesList.Add(texttoPDFByte1("<br/><br/><br/><br/><br/> <br/><br/> <br/><br/> <div style='font-size:30px;text-align:center;height:600px;margin-top: 160px;'>Any other relevant information </div>"));
                                if (m.FileType.Contains("pdf"))
                                {
                                    byte[] filebyte = ViewDocumentasbyte(m.FilePath, m.FileType);
                                    if (filebyte.Length > 100)
                                    {
                                        PDFBytesList.Add(filebyte);
                                    }
                                    //PDFBytesList.Add(ViewDocumentasbyte(m.FilePath, m.FileType));
                                }
                                else
                                {
                                    byte[] filebyte = ImagesToPdf(ResizeImage(ViewDocumentasbyte(m.FilePath, m.FileType), 500));
                                    if (filebyte.Length > 100)
                                    {
                                        PDFBytesList.Add(filebyte);
                                    }
                                    //PDFBytesList.Add(ImagesToPdf(ResizeImage(ViewDocumentasbyte(m.FilePath, m.FileType), 500)));
                                }
                            }
                            //else if (m.DocumentType == "PC")
                            //{
                            //    PDFBytesList.Add(texttoPDFByte1("<br/><br/><br/><br/><br/> <br/><br/> <br/><br/> <div style='font-size:30px;text-align:center;line-height: 40px;height:600px;margin-top: 160px;'>Proof of making of Complaint</div>"));
                            //    if (m.FileType.Contains("pdf"))
                            //    {
                            //        PDFBytesList.Add(ViewDocumentasbyte(m.FilePath, m.FileType));
                            //    }
                            //    else
                            //    {
                            //        PDFBytesList.Add(ImagesToPdf(ResizeImage(ViewDocumentasbyte(m.FilePath, m.FileType), 500)));
                            //    }
                            //}
                            //else if (m.DocumentType == "OTHC")
                            //{
                            //    if (oldfiletype != m.DocumentType)
                            //    {
                            //        oldfiletype = m.DocumentType;
                            //        PDFBytesList.Add(texttoPDFByte1("<br/><br/><br/><br/><br/> <br/><br/> <br/><br/> <div style='font-size:30px;line-height: 40px;text-align:center;height:600px;margin-top: 160px;'>Any other correspondence made with the Grievance Officer </div>"));
                            //    }

                            //    if (m.FileType.Contains("pdf"))
                            //    {
                            //        PDFBytesList.Add(ViewDocumentasbyte(m.FilePath, m.FileType));
                            //    }
                            //    else
                            //    {
                            //        PDFBytesList.Add(ImagesToPdf(ResizeImage(ViewDocumentasbyte(m.FilePath, m.FileType), 500)));
                            //    }
                            //}


                        }
                    }
                    if (PDFBytesList.Count > 0 && PDFBytesList != null)
                    {
                        MergedPDFBytes = MergeDocs(PDFBytesList);
                    }
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            }
            catch (Exception ex) { }

            return MergedPDFBytes != null
               ? File(MergedPDFBytes, "application/pdf")
               : (ActionResult)File(MergedPDFBytes, "application/pdf");
        }
        public byte[] ResizeImage(byte[] data, int width)
        {
            var maxHeight = 500;
            using (var stream = new MemoryStream(data))
            {
                var image = System.Drawing.Image.FromStream(stream);
                var ratioX = (double)width / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);

                //return newImage;
                MemoryStream m = new MemoryStream();
                newImage.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = m.ToArray();
                return imageBytes;
                //var height = (width * image.Height) / image.Width;
                //var thumbnail = image.GetThumbnailImage(width, height, null, IntPtr.Zero);

                //using (var thumbnailStream = new MemoryStream())
                //{
                //    thumbnail.Save(thumbnailStream, System.Drawing.Imaging.ImageFormat.Png);
                //    return thumbnailStream.ToArray();
                //}
            }
        }

        [Obsolete]
        public byte[] texttoPDFByte1(string htmlPdf)
        {
            var str = "<html><head></head><body>" + htmlPdf + "</body></html>";
            byte[] bytes = new byte[] { };
            try
            {
                StringReader sr = new StringReader(htmlPdf);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();

                    htmlparser.Parse(sr);
                    pdfDoc.Close();

                    bytes = memoryStream.ToArray();
                    memoryStream.Close();
                }
            }

            catch (Exception)
            {

                throw;
            }
            return bytes;
        }
        public byte[] ImagesToPdf(byte[] image)
        {

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                //Initialize the PDF document object.
                using (Document pdfDoc = new Document(iTextSharp.text.PageSize.A4, 10f, 10f, 10f, 0f))
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    //Add the Image file to the PDF document object.
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(image);
                    pdfDoc.Add(img);
                    pdfDoc.Close();

                    //Download the PDF file.
                    return stream.ToArray();
                }
            }
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
        public byte[] ViewDocumentasbyte(string FilePath, string FileType)
        {
            byte[] filebyte = new byte[] { };
            //string FilePath = string.Empty; string FileType = string.Empty;
            //var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
            //FilePath = Params["FilePath"];
            //FileType = Params["FileType"];

            string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
            string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
            string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];
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

                var tm = FilePath.Split('0');
                string contentType = "application/" + FileType.ToLower().ToString();
                string fileNameDisplayedToUser = FilePath.Split('/')[2];
                MemoryStream stream = new MemoryStream();
                response.GetResponseStream().CopyTo(stream);
                filebyte = stream.ToArray();
                //using (MemoryStream stream = new MemoryStream())
                //{
                //    //Download the File.
                //    response.GetResponseStream().CopyTo(stream);
                //    Response.AddHeader("content-disposition", "inline;filename=" + fileNameDisplayedToUser);
                //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //    Response.BinaryWrite(stream.ToArray());
                //    Response.End();

                //}
                ftpStream.Close();
                response.Close();
                //return View();
            }
            catch (WebException ex)
            {
                //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
                filebyte = new byte[] { };
            }

            return filebyte;
        }

        public ActionResult AppealPDFOLD(string qs)
        {
            try
            {
                string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
                var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
                RegistrationYear = Params["RegistrationYear"];
                GrievanceID = Params["GrievanceId"];
                var objIntermediaryDetailPreview = new mIntermediaryDetailPart();
                //if (Session["CitizenEmailMobile"] == null)return RedirectToAction("Index", "Home", new { Area = "" });
                //StringBuilder str = new StringBuilder();
                objIntermediaryDetailPreview.MobileNo = Convert.ToString(Session["CitizenEmailMobile"]);
                RegistrationYear = (RegistrationYear == null ? "" : RegistrationYear);
                GrievanceID = (GrievanceID == null ? "" : GrievanceID);
                objIntermediaryDetailPreview.RegistrationYear = RegistrationYear;
                objIntermediaryDetailPreview.GrievanceID = GrievanceID;
                if (!string.IsNullOrEmpty(RegistrationYear) & !string.IsNullOrEmpty(GrievanceID))
                {
                    objIntermediaryDetailPreview = objcm.fillIntermediaryDetailPart(Convert.ToString(Session["CitizenEmailMobile"]), RegistrationYear, GrievanceID);
                }

                List<mAppealDocument> list = new List<mAppealDocument>();
                list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
                ViewData["AppealDocList"] = list;

                return new ViewAsPdf("AppealDetailasPDF", objIntermediaryDetailPreview)
                {
                    PageMargins = { Left = 10, Right = 10 },
                    CustomSwitches = "--print-media-type --footer-right \"Appeal details as on - [date]\" --footer-left \"Grievance Appellate Committee\" --footer-center \"Page [page] of [topage] \" --footer-font-size 8"
                };
            }
            catch (Exception ex) { }
            return new EmptyResult();
        }

        
        //public ActionResult DeleteIntermediaryDetail(string qs)
        //{
        //    if (Session["CitizenEmailMobile"] == null)
        //        return RedirectToAction("Index", "Home", new { Area = "" });
        //    string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
        //    var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
        //    RegistrationYear = Params["RegistrationYear"];
        //    GrievanceID = Params["GrievanceId"];
        //    FTPHelper objFTPHerlper = new FTPHelper();
        //    List<mAppealDocument> list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
        //    foreach (mAppealDocument ad in list)
        //    {
        //        if (ad.EvidenceType == "F")
        //            objFTPHerlper.DeleteFile(ad.FilePath);
        //    }

        //    var methodParameter = new List<KeyValuePair<string, string>>();
        //    methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
        //    methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
        //    methodParameter.Add(new KeyValuePair<string, string>("@UserMobile", Session["CitizenEmailMobile"].ToString()));
        //    DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "DeleteAppealCitizen", methodParameter);

        //    return RedirectToAction("CitizenDashboard", "Registration", new { Message = "AppealDeleted" });
        //}
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeleteIntermediaryDetail(string RegistrationYear,string GrievanceID)
        {
            if (Session["CitizenEmailMobile"] == null)
                return RedirectToAction("Index", "Home", new { Area = "" });
            //string RegistrationYear = string.Empty; string GrievanceID = string.Empty;
            //var Params = DigitalNagrik.Models.QueryString.GetDecryptedParameters(qs);
            //RegistrationYear = Params["RegistrationYear"];
            //GrievanceID = Params["GrievanceId"];
            FTPHelper objFTPHerlper = new FTPHelper();
            List<mAppealDocument> list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
            foreach (mAppealDocument ad in list)
            {
                if (ad.EvidenceType == "F")
                    objFTPHerlper.DeleteFile(ad.FilePath);
            }

            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            methodParameter.Add(new KeyValuePair<string, string>("@UserMobile", Session["CitizenEmailMobile"].ToString()));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "DeleteAppealCitizen", methodParameter);

            return RedirectToAction("CitizenDashboard", "Registration", new { Message = "AppealDeleted" });
        }

        public ActionResult CitizenDashboard(string GrievanceStatus, string Message)
        {
            if (UserSession.RoleID.Equals("99"))
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
                GrievanceStatus = (GrievanceStatus == null ? "-1" : GrievanceStatus);
                if (Session["CitizenEmailMobile"] == null) { return RedirectToAction("Index", "Home", new { Area = "" }); }
                if (Message != null)
                {
                    checkMessage(Message, "");
                }
                var objCitizenDashboard = new CitizenDashboard();
                //if (GrievanceStatus == "-1")
                //{
                //    objCitizenDashboard.ListHeaderText = "Appeals - Total";
                //}
                //else if (GrievanceStatus == "1")
                //{
                //    objCitizenDashboard.ListHeaderText = "Appeals - Draft";
                //}
                //else if (GrievanceStatus == "2")
                //{
                //    objCitizenDashboard.ListHeaderText = "Appeals - Under Process";
                //}

                //else if (GrievanceStatus == "4")
                //{
                //    objCitizenDashboard.ListHeaderText = "Appeals - Transferred";
                //}
                //else if (GrievanceStatus == "5")
                //{
                //    objCitizenDashboard.ListHeaderText = "Appeals - Not Admitted";
                //}
                //else if (GrievanceStatus == "6")
                //{
                //    objCitizenDashboard.ListHeaderText = "Appeals - Disposed of";
                //}
                objCitizenDashboard.GrievanceStatus = GrievanceStatus;
                //objCitizenDashboard.OTPFunc = "A";
                var objCitizenDashboardCount = new CitizenDashboardCount();
                objCitizenDashboardCount = objcm.fillDashboardCount(Session["UserID"].ToString());
                objCitizenDashboard.citizenDashboardCount = objCitizenDashboardCount;
                objCitizenDashboard.citizenProfile = objcm.fillCitizenProfile(Session["UserID"].ToString());
                //if (string.IsNullOrEmpty(objCitizenDashboard.citizenProfile.txtFirstName))
                //{
                //    return RedirectToAction("NewIntermediaryDetail", "Registration", new { from = "CheckList" });
                //}
                objCitizenDashboard.listIntermediaryDetail = objcm.fillUserAppealList(Session["CitizenEmailMobile"].ToString(), GrievanceStatus);
                return View(objCitizenDashboard);
            }
            else
            {
                //RedirectToAction("Error", "Home", new { Area = "" });
                return RedirectToAction("Error", "Home", new { Area = "" });
            }      
        }
        [HttpPost]
        public JsonResult fillIntermediary(string IntermediaryTitle, string URL, string func, string IntermediaryId = "")
        {

            var TempList = new List<IntermediaryMaster>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@IntermediaryTitle", IntermediaryTitle));
            methodParameter.Add(new KeyValuePair<string, string>("@URL", URL));
            methodParameter.Add(new KeyValuePair<string, string>("@IntermediaryId", IntermediaryId));
            methodParameter.Add(new KeyValuePair<string, string>("@func", func));

            var ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "getIntermediaryMaster", methodParameter);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TempList.Add(new IntermediaryMaster
                        {
                            IntermediaryId = Convert.ToString(dr["IntermediaryId"]),
                            IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                            URL = Convert.ToString(dr["URL"]),
                            GOEmail = Convert.ToString(dr["GOEmail"]),
                            Address = Convert.ToString(dr["Address"]),
                            GOName = Convert.ToString(dr["GOName"])

                        });
                    }

                }
            }
            return Json(TempList);
        }
        [HttpPost]
        public JsonResult fillGroundforAppeal(string Groundtext, string Func, string GroundId)
        {
            var TempList = new List<mGroundMaster>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@Groundtext", Groundtext));
            methodParameter.Add(new KeyValuePair<string, string>("@Func", Func));

            var ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "getAppealGroundMaster", methodParameter);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TempList.Add(new mGroundMaster
                        {
                            GroundId = Int32.Parse(dr["GroundId"].ToString()),
                            GroundTitle = Convert.ToString(dr["GroundTitle"]),
                            CorrespondingITRule = dr["CorrespondingITRule"].ToString(),
                            EntryRequired = Convert.ToString(dr["EntryRequired"]),
                            EntryFieldLabel = Convert.ToString(dr["EntryFieldLabel"]),
                            listorder = Int32.Parse(dr["ListOrder"].ToString()),
                            ExtractITRule = dr["ExtractITRule"].ToString()
                        });
                    }

                }
            }
            return Json(TempList);
        }


    }
}