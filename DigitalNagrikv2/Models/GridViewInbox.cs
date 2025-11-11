using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class GridViewInbox
    {
        public class MenuStatus
        {
            public const string Inbox = "P";
            public const string Assigned = "AS";
            public const string Unassigned = "UA";
            public const string Rejected = "RJ";
            public const string ReferredBack = "RF";
            public const string Disposed = "D";
        }

        public class GrievanceStatus
        {
            public const string Final = "2";
            public const string Accepted = "3";
            public const string ReferBack = "4";
            public const string Rejected = "5";
            public const string Disposed = "6";
        }

        public class Action
        {
            public const string Accept = "1";
            public const string ReferBack = "2";
            public const string Reject = "3";
        }

        public static string ConvertMinToDHM(string Minutes)
        {
            if (double.TryParse(Minutes, out double Mins))
            {
                double Day = Math.Floor(Mins / (60 * 24));
                double Hour = Math.Floor((Mins - (Day * 60 * 24)) / 60);
                double Min = Math.Round(Mins % 60);
                return (Day != 0 ? "<span>" + Day + "d </span>" : "") + (Hour != 0 ? "<span>" + Hour + "h </span>" : "") + (Min != 0 || (Day == 0 && Hour == 0 && Min == 0) ? "<span>" + Min + "m </span>" : "");
            }
            return "<span>0m</span>";
        }

        public List<GridViewInbox_Menu> MenuList { get; set; }

        public static GridViewInbox GetMenuList()
        {
            GridViewInbox Menu = new GridViewInbox() { MenuList = new List<GridViewInbox_Menu>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "MENU")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_GridViewInbox", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Menu.MenuList.Add(new GridViewInbox_Menu
                    {
                        MenuNm = Convert.ToString(dr["MenuNm"]),
                        MenuStatusCd = Convert.ToString(dr["MenuStatusCd"]),
                        Icon = Convert.ToString(dr["Icon"]),
                        CssClass = Convert.ToString(dr["CssClass"]),
                        AppealCount = Convert.ToString(dr["AppealCount"])
                    });
                }
            }
            return Menu;
        }
    }

    public class GridViewInbox_Appeal
    {
        public string MenuStatusCd { get; set; }
        public string IsDateGroup { get; set; }
        public string MenuNm { get; set; }
        public string Icon { get; set; }
        public string CssClass { get; set; }
        public List<GridViewInbox_AppealList> AppealList { get; set; }

        public static GridViewInbox_Appeal GetAppealList(string MenuStatusCd)
        {
            GridViewInbox_Appeal Appeal = new GridViewInbox_Appeal() { AppealList = new List<GridViewInbox_AppealList>(), MenuStatusCd = MenuStatusCd };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@MenuStatusCd", MenuStatusCd),
                new KeyValuePair<string, string>("@Mode", "APPEAL_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_GridViewInbox", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Appeal.AppealList.Add(new GridViewInbox_AppealList
                        {
                            DayDesc = Convert.ToString(dr["DayDesc"]),
                            DayRange = Convert.ToString(dr["DayRange"]),
                            ReceiptDate = Convert.ToString(dr["ReceiptDate"]),
                            RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                            GrievanceId = Convert.ToString(dr["GrievanceId"]),
                            GrievanceNo = Convert.ToString(dr["GrievanceNo"]),
                            GrievanceDesc = Convert.ToString(dr["GrievanceDesc"]),
                            ContentClassificationDesc = Convert.ToString(dr["ContentClassificationDesc"]),
                            SubContentClassificationDesc = Convert.ToString(dr["SubContentClassificationDesc"]),
                            IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                            PendingForMins = Convert.ToString(dr["PendingForMins"]),
                            GrievanceStatus = Convert.ToString(dr["GrievnaceStatus"]),
                            AppellantNm = Convert.ToString(dr["AppellantNm"]),
                            GACAbbr = Convert.ToString(dr["GACAbbr"]),
                            GACTitle = Convert.ToString(dr["GACTitle"])
                        });
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    Appeal.MenuNm = Convert.ToString(ds.Tables[1].Rows[0]["MenuNm"]);
                    Appeal.Icon = Convert.ToString(ds.Tables[1].Rows[0]["Icon"]);
                    Appeal.CssClass = Convert.ToString(ds.Tables[1].Rows[0]["CssClass"]);
                }
            }
            return Appeal;
        }
    }

    public class GridViewInbox_AppealList
    {
        public string DayDesc { get; set; }
        public string DayRange { get; set; }
        public string ReceiptDate { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string GrievanceNo { get; set; }
        public string GrievanceDesc { get; set; }
        public string ContentClassificationDesc { get; set; }
        public string SubContentClassificationDesc { get; set; }
        public string IntermediaryTitle { get; set; }
        public string PendingForMins { get; set; }
        public string GrievanceStatus { get; set; }
        public string AppellantNm { get; set; }
        public string GACAbbr { get; set; }
        public string GACTitle { get; set; }
    }

    public class GridViewInbox_AppealDetails
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string GrievanceNo { get; set; }
        public string GrievanceDesc { get; set; }
        public string Appeal_ReceiptDate { get; set; }
        public string Appeal_Reason { get; set; }
        public string Appeal_ContentClassificationDesc { get; set; }
        public string Appeal_SubContentClassificationDesc { get; set; }

        public string User_UserId { get; set; }
        public string User_UserProfileId { get; set; }
        public string User_FullNm { get; set; }
        public string User_Mobile { get; set; }
        public string User_Email { get; set; }
        public string User_Address { get; set; }
        public string User_Occupation { get; set; }


        public string Intermediary_Id { get; set; }
        public string Intermediary_Title { get; set; }
        public string Intermediary_Url { get; set; }
        public string Intermediary_Address { get; set; }
        public string Intermediary_GROName { get; set; }
        public string Intermediary_GROEmail { get; set; }

        public string PlatformNm { get; set; }

        public string Content_GACId { get; set; }
        public string Assign_GACId { get; set; }
        public string Assign_GACDesc { get; set; }
        public string Assign_GACAbbr { get; set; }

        public string GrievanceStatusCd { get; set; }
        public string GrievanceStatusDesc { get; set; }
        public string GrievanceStatusDt { get; set; }

        public static GridViewInbox_AppealDetails GetAppealDetails(string RegistrationYear, string GrievanceId)
        {
            GridViewInbox_AppealDetails Appeal = new GridViewInbox_AppealDetails();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "APPEAL_DETAILS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_GridViewInbox", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Appeal.RegistrationYear = Convert.ToString(ds.Tables[0].Rows[0]["RegistrationYear"]);
                Appeal.GrievanceId = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceId"]);
                Appeal.GrievanceNo = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceNo"]);
                Appeal.GrievanceDesc = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceDesc"]);
                Appeal.Appeal_ReceiptDate = Convert.ToString(ds.Tables[0].Rows[0]["ReceiptDate"]);
                Appeal.Appeal_Reason = Convert.ToString(ds.Tables[0].Rows[0]["ReasonForAppeal"]);
                Appeal.Appeal_ContentClassificationDesc = Convert.ToString(ds.Tables[0].Rows[0]["ContentClassificationDesc"]);
                Appeal.Appeal_SubContentClassificationDesc = Convert.ToString(ds.Tables[0].Rows[0]["SubContentClassificationDesc"]);
                Appeal.User_UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserId"]);
                Appeal.User_UserProfileId = Convert.ToString(ds.Tables[0].Rows[0]["UserProfileId"]);
                Appeal.User_FullNm = Convert.ToString(ds.Tables[0].Rows[0]["FullNm"]);
                Appeal.User_Mobile = Convert.ToString(ds.Tables[0].Rows[0]["UserMobile"]);
                Appeal.User_Email = Convert.ToString(ds.Tables[0].Rows[0]["UserEmail"]);
                Appeal.User_Address = Convert.ToString(ds.Tables[0].Rows[0]["Address"]);
                Appeal.User_Occupation = Convert.ToString(ds.Tables[0].Rows[0]["Occupation"]);
                Appeal.Intermediary_Id = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryId"]);
                Appeal.Intermediary_Title = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryTitle"]);
                Appeal.Intermediary_Url = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryURL"]);
                Appeal.Intermediary_Address = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryAddress"]);
                Appeal.Intermediary_GROName = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryGROName"]);
                Appeal.Intermediary_GROEmail = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryGROEmail"]);
                Appeal.PlatformNm = Convert.ToString(ds.Tables[0].Rows[0]["PlatformTypeTitle"]);
                Appeal.Content_GACId = Convert.ToString(ds.Tables[0].Rows[0]["ContentGACId"]);
                Appeal.Assign_GACId = Convert.ToString(ds.Tables[0].Rows[0]["AssignedToGAC"]);
                Appeal.Assign_GACDesc = Convert.ToString(ds.Tables[0].Rows[0]["GACTitle"]);
                Appeal.Assign_GACAbbr = Convert.ToString(ds.Tables[0].Rows[0]["GACAbbr"]);
                Appeal.GrievanceStatusCd = Convert.ToString(ds.Tables[0].Rows[0]["GrievnaceStatus"]);
                Appeal.GrievanceStatusDesc = Convert.ToString(ds.Tables[0].Rows[0]["StatusDesc"]);
                Appeal.GrievanceStatusDt = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceStatusDt"]);
            }
            return Appeal;
        }
    }

    public class GridViewInbox_Menu
    {
        public string MenuNm { get; set; }
        public string MenuStatusCd { get; set; }
        public string Icon { get; set; }
        public string AppealCount { get; set; }
        public string CssClass { get; set; }
    }
}