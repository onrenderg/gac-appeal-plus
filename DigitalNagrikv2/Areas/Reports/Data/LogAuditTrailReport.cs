using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.Reports.Data
{
    public class LogAuditTrailReport
    {
    }
    public class LogAuditTrailParams
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string FromDate { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string ToDate { get; set; }
        //[Required(ErrorMessage = "Mandatory Field!")]
        public string UserTypeID { get; set; }
        public List<SelectListItem> UserTypeList { get; set; }
        //[Required(ErrorMessage = "Mandatory Field!")]
        public string UserID { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public static LogAuditTrailParams GetData()
        {
            LogAuditTrailParams Data = new LogAuditTrailParams();
            Data.UserTypeList = new List<SelectListItem>();
            Data.UserList = new List<SelectListItem>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "Params")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "LogAuditTrailReport", SP_Parameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.UserTypeList.Add(new SelectListItem { Value = Convert.ToString(dr["UserTypeId"]), Text = Convert.ToString(dr["UserTypeName"]), });
                    }
                }
            }

            return Data;
        }
    }
    public class LogAuditTrailReportData
    {
        public List<ReportDataList_LoginStatus> ReportDataList_LoginStatus_List { get; set; }
        public List<ReportDataList_Actions> ReportDataList_Actions_List { get; set; }
        public static LogAuditTrailReportData GetData(string UserID = "", string UserTypeID = "", string FromDate = "", string ToDate = "")
        {
            LogAuditTrailReportData Data = new LogAuditTrailReportData();
            Data.ReportDataList_LoginStatus_List = new List<ReportDataList_LoginStatus>();
            Data.ReportDataList_Actions_List = new List<ReportDataList_Actions>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "DataList"),
                new KeyValuePair<string, string>("@UserID", UserID),
                new KeyValuePair<string, string>("@UserTypeID", UserTypeID),
                new KeyValuePair<string, string>("@FromDate", FromDate),
                new KeyValuePair<string, string>("@ToDate", ToDate),
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "LogAuditTrailReport", SP_Parameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.ReportDataList_LoginStatus_List.Add(new ReportDataList_LoginStatus
                        {
                            UserId = Convert.ToString(dr["UserId"]),
                            AuthenticationStatus = Convert.ToString(dr["AuthenticationStatus"]),
                            IPAddress = Convert.ToString(dr["IPAddress"]),
                            LoginDate = Convert.ToString(dr["LoginDate"]),
                            LoginDateTime = Convert.ToString(dr["LoginDateTime"]),
                            StartTime = Convert.ToString(dr["StartTime"]),
                            LogoutDate = Convert.ToString(dr["LogoutDate"]),                       
                            LogoutDateTime = Convert.ToString(dr["LogoutDateTime"]),                       
                            EndTime = Convert.ToString(dr["EndTime"]),                       
                            NewLoginDateTime = Convert.ToString(dr["NewLoginDateTime"]),                       
                            UserType = Convert.ToString(dr["UserType"]),                       
                            UserName = Convert.ToString(dr["UserName"]),                       
                        });
                    }

                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        Data.ReportDataList_Actions_List.Add(new ReportDataList_Actions
                        {
                            ActionBy = Convert.ToString(dr["ActionBy"]),
                            ActionByIP = Convert.ToString(dr["ActionByIP"]),
                            ActionDateTime = Convert.ToString(dr["ActionDateTime"]),
                            ActionDate = Convert.ToString(dr["ActionDate"]),
                            StartTime = Convert.ToString(dr["StartTime"]),
                            AppealNumber = Convert.ToString(dr["AppealNumber"]),
                            ActionDesc = Convert.ToString(dr["ActionDesc"]),
                            //GrievanceId = Convert.ToString(dr["GrievanceId"]),
                            //RegistrationYear = Convert.ToString(dr["RegistrationYear"])
                        });
                    }

                }
            }
            return Data;
        }
    }
    public class ReportDataList_LoginStatus
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string AuthenticationStatus { get; set; }
        public string LoginDate { get; set; }
        public string NewLoginDateTime { get; set; }
        public string LoginDateTime { get; set; }
        public string StartTime { get; set; }
        public string LogoutDate { get; set; }
        public string LogoutDateTime { get; set; }
        public string EndTime { get; set; }
        public string IPAddress { get; set; }
    }
    public class ReportDataList_Actions
    {
        public string AppealNumber { get; set; }
        //public string RegistrationYear { get; set; }
        //public string GrievanceId { get; set; }
        public string ActionDate { get; set; }
        public string ActionDateTime { get; set; }
        public string StartTime { get; set; }
        public string ActionByIP { get; set; }
        public string ActionBy { get; set; }
        public string ActionDesc { get; set; }
    }
}
