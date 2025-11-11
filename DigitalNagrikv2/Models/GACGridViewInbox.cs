using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class GACGridViewInbox
    {
        public List<GridViewInbox_Menu> MenuList { get; set; }

        public static GACGridViewInbox GetMenuList(string GACID)
        {
            GACGridViewInbox Menu = new GACGridViewInbox() { MenuList = new List<GridViewInbox_Menu>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACID", GACID),
                new KeyValuePair<string, string>("@Mode", "MENU")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox", SP_Parameter);
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

    public class GACGridViewInbox_Appeal
    {
        public string MenuStatusCd { get; set; }
        public string IsDateGroup { get; set; }
        public string MenuNm { get; set; }
        public string Icon { get; set; }
        public string CssClass { get; set; }
        public List<GACGridViewInbox_AppealList> AppealList { get; set; }

        public static GACGridViewInbox_Appeal GetAppealList(string MenuStatusCd, string GACID)
        {
            GACGridViewInbox_Appeal Appeal = new GACGridViewInbox_Appeal() { AppealList = new List<GACGridViewInbox_AppealList>(), MenuStatusCd = MenuStatusCd };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACID", GACID),
                new KeyValuePair<string, string>("@MenuStatusCd", MenuStatusCd),
                new KeyValuePair<string, string>("@Mode", "APPEAL_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Appeal.AppealList.Add(new GACGridViewInbox_AppealList
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
                            GACTitle = Convert.ToString(dr["GACTitle"]),
                            AssignedByGACUserId = Convert.ToString(dr["AssignedBy"]),
                            AssignedByGACUserNm = Convert.ToString(dr["as_byUserName"]),
                            AssignedByGACUserDesig = Convert.ToString(dr["as_byDesignation"]),
                            AssignedToGACMemberId = Convert.ToString(dr["AssingedToGACMember"]),
                            AssignedToGACMemberNm = Convert.ToString(dr["as_toUserName"]),
                            AssignedToGACMemberDesig = Convert.ToString(dr["as_toDesignation"]),
                            DisposedType = Convert.ToString(dr["ActionTitle"])
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

    public class GACGridViewInbox_AppealList
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

        public string AssignedByGACUserId { get; set; }
        public string AssignedByGACUserNm { get; set; }
        public string AssignedByGACUserDesig { get; set; }

        public string AssignedToGACMemberId { get; set; }
        public string AssignedToGACMemberNm { get; set; }
        public string AssignedToGACMemberDesig { get; set; }
        public string DisposedType { get; set; }
    }

    public class GACGridViewInbox_AppealDetails
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

        public string AssignedByGACUserId { get; set; }
        public string AssignedByGACUserNm { get; set; }
        public string AssignedByGACUserDesig { get; set; }
        public string AssignedRemarks { get; set; }
        public string AssignedDate { get; set; }

        public string AssignedToGACMemberId { get; set; }
        public string AssignedToGACMemberNm { get; set; }
        public string AssignedToGACMemberDesig { get; set; }
        public string DisposedType { get; set; }
        public string DateOfDisposal { get; set; }
        public string DisposalRemarks { get; set; }

        public static GACGridViewInbox_AppealDetails GetAppealDetails(string RegistrationYear, string GrievanceId)
        {
            GACGridViewInbox_AppealDetails Appeal = new GACGridViewInbox_AppealDetails();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "APPEAL_DETAILS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox", SP_Parameter);
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
                Appeal.AssignedByGACUserId = Convert.ToString(ds.Tables[0].Rows[0]["AssignedBy"]);
                Appeal.AssignedByGACUserNm = Convert.ToString(ds.Tables[0].Rows[0]["as_byUserName"]);
                Appeal.AssignedByGACUserDesig = Convert.ToString(ds.Tables[0].Rows[0]["as_byDesignation"]);
                Appeal.AssignedRemarks = Convert.ToString(ds.Tables[0].Rows[0]["AssignedToGACRemarks"]);
                Appeal.AssignedDate = Convert.ToString(ds.Tables[0].Rows[0]["AssignedDateTime"]);
                Appeal.AssignedToGACMemberId = Convert.ToString(ds.Tables[0].Rows[0]["AssingedToGACMember"]);
                Appeal.AssignedToGACMemberNm = Convert.ToString(ds.Tables[0].Rows[0]["as_toUserName"]);
                Appeal.AssignedToGACMemberDesig = Convert.ToString(ds.Tables[0].Rows[0]["as_toDesignation"]);
                Appeal.DisposedType = Convert.ToString(ds.Tables[0].Rows[0]["ActionTitle"]);
                Appeal.DateOfDisposal = Convert.ToString(ds.Tables[0].Rows[0]["DateOfDisposal"]);
                Appeal.DisposalRemarks = Convert.ToString(ds.Tables[0].Rows[0]["DisposalRemarks"]);
            }
            return Appeal;
        }
    }
}