using DigitalNagrik.Areas.Public.Models;
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

namespace DigitalNagrik.Areas.Public.Controllers
{
    public class MobileUploadController : MainController
    {
        // GET: Public/MobileUpload
        CommonFunctions objcm = new CommonFunctions();
        public ActionResult Index(string RegistrationYear, string GrievanceID, string DocumentType, string UserEmailMobile,string EmailorMobile)
        {

            UploadDocument objUploadDocument = new UploadDocument();

            objUploadDocument.RegistrationYear = AESCryptography.DecryptAES(RegistrationYear);
            objUploadDocument.GrievanceID = AESCryptography.DecryptAES(GrievanceID);
            objUploadDocument.DocumentType = AESCryptography.DecryptAES(DocumentType);
            objUploadDocument.UserEmailMobile = AESCryptography.DecryptAES(UserEmailMobile);
            objUploadDocument.EmailorMobile = AESCryptography.DecryptAES(EmailorMobile);
            ViewData["DocumentFileType"] = objcm.fillDocumentFileTypeMaster();
            ViewData["AppealDocumentType"] = DocumentType;
            return View(objUploadDocument);
        }

        [HttpPost]
        public ActionResult Index(UploadDocument uploadDocument)
        {
            uploadDocument.RegistrationYear = AESCryptography.DecryptAES(uploadDocument.RegistrationYear);
            uploadDocument.GrievanceID = AESCryptography.DecryptAES(uploadDocument.GrievanceID);
            uploadDocument.DocumentType = AESCryptography.DecryptAES(uploadDocument.DocumentType);
            uploadDocument.UserEmailMobile = AESCryptography.DecryptAES(uploadDocument.UserEmailMobile);
            uploadDocument.EmailorMobile = AESCryptography.DecryptAES(uploadDocument.EmailorMobile);
            FTPHelper objFTPHerlper = new FTPHelper();
            returnMobileUpload returnMobile = new returnMobileUpload();
            returnMobile.statusCode = "200";
            string RootDir = uploadDocument.RegistrationYear + uploadDocument.GrievanceID;
            if (!objFTPHerlper.FtpDirectoryExist(RootDir + "/"))
            {
                objFTPHerlper.CreateFTPDir(RootDir);
            }
            if (!objFTPHerlper.FtpDirectoryExist(RootDir + "/CopyofComplaint/"))
            {
                objFTPHerlper.CreateFTPDir(RootDir + "/CopyofComplaint");
            }
            if (!objFTPHerlper.FtpDirectoryExist(RootDir + "/CopyofDecision/"))
            {
                objFTPHerlper.CreateFTPDir(RootDir + "/CopyofDecision");
            }
            if (!objFTPHerlper.FtpDirectoryExist(RootDir + "/ProofOfComplaint/"))
            {
                objFTPHerlper.CreateFTPDir(RootDir + "/ProofOfComplaint");
            }
            if (!objFTPHerlper.FtpDirectoryExist(RootDir + "/OTHC/"))
            {
                objFTPHerlper.CreateFTPDir(RootDir + "/OTHC");
            }
            if (!objFTPHerlper.FtpDirectoryExist(RootDir + "/Evidenceimage/"))
            {
                objFTPHerlper.CreateFTPDir(RootDir + "/Evidenceimage");
            }
            if (!objFTPHerlper.FtpDirectoryExist(RootDir + "/Evidencevideo/"))
            {
                objFTPHerlper.CreateFTPDir(RootDir + "/Evidencevideo");
            }
            if (!objFTPHerlper.FtpDirectoryExist(RootDir + "/Evidencevoice/"))
            {
                objFTPHerlper.CreateFTPDir(RootDir + "/Evidencevoice");
            }
            returnMobile.statusMessage = "ErrorFileUpload";
            string FileType = "", FileName = "";
            string FileTypeID = "0";
            var file = "";
            try
            {
                if (uploadDocument.fileUpload != null)
                {
                    byte[] fileBytes = null;
                    List<DocumentFileTypeMaster> fileExtList = objcm.fillDocumentFileTypeMaster();
                    FileType = Path.GetExtension(uploadDocument.fileUpload.FileName);
                    FileName = uploadDocument.fileUpload.FileName;
                    if (objcm.checkExtension(fileExtList, FileType.Replace(".", "").ToLower(), uploadDocument.DocumentType))
                    {
                        using (var binaryReaderPDF = new System.IO.BinaryReader(uploadDocument.fileUpload.InputStream))
                        {
                            fileBytes = binaryReaderPDF.ReadBytes(uploadDocument.fileUpload.ContentLength);
                        }
                        if (FileType.ToLower().Contains("pdf")) { FileTypeID = "4"; }
                        else
                        {
                            if (uploadDocument.DocumentType == "EI")
                                FileTypeID = "1";
                            else if (uploadDocument.DocumentType == "EVI")
                                FileTypeID = "2";
                            else if (uploadDocument.DocumentType == "EVO")
                                FileTypeID = "3";
                            else
                                FileTypeID = "1";
                        }
                        List<mAppealDocument> list = new List<mAppealDocument>();
                        list = objcm.fillAppealDocuments(uploadDocument.RegistrationYear, uploadDocument.GrievanceID);
                        if (fileBytes != null)
                        {
                            file = objcm.makeFTPPath(uploadDocument.RegistrationYear, uploadDocument.GrievanceID, uploadDocument.DocumentType) + FileName;
                            file = replaceCharacter(file);
                            var tmp = list.Where(x => x.DocumentType == uploadDocument.DocumentType && x.EvidenceType == "F").ToList();
                            // FTPHelper objFTPHerlper = new FTPHelper();
                            if (tmp.Count > 0)
                            {
                                if (uploadDocument.DocumentType == "OTHC")
                                {
                                    if (tmp.Count >= Int32.Parse(fileExtList.Where(x => x.AppealDocumentType == "OTHC").Select(x => x.UploadLimit).FirstOrDefault()))
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
                            objFTPHerlper.saveFTPfile(fileBytes, "/" + file);


                            objcm.saveAppealDocument(uploadDocument.RegistrationYear, uploadDocument.GrievanceID, "F", file, FileTypeID, FileType, uploadDocument.DocumentType);
                            returnMobile.statusMessage = "SuccessFileUpload";
                            returnMobile.statusCode = "200";

                        }
                        else
                        {
                            returnMobile.statusMessage = "Not valid File";
                            returnMobile.statusCode = "400";
                        }
                    }
                    else
                    {
                        returnMobile.statusMessage = "Not valid File";
                        returnMobile.statusCode = "400";
                    }

                }
            }
            catch (Exception)
            {

                returnMobile.statusMessage = "Error while uploading file";
                returnMobile.statusCode = "400";
            }


            return RedirectToAction("responseFromUpload", new { statusCode = AESCryptography.EncryptAES(returnMobile.statusCode ?? "500"), statusMessage = AESCryptography.EncryptAES(returnMobile.statusMessage ?? "Error"), path = AESCryptography.EncryptAES(file), fileType = AESCryptography.EncryptAES(FileType) });


        }

        public ActionResult AppealPDF(string RegistrationYear, string GrievanceID)
        {
            try
            {
                RegistrationYear = AESCryptography.DecryptAES(RegistrationYear);
                GrievanceID = AESCryptography.DecryptAES(GrievanceID);
                var objIntermediaryDetailPreview = new mIntermediaryDetailPart();



                RegistrationYear = (RegistrationYear == null ? "" : RegistrationYear);
                GrievanceID = (GrievanceID == null ? "" : GrievanceID);
                objIntermediaryDetailPreview.RegistrationYear = RegistrationYear;
                objIntermediaryDetailPreview.GrievanceID = GrievanceID;
                if (!string.IsNullOrEmpty(RegistrationYear) & !string.IsNullOrEmpty(GrievanceID))
                {
                    objIntermediaryDetailPreview = objcm.fillIntermediaryDetailPart("",RegistrationYear, GrievanceID);
                }
               
                List<mAppealDocument> list = new List<mAppealDocument>();
                list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
                ViewData["AppealDocList"] = list;
                return new ViewAsPdf("AppealDetailasPDF", objIntermediaryDetailPreview)
                {
                    PageMargins = { Left = 10, Right = 10 },
                    CustomSwitches = "--print-media-type --footer-right \"Appeal details as on - [date]\" --footer-left \"MeitY (GAC ePortal)\" --footer-center \"Page [page] of [topage] \" --footer-font-size 8"
                };
            }
            catch (Exception ex) { }
            return new EmptyResult();
        }
        public ActionResult sendMail(string RegistrationYear, string GrievanceID) {
            returnMobileUpload returnMobile = new returnMobileUpload();
            try
            {
                RegistrationYear = AESCryptography.DecryptAES(RegistrationYear);
                GrievanceID = AESCryptography.DecryptAES(GrievanceID);
                var objDetailPreview = new mIntermediaryDetailPart();



                RegistrationYear = (RegistrationYear == null ? "" : RegistrationYear);
                GrievanceID = (GrievanceID == null ? "" : GrievanceID);
                objDetailPreview.RegistrationYear = RegistrationYear;
                objDetailPreview.GrievanceID = GrievanceID;
                if (!string.IsNullOrEmpty(RegistrationYear) & !string.IsNullOrEmpty(GrievanceID))
                {
                    objDetailPreview = objcm.fillIntermediaryDetailPart("",RegistrationYear, GrievanceID);
                }
               
                string citizenEmail = "", IntermediaryGROEmail = "";
                DigitalNagrik.Models.SendSMS smsObj = new DigitalNagrik.Models.SendSMS();
                IntermediaryGROEmail = objDetailPreview.GROIntermediaryEmail;
                string emailMsg = ConvertViewToString("~/Areas/Public/Views/Message/_SendMailtoIntermediary.cshtml", objDetailPreview);
                string emailSubject = "GAC Portal : Appeal number " + objDetailPreview.GrievanceID+"/" + objDetailPreview.RegistrationYear + " from " + objDetailPreview.CitizenName + " against " + objDetailPreview.txtIntermediary;
                smsObj.SendMailToUser(emailMsg, emailSubject, IntermediaryGROEmail, "Grievance Appellate Committee ePortal", Session["CitizenMobile"].ToString());
                returnMobile.statusMessage = "SuccessSendMail";
                returnMobile.statusCode = "200";
            }
            catch (Exception)
            {
                returnMobile.statusMessage = "ErrorSendingMail";
                returnMobile.statusCode = "200";
            }
            return RedirectToAction("responseFromUpload", new { statusCode = AESCryptography.EncryptAES(returnMobile.statusCode ?? "500"), statusMessage = AESCryptography.EncryptAES(returnMobile.statusMessage ?? "Error")});

        }
        public ActionResult responseFromUpload(string msgcode, string message, string path, string fileType)
        {
            return View();
        }
        public ActionResult DownloadDocument(string FilePath, string FileType)
        {
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

                var tm = FilePath.Split('0');
                string contentType = "application/" + FileType.Replace(".", "").ToLower().ToString();
                string fileNameDisplayedToUser = FilePath.Split('/')[2];

                MemoryStream stream = new MemoryStream();
                response.GetResponseStream().CopyTo(stream);
                ftpStream.Close();
                response.Close();
                //using (MemoryStream stream = new MemoryStream())
                //{
                //    //Download the File.
                //    response.GetResponseStream().CopyTo(stream);


                //}
                Response.ContentType = "application/" + FileType;
                Response.AddHeader("content-disposition", "attachment;filename=" + fileNameDisplayedToUser);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(stream.ToArray());
                Response.End();

                return View();
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
        }
        public ActionResult DeleteFile(string RegistrationYear, string GrievanceID, string FileID)
        {
            returnMobileUpload returnMobile = new returnMobileUpload();
            try
            {
                RegistrationYear = AESCryptography.DecryptAES(RegistrationYear);
                GrievanceID = AESCryptography.DecryptAES(GrievanceID);
                FileID = AESCryptography.DecryptAES(FileID);

                List<mAppealDocument> list = new List<mAppealDocument>();
                list = objcm.fillAppealDocuments(RegistrationYear, GrievanceID);
                FTPHelper objFTPHelper = new FTPHelper();
                var tmp = list.Where(x => x.FileId == FileID && x.EvidenceType == "F").ToList();
                objFTPHelper.DeleteFile(tmp.FirstOrDefault().FilePath);
                objcm.deleteAppealDocument(RegistrationYear, GrievanceID, tmp.FirstOrDefault().FileId);
                returnMobile.statusMessage = "SuccessFileDelete";
                returnMobile.statusCode = "200";
            }
            catch (Exception)
            {
                returnMobile.statusMessage = "ErrorFileDelete";
                returnMobile.statusCode = "400";
            }

            return RedirectToAction("responseFromUpload", new { statusCode = AESCryptography.EncryptAES(returnMobile.statusCode ?? "500"), statusMessage = AESCryptography.EncryptAES(returnMobile.statusMessage ?? "Error") });


        }
        public static string replaceCharacter(string str)
        {
            List<char> charsToRemove = new List<char>() { '@', '_', ',', '(', ')', ' ' };

            return String.Concat(str.Split(charsToRemove.ToArray()));
        }

    }
}