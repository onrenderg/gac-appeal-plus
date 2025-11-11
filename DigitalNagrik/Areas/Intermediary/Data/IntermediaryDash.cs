using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.Intermediary.Data
{
    public class IntermediaryDash
    {
    }
    public class LoginviaOTP
    {
        [Required(ErrorMessage = "Enter User Id")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Only 5 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Characters in Email Address!")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "Enter OTP")]
        public string OTP { get; set; }
        [Required(ErrorMessage = "Enter Captcha")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Only 4 digits")]
        public string Captcha { get; set; }
        [Required(ErrorMessage = "Enter Captcha")]
        public string CaptchaV { get; set; }
        //public string HashPassword { get; set; }
        //public string Seed { get; set; }
    }
    public class IntermediaryDash_Stats
    {
        public string IsVerified { get; set; }
        public string IntermediaryId { get; set; }
        public string AppealsReceived { get; set; }
        public string AppealsSubmitted { get; set; }
        public string AppealsNotAdmitted { get; set; }
        public string AppealsPending { get; set; }
        public string AppealsQuery { get; set; }
        public string AppealsQueryY { get; set; }
        public string AppealsQueryN { get; set; }
        public string AppealsDisposed { get; set; }
        public string AppealsImplemented { get; set; }
        public string AppealsImplementedY { get; set; }
        public string AppealsImplementedN { get; set; }
        public string Pending_Percent { get; set; }
        public string Submit_Percent { get; set; }
        public string Query_Percent { get; set; }
        public string Disposed_Percent { get; set; }
        public string Implemented_Percent { get; set; }
        public string AppealsOverdue { get; set; }
        public string ResponseIntermediary_ResponseTimeExceeded { get; set; }
        public static IntermediaryDash_Stats GetData(string IntermediaryID)
        {
            IntermediaryDash_Stats Data = new IntermediaryDash_Stats();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@IntermediaryID", IntermediaryID),
                new KeyValuePair<string, string>("@Mode", "STATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Data.IsVerified = Convert.ToString(ds.Tables[0].Rows[0]["IsVerified"]);
                    Data.IntermediaryId = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryId"]);
                    Data.AppealsReceived = Convert.ToString(ds.Tables[0].Rows[0]["AppealsReceived"]);
                    Data.AppealsPending = Convert.ToString(ds.Tables[0].Rows[0]["AppealsPending"]);
                    Data.AppealsSubmitted = Convert.ToString(ds.Tables[0].Rows[0]["AppealsSubmitted"]);
                    Data.AppealsDisposed = Convert.ToString(ds.Tables[0].Rows[0]["AppealsDisposed"]);
                    Data.AppealsImplemented = Convert.ToString(ds.Tables[0].Rows[0]["AppealsImplemented"]);
                    Data.AppealsImplementedY = Convert.ToString(ds.Tables[0].Rows[0]["AppealsImplementedY"]);
                    Data.AppealsImplementedN = Convert.ToString(ds.Tables[0].Rows[0]["AppealsImplementedN"]);
                    Data.AppealsQuery = Convert.ToString(ds.Tables[0].Rows[0]["AppealsQuery"]);
                    Data.AppealsQueryY = Convert.ToString(ds.Tables[0].Rows[0]["AppealsQueryY"]);
                    Data.AppealsQueryN = Convert.ToString(ds.Tables[0].Rows[0]["AppealsQueryN"]);
                    Data.Disposed_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Disposed_Percent"]);
                    Data.Pending_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Pending_Percent"]);
                    Data.Implemented_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Implemented_Percent"]);
                    Data.Query_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Query_Percent"]);
                    Data.Submit_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Submit_Percent"]);
                    Data.AppealsOverdue = Convert.ToString(ds.Tables[0].Rows[0]["AppealsOverdue"]);
                    Data.AppealsNotAdmitted = Convert.ToString(ds.Tables[0].Rows[0]["AppealsNotAdmitted"]);

                }
            }
            return Data;
        }

    }
    public class Intermediary_AppealList
    {
        public string ListName { get; set; }
        public List<AppealList> AppealforIntermediary { get; set; }
        public List<AdditionalInputList> AdditionalInputforIntermediary { get; set; }
        public List<ComplianceList> ComplianceforIntermediary { get; set; }

        public static Intermediary_AppealList GetData(string IntermediaryID, string ListType = "RECEIVED",Dictionary<string, string> LabelDictionary = null)
        {
            string ListName = string.Empty;
            Intermediary_AppealList Data = new Intermediary_AppealList();
            if (ListType == "RECEIVED")
            {
                ListName = LabelDictionary["NoticeReceived"];
            }
            else if (ListType == "PENDING")
            {
                ListName = LabelDictionary["PendingforResponse"];
            }
            else if (ListType == "DISPOSED")
            {
                ListName = LabelDictionary["Decided"];
            }
            else if (ListType == "QUERY")
            {
                ListName = LabelDictionary["AdditionalInputSought"];
            }
            else if (ListType == "QUERY_Y")
            {
                ListName = LabelDictionary["AdditionalInputSought(Replied)"];
            }
            else if (ListType == "QUERY_N")
            {
                ListName = LabelDictionary["AdditionalInputSought(Pending)"];
            }
            else if (ListType == "SUBMITTED")
            {
                ListName = LabelDictionary["ResponseSubmitted"];
            }
            else if (ListType == "COMPLIANCE")
            {
                ListName = LabelDictionary["Compliance"];
            }
            else if (ListType == "COMPLIANCE_P")
            {
                ListName = LabelDictionary["Compliance(Pending)"];
            }
            else if (ListType == "OVERDUE")
            {
                ListName = LabelDictionary["Overdue(Noresponsein96hrs)"];
            }
            else if (ListType == "NOTADMIT")
            {
                ListName = "Appeal not admitted by GAC";
            }
            Data.ListName = ListName;
            Data.AppealforIntermediary = new List<AppealList>();
            Data.AdditionalInputforIntermediary = new List<AdditionalInputList>();
            Data.ComplianceforIntermediary = new List<ComplianceList>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@IntermediaryID", IntermediaryID),
                new KeyValuePair<string, string>("@ListType", ListType),
                new KeyValuePair<string, string>("@Mode", "LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (ListType == "QUERY" || ListType == "QUERY_Y" || ListType == "QUERY_N")
                        {
                            Data.AdditionalInputforIntermediary.Add(new AdditionalInputList
                            {
                                GrievanceId = Convert.ToString(dr["GrievanceId"]),
                                Justification = Convert.ToString(dr["Justification"]),
                                RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                                IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                                ReceiptDate = Convert.ToString(dr["ReceiptDate"]),
                                CitizenName = Convert.ToString(dr["CitizenName"]),
                                CorrespondingITRule = Convert.ToString(dr["CorrespondingITRule"]),
                                GOEmail = Convert.ToString(dr["GOEmail"]),
                                GOName = Convert.ToString(dr["GOName"]),
                                GroundTitle = Convert.ToString(dr["GroundTitle"]),
                                HelpLink = Convert.ToString(dr["HelpLink"]),
                                IntermediaryURL = Convert.ToString(dr["IntermediaryURL"]),
                                IntermediayAddress = Convert.ToString(dr["IntermediayAddress"]),
                                ReliefTitle = Convert.ToString(dr["ReliefTitle"]),
                                RelieftSoughtSpecification = Convert.ToString(dr["RelieftSoughtSpecification"]),
                                UserEmail = Convert.ToString(dr["UserEmail"]),
                                UserMobile = Convert.ToString(dr["UserMobile"]),
                                IsResponded = Convert.ToString(dr["IsResponded"]),
                                ResponseTime = Convert.ToString(dr["ResponseTime"]),
                                InputId = Convert.ToString(dr["InputId"]),
                                InputSeekRemarks = Convert.ToString(dr["InputSeekRemarks"]),
                                InputRequestNo = Convert.ToString(dr["InputRequestNo"]),
                                InputSeekDateTime = Convert.ToString(dr["InputSeekDateTime"]),
                                InputReplyDateTime = Convert.ToString(dr["InputReplyDateTime"]),
                                InputReplyRemarks = Convert.ToString(dr["InputReplyRemarks"]),
                                IsReplied = Convert.ToString(dr["IsReplied"]),
                                IntermediaryResponseNeeded = Convert.ToString(dr["IntermediaryResponseNeeded"]),
                                Is72hrs = Convert.ToString(dr["is72hrs"]),
                                FilePath = Convert.ToString(dr["FilePath"]),                             
                            });
                        }
                        else if (ListType == "COMPLIANCE")
                        {
                            Data.ComplianceforIntermediary.Add(new ComplianceList
                            {
                                GrievanceId = Convert.ToString(dr["GrievanceId"]),
                                Justification = Convert.ToString(dr["Justification"]),
                                RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                                IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                                ReceiptDate = Convert.ToString(dr["ReceiptDate"]),
                                CitizenName = Convert.ToString(dr["CitizenName"]),
                                CorrespondingITRule = Convert.ToString(dr["CorrespondingITRule"]),
                                GOEmail = Convert.ToString(dr["GOEmail"]),
                                GOName = Convert.ToString(dr["GOName"]),
                                GroundTitle = Convert.ToString(dr["GroundTitle"]),
                                HelpLink = Convert.ToString(dr["HelpLink"]),
                                IntermediaryURL = Convert.ToString(dr["IntermediaryURL"]),
                                IntermediayAddress = Convert.ToString(dr["IntermediayAddress"]),
                                ReliefTitle = Convert.ToString(dr["ReliefTitle"]),
                                RelieftSoughtSpecification = Convert.ToString(dr["RelieftSoughtSpecification"]),
                                UserEmail = Convert.ToString(dr["UserEmail"]),
                                UserMobile = Convert.ToString(dr["UserMobile"]),
                                IsResponded = Convert.ToString(dr["IsResponded"]),
                                ResponseTime = Convert.ToString(dr["ResponseTime"]),
                                DecisionDate = Convert.ToString(dr["DecisionDate"]),
                                DecisionDesc = Convert.ToString(dr["DecisionDesc"]),
                                DecisionRemarks = Convert.ToString(dr["DecisionRemarks"]),
                                IsComplianceAdded = Convert.ToString(dr["IsComplianceAdded"]),
                                IntermediaryResponseNeeded = Convert.ToString(dr["IntermediaryResponseNeeded"]),
                                Is72hrs = Convert.ToString(dr["is72hrs"]),
                                FilePath = Convert.ToString(dr["FilePath"])
                            });
                        }
                        else
                        {
                            Data.AppealforIntermediary.Add(new AppealList
                            {
                                GrievanceId = Convert.ToString(dr["GrievanceId"]),
                                Justification = Convert.ToString(dr["Justification"]),
                                RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                                IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                                ReceiptDate = Convert.ToString(dr["ReceiptDate"]),
                                CitizenName = Convert.ToString(dr["CitizenName"]),
                                CorrespondingITRule = Convert.ToString(dr["CorrespondingITRule"]),
                                GOEmail = Convert.ToString(dr["GOEmail"]),
                                GOName = Convert.ToString(dr["GOName"]),
                                GroundTitle = Convert.ToString(dr["GroundTitle"]),
                                HelpLink = Convert.ToString(dr["HelpLink"]),
                                IntermediaryURL = Convert.ToString(dr["IntermediaryURL"]),
                                IntermediayAddress = Convert.ToString(dr["IntermediayAddress"]),
                                ReliefTitle = Convert.ToString(dr["ReliefTitle"]),
                                RelieftSoughtSpecification = Convert.ToString(dr["RelieftSoughtSpecification"]),
                                UserEmail = Convert.ToString(dr["UserEmail"]),
                                UserMobile = Convert.ToString(dr["UserMobile"]),
                                IsResponded = Convert.ToString(dr["IsResponded"]),
                                ResponseTime = Convert.ToString(dr["ResponseTime"]),
                                DecisionDate = Convert.ToString(dr["DecisionDate"]),
                                DecisionDesc = Convert.ToString(dr["DecisionDesc"]),
                                DecisionRemarks = Convert.ToString(dr["DecisionRemarks"]),
                                IntermediaryResponseNeeded = Convert.ToString(dr["IntermediaryResponseNeeded"]),
                                Is72hrs = Convert.ToString(dr["is72hrs"]),
                                IsComplianceAdded = Convert.ToString(dr["IsComplianceAdded"]),
                                FilePath = Convert.ToString(dr["FilePath"]),
                                ApprovalStatusofTimeExtended = Convert.ToString(dr["ApprovalStatusofTimeExtended"]),
                                NewReceiptDate = Convert.ToString(dr["NewReceiptDate"]),
                            });
                        }


                    }
                }
            }
            return Data;
        }
    }
    public class AppealList
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string ReceiptDate { get; set; }
        public string CitizenName { get; set; }
        public string Justification { get; set; }
        public string UserMobile { get; set; }
        public string UserEmail { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryURL { get; set; }
        public string GOName { get; set; }
        public string GOEmail { get; set; }
        public string HelpLink { get; set; }
        public string IntermediayAddress { get; set; }
        public string GroundTitle { get; set; }
        public string CorrespondingITRule { get; set; }
        public string ReliefTitle { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string IsResponded { get; set; }
        public string ResponseTime { get; set; }
        public string DecisionDate { get; set; }
        public string DecisionDesc { get; set; }
        public string DecisionRemarks { get; set; }
        public string IntermediaryResponseNeeded { get; set; }
        public string Is72hrs { get; set; }
        public string IsComplianceAdded { get; set; }
        public string FilePath { get; set; }
        public string ApprovalStatusofTimeExtended { get; set; }
        public string NewReceiptDate { get; set; }

    }
    public class AdditionalInputList
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string ReceiptDate { get; set; }
        public string CitizenName { get; set; }
        public string Justification { get; set; }
        public string UserMobile { get; set; }
        public string UserEmail { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryURL { get; set; }
        public string GOName { get; set; }
        public string GOEmail { get; set; }
        public string HelpLink { get; set; }
        public string IntermediayAddress { get; set; }
        public string GroundTitle { get; set; }
        public string CorrespondingITRule { get; set; }
        public string ReliefTitle { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string IsResponded { get; set; }
        public string ResponseTime { get; set; }
        public string InputId { get; set; }
        public string InputSeekRemarks { get; set; }
        public string InputRequestNo { get; set; }
        public string InputSeekDateTime { get; set; }
        public string InputReplyDateTime { get; set; }
        public string InputReplyRemarks { get; set; }
        public string IsReplied { get; set; }
        public string IsComplianceAdded { get; set; }
        public string IntermediaryResponseNeeded { get; set; }
        public string Is72hrs { get; set; }
        public string FilePath { get; set; }

    }
    public class ComplianceList
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string ReceiptDate { get; set; }
        public string CitizenName { get; set; }
        public string Justification { get; set; }
        public string UserMobile { get; set; }
        public string UserEmail { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryURL { get; set; }
        public string GOName { get; set; }
        public string GOEmail { get; set; }
        public string HelpLink { get; set; }
        public string IntermediayAddress { get; set; }
        public string GroundTitle { get; set; }
        public string CorrespondingITRule { get; set; }
        public string ReliefTitle { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string IsResponded { get; set; }
        public string ResponseTime { get; set; }
        public string DecisionDate { get; set; }
        public string DecisionDesc { get; set; }
        public string DecisionRemarks { get; set; }
        public string IsComplianceAdded { get; set; }
        public string IntermediaryResponseNeeded { get; set; }
        public string Is72hrs { get; set; }
        public string FilePath { get; set; }

    }
    public class AddCompliance
    {
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        public string HeaderText { get; set; }
        //New
        public string GrievanceDesc { get; set; }
        public string ReceiptDate { get; set; }
        public string GroundTitle { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string ReliefTitle { get; set; }
        [Required(ErrorMessage = "Select Date")]
        public string ComplianceDate { get; set; }
        public string BriefofComplaint { get; set; }
        public string isSupportingDocument { get; set; }
        [Required(ErrorMessage = "Enter Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        public string Remarks { get; set; }
        [Required(ErrorMessage = "Enter URL")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',:.]+$", ErrorMessage = "Invalid Characters!")]
        public string URL { get; set; }
        public string FilePath { get; set; }
        [Required(ErrorMessage = "Add Document")]
        public HttpPostedFileBase SupportingDocument { get; set; }
    }
    public class Intermediarys
    {
        public List<IntermediaryList> List { get; set; }
    }
    public class IntermediaryList
    {
        public List<SelectListItem> IntermediariesList { get; set; }
        public string MultipleIntemediaries { get; set; }
        public string Type { get; set; }
        public string HeaderText { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Intermediary Id.")]
        public string IntermediaryId { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        //[RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s.]+$", ErrorMessage = "Invalid Characters!")]
        [StringLength(100, ErrorMessage = "Max. 100 characters allowed!")]
        public string IntermediaryTitle { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [Url( ErrorMessage = "Invalid URL!")]
        [StringLength(150, ErrorMessage = "Max. 150 characters allowed!")]
       [UrlValid(ErrorMessage = "The URL entered is not live.")]
        public string URL { get; set; }
        public string IsActive { get; set; }
        public string IntermediaryType { get; set; }
        //[Required(ErrorMessage = "Mandatory Field!")]
        //[StringLength(150, ErrorMessage = "Max. 150 characters allowed!")]
        [RegularExpression(@"^[a-zA-Z.]+$", ErrorMessage = "Invalid Characters!")]
        [StringLength(100, ErrorMessage = "Max. 100 characters allowed!")]
        public string GOName { get; set; }
        
       
        [Required(ErrorMessage = "Mandatory Field!")]
        [EmailAddress(ErrorMessage = "Invalid Email Format.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Characters in Email Address!")]
        [StringLength(100, ErrorMessage = "Max. 100 characters allowed!")]
        public string GOEmail { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Characters in Email Address!")]
        [EmailAddress(ErrorMessage = "Invalid Email Format.")]
        [StringLength(100, ErrorMessage = "Max. 100 characters allowed!")]
        public string LoginEmail { get; set; }

        //[RegularExpression(@"^[a-zA-Z0-9()\s\-/',.:?=]+$", ErrorMessage = "Invalid Characters!")]
        [Url( ErrorMessage = "Invalid Characters!")]
        [UrlValid(ErrorMessage = "The URL entered is not live.")]
        public string HelpLink { get; set; }
        
        //[RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-',.]+$", ErrorMessage = "Invalid Characters!")]
        [StringLength(300, ErrorMessage = "Max. 300 characters allowed!")]
        public string Address { get; set; }

    }

    public class UrlValidAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string url = (string)value;
            if (string.IsNullOrEmpty(url))
                return true;

            HttpWebResponse res;
            try
            {
                Uri uri = new Uri(url, UriKind.Absolute);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                res = (HttpWebResponse)req.GetResponse();
            }
            catch
            {
                return false;
            }

            switch (res.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Redirect:
                case HttpStatusCode.RedirectKeepVerb:
                case HttpStatusCode.RedirectMethod:
                    return true;
            }

            return false;
        }
    }

    public class ReliefSoughts
    {
        public List<ReliefSoughtList> List { get; set; }
    }
    public class ReliefSoughtList
    {
        public string ReliefId { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(150, ErrorMessage = "Max. 150 characters allowed!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        public string ReliefTitle { get; set; }
        public string IsActive { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(150, ErrorMessage = "Max. 150 characters allowed!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        public string SpecificationLabel { get; set; }
       
    }
    public class GroundforAppeals
    {
        public List<GroundforAppealsList> List { get; set; }
    }
    public class GroundforAppealsList
    {
        public string GroundId { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(200, ErrorMessage = "Max. 200 characters allowed!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        public string GroundTitle { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(50, ErrorMessage = "Max. 50 characters allowed!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        public string CorrespondingITRule { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string GACId { get; set; }
        public List<SelectListItem> GACList { get; set; }
        public string GACName { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only numbers are allowed!")]

        public string ListOrder { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string EntryRequired { get; set; }
        //[Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(100, ErrorMessage = "Max. 100 characters allowed!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        public string EntryFieldLabel { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string IntermediaryResponseNeeded { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(4000, ErrorMessage = "Max. 4000 characters allowed!")]
        public string ITRuleExtract { get; set; }
        //[Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(1000, ErrorMessage = "Max. 1000 characters allowed!")]
        public string Grounds { get; set; }
        public string IsActive { get; set; }
    }
    public class ExtendResponseTimeforAppeal
    {
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        public string ReceiptDate { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(200, ErrorMessage = "Max. 200 characters allowed!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        public string Remarks { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string ResponseTimeID { get; set; }
        public string ResponseTimeDesc { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalStatusDesc { get; set; }
        public string ApprovalDateTime { get; set; }
        public string ApprovalRemarks { get; set; }
        public List<SelectListItem> ResponseTimeList { get; set; }
    }
    public class ExtendResponseTimeforAppeal_List
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string OpenFrom { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string ResponseTimeID { get; set; }
        public List<SelectListItem> ResponseTimeList { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string ApproveReject { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(200, ErrorMessage = "Max. 200 characters allowed!")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]
        public string Remarks { get; set; }
        public List<_ExtendResponseTimeforAppeal_List> ExtendResponseTimeforAppealList { get; set; }
        public static ExtendResponseTimeforAppeal_List GetData()
        {
            ExtendResponseTimeforAppeal_List Appeal = new ExtendResponseTimeforAppeal_List { ExtendResponseTimeforAppealList = new List<_ExtendResponseTimeforAppeal_List>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "List")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_ExtendTimeRequestApprove", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Appeal.ExtendResponseTimeforAppealList.Add(new _ExtendResponseTimeforAppeal_List
                        {
                            ApprovalStatus = Convert.ToString(dr["ApprovalStatus"]),
                            CreationIP = Convert.ToString(dr["CreationIP"]),
                            GrievanceID = Convert.ToString(dr["GrievanceID"]),
                            Remarks = Convert.ToString(dr["Remarks"]),
                            ApprovalStatusDesc = Convert.ToString(dr["ApprovalStatusDesc"]),
                            RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                            CreatedBy = Convert.ToString(dr["CreatedBy"]),
                            CreationDateTime = Convert.ToString(dr["CreationDateTime"]),
                            ReceiptDate = Convert.ToString(dr["ReceiptDate"]),
                            ResponseTimeDesc = Convert.ToString(dr["ResponseTimeDesc"]),
                            ResponseTimeID = Convert.ToString(dr["ResponseTimeID"]),
                            IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                            IntermediaryGROEmail = Convert.ToString(dr["IntermediaryGROEmail"]),
                            ApprovalDateTime = Convert.ToString(dr["ApprovalDateTime"]),
                            ApprovalRemarks = Convert.ToString(dr["ApprovalRemarks"]),
                        });
                    }
                }
            }
            return Appeal;
        }


    }
    public class _ExtendResponseTimeforAppeal_List
    {
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string ReceiptDate { get; set; }
        public string ResponseTimeID { get; set; }
        public string ResponseTimeDesc { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalStatusDesc { get; set; }
        public string Remarks { get; set; }
        public string CreationDateTime { get; set; }
        public string CreationIP { get; set; }
        public string CreatedBy { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryGROEmail { get; set; }
        public string ApprovalDateTime { get; set; }
        public string ApprovalRemarks { get; set; }

    }
    public class ChangePasswordIntermediary
    {
        public string UserId { get; set; }
        public string IsPasswordSet { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        //[MinLength(8, ErrorMessage = "Min. 8 characters allowed!")]
        [StringLength(15, ErrorMessage = "Max. 15 characters allowed!")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [MinLength(8, ErrorMessage = "Min. 8 characters allowed!")]
        [StringLength(15, ErrorMessage = "Max. 15 characters allowed!")]

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*(_|[^\w])).+$", ErrorMessage = "The password does not comply with the password policy. Please check the Password requirements!")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [MinLength(8, ErrorMessage = "Min. 8 characters allowed!")]
        [StringLength(15, ErrorMessage = "Max. 15 characters allowed!")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string HashCurrentPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string HashNewPassword { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(6, ErrorMessage = "Max. 6 characters allowed!")]
        public string OTP { get; set; }
    }
}