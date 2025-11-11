using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.SubjectExpert.Data
{
    public class SubjectExpertDash
    {
    }
    public class LoginviaOTP
    {
        [Required(ErrorMessage = "Enter Email Id")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Only 5 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Characters in Email Address!")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "Enter OTP")]
        public string OTP { get; set; }
        [Required(ErrorMessage = "Enter Captcha")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Only 4 digits")]
        public string Captcha { get; set; }
    }

    public class SubjectExpertDash_Stats
    {
        public string AppealsReceived { get; set; }
        public string AppealsSubmitted { get; set; }
        public string AppealsPending { get; set; }
        public string Pending_Percent { get; set; }
        public string Submit_Percent { get; set; }
        public static SubjectExpertDash_Stats GetData(string MobileNo)
        {
            SubjectExpertDash_Stats Data = new SubjectExpertDash_Stats();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@MobileNo", MobileNo),
                new KeyValuePair<string, string>("@Mode", "STATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "SubjectExpert_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Data.AppealsReceived = Convert.ToString(ds.Tables[0].Rows[0]["AppealsReceived"]);
                    Data.AppealsPending = Convert.ToString(ds.Tables[0].Rows[0]["AppealsPending"]);
                    Data.AppealsSubmitted = Convert.ToString(ds.Tables[0].Rows[0]["AppealsSubmitted"]);
                    Data.Pending_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Pending_Percent"]);
                    Data.Submit_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Submit_Percent"]);
                }
            }
            return Data;
        }

    }

    public class SubjectExpert_AppealList
    {
        public string ListName { get; set; }
        public List<AppealList> AppealsList { get; set; }
        public static SubjectExpert_AppealList GetData(string MobileNo, string ListType = "PENDING", Dictionary<string, string> LabelDictionary = null)
        {
            string ListName = string.Empty;
            SubjectExpert_AppealList Data = new SubjectExpert_AppealList();
            if (ListType == "RECEIVED")
            {
                ListName = LabelDictionary["AssistanceRequested"];
            }
            else if (ListType == "PENDING")
            {
                ListName = LabelDictionary["PendingforInput"];
            }
            else if (ListType == "SUBMITTED")
            {
                ListName = LabelDictionary["InputSubmitted"];
            }

            Data.ListName = ListName;
            Data.AppealsList = new List<AppealList>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@MobileNo", MobileNo),
                new KeyValuePair<string, string>("@ListType", ListType),
                new KeyValuePair<string, string>("@Mode", "LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "SubjectExpert_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        Data.AppealsList.Add(new AppealList
                        {
                            GrievanceId = Convert.ToString(dr["GrievanceId"]),
                            Justification = Convert.ToString(dr["Justification"]),
                            RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                            IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                            ReceiptDate = Convert.ToString(dr["ReceiptDate"]),
                            CitizenName = Convert.ToString(dr["CitizenName"]),
                            CorrespondingITRule = Convert.ToString(dr["CorrespondingITRule"]),
                            GroundTitle = Convert.ToString(dr["GroundTitle"]),
                            IntermediaryURL = Convert.ToString(dr["IntermediaryURL"]),
                            ReliefTitle = Convert.ToString(dr["ReliefTitle"]),
                            RelieftSoughtSpecification = Convert.ToString(dr["RelieftSoughtSpecification"]),
                            UserEmail = Convert.ToString(dr["UserEmail"]),
                            UserMobile = Convert.ToString(dr["UserMobile"]),
                            InputId = Convert.ToString(dr["InputId"]),
                            InputSeekRemarks = Convert.ToString(dr["InputSeekRemarks"]),
                            InputRequestNo = Convert.ToString(dr["InputRequestNo"]),
                            InputSeekDateTime = Convert.ToString(dr["InputSeekDateTime"]),
                            InputReplyDateTime = Convert.ToString(dr["InputReplyDateTime"]),
                            InputReplyRemarks = Convert.ToString(dr["InputReplyRemarks"]),
                            IsReplied = Convert.ToString(dr["IsReplied"]),
                        });
                    }
                }         
            }
            return Data;
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
            public string GroundTitle { get; set; }
            public string CorrespondingITRule { get; set; }
            public string ReliefTitle { get; set; }
            public string RelieftSoughtSpecification { get; set; }
            public string InputId { get; set; }
            public string InputSeekRemarks { get; set; }
            public string InputRequestNo { get; set; }
            public string InputSeekDateTime { get; set; }
            public string InputReplyDateTime { get; set; }
            public string InputReplyRemarks { get; set; }
            public string IsReplied { get; set; }

        }
    }
    public class Experts
    {
        public List<ExpertsList> List { get; set; }
    }
    public class ExpertsList
    {
        public string UserID { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(1000, ErrorMessage = "Max. 150 characters allowed!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Please enter a valid mobile number!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Please enter a valid mobile number!")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(150, ErrorMessage = "Max. 150 characters allowed!")]
        public string AreaofExpertise { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Only 10 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(((gov|nic)\.in))+$", ErrorMessage = "Invalid Characters in Email Address!")]
        public string EmailID { get; set; }
        public string IsActive { get; set; }

    }
}