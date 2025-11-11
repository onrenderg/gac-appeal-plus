using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DigitalNagrik.Areas.GridView.Data
{
    public class AppealHistoryReport
    {
        public string FromDt { get; set; }
        public string ToDt { get; set; }
        public List<AppealHistoryReport_List> HistoryList { get; set; }

        public static AppealHistoryReport GetAppealHistoryReportData(string FromDt, string ToDt)
        {
            AppealHistoryReport Appeal = new AppealHistoryReport() { HistoryList = new List<AppealHistoryReport_List>(), FromDt = FromDt, ToDt = ToDt };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@FromDt", FromDt),
                new KeyValuePair<string, string>("@ToDt", ToDt),
                new KeyValuePair<string, string>("@Mode", "REPORT")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AppealHistoryReport", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Appeal.HistoryList.Add(new AppealHistoryReport_List
                        {
                            RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                            GrievanceId = Convert.ToString(dr["GrievanceId"]),
                            AppealDt = Convert.ToString(dr["AppealDt"]),
                            AppellantNm = Convert.ToString(dr["AppellantNm"]),
                            IntermediaryNm = Convert.ToString(dr["IntermediaryNm"]),
                            MatterSummary = Convert.ToString(dr["MatterSummary"]),
                            GACId = Convert.ToString(dr["AssignedToGAC"]),
                            GACNm = Convert.ToString(dr["GACTitle"]),
                            AssistantManagerObservation = Convert.ToString(dr["ObservationText"]),
                            IntermediaryResponseText = Convert.ToString(dr["ResponseText"]),
                            IntermediaryResponseValue = Convert.ToString(dr["ResponseValue"]),
                            IntermediaryResponseDetails = Convert.ToString(dr["ResponseDetails"]),
                            IntermediaryResponseURLs = Convert.ToString(dr["ResponseURLs"]),
                            IntermediaryResponseFile = Convert.ToString(dr["IntermediaryFile"]),
                            MemberComment_1 = Convert.ToString(dr["MemberComment_1"]),
                            MemberComment_2 = Convert.ToString(dr["MemberComment_2"]),
                            ChairpersonComment = Convert.ToString(dr["ChairpersonComent"]),
                            FinalDecision = Convert.ToString(dr["DraftDecision"]),
                            NotifAppellant = Convert.ToString(dr["AppellantNotif"]),
                        });
                    }
                }
            }
            return Appeal;
        }

        public static DataTable GetAppealHistoryReportDataTable(string FromDt, string ToDt)
        {
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@FromDt", FromDt),
                new KeyValuePair<string, string>("@ToDt", ToDt),
                new KeyValuePair<string, string>("@Mode", "REPORT_EXCEL")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AppealHistoryReport", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            return new DataTable();
        }
    }

    public class AppealHistoryReport_List
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string AppealDt { get; set; }
        public string AppellantNm { get; set; }
        public string IntermediaryNm { get; set; }
        public string MatterSummary { get; set; }
        public string GACId { get; set; }
        public string GACNm { get; set; }
        public string AssistantManagerObservation { get; set; }
        public string IntermediaryResponseText { get; set; }
        public string IntermediaryResponseValue { get; set; }
        public string IntermediaryResponseDetails { get; set; }
        public string IntermediaryResponseURLs { get; set; }
        public string IntermediaryResponseFile { get; set; }
        public string MemberComment_1 { get; set; }
        public string MemberComment_2 { get; set; }
        public string ChairpersonComment { get; set; }
        public string FinalDecision { get; set; }
        public string NotifAppellant { get; set; }
    }

    public class SearchAppealDetails
    {
        public List<SearchAppealDetails_List> AppealDetailsList { get; set; }
        public string RegistrationYear { get; set; }
        [Required(ErrorMessage = "Select Parameter!")]
        public string SearchParameter { get; set; }
        [Required(ErrorMessage = "Enter Search Text!")]
        public string SearchText { get; set; }
        public string GrievanceId { get; set; }
        public string AppealID { get; set; }
        public static SearchAppealDetails GetAppealDetails(string SearchParam, string SearchText)
        {
            string SearchTextDetails = string.Empty; string GrievanceID = string.Empty; string RegistrationYear = string.Empty;
            SearchAppealDetails Data = new SearchAppealDetails();
            Data.AppealDetailsList = new List<SearchAppealDetails_List>();
            SearchTextDetails = SearchText;
            if (SearchParam == "A")
            {
                GrievanceID = SearchTextDetails.Split('/')[0];
                RegistrationYear = SearchTextDetails.Split('/')[1];
                Data.RegistrationYear = RegistrationYear;
                Data.GrievanceId = GrievanceID;
            }
            string GACID = DigitalNagrik.Models.UserSession.RoleID == DigitalNagrik.Models.UserRoles.Secretariat ? "" : DigitalNagrik.Models.UserSession.GACID;
            List <KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceID),
                new KeyValuePair<string, string>("@SearchTextDetails", SearchTextDetails),
                new KeyValuePair<string, string>("@SearchParam", SearchParam),
                new KeyValuePair<string, string>("@GacID", GACID),
                new KeyValuePair<string, string>("@Mode", "Get_Details")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "SearchAppeal_Action", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.AppealDetailsList.Add(new SearchAppealDetails_List
                        {
                            GrievanceId = (Convert.ToString(dr["GrievanceId"])),
                            ActionDesc = (Convert.ToString(dr["ActionDesc"])),
                            ActionId = (Convert.ToString(dr["ActionId"])),
                            AssignedDateTime = (Convert.ToString(dr["AssignedDateTime"])),
                            GACAbbr = (Convert.ToString(dr["GACAbbr"])),
                            GACId = (Convert.ToString(dr["GACId"])),
                            AppealantMobile = (Convert.ToString(dr["UserMobile"])),
                            AppealantName = (Convert.ToString(dr["UserName"])),
                            RegistrationYear = (Convert.ToString(dr["RegistrationYear"])),
                            AppealDate = (Convert.ToString(dr["AppealDate"])),
                            //AppealID = (Convert.ToString(dr["RegistrationYear"])),   
                        });
                    }
                }
            }
            return Data;
        }
    }
    public class SearchAppealDetails_List
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string AppealID { get; set; }
        public string GACId { get; set; }
        public string GACAbbr { get; set; }
        public string AssignedDateTime { get; set; }
        public string ActionId { get; set; }
        public string ActionDesc { get; set; }
        public string AppealantMobile { get; set; }
        public string AppealantName { get; set; }
        public string AppealDate { get; set; }
    }
}