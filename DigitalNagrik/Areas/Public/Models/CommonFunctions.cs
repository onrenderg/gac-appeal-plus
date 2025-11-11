using DigitalNagrik.Models;
using Newtonsoft.Json;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class CommonFunctions
    {
        public List<mLanguageMaster> fillLanguageMaster()
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            var objList = new List<mLanguageMaster>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", "", "SelectMSSql", "getLanguage", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable evdt = ds.Tables[0];
                    foreach (DataRow r in evdt.Rows)
                    {
                        objList.Add(new mLanguageMaster
                        {
                            languageCode = Convert.ToString(r["languageCode"]),
                            languageDescription = Convert.ToString(r["languageDescription"]),
                            languageDescriptionlocal = Convert.ToString(r["languageDescriptionlocal"])
                        });
                    }

                }
                return objList;
            }
            return objList;
        }
        public mAppealDisposalDetail fillAppealDisposalDetail(string RegistrationYear,string GrievanceID)
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            var obj = new mAppealDisposalDetail();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", "", "SelectMSSql", "getAppealDisposalDetail", methodParameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];
                    obj.LetterCopy = dt.Rows[0]["LetterCopy"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["LetterCopy"];
                    obj.Remarks = Convert.ToString(dt.Rows[0]["Remarks"]);
                    obj.DateOfDisposal = Convert.ToString(dt.Rows[0]["DateOfDisposal"]);
                    obj.DecisionDate = Convert.ToString(dt.Rows[0]["DecisionDate"]);
                }
            }
            return obj;
        }
        public mAppealComplianceDetail fillAppealComplianceDetail(string RegistrationYear, string GrievanceID)
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            var obj = new mAppealComplianceDetail();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", "", "SelectMSSql", "getAppealComplianceDetail", methodParameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];
                    obj.SupportingDocument = dt.Rows[0]["SupportingDocument"] == DBNull.Value ? null : (byte[])ds.Tables[0].Rows[0]["SupportingDocument"];
                    obj.Remarks = Convert.ToString(dt.Rows[0]["Remarks"]);
                    obj.ComplianceDate = Convert.ToString(dt.Rows[0]["ComplianceDate"]);
                }
            }
            return obj;
        }
        public CitizenProfile fillCitizenProfile(string UserID)
        {

            var methodParameter = new List<KeyValuePair<string, string>>();
            var objCitizenProfile = new CitizenProfile();
            methodParameter.Add(new KeyValuePair<string, string>("@UserID", UserID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", "", "SelectMSSql", "getCitizenProfile_new", methodParameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {

                    var dt = ds.Tables[0];

                    //Session["UserName"] = Convert.ToString(dt.Rows[0]["FirstName"]);
                    HttpContext.Current.Session["UserName"] = dt.Rows[0]["UserName"].ToString();
                    objCitizenProfile.UserId = Convert.ToString(dt.Rows[0]["UserId"]);
                    objCitizenProfile.UserProfilleId = Convert.ToString(dt.Rows[0]["UserProfilleId"]);
                    objCitizenProfile.UserEmail = Convert.ToString(dt.Rows[0]["UserEmail"]);
                    objCitizenProfile.txtFirstName = Convert.ToString(dt.Rows[0]["FirstName"]);
                    objCitizenProfile.txtMiddleName = Convert.ToString(dt.Rows[0]["MiddleName"]);
                    objCitizenProfile.txtLastName = Convert.ToString(dt.Rows[0]["LastName"]);
                    objCitizenProfile.MobileNo = Convert.ToString(dt.Rows[0]["UserMobile"]);
                    objCitizenProfile.AadhaarVerificationStatus = Convert.ToString(dt.Rows[0]["AadhaarVerificationStatus"]);
                }
            }
            return objCitizenProfile;
        }

        public bool ValidateCaptcha(string CaptchaText)
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(CaptchaText))
            {
                if (HttpContext.Current.Session["RegisterCaptcha"] != null)
                {
                    if (CaptchaText.Trim() ==HttpContext.Current.Session["RegisterCaptcha"].ToString())
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        public bool checkExtension(List<DocumentFileTypeMaster> listtocheck, string fileExtension, string DocumentType)
        {
            string[] validExtension = (string[])listtocheck.FindAll(x => x.AppealDocumentType == DocumentType).Select(o => o.FileExtension).ToArray();
            if (validExtension.Contains(fileExtension))
            {
                return true;
            }
            
            return false;
        }
        public bool checkFileSize(List<DocumentFileTypeMaster> listtocheck,  string DocumentType,Int32  fileSize)
        {
            Int32 maxFileSize = Int32.Parse( listtocheck.FindAll(x => x.AppealDocumentType == DocumentType).Select(o => o.MaxFileSize).FirstOrDefault());
            maxFileSize = maxFileSize * 1024 * 1024;
            if (fileSize > maxFileSize)
            {
                return false;
            }
           
            return true;
        }
        public mIntermediaryDetailPart fillIntermediaryDetailPart(string UserEmailMobile, string RegistrationYear, string GrievanceID)
        {
            var objIntermediaryDetail = new mIntermediaryDetailPart();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", UserEmailMobile));
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getAppealDetail_new", methodParameter);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count == 0) { return objIntermediaryDetail; }
                objIntermediaryDetail.MobileNo = dt.Rows[0]["UserMobile"].ToString();
                objIntermediaryDetail.txtEmail = dt.Rows[0]["UserEmail"].ToString();
                objIntermediaryDetail.RegistrationYear = dt.Rows[0]["RegistrationYear"].ToString();
                objIntermediaryDetail.GrievanceID = dt.Rows[0]["GrievanceId"].ToString();
                objIntermediaryDetail.txturlofintermediary = dt.Rows[0]["IntermediaryURL"].ToString().Trim();

                objIntermediaryDetail.IntermediaryId = dt.Rows[0]["IntermediaryId"].ToString();
                objIntermediaryDetail.txtIntermediary = dt.Rows[0]["IntermediaryTitle"].ToString();
                objIntermediaryDetail.txturlofintermediary = dt.Rows[0]["IntermediaryURL"].ToString();


                objIntermediaryDetail.GROIntermediaryEmail = dt.Rows[0]["IntermediaryGROEmail"].ToString().Trim();
                objIntermediaryDetail.txtCorrespondenceAddress = dt.Rows[0]["IntermediaryAddress"].ToString().Trim();


                objIntermediaryDetail.txtJustification = dt.Rows[0]["Justification"].ToString().Trim();




                objIntermediaryDetail.ddlReliefSought = dt.Rows[0]["ReliefSoughtID"].ToString();
                objIntermediaryDetail.ReliefSoughtTitle = dt.Rows[0]["ReliefTitle"].ToString();
                objIntermediaryDetail.ReliefSoughtSpecification = dt.Rows[0]["RelieftSoughtSpecification"].ToString();

                objIntermediaryDetail.ddlGroundAppeal = dt.Rows[0]["GroundAppealID"].ToString();
                objIntermediaryDetail.GroundAppealLawText = dt.Rows[0]["GroundAppealLawText"].ToString();
                objIntermediaryDetail.GroundTitle = dt.Rows[0]["GroundTitle"].ToString();
                objIntermediaryDetail.GrievnaceStatus = dt.Rows[0]["GrievnaceStatus"].ToString();
                objIntermediaryDetail.StatusTitle = dt.Rows[0]["StatusTitle"].ToString();
                objIntermediaryDetail.ReceiptDate = dt.Rows[0]["ReceiptDate"].ToString();
                objIntermediaryDetail.LastUpdatedOn = dt.Rows[0]["LastUpdatedOn"].ToString();


                objIntermediaryDetail.txtFirstName = dt.Rows[0]["FirstName"].ToString().Trim();
                objIntermediaryDetail.txtMiddleName = dt.Rows[0]["MiddleName"].ToString().Trim();
                objIntermediaryDetail.txtLastName = dt.Rows[0]["LastName"].ToString().Trim();
                objIntermediaryDetail.CitizenName = dt.Rows[0]["CitizenName"].ToString().Trim();

                objIntermediaryDetail.ReceiptDateTime = dt.Rows[0]["ReceiptDateTime"].ToString();
                objIntermediaryDetail.LastResponseTime = dt.Rows[0]["LastResponseTime"].ToString();
                objIntermediaryDetail.EntryFieldLabel = dt.Rows[0]["EntryFieldLabel"].ToString();
                objIntermediaryDetail.SpecificationLabel = dt.Rows[0]["SpecificationLabel"].ToString();
                objIntermediaryDetail.SubmitDate = dt.Rows[0]["SubmitDate"].ToString();
                objIntermediaryDetail.GroundTitleText = dt.Rows[0]["GroundTitleText"].ToString();
                objIntermediaryDetail.txtDateofComplaint = dt.Rows[0]["dateofComplaint"].ToString();
                objIntermediaryDetail.txtdateofDecision = dt.Rows[0]["dateofDecision"].ToString();
                objIntermediaryDetail.BriefofComplaint = dt.Rows[0]["BriefofComplaint"].ToString();
                objIntermediaryDetail.Keyword = dt.Rows[0]["Keyword"].ToString();
                objIntermediaryDetail.TicketDocketNumber = dt.Rows[0]["TicketDocketNumber"].ToString();

            }
            return objIntermediaryDetail;
        }

        public AppellantDetail fillAppellantDetail(string RegistrationYear, string GrievanceID)
        {
            var obj = new AppellantDetail();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getCitizenDetailUsingAppeal", methodParameter);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count == 0) { return obj; }
                obj.UserID = dt.Rows[0]["UserID"].ToString();
                obj.UserProfileID = dt.Rows[0]["UserProfileID"].ToString();
                obj.LoginByMorE = dt.Rows[0]["LoginByMorE"].ToString();
                obj.FirstName = dt.Rows[0]["FirstName"].ToString().Trim();

                obj.MiddleName = dt.Rows[0]["MiddleName"].ToString();
                obj.LastName = dt.Rows[0]["LastName"].ToString();

                obj.UserMobile = dt.Rows[0]["UserMobile"].ToString().Trim();
                obj.UserEmail = dt.Rows[0]["UserEmail"].ToString().Trim();
            }
            return obj;
        }
        public void UpdateAadhaarVerfication(string UserID,string FirstName, string MiddleName, string LastName)
        {
            var obj = new AppellantDetail();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserID", UserID));
            methodParameter.Add(new KeyValuePair<string, string>("@FirstName", FirstName));
            methodParameter.Add(new KeyValuePair<string, string>("@MiddleName", MiddleName));
            methodParameter.Add(new KeyValuePair<string, string>("@LastName", LastName));
            methodParameter.Add(new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "UpdateAadhaarVerfication", methodParameter);
            
        }
        public void AadharVerificationLog(string RequestMethod, string Responsetext)
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RequestMethod", RequestMethod));
            methodParameter.Add(new KeyValuePair<string, string>("@Responsetext", Responsetext));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "insertAadharVerificationLog", methodParameter);

        }

        public string makeFTPPath(string RegistrationYear, string GrievanceID, string DocumnetType)
        {
            string dirpath = RegistrationYear + GrievanceID;
            if (DocumnetType == "CC")
                dirpath = dirpath + "/CopyofComplaint/";
            else if (DocumnetType == "CD")
                dirpath = dirpath + "/CopyofDecision/";
            else if (DocumnetType == "PC")
                dirpath = dirpath + "/ProofOfComplaint/";
            else if (DocumnetType == "OTHC")
                dirpath = dirpath + "/OTHC/";
            else if (DocumnetType == "EI")
                dirpath = dirpath + "/Evidenceimage/";
            else if (DocumnetType == "EVI")
                dirpath = dirpath + "/Evidencevideo/";
            else if (DocumnetType == "EVO")
                dirpath = dirpath + "/Evidencevoice/";
            return dirpath;
        }
        public List<mAppealDocument> fillAppealDocuments(string RegistrationYear, string GrievanceID)
        {
            List<mAppealDocument> appealDocuments = new List<mAppealDocument>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getAppealDocument", methodParameter);
            if (ds.Tables.Count > 0)
            {
                DataTable evdt = ds.Tables[0];
                foreach (DataRow r in evdt.Rows)
                {
                    appealDocuments.Add(new mAppealDocument
                    {
                        RegistrationYear = Convert.ToString(r["RegistrationYear"]),
                        GrievanceID = Convert.ToString(r["GrievanceId"]),
                        FileId = Convert.ToString(r["FileId"]),
                        DocumentTitle = Convert.ToString(r["DocumentTitle"]),
                        FileTypeID = Convert.ToString(r["FileTypeID"]),
                        FilePath = Convert.ToString(r["FilePath"]),
                        FileType = Convert.ToString(r["FileType"]),
                        EvidenceType = Convert.ToString(r["EvidenceType"]),
                        DocumentType = Convert.ToString(r["DocumentType"])
                    });
                }

            }
            return appealDocuments;
        }
       

        public List<mAppealAction> fillAppealAction(string RegistrationYear, string GrievanceID)
        {
            List<mAppealAction> list = new List<mAppealAction>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getAppealAction", methodParameter);
            if (ds != null)
            {
                DataTable evdt = ds.Tables[0];
                foreach (DataRow r in evdt.Rows)
                {
                    list.Add(new mAppealAction
                    {
                        ActionTitle = Convert.ToString(r["ActionTitle"]),
                        ActionDesc = Convert.ToString(r["ActionDesc"]),
                        ActionLevel = Convert.ToString(r["ActionLevel"]),
                        ActionBy = Convert.ToString(r["ActionBy"]),
                        Remarks = Convert.ToString(r["Remarks"]),
                        ActionDateTime = Convert.ToString(r["ActionDateTime"])
                    });
                }
            }
            return list;

        }
        public void saveCaseHistoryFile(string RegistrationYear, string GrievanceID, string CaseHistoryFilePath, string CaseHistoryFileType)
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            methodParameter.Add(new KeyValuePair<string, string>("@CaseHistoryFilePath", CaseHistoryFilePath));
            methodParameter.Add(new KeyValuePair<string, string>("@CaseHistoryFileType", CaseHistoryFileType));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "updateCaseHistoryFile", methodParameter);
        }
        public void CreateUser(string UserEmailMobile)
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserMobile", (UserEmailMobile == null ? "" : UserEmailMobile)));
            methodParameter.Add(new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "Insert_CreateCitizenUserID", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    HttpContext.Current.Session["UserID"] = ds.Tables[0].Rows[0]["UserID"].ToString();
                    UserSession.UserID = ds.Tables[0].Rows[0]["UserID"].ToString();
                }
            }
        }


        public string CheckUserExists(string UserEmailorMobile)
        {
            string statusMessage = "Exists";
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserEmailorMobile", UserEmailorMobile));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "CheckUserExists", methodParameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    statusMessage = ds.Tables[0].Rows[0]["statuMessage"].ToString();
                }
            }
            return statusMessage;
        }

        public DigitalNagrik.Models.mHome DashboardCount()
        {
            DigitalNagrik.Models.mHome obj = new DigitalNagrik.Models.mHome();
            obj.Submitted = "0";
            obj.Disposed = "0";
            var methodParameter = new List<KeyValuePair<string, string>>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getDashboardCount", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    obj.Submitted = ds.Tables[0].Rows[0]["Submitted"].ToString();
                    obj.Disposed = ds.Tables[0].Rows[0]["Disposed"].ToString();
                }
            }
            return obj;
        }


        public List<mGroundMaster> getAppealGroundMaster()
        {
            var tmpList = new List<mGroundMaster>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getAppealGroundMaster", methodParameter);
            //tmpList.Add(new mGroundMaster
            //{
            //    GroundId = Convert.ToString("0"),
            //    GroundTitle = Convert.ToString("-Select-"),
            //    CorrespondingITRule = Convert.ToString("")
            //});
            if (ds != null)
            {
                DataTable evdt = ds.Tables[0];
                foreach (DataRow r in evdt.Rows)
                {
                    tmpList.Add(new mGroundMaster
                    {
                        GroundId = Int32.Parse(r["GroundId"].ToString()),
                        GroundTitle = Convert.ToString(r["GroundTitle"]),
                        CorrespondingITRule = Convert.ToString(r["CorrespondingITRule"]),
                        EntryRequired = Convert.ToString(r["EntryRequired"]),
                        EntryFieldLabel = Convert.ToString(r["EntryFieldLabel"]),
                        listorder = Int32.Parse(r["listorder"].ToString()),
                        ExtractITRule = Convert.ToString(r["ExtractITRule"])
                    });
                }
            }
            return tmpList;
        }
        public List<mAppealReliefMaster> getAppealReliefMaster()
        {
            var tmpList = new List<mAppealReliefMaster>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getAppealReliefMaster", methodParameter);
            if (ds != null)
            {
                DataTable evdt = ds.Tables[0];
                foreach (DataRow r in evdt.Rows)
                {
                    tmpList.Add(new mAppealReliefMaster
                    {
                        ReliefId = Convert.ToString(r["ReliefId"]),
                        ReliefTitle = Convert.ToString(r["ReliefTitle"]),
                        SpecificationLabel = Convert.ToString(r["SpecificationLabel"])
                    });
                }
            }
            return tmpList;
        }
        public void saveAppealDocument(string RegistrationYear, string GrievanceID, string EvidanceType, string FilePath, string FileTypeID, string FileType, string DocumentType)
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            methodParameter.Add(new KeyValuePair<string, string>("@EvidenceType", EvidanceType));
            methodParameter.Add(new KeyValuePair<string, string>("@FilePath", FilePath));
            methodParameter.Add(new KeyValuePair<string, string>("@FileTypeID", FileTypeID));
            methodParameter.Add(new KeyValuePair<string, string>("@FileType", FileType));
            methodParameter.Add(new KeyValuePair<string, string>("@DocumentType", DocumentType));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "InsertUpdateAppealDocument", methodParameter);
        }
        public void deleteAppealDocument(string RegistrationYear, string GrievanceID, string FileId)
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceID", GrievanceID));
            methodParameter.Add(new KeyValuePair<string, string>("@FileId", FileId));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "DeleteAppealDocument", methodParameter);
        }
   
        public CitizenDashboardCount fillDashboardCount(string UserID)
        {
            var methodParameter = new List<KeyValuePair<string, string>>();
            var objCitizenDashboardCount = new CitizenDashboardCount();
            methodParameter.Add(new KeyValuePair<string, string>("@UserID", UserID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getCitizenDashboardList", methodParameter);
            if (ds != null)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                objCitizenDashboardCount.Total = dr["Total"].ToString();
                objCitizenDashboardCount.Draft = dr["Draft"].ToString();
                objCitizenDashboardCount.Submitted = dr["Submitted"].ToString();
                objCitizenDashboardCount.Referback = dr["Referback"].ToString();
                objCitizenDashboardCount.Rejected = dr["Rejected"].ToString();
                objCitizenDashboardCount.Disposed = dr["Disposed"].ToString();

            }
            return objCitizenDashboardCount;
        }

        public List<mIntermediaryDetailPart> fillUserAppealList(string UserEmailMobile, string GrievanceStatus)
        {
            List<mIntermediaryDetailPart> list = new List<mIntermediaryDetailPart>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", UserEmailMobile));
            methodParameter.Add(new KeyValuePair<string, string>("@GrievanceStatus", GrievanceStatus.ToString()));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getCitizenAppealList_new", methodParameter);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow r in dt.Rows)
                {
                    list.Add(new mIntermediaryDetailPart
                    {


                        //txtEmail = dt.Rows[0]["UserEmail"].ToString(),
                        RegistrationYear = r["RegistrationYear"].ToString(),
                        GrievanceID = r["GrievanceId"].ToString(),
                        txturlofintermediary = r["IntermediaryURL"].ToString().Trim(),
                        IntermediaryId = r["IntermediaryId"].ToString(),
                        txtIntermediary = r["IntermediaryTitle"].ToString(),
                        GROIntermediaryEmail = r["IntermediaryGROEmail"].ToString().Trim(),
                        txtCorrespondenceAddress = r["IntermediaryAddress"].ToString().Trim(),
                        txtJustification = r["Justification"].ToString().Trim(),
                        ddlReliefSought = r["ReliefSoughtID"].ToString(),
                        ReliefSoughtTitle = r["ReliefTitle"].ToString(),
                        ReliefSoughtSpecification = r["RelieftSoughtSpecification"].ToString(),
                        ddlGroundAppeal = r["GroundAppealID"].ToString(),
                        GroundAppealLawText = r["GroundAppealLawText"].ToString(),
                        GroundTitle = r["GroundTitle"].ToString(),
                        GrievnaceStatus = r["GrievnaceStatus"].ToString(),
                        StatusTitle = r["StatusTitle"].ToString(),
                        ReceiptDate = r["ReceiptDate"].ToString(),
                        LastUpdatedOn = r["LastUpdatedOn"].ToString(),
                        ReceiptDateTime = r["ReceiptDateTime"].ToString(),
                        LastResponseTime = r["LastResponseTime"].ToString(),
                        ComplianceURL = r["ComplianceURL"].ToString(),
                        DecisionFilePath = r["DecisionFilePath"].ToString()
                    });
                }



            }
            return list;
        }
        public List<mAnnexureC> fillAnnexureC(int GoundID)
        {
            List<mAnnexureC> list = new List<mAnnexureC>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@GroundAppealID", GoundID.ToString()));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getAnnexureCMaster", methodParameter);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new mAnnexureC
                        {
                            Section = Convert.ToString(dr["Section"]),
                            Sequence = Convert.ToString(dr["Sequence"]),
                            ExtractITRule = Convert.ToString(dr["ExtractITRule"]),
                            RelatedSubjectEntry = Convert.ToString(dr["RelatedSubjectEntry"]),
                            ConcernedMinistry = Convert.ToString(dr["ConcernedMinistry"])
                        });
                    }

                }
            }
            return list;
        }


        public string CheckAppealMobileorEmail(string Mobile, string UserID)
        {
            string statusCode = "";
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@Mobile", Mobile));
            methodParameter.Add(new KeyValuePair<string, string>("@UserID", UserID));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "CheckAppealMobileorEmail", methodParameter);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];
                    statusCode = dt.Rows[0]["statusCode"].ToString();

                }
            }
            return statusCode;
        }
        public mIntermediaryDetailPart GenerateAppealID_new(string UserEmailMobile)
        {
            mIntermediaryDetailPart obj = new mIntermediaryDetailPart();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", (UserEmailMobile == null ? "" : UserEmailMobile)));
            methodParameter.Add(new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getGenerateAppealID_new", methodParameter);
            if (ds != null)
            {
                obj.RegistrationYear = ds.Tables[0].Rows[0]["RegistrationYear"].ToString();
                obj.GrievanceID = ds.Tables[0].Rows[0]["GrievanceID"].ToString();
            }
            return obj;
        }
        public void createFTPPath(string RegistrationYear, string GrievanceID)
        {
            FTPHelper objFTPHerlper = new FTPHelper();
            string RootDir = RegistrationYear + GrievanceID;
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
        }

        public List<DocumentFileTypeMaster> fillDocumentFileTypeMaster()
        {
            List<DocumentFileTypeMaster> objlist = new List<DocumentFileTypeMaster>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getDocumentFileType", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            objlist.Add(new DocumentFileTypeMaster
                            {
                                AppealDocumentType = Convert.ToString(dr["AppealDocumentType"]),
                                FileTypeId = Convert.ToString(dr["FileTypeId"]),
                                FileTypeTitle = Convert.ToString(dr["FileTypeTitle"]),
                                FileExtension = Convert.ToString(dr["FileExtension"]),
                                MaxFileSize = Convert.ToString(dr["MaxFileSize"]),
                                UploadLimit = Convert.ToString(dr["UploadLimit"])
                            });
                        }

                    }
                }
            }
            return objlist;
        }
        public List<IntermediaryMaster> fillIntermediaryMaster()
        {
            var TempList = new List<IntermediaryMaster>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@IntermediaryTitle", ""));
            methodParameter.Add(new KeyValuePair<string, string>("@URL", ""));
            methodParameter.Add(new KeyValuePair<string, string>("@func", "All"));

            var ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "getIntermediaryMaster", methodParameter);
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
                            GOName = Convert.ToString(dr["GOName"]),
                            IsActive = Convert.ToString(dr["IsActive"]),
                            HelpLink = Convert.ToString(dr["HelpLink"])

                        });
                    }

                }
            }
            return TempList;
        }
        public string GetGroundKeywords()
        {
            var TempList = new List<string>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@SearchString", ""));
            var ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Current.Request.Url.Host.ToString(), "SelectMSSql", "GetGroundKeywords", methodParameter);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TempList.Add(dr["Keyword"].ToString());
                    }
                }
            }
            return JsonConvert.SerializeObject(TempList);
        }


    }
}