using DigitalNagrik.Models;
using DigitalNagrik.Validators;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.GridView.Data
{
    public class GACGridViewInbox
    {
        public const string Dir_FinalDecision = "FinalDecision";
        public const string Dir_IntermediaryResponse = "IntermediaryResponse";
        public const string Dir_IntermediaryAdditionalInput = "IntermediaryAdditionalInput";
        public const string Dir_ExpertInput = "ExpertInput";
        public const string Dir_IntermediaryCompliance = "IntermediaryCompliance";

        public class MenuStatus
        {
            public const string All = "AL";
            public const string Inbox = "I";
            public const string InProcess = "IP";
            public const string DraftMatterSummary = "DM";
            public const string Disposed = "D";
            public const string Rejected = "RJ";
            public const string ComplianceReceived = "CR";

            public const string AssignmentPending = "ASP";
            public const string AssignedToGAC = "ASN";


            public const string IntermediaryResponse = "INR";
            public const string TransferRequestPending = "TR";
            public const string DelayResponse = "DR";
            public const string NoMatterSummary = "MS";
            public const string DraftDecisionPending = "DFP";
            public const string DecisionPending = "DPN";
            public const string MemberOpinionPending = "MOP";
            public const string ExpertOpinionPending = "EOP";

            public const string SelfOpinionPending = "SOP";
            public const string SelfOpinionSubmitted = "SOS";
            public const string PendingInDays = "PIN";
            public const string Section32B = "32B";
        }

        public class GrievanceStatus
        {
            public const string Final = "2";
            public const string Rejected = "5";
            public const string Disposed = "6";
            public const string UnapprovedIntermediary = "7";
        }

        public class AppealAction
        {
            public const string Submitted = "101";
            public const string AssignedToGAC = "102";
            public const string ReassignGAC = "51";
            public const string PrepareSummary = "52";
            public const string Reject = "53";
            public const string InformationFromIntermediary = "55";
            public const string SeekExpertAssistance = "56";
            public const string DecisionGAC = "57";

            public const string IntermediaryResponse = "201";
            public const string SubmitTransRequest = "202";
            public const string RejectTransRequest = "203";
            public const string VerifyIntermediary = "204";
            public const string AdditionInputReceived = "206";
            public const string ExpertOpinionReceived = "207";

            public const string DecisonByMember = "250";
            public const string DecisonByChairperson = "251";
            public const string ComplianceGiven = "252";
        }

        public class TransferRequestStatus
        {
            public const string NoRequest = "N";
            public const string Submitted = "S";
            public const string Approved = "A";
            public const string Rejected = "R";
        }

        public class IntermediaryAction
        {
            public const string MapWithExisting = "M";
            public const string NewIntermediary = "N";
            public const string Reject = "R";
        }

        public List<GACGridViewInbox_Menu> MenuList { get; set; }

        public static GACGridViewInbox GetMenuList(string GACID, string UserId)
        {
            GACGridViewInbox Menu = new GACGridViewInbox() { MenuList = new List<GACGridViewInbox_Menu>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACID", GACID),
                new KeyValuePair<string, string>("@UserId", UserId),
                new KeyValuePair<string, string>("@LangCulture", UserSession.LangCulture),
                new KeyValuePair<string, string>("@Mode", "MENU")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Menu.MenuList.Add(new GACGridViewInbox_Menu
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

        public static List<SelectListItem> GetMappedGACList(string UserId)
        {

            List<SelectListItem> List = new List<SelectListItem>();
            try
            {

                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@UserId", UserId),
                new KeyValuePair<string, string>("@Mode", "GAC_MAPPED")
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox", SP_Parameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        List.Add(new SelectListItem { Value = Convert.ToString(dr["GACId"]), Text = Convert.ToString(dr["GACTitle"]), });
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return List;
        }
        public static List<SelectListItem> GetNotifications(string UserId, string GACID)
        {
            List<SelectListItem> List = new List<SelectListItem>();
            try
            {

                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@UserId", UserId),
                new KeyValuePair<string, string>("@GACID", GACID),
                new KeyValuePair<string, string>("@Mode", "NOTIFICATIONS")
            };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox", SP_Parameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        List.Add(new SelectListItem { Value = Convert.ToString(dr["NotificationID"]), Text = Convert.ToString(dr["NotificationDesc"]), });
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return List;
        }

    }

    public class GACGridViewInbox_Appeal
    {
        public string qs { get; set; }
        public string MenuStatusCd { get; set; }
        public string IsDateGroup { get; set; }
        public string MenuNm { get; set; }
        public string Icon { get; set; }
        public string CssClass { get; set; }
        public string SearchText { get; set; }
        public string Days { get; set; }
        public List<GACGridViewInbox_AppealList> AppealList { get; set; }

        public static GACGridViewInbox_Appeal GetAppealList(string MenuStatusCd, string GACID, string UserId, string SearchText = "", string Days = "5")
        {
            GACGridViewInbox_Appeal Appeal = new GACGridViewInbox_Appeal() { AppealList = new List<GACGridViewInbox_AppealList>(), MenuStatusCd = MenuStatusCd };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACID", GACID),
                new KeyValuePair<string, string>("@MenuStatusCd", MenuStatusCd),
                new KeyValuePair<string, string>("@SearchText", SearchText),
                new KeyValuePair<string, string>("@Days", Days),
                new KeyValuePair<string, string>("@UserId", UserId),
                new KeyValuePair<string, string>("@LangCulture", UserSession.LangCulture),
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
                            IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                            IntermediaryURL = Convert.ToString(dr["IntermediaryURL"]),
                            IsIntermediaryReply = Convert.ToString(dr["IsIntermediaryReply"]),
                            IntermediaryReplyDt = Convert.ToString(dr["IntermediaryReplyDateTime"]),
                            IntermediaryReplyPendingForMins = Convert.ToString(dr["IntermediaryReplyTimeTaken"]),
                            GrievanceStatusCd = Convert.ToString(dr["GrievnaceStatus"]),
                            GrievanceActionId = Convert.ToString(dr["ActionId"]),
                            AppellantNm = Convert.ToString(dr["AppellantNm"]),
                            IsChairpersonDecision = Convert.ToString(dr["IsChairpersonDecision"]),
                            ChairpersonDecisionDate = Convert.ToString(dr["DecisionDate"]),
                            OpinionGiven = Convert.ToString(dr["OpinionGiven"]),
                            OpinionAgree = Convert.ToString(dr["OpinionAgree"]),
                            OpinionDisagree = Convert.ToString(dr["OpinionDisagree"]),
                            ActionDt = Convert.ToString(dr["ActionDt"]),
                            TransferFromGAC = Convert.ToString(dr["GACTitle"]),
                            IntermediaryResponseNeeded = Convert.ToString(dr["IntermediaryResponseNeeded"]),
                            CorrespondingITRule = Convert.ToString(dr["CorrespondingITRule"]),
                            GroundTitle = Convert.ToString(dr["GroundTitle"]),
                            Is72Hrs = Convert.ToString(dr["Is72Hrs"]),
                            ComplianceDate = Convert.ToString(dr["ComplianceDate"]),
                            DisposeTimeLeft = Convert.ToString(dr["DisposeTimeLeft"]),
                            DraftMatterSummaryPrepared = Convert.ToString(dr["DraftMatterSummaryPrepared"]),
                            DraftMatterSummaryPreparedDt = Convert.ToString(dr["DraftMatterSummaryPreparedDt"]),
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
        public string IntermediaryTitle { get; set; }
        public string IntermediaryURL { get; set; }
        public string IsIntermediaryReply { get; set; }
        public string IntermediaryReplyDt { get; set; }
        public string IntermediaryReplyPendingForMins { get; set; }
        public string IntermediaryReplyTimeLeft { get; set; }
        public string GrievanceStatusCd { get; set; }
        public string GrievanceActionId { get; set; }
        public string GACNm { get; set; }
        public string AppellantNm { get; set; }
        public string UserEmail { get; set; }
        public string UserMobile { get; set; }
        public string TransferFromGAC { get; set; }
        public string IsChairpersonDecision { get; set; }
        public string ChairpersonDecisionDate { get; set; }
        public string OpinionGiven { get; set; }
        public string OpinionAgree { get; set; }
        public string OpinionDisagree { get; set; }
        public string IntermediaryResponseNeeded { get; set; }
        public string ActionDt { get; set; }
        public string CorrespondingITRule { get; set; }
        public string GroundTitle { get; set; }
        public string ComplianceDate { get; set; }
        public string DecisionFilePath { get; set; }
        public string ComplianceFilePath { get; set; }
        public string ComplianceUrl { get; set; }
        public string Is72Hrs { get; set; }
        public string DisposeTimeLeft { get; set; }
        public string DraftMatterSummaryPrepared { get; set; }
        public string DraftMatterSummaryPreparedDt { get; set; }
    }

    public class GACGridViewInbox_AppealDetails
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string GrievanceNo { get; set; }
        public string GrievanceDesc { get; set; }
        public string GACId { get; set; }
        public string Appeal_ReceiptDate { get; set; }
        public string Appeal_Reason { get; set; }
        public string ReliefTitle { get; set; }
        public string GroundTitle { get; set; }
        public string CorrespondingITRule { get; set; }
        public string LawText { get; set; }
        public string User_UserId { get; set; }
        public string User_UserProfileId { get; set; }
        public string User_FullNm { get; set; }
        public string User_Mobile_Email { get; set; }
        public string User_Address { get; set; }
        public string Intermediary_Id { get; set; }
        public string Intermediary_Title { get; set; }
        public string Intermediary_Url { get; set; }
        public string Intermediary_Address { get; set; }
        public string Intermediary_GROName { get; set; }
        public string Intermediary_GROEmail { get; set; }
        public string Transfer_RequestStatus { get; set; }
        public string Transfer_RequestId { get; set; }
        public string Transfer_RequestRemarks { get; set; }
        public string Transfer_SuggestedGACId { get; set; }
        public string Transfer_SuggestedGAC { get; set; }
        public string Transfer_FromGAC { get; set; }
        public string Transfer_TransRequestDate { get; set; }
        public string Transfer_ToGAC { get; set; }
        public string Transfer_RequestStatusDate { get; set; }
        public string Transfer_RequestStatusRemarks { get; set; }
        public string IntermediaryResponseNeeded { get; set; }
        public string IsIntermediaryReply { get; set; }
        public string IntermediaryReplyDt { get; set; }
        public string IntermediaryReplyPendingForMins { get; set; }
        public string GrievanceStatusCd { get; set; }
        public string GrievanceActionId { get; set; }
        public string MatterSummary { get; set; }
        public string IsChairpersonDecision { get; set; }
        public string DraftDecision { get; set; }
        public string DecisionFilePath { get; set; }
        public string DecisionType { get; set; }
        public string Is72Hrs { get; set; }
        public string GroundTitleText { get; set; }
        public string DateOfComplaint { get; set; }
        public string DateOfDecision { get; set; }
        public string BriefOfComplaint { get; set; }
        public string Keyword { get; set; }
        public string ComplianceDate { get; set; }
        public string IsTimeExtensionRequested { get; set; }
        public List<GACAppealAdditionalInput> AdditionalInputList { get; set; }
        public List<GACAppealExpertOpinion> ExpertOpinionList { get; set; }
        public List<GACGridViewInbox_AppealDetails_Docs> AppealDocsList { get; set; }

        public static GACGridViewInbox_AppealDetails GetAppealDetails(string RegistrationYear, string GrievanceId)
        {
            GACGridViewInbox_AppealDetails Appeal = new GACGridViewInbox_AppealDetails { AppealDocsList = new List<GACGridViewInbox_AppealDetails_Docs>(), ExpertOpinionList = new List<GACAppealExpertOpinion>(), AdditionalInputList = new List<GACAppealAdditionalInput>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "APPEAL_DETAILS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Appeal.GACId = Convert.ToString(ds.Tables[0].Rows[0]["AssignedToGAC"]);
                    Appeal.RegistrationYear = Convert.ToString(ds.Tables[0].Rows[0]["RegistrationYear"]);
                    Appeal.GrievanceId = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceId"]);
                    Appeal.GrievanceNo = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceNo"]);
                    Appeal.GrievanceDesc = Convert.ToString(ds.Tables[0].Rows[0]["GrievanceDesc"]);
                    Appeal.Appeal_ReceiptDate = Convert.ToString(ds.Tables[0].Rows[0]["ReceiptDate"]);
                    Appeal.Appeal_Reason = Convert.ToString(ds.Tables[0].Rows[0]["ReasonForAppeal"]);
                    Appeal.ReliefTitle = Convert.ToString(ds.Tables[0].Rows[0]["ReliefTitle"]);
                    Appeal.GroundTitle = Convert.ToString(ds.Tables[0].Rows[0]["GroundTitle"]);
                    Appeal.CorrespondingITRule = Convert.ToString(ds.Tables[0].Rows[0]["CorrespondingITRule"]);
                    Appeal.LawText = Convert.ToString(ds.Tables[0].Rows[0]["GroundAppealLawText"]);
                    Appeal.User_UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserId"]);
                    Appeal.User_UserProfileId = Convert.ToString(ds.Tables[0].Rows[0]["UserProfileId"]);
                    Appeal.User_FullNm = Convert.ToString(ds.Tables[0].Rows[0]["FullNm"]);
                    Appeal.User_Mobile_Email = Convert.ToString(ds.Tables[0].Rows[0]["UserMobile"]);
                    Appeal.User_Address = Convert.ToString(ds.Tables[0].Rows[0]["Address"]);
                    Appeal.Intermediary_Id = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryId"]);
                    Appeal.Intermediary_Title = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryTitle"]);
                    Appeal.Intermediary_Url = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryURL"]);
                    Appeal.Intermediary_Address = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryAddress"]);
                    Appeal.Intermediary_GROName = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryGROName"]);
                    Appeal.Intermediary_GROEmail = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryGROEmail"]);

                    Appeal.Transfer_RequestId = Convert.ToString(ds.Tables[0].Rows[0]["TransRequestID"]);
                    Appeal.Transfer_RequestStatus = Convert.ToString(ds.Tables[0].Rows[0]["TransRequestStatus"]);
                    Appeal.Transfer_RequestRemarks = Convert.ToString(ds.Tables[0].Rows[0]["TransRequestRemarks"]);
                    Appeal.Transfer_SuggestedGACId = Convert.ToString(ds.Tables[0].Rows[0]["SuggestedGACId"]);
                    Appeal.Transfer_SuggestedGAC = Convert.ToString(ds.Tables[0].Rows[0]["SuggestedGAC"]);
                    Appeal.Transfer_TransRequestDate = Convert.ToString(ds.Tables[0].Rows[0]["TransRequestDate"]);
                    Appeal.Transfer_ToGAC = Convert.ToString(ds.Tables[0].Rows[0]["ToGAC"]);
                    Appeal.Transfer_RequestStatusDate = Convert.ToString(ds.Tables[0].Rows[0]["TransRequestStatusDate"]);
                    Appeal.Transfer_RequestStatusRemarks = Convert.ToString(ds.Tables[0].Rows[0]["TransRequestStatusRemarks"]);

                    Appeal.IsIntermediaryReply = Convert.ToString(ds.Tables[0].Rows[0]["IsIntermediaryReply"]);
                    Appeal.IntermediaryReplyDt = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryReplyDateTime"]);
                    Appeal.IntermediaryReplyPendingForMins = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryReplyTimeTaken"]);
                    Appeal.GrievanceStatusCd = Convert.ToString(ds.Tables[0].Rows[0]["GrievnaceStatus"]);
                    Appeal.GrievanceActionId = Convert.ToString(ds.Tables[0].Rows[0]["ActionId"]);
                    Appeal.MatterSummary = Convert.ToString(ds.Tables[0].Rows[0]["MatterSummary"]);
                    Appeal.DraftDecision = Convert.ToString(ds.Tables[0].Rows[0]["DraftDecision"]);
                    Appeal.DecisionFilePath = Convert.ToString(ds.Tables[0].Rows[0]["DecisionFilePath"]);
                    Appeal.IsChairpersonDecision = Convert.ToString(ds.Tables[0].Rows[0]["IsChairpersonDecision"]);
                    Appeal.DecisionType = Convert.ToString(ds.Tables[0].Rows[0]["DecisionType"]);
                    Appeal.Is72Hrs = Convert.ToString(ds.Tables[0].Rows[0]["Is72Hrs"]);
                    Appeal.IntermediaryResponseNeeded = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryResponseNeeded"]);
                    Appeal.Transfer_FromGAC = Convert.ToString(ds.Tables[0].Rows[0]["FromGAC"]);
                    Appeal.GroundTitleText = Convert.ToString(ds.Tables[0].Rows[0]["GroundTitleText"]);
                    Appeal.DateOfComplaint = Convert.ToString(ds.Tables[0].Rows[0]["dateofComplaint"]);
                    Appeal.DateOfDecision = Convert.ToString(ds.Tables[0].Rows[0]["dateofDecision"]);
                    Appeal.BriefOfComplaint = Convert.ToString(ds.Tables[0].Rows[0]["BriefofComplaint"]);
                    Appeal.Keyword = Convert.ToString(ds.Tables[0].Rows[0]["Keyword"]);
                    Appeal.ComplianceDate = Convert.ToString(ds.Tables[0].Rows[0]["ComplianceDate"]);
                    Appeal.IsTimeExtensionRequested = Convert.ToString(ds.Tables[0].Rows[0]["IsTimeExtensionRequested"]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        Appeal.AppealDocsList.Add(new GACGridViewInbox_AppealDetails_Docs
                        {
                            FileId = Convert.ToString(dr["FileId"]),
                            FileTypeID = Convert.ToString(dr["FileTypeID"]),
                            FileType = Convert.ToString(dr["FileType"]),
                            FilePath = Convert.ToString(dr["FilePath"]),
                            DocumentType = Convert.ToString(dr["AppealDocumentTypeDesc"]),
                            EvidenceType = Convert.ToString(dr["EvidenceType"]),
                        });
                    }
                }
                if (ds.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[2].Rows)
                    {
                        Appeal.AdditionalInputList.Add(new GACAppealAdditionalInput
                        {
                            InputId = Convert.ToString(dr["InputId"]),
                            InputRequestNo = Convert.ToString(dr["InputRequestNo"]),
                            SeekQuery = Convert.ToString(dr["InputSeekRemarks"]),
                            SeekDate = Convert.ToString(dr["InputSeekDateTime"]),
                            InputRemarks = Convert.ToString(dr["InputReplyRemarks"]),
                            InputDate = Convert.ToString(dr["InputReplyDateTime"]),
                            FilePath = Convert.ToString(dr["FilePath"]),
                        });
                    }
                }
                if (ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[3].Rows)
                    {
                        Appeal.ExpertOpinionList.Add(new GACAppealExpertOpinion
                        {
                            InputId = Convert.ToString(dr["InputId"]),
                            ExpertName = Convert.ToString(dr["ExpertName"]),
                            ExpertEmail = Convert.ToString(dr["ExpertEmail"]),
                            ExpertMobile = Convert.ToString(dr["ExpertMobile"]),
                            InputRequestNo = Convert.ToString(dr["InputRequestNo"]),
                            SeekRemarks = Convert.ToString(dr["InputSeekRemarks"]),
                            SeekDate = Convert.ToString(dr["InputSeekDateTime"]),
                            InputRemarks = Convert.ToString(dr["InputReplyRemarks"]),
                            InputDate = Convert.ToString(dr["InputReplyDateTime"]),
                            FilePath = Convert.ToString(dr["FilePath"]),
                        });
                    }
                }
            }
            return Appeal;
        }
    }

    public class GACGridViewInbox_AppealDetails_Docs
    {
        public string FileId { get; set; }
        public string DocumentType { get; set; }
        public string FileTypeID { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public string EvidenceType { get; set; }
    }

    public class GACGridViewInbox_Menu
    {
        public string MenuNm { get; set; }
        public string MenuStatusCd { get; set; }
        public string Icon { get; set; }
        public string AppealCount { get; set; }
        public string CssClass { get; set; }
    }

    //----------------------------------------------------Actions----------------------------------------------------------//
    //---------------------------------------------------------------------------------------------------------------------//

    public class GACAppealAssign
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        public string GACId { get; set; }
        public string GACNm { get; set; }
        public List<SelectListItem> GACList { get; set; }

        [Required(ErrorMessage = "Enter Remarks!")]
        [StringLength(1000, ErrorMessage = "Max. 1000 characters allowed!")]
        public string Remarks { get; set; }

        public static List<SelectListItem> GetGACList(string RegistrationYear, string GrievanceId)
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "GAC_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List.Add(new SelectListItem { Value = Convert.ToString(dr["GACId"]), Text = Convert.ToString(dr["GACTitle"]), });
                }
            }
            return List;
        }
    }

    public class GACAppealTransferApprove
    {

        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string TransferRequestId { get; set; }

        [Required(ErrorMessage = "Select GAC!")]
        public string GACId { get; set; }
        public string GACNm { get; set; }
        public List<SelectListItem> GACList { get; set; }

        [Required(ErrorMessage = "Select GAC!")]
        public string TransferRequestStatus { get; set; }

        [Required(ErrorMessage = "Enter Remarks!")]
        [StringLength(1000, ErrorMessage = "Max. 1000 characters allowed!")]
        public string Remarks { get; set; }

        public static List<SelectListItem> GetGACList(string RegistrationYear, string GrievanceId)
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "GAC_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List.Add(new SelectListItem { Value = Convert.ToString(dr["GACId"]), Text = Convert.ToString(dr["GACTitle"]), });
                }
            }
            return List;
        }
    }

    public class GACAppealMatterSummary
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Mandatory Field!")]
        public string Summary { get; set; }
        public string PreparedSummary { get; set; }

        public string MatterSummaryFormat { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string SubmitType { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(5000, ErrorMessage = "Max. 5000 characters allowed!")]
        public string ObservationText { get; set; }

        public string RecommendationText { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string IsAdmit { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string NoAdmitTypeId { get; set; }
        public List<SelectListItem> NoAdmitTypeList { get; set; }

        public string RecTypeId { get; set; }
        public List<SelectListItem> RecTypeList { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Mandatory Field!")]
        public string RejectSummary { get; set; }
        public string PreparedRejectSummary { get; set; }
        public string RejectSummaryFormat { get; set; }

        public string RecOtherText { get; set; }

        public static GACAppealMatterSummary GetMatterSummaryDetails(string RegistrationYear, string GrievanceId)
        {
            string MatterSummaryLetterFinal = ""; string RejectSummaryLetterFinal = "";
            GACAppealMatterSummary Data = new GACAppealMatterSummary { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "FETCH_SUMMARY")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", SP_Parameter);
            if (ds != null && ds.Tables.Count > 1)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Data.PreparedSummary = Convert.ToString(ds.Tables[0].Rows[0]["MatterSummary"]);
                    Data.MatterSummaryFormat = Convert.ToString(ds.Tables[0].Rows[0]["MatterSummaryFormat"]);
                    Data.RecommendationText = Convert.ToString(ds.Tables[0].Rows[0]["RecommendationText"]);
                    Data.ObservationText = Convert.ToString(ds.Tables[0].Rows[0]["ObservationText"]);
                    Data.RecTypeId = Convert.ToString(ds.Tables[0].Rows[0]["RecTypeId"]);
                    Data.RecOtherText = Convert.ToString(ds.Tables[0].Rows[0]["RecOtherText"]);
                    Data.IsAdmit = Convert.ToString(ds.Tables[0].Rows[0]["IsAdmit"]);
                    Data.NoAdmitTypeId = Convert.ToString(ds.Tables[0].Rows[0]["NoAdmitTypeId"]);
                    Data.PreparedRejectSummary = Convert.ToString(ds.Tables[0].Rows[0]["RejectSummary"]);
                    Data.RejectSummaryFormat = Convert.ToString(ds.Tables[0].Rows[0]["RejectSummaryFormat"]);
                    Data.Summary = string.IsNullOrWhiteSpace(Data.PreparedSummary) ? Data.MatterSummaryFormat : Data.PreparedSummary;
                    Data.RejectSummary = string.IsNullOrWhiteSpace(Data.PreparedRejectSummary) ? Data.RejectSummaryFormat : Data.PreparedRejectSummary;
                }
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    Dictionary<string, string> keyword = new Dictionary<string, string>
                    {
                        { "@GacTitle", (ds.Tables[1].Rows[0]["GACTitle"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["GACTitle"].ToString()) },
                        { "@DecisionTitle", (ds.Tables[1].Rows[0]["DecisionTitle"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["DecisionTitle"].ToString()) },
                        { "@AppealDate", (ds.Tables[1].Rows[0]["AppealDate"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["AppealDate"].ToString()) },
                        { "@AppellantDetails", (ds.Tables[1].Rows[0]["AppellantDetails"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["AppellantDetails"].ToString()) },
                        { "@IntemediaryTitle", (ds.Tables[1].Rows[0]["IntermediaryTitle"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["IntermediaryTitle"].ToString()) },
                        { "@GREmail", (ds.Tables[1].Rows[0]["IntermediaryGROEmail"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["IntermediaryGROEmail"].ToString()) },
                        { "@GRAddress", (ds.Tables[1].Rows[0]["IntermediaryAddress"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["IntermediaryAddress"].ToString()) },
                        { "@AppealID", (ds.Tables[1].Rows[0]["AppealID"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["AppealID"].ToString()) },
                        { "@GroundofAppeal", (ds.Tables[1].Rows[0]["GroundTitle"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["GroundTitle"].ToString()) },
                        { "@Justification", (ds.Tables[1].Rows[0]["Justification"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["Justification"].ToString()) },
                        { "@ReliefSought", (ds.Tables[1].Rows[0]["ReliefTitle"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["ReliefTitle"].ToString()) },
                        { "@ResponsefromIntermediary", (ds.Tables[1].Rows[0]["ResponsefromIntermediary"] == DBNull.Value ? "" : ds.Tables[1].Rows[0]["ResponsefromIntermediary"].ToString()) }
                    };
                    MatterSummaryLetterFinal = MatterSummaryLetterFinal + Data.Summary;
                    foreach (KeyValuePair<string, string> entry in keyword)
                    {
                        MatterSummaryLetterFinal = MatterSummaryLetterFinal.Replace(entry.Key.ToString(), entry.Value.ToString());
                    }
                    Data.Summary = MatterSummaryLetterFinal;
                }

                if (ds.Tables.Count > 1 && ds.Tables[2].Rows.Count > 0)
                {
                    Dictionary<string, string> keyword = new Dictionary<string, string>
                    {
                        { "@AppellantName", (ds.Tables[2].Rows[0]["AppellantDetails"] == DBNull.Value ? "" : ds.Tables[2].Rows[0]["AppellantDetails"].ToString()) },
                        { "@AppealNo", (ds.Tables[2].Rows[0]["AppealID"] == DBNull.Value ? "" : ds.Tables[2].Rows[0]["AppealID"].ToString()) },
                        { "@AppealDate", (ds.Tables[2].Rows[0]["AppealDate"] == DBNull.Value ? "" : ds.Tables[2].Rows[0]["AppealDate"].ToString()) },
                        { "@Reason", (ds.Tables[2].Rows[0]["Reason"] == DBNull.Value ? "" : ds.Tables[2].Rows[0]["Reason"].ToString()) },
                    };
                    RejectSummaryLetterFinal = RejectSummaryLetterFinal + Data.RejectSummary;
                    foreach (KeyValuePair<string, string> entry in keyword)
                    {
                        RejectSummaryLetterFinal = RejectSummaryLetterFinal.Replace(entry.Key.ToString(), entry.Value.ToString());
                    }
                    Data.RejectSummary = RejectSummaryLetterFinal;
                }
            }
            return Data;
        }

        public static List<SelectListItem> GetNoAdmitTypeList()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetNoAdmitTypeList", new List<KeyValuePair<string, string>>());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List.Add(new SelectListItem { Value = Convert.ToString(dr["NoAdmitTypeId"]), Text = Convert.ToString(dr["NoAdmitTypeDesc"]), });
                }
            }
            return List;
        }

        public static List<SelectListItem> GetRecTypeList()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetRecommendationTypeList", new List<KeyValuePair<string, string>>());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List.Add(new SelectListItem { Value = Convert.ToString(dr["RecTypeId"]), Text = Convert.ToString(dr["RecTypeDesc"]), });
                }
            }
            return List;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------//

    public class GACRecomendations
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        [AllowHtml]
        public string Summary { get; set; }
        public string UserName { get; set; }
        public string ParaId { get; set; }
        public string ObservationText { get; set; }
        public string RecommendationText { get; set; }
        public string isAdmit { get; set; }
        public string NoAdmitTypeDesc { get; set; }
        public string RecTypeDesc { get; set; }
        public string RecOtherText { get; set; }

        [Required(ErrorMessage = "Select Option!")]
        public string AgreeDisagree { get; set; }
        public string Recomendations { get; set; }
        [AllowHtml]
        public string DecisionFormat { get; set; }
        [AllowHtml]
        public string DraftDecision { get; set; }
        public string IsWindowOpened { get; set; }
        public string TimeDifference { get; set; }
        public string LastActionDateTime { get; set; }
        public string OpenedBy { get; set; }
        public string ChairpersonDecision { get; set; }
        public List<DraftDecisionHistoryList> DraftDecisionnHistory { get; set; }

        //public List<GACRecomendations_Observations> ObservationsList { get; set; }
        //Recomendations
        public static GACRecomendations GetRecomendations(string RegistrationYear, string GrievanceId)
        {
            string DecisionLetterFinal = "";
            GACRecomendations Data = new GACRecomendations { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            Data.DraftDecisionnHistory = new List<DraftDecisionHistoryList>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@userID",Models.UserSession.UserID),
                new KeyValuePair<string, string>("@SessionID",HttpContext.Current.Session.SessionID),
                new KeyValuePair<string, string>("@VMName",System.Environment.MachineName),
                new KeyValuePair<string, string>("@IPAddress",BALCommon.GetIPAddress()),
                new KeyValuePair<string, string>("@Mode", "FETCH_RECOMENDATIONS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Recomendations", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Data.Summary = Convert.ToString(ds.Tables[0].Rows[0]["MatterSummary"]);
                    Data.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                    Data.DecisionFormat = Convert.ToString(ds.Tables[0].Rows[0]["DecisionFormat"]);
                    Data.ChairpersonDecision = Convert.ToString(ds.Tables[0].Rows[0]["ChairpersonDecision"]);
                    Data.TimeDifference = BALCommon.ConvertMinToDHM(Convert.ToString(ds.Tables[0].Rows[0]["TimeDifference"]));
                    Data.DraftDecision = Convert.ToString(ds.Tables[0].Rows[0]["DraftDecision"]);
                    Data.LastActionDateTime = Convert.ToString(ds.Tables[0].Rows[0]["LastActionDateTime"]);
                    Data.Summary = string.IsNullOrWhiteSpace(Data.Summary) ? Data.Summary : Data.Summary;
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    Data.GrievanceId = Convert.ToString(ds.Tables[1].Rows[0]["GrievanceId"]);
                    Data.ObservationText = Convert.ToString(ds.Tables[1].Rows[0]["ObservationText"]);
                    Data.RecommendationText = Convert.ToString(ds.Tables[1].Rows[0]["RecommendationText"]);
                    Data.RegistrationYear = Convert.ToString(ds.Tables[1].Rows[0]["RegistrationYear"]);
                    Data.ParaId = Convert.ToString(ds.Tables[1].Rows[0]["ParaId"]);
                    Data.isAdmit = Convert.ToString(ds.Tables[1].Rows[0]["isAdmit"]);
                    Data.NoAdmitTypeDesc = Convert.ToString(ds.Tables[1].Rows[0]["NoAdmitTypeDesc"]);
                    Data.RecTypeDesc = Convert.ToString(ds.Tables[1].Rows[0]["RecTypeDesc"]);
                    Data.RecOtherText = Convert.ToString(ds.Tables[1].Rows[0]["RecOtherText"]);
                    Data.AgreeDisagree = Convert.ToString(ds.Tables[1].Rows[0]["AgreeDisagree"]);
                    Data.Recomendations = Convert.ToString(ds.Tables[1].Rows[0]["Recomendations"]);
                    Data.IsWindowOpened = Convert.ToString(ds.Tables[1].Rows[0]["IsWindowOpened"]);
                    Data.OpenedBy = Convert.ToString(ds.Tables[1].Rows[0]["OpenedBy"]);
                }
                if (ds.Tables[2].Rows.Count > 0)
                {
                    Dictionary<string, string> keyword = new Dictionary<string, string>
                    {
                        { "@GacTitle", (ds.Tables[2].Rows[0]["GACTitle"] == null ? "" : ds.Tables[2].Rows[0]["GACTitle"].ToString()) },
                        { "@DecisionTitle", (ds.Tables[2].Rows[0]["DecisionTitle"] == null ? "" : ds.Tables[2].Rows[0]["DecisionTitle"].ToString()) },
                        { "@AppealDate", (ds.Tables[2].Rows[0]["AppealDate"] == null ? "" : ds.Tables[2].Rows[0]["AppealDate"].ToString()) },
                        { "@AppellantDetails", (ds.Tables[2].Rows[0]["AppellantDetails"] == null ? "" : ds.Tables[2].Rows[0]["AppellantDetails"].ToString()) },
                        { "@IntemediaryTitle", (ds.Tables[2].Rows[0]["IntermediaryTitle"] == null ? "" : ds.Tables[2].Rows[0]["IntermediaryTitle"].ToString()) },
                        { "@GREmail", (ds.Tables[2].Rows[0]["IntermediaryGROEmail"] == null ? "" : ds.Tables[2].Rows[0]["IntermediaryGROEmail"].ToString()) },
                        { "@GRAddress", (ds.Tables[2].Rows[0]["IntermediaryAddress"] == null ? "" : ds.Tables[2].Rows[0]["IntermediaryAddress"].ToString()) },
                        { "@AppealID", (ds.Tables[2].Rows[0]["AppealID"] == null ? "" : ds.Tables[2].Rows[0]["AppealID"].ToString()) },
                        { "@GroundofAppeal", (ds.Tables[2].Rows[0]["GroundTitle"] == null ? "" : ds.Tables[2].Rows[0]["GroundTitle"].ToString()) },
                        { "@Justification", (ds.Tables[2].Rows[0]["Justification"] == null ? "" : ds.Tables[2].Rows[0]["Justification"].ToString()) },
                        { "@ReliefSought", (ds.Tables[2].Rows[0]["ReliefTitle"] == null ? "" : ds.Tables[2].Rows[0]["ReliefTitle"].ToString()) },
                        { "@Observation", (ds.Tables[2].Rows[0]["ObservationText"] == null ? "" : ds.Tables[2].Rows[0]["ObservationText"].ToString()) },
                        { "@Recommendation", (ds.Tables[2].Rows[0]["RecommendationText"] == null ? "" : ds.Tables[2].Rows[0]["RecommendationText"].ToString()) },
                        { "@Decision", (ds.Tables[2].Rows[0]["RecTypeDesc"] == null ? "" : ds.Tables[2].Rows[0]["RecTypeDesc"].ToString()) },
                        { "@Summary", (ds.Tables[2].Rows[0]["Remarks"] == null ? "" : ds.Tables[2].Rows[0]["Remarks"].ToString()) },
                        { "@AppellantUrl", (ds.Tables[2].Rows[0]["AppellantUrls"] == null ? "" : ds.Tables[2].Rows[0]["AppellantUrls"].ToString()) }
                    };
                    DecisionLetterFinal = Data.DecisionFormat;
                    foreach (KeyValuePair<string, string> entry in keyword)
                    {
                        DecisionLetterFinal = DecisionLetterFinal.Replace(entry.Key.ToString(), entry.Value.ToString());
                    }
                }
                Data.DraftDecision = string.IsNullOrWhiteSpace(Data.DraftDecision) ? DecisionLetterFinal : Data.DraftDecision;
                if (ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[3].Rows)
                    {
                        Data.DraftDecisionnHistory.Add(new DraftDecisionHistoryList
                        {
                            RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                            DraftDecision = Convert.ToString(dr["DraftDecision"]),
                            GrievanceId = Convert.ToString(dr["GrievanceId"]),
                            UpdationDateTime = Convert.ToString(dr["UpdationDateTime"]),
                            TimeDiifference = BALCommon.ConvertMinToDHM(Convert.ToString(dr["TimeDiifference"])),
                            UserDetails = Convert.ToString(dr["UserDetails"]),
                        });
                    }

                }
            }
            return Data;
        }
        //Recomendations
    }
    public class DraftDecisionHistoryList
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string UserDetails { get; set; }
        public string DraftDecision { get; set; }
        public string UpdationDateTime { get; set; }
        public string TimeDiifference { get; set; }
    }

    public class GACDecisionbyChairperson
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }
        public string UserName { get; set; }

        [AllowHtml]
        public string Summary { get; set; }
        public string RecommendationText { get; set; }
        public string ObservationText { get; set; }
        public string isAdmit { get; set; }
        public string NoAdmitTypeDesc { get; set; }
        public string RecTypeDesc { get; set; }
        public string RecTypeId { get; set; }
        public string RecOtherText { get; set; }

        public string DecisonRemarks { get; set; }
        public string DecisonIDDesc { get; set; }
        public string ChairpersonRecommendations { get; set; }
        public string ChairpersonAgreeDisagree { get; set; }

        [Required(ErrorMessage = "Select Option!")]
        public string AgreeDisagree { get; set; }
        [Required(ErrorMessage = "Select Option!")]
        public string DecisionType { get; set; }
        public string Recomendations { get; set; }
        public string ParaId { get; set; }
        //public string ObservationText1 { get; set; }
        [Required(ErrorMessage = "Select Option!")]
        public string DecisionTypeId { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string Remarks { get; set; }
        //public string RecommendationText1 { get; set; }

        [AllowHtml]
        public string DecisionFormat { get; set; }
        [AllowHtml]
        public string DraftDecision { get; set; }
        public string IsWindowOpened { get; set; }
        public string TimeDifference { get; set; }
        public string LastActionDateTime { get; set; }
        public string OpenedBy { get; set; }
        public string SubmitType { get; set; }
        public List<DraftDecisionHistoryChairpersonList> DraftDecisionnHistory { get; set; }
        public static GACDecisionbyChairperson GetDecisionbyChairperson(string RegistrationYear, string GrievanceId)
        {
            string DecisionLetterFinal = "";
            GACDecisionbyChairperson Data = new GACDecisionbyChairperson { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            //Data.AgreeDisgreeCountList = new List<GACDecisionbyChairperson_AgreeDisgreeCount>();
            //Data.RemarksList = new List<GACDecisionbyChairperson_Remarks>();
            //Data.ObservationsList = new List<GACDecisionbyChairperson_Observations>();
            Data.DraftDecisionnHistory = new List<DraftDecisionHistoryChairpersonList>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@userID",Models.UserSession.UserID),
                new KeyValuePair<string, string>("@SessionID",HttpContext.Current.Session.SessionID),
                new KeyValuePair<string, string>("@VMName",System.Environment.MachineName),
                new KeyValuePair<string, string>("@IPAddress",BALCommon.GetIPAddress()),
                new KeyValuePair<string, string>("@Mode", "FETCH_RECOMENDATIONS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_DecisionbyChairperson", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Data.Summary = Convert.ToString(ds.Tables[0].Rows[0]["MatterSummary"]);
                    Data.DraftDecision = Convert.ToString(ds.Tables[0].Rows[0]["DraftDecision"]);
                    Data.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                    Data.ObservationText = Convert.ToString(ds.Tables[0].Rows[0]["ObservationText"]);
                    Data.LastActionDateTime = Convert.ToString(ds.Tables[0].Rows[0]["LastActionDateTime"]);
                    Data.isAdmit = Convert.ToString(ds.Tables[0].Rows[0]["isAdmit"]);
                    Data.ParaId = Convert.ToString(ds.Tables[0].Rows[0]["ParaId"]);
                    Data.RecTypeId = Convert.ToString(ds.Tables[0].Rows[0]["RecTypeId"]);
                    Data.NoAdmitTypeDesc = Convert.ToString(ds.Tables[0].Rows[0]["NoAdmitTypeDesc"]);
                    Data.RecTypeDesc = Convert.ToString(ds.Tables[0].Rows[0]["RecTypeDesc"]);
                    Data.RecOtherText = Convert.ToString(ds.Tables[0].Rows[0]["RecOtherText"]);
                    Data.RecommendationText = Convert.ToString(ds.Tables[0].Rows[0]["RecommendationText"]);
                    Data.DecisonRemarks = Convert.ToString(ds.Tables[0].Rows[0]["DecisonRemarks"]);
                    Data.Remarks = Convert.ToString(ds.Tables[0].Rows[0]["DecisonRemarks"]);
                    Data.DecisionTypeId = Convert.ToString(ds.Tables[0].Rows[0]["DecisionTypeId"]);
                    Data.DecisonIDDesc = Convert.ToString(ds.Tables[0].Rows[0]["DecisonIDDesc"]);
                    Data.ChairpersonRecommendations = Convert.ToString(ds.Tables[0].Rows[0]["ChairpersonRecommendations"]);
                    Data.ChairpersonAgreeDisagree = Convert.ToString(ds.Tables[0].Rows[0]["ChairpersonAgreeDisagree"]);
                    Data.SubmitType = Convert.ToString(ds.Tables[0].Rows[0]["SubmitType"]);
                    Data.Summary = string.IsNullOrWhiteSpace(Data.Summary) ? Data.Summary : Data.Summary;
                    Data.DecisionFormat = Convert.ToString(ds.Tables[0].Rows[0]["DecisionFormat"]);
                    //Data.DecisionFormat = Convert.ToString(ds.Tables[0].Rows[0]["DecisionFormat"]);
                    Data.TimeDifference = BALCommon.ConvertMinToDHM(Convert.ToString(ds.Tables[0].Rows[0]["TimeDifference"]));
                    Data.Summary = string.IsNullOrWhiteSpace(Data.Summary) ? Data.Summary : Data.Summary;
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    Data.GrievanceId = Convert.ToString(ds.Tables[1].Rows[0]["GrievanceId"]);
                    Data.ObservationText = Convert.ToString(ds.Tables[1].Rows[0]["ObservationText"]);
                    Data.RecommendationText = Convert.ToString(ds.Tables[1].Rows[0]["RecommendationText"]);
                    Data.RegistrationYear = Convert.ToString(ds.Tables[1].Rows[0]["RegistrationYear"]);
                    Data.ParaId = Convert.ToString(ds.Tables[1].Rows[0]["ParaId"]);
                    Data.isAdmit = Convert.ToString(ds.Tables[1].Rows[0]["isAdmit"]);
                    Data.NoAdmitTypeDesc = Convert.ToString(ds.Tables[1].Rows[0]["NoAdmitTypeDesc"]);
                    Data.RecTypeDesc = Convert.ToString(ds.Tables[1].Rows[0]["RecTypeDesc"]);
                    Data.RecOtherText = Convert.ToString(ds.Tables[1].Rows[0]["RecOtherText"]);
                    Data.AgreeDisagree = Convert.ToString(ds.Tables[1].Rows[0]["AgreeDisagree"]);
                    Data.Recomendations = Convert.ToString(ds.Tables[1].Rows[0]["Recomendations"]);
                    Data.IsWindowOpened = Convert.ToString(ds.Tables[1].Rows[0]["IsWindowOpened"]);
                    Data.OpenedBy = Convert.ToString(ds.Tables[1].Rows[0]["OpenedBy"]);
                }
                if (ds.Tables[2].Rows.Count > 0)
                {
                    Dictionary<string, string> keyword = new Dictionary<string, string>
                    {
                        { "@GacTitle", (ds.Tables[2].Rows[0]["GACTitle"] == null ? "" : ds.Tables[2].Rows[0]["GACTitle"].ToString()) },
                        { "@DecisionTitle", (ds.Tables[2].Rows[0]["DecisionTitle"] == null ? "" : ds.Tables[2].Rows[0]["DecisionTitle"].ToString()) },
                        { "@AppealDate", (ds.Tables[2].Rows[0]["AppealDate"] == null ? "" : ds.Tables[2].Rows[0]["AppealDate"].ToString()) },
                        { "@AppellantDetails", (ds.Tables[2].Rows[0]["AppellantDetails"] == null ? "" : ds.Tables[2].Rows[0]["AppellantDetails"].ToString()) },
                        { "@IntemediaryTitle", (ds.Tables[2].Rows[0]["IntermediaryTitle"] == null ? "" : ds.Tables[2].Rows[0]["IntermediaryTitle"].ToString()) },
                        { "@GREmail", (ds.Tables[2].Rows[0]["IntermediaryGROEmail"] == null ? "" : ds.Tables[2].Rows[0]["IntermediaryGROEmail"].ToString()) },
                        { "@GRAddress", (ds.Tables[2].Rows[0]["IntermediaryAddress"] == null ? "" : ds.Tables[2].Rows[0]["IntermediaryAddress"].ToString()) },
                        { "@AppealID", (ds.Tables[2].Rows[0]["AppealID"] == null ? "" : ds.Tables[2].Rows[0]["AppealID"].ToString()) },
                        { "@GroundofAppeal", (ds.Tables[2].Rows[0]["GroundTitle"] == null ? "" : ds.Tables[2].Rows[0]["GroundTitle"].ToString()) },
                        { "@Justification", (ds.Tables[2].Rows[0]["Justification"] == null ? "" : ds.Tables[2].Rows[0]["Justification"].ToString()) },
                        { "@ReliefSought", (ds.Tables[2].Rows[0]["ReliefTitle"] == null ? "" : ds.Tables[2].Rows[0]["ReliefTitle"].ToString()) },
                        { "@Observation", (ds.Tables[2].Rows[0]["ObservationText"] == null ? "" : ds.Tables[2].Rows[0]["ObservationText"].ToString()) },
                        { "@Recommendation", (ds.Tables[2].Rows[0]["RecommendationText"] == null ? "" : ds.Tables[2].Rows[0]["RecommendationText"].ToString()) },
                        { "@Decision", (ds.Tables[2].Rows[0]["RecTypeDesc"] == null ? "" : ds.Tables[2].Rows[0]["RecTypeDesc"].ToString()) },
                        { "@Summary", (ds.Tables[2].Rows[0]["Remarks"] == null ? "" : ds.Tables[2].Rows[0]["Remarks"].ToString()) },
                        { "@AppellantUrl", (ds.Tables[2].Rows[0]["AppellantUrls"] == null ? "" : ds.Tables[2].Rows[0]["AppellantUrls"].ToString()) }
                    };
                    DecisionLetterFinal = Data.DecisionFormat;
                    foreach (KeyValuePair<string, string> entry in keyword)
                    {
                        DecisionLetterFinal = DecisionLetterFinal.Replace(entry.Key.ToString(), entry.Value.ToString());
                    }
                }
                Data.DraftDecision = string.IsNullOrWhiteSpace(Data.DraftDecision) ? DecisionLetterFinal : Data.DraftDecision;
                if (ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[3].Rows)
                    {
                        Data.DraftDecisionnHistory.Add(new DraftDecisionHistoryChairpersonList
                        {
                            RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                            DraftDecision = Convert.ToString(dr["DraftDecision"]),
                            GrievanceId = Convert.ToString(dr["GrievanceId"]),
                            UpdationDateTime = Convert.ToString(dr["UpdationDateTime"]),
                            TimeDiifference = BALCommon.ConvertMinToDHM(Convert.ToString(dr["TimeDiifference"])),
                            UserDetails = Convert.ToString(dr["UserDetails"]),
                            AgreeDisagree = Convert.ToString(dr["AgreeDisagree"]),
                            CreationDateTime = Convert.ToString(dr["CreationDateTime"]),
                            Recommendations = Convert.ToString(dr["Recomendations"]),
                        });
                    }

                }
            }
            return Data;
        }
        //Recomendations
    }
    public class DraftDecisionHistoryChairpersonList
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string UserDetails { get; set; }
        public string DraftDecision { get; set; }
        public string UpdationDateTime { get; set; }
        public string TimeDiifference { get; set; }
        public string AgreeDisagree { get; set; }
        public string Recommendations { get; set; }
        public string CreationDateTime { get; set; }
        public string DecisionType { get; set; }
        public string DecisionRemarks { get; set; }
        public string DecisionTypeId { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------//

    public class GACIntermediaryVerification
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string IntermediaryActionId { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string IntermediaryId { get; set; }

        public GACIntermediaryVerification_IntermiList IntermediaryDetails { get; set; }

        public List<GACIntermediaryVerification_IntermiList> IntermediaryList { get; set; }

        public static GACIntermediaryVerification GetIntermediaryVerification(string RegistrationYear, string GrievanceId)
        {
            GACIntermediaryVerification Data = new GACIntermediaryVerification { IntermediaryDetails = new GACIntermediaryVerification_IntermiList(), RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "INTERIM_DETAILS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_VerifyIntermediary", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Data.IntermediaryDetails.IntermediaryId = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryId"]);
                Data.IntermediaryDetails.IntermediaryTitle = Convert.ToString(ds.Tables[0].Rows[0]["IntermediaryTitle"]);
                Data.IntermediaryDetails.IntermediaryUrl = Convert.ToString(ds.Tables[0].Rows[0]["URL"]);
                Data.IntermediaryDetails.GOName = Convert.ToString(ds.Tables[0].Rows[0]["GOName"]);
                Data.IntermediaryDetails.GOEmail = Convert.ToString(ds.Tables[0].Rows[0]["GOEmail"]);
                Data.IntermediaryDetails.IntermediaryAddress = Convert.ToString(ds.Tables[0].Rows[0]["Address"]);
                Data.IntermediaryDetails.Helplink = Convert.ToString(ds.Tables[0].Rows[0]["HelpLink"]);

            }
            return Data;
        }

        public static List<GACIntermediaryVerification_IntermiList> GetIntermediaryList()
        {
            List<GACIntermediaryVerification_IntermiList> List = new List<GACIntermediaryVerification_IntermiList>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "INTERIM_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_VerifyIntermediary", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List.Add(new GACIntermediaryVerification_IntermiList
                    {
                        IntermediaryId = Convert.ToString(dr["IntermediaryId"]),
                        IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                        IntermediaryUrl = Convert.ToString(dr["URL"]),
                        GOName = Convert.ToString(dr["GOName"]),
                        GOEmail = Convert.ToString(dr["GOEmail"]),
                        IntermediaryAddress = Convert.ToString(dr["Address"]),
                        Helplink = Convert.ToString(dr["HelpLink"]),
                    });
                }
            }
            return List;
        }
    }

    public class GACIntermediaryVerification_IntermiList
    {
        public string IntermediaryId { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(100, ErrorMessage = "Max. 100 characters allowed!")]
        public string IntermediaryTitle { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [Url(ErrorMessage = "Invalid URL")]
        [StringLength(200, ErrorMessage = "Max. 200 characters allowed!")]
        public string IntermediaryUrl { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(100, ErrorMessage = "Max. 100 characters allowed!")]
        public string GOName { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(100, ErrorMessage = "Max. 100 characters allowed!")]
        public string GOEmail { get; set; }

        [StringLength(300, ErrorMessage = "Max. 300 characters allowed!")]
        public string IntermediaryAddress { get; set; }

        [StringLength(200, ErrorMessage = "Max. 200 characters allowed!")]
        public string Helplink { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------//

    public class GACAppealDecision
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        public string DecisionFormat { get; set; }
        public string OriginalFormat { get; set; }
        public string MatterSummary { get; set; }
        //New
        public List<DraftDecisionHistoryChairpersonList> DraftDecisionnHistory { get; set; }
        //New

        [AllowHtml]
        [Required(ErrorMessage = "Mandatory Field!")]
        public string DraftDecision { get; set; }

        public static GACAppealDecision GetOrderSummaryDetails(string RegistrationYear, string GrievanceId)
        {
            string DecisionLetterFinal = "";
            GACAppealDecision Data = new GACAppealDecision { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            Data.DraftDecisionnHistory = new List<DraftDecisionHistoryChairpersonList>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "FETCH_ORDER")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Data.DecisionFormat = Convert.ToString(ds.Tables[0].Rows[0]["DecisionFormat"]);
                Data.DraftDecision = Convert.ToString(ds.Tables[0].Rows[0]["DraftDecision"]);
                Data.MatterSummary = Convert.ToString(ds.Tables[0].Rows[0]["MatterSummary"]);
                if (ds.Tables.Count > 1)
                {
                    Dictionary<string, string> keyword = new Dictionary<string, string>
                    {
                        { "@GacTitle", (ds.Tables[1].Rows[0]["GACTitle"] == null ? "" : ds.Tables[1].Rows[0]["GACTitle"].ToString()) },
                        { "@DecisionTitle", (ds.Tables[1].Rows[0]["DecisionTitle"] == null ? "" : ds.Tables[1].Rows[0]["DecisionTitle"].ToString()) },
                        { "@AppealDate", (ds.Tables[1].Rows[0]["AppealDate"] == null ? "" : ds.Tables[1].Rows[0]["AppealDate"].ToString()) },
                        { "@AppellantDetails", (ds.Tables[1].Rows[0]["AppellantDetails"] == null ? "" : ds.Tables[1].Rows[0]["AppellantDetails"].ToString()) },
                        { "@IntemediaryTitle", (ds.Tables[1].Rows[0]["IntermediaryTitle"] == null ? "" : ds.Tables[1].Rows[0]["IntermediaryTitle"].ToString()) },
                        { "@GREmail", (ds.Tables[1].Rows[0]["IntermediaryGROEmail"] == null ? "" : ds.Tables[1].Rows[0]["IntermediaryGROEmail"].ToString()) },
                        { "@GRAddress", (ds.Tables[1].Rows[0]["IntermediaryAddress"] == null ? "" : ds.Tables[1].Rows[0]["IntermediaryAddress"].ToString()) },
                        { "@AppealID", (ds.Tables[1].Rows[0]["AppealID"] == null ? "" : ds.Tables[1].Rows[0]["AppealID"].ToString()) },
                        { "@GroundofAppeal", (ds.Tables[1].Rows[0]["GroundTitle"] == null ? "" : ds.Tables[1].Rows[0]["GroundTitle"].ToString()) },
                        { "@Justification", (ds.Tables[1].Rows[0]["Justification"] == null ? "" : ds.Tables[1].Rows[0]["Justification"].ToString()) },
                        { "@ReliefSought", (ds.Tables[1].Rows[0]["ReliefTitle"] == null ? "" : ds.Tables[1].Rows[0]["ReliefTitle"].ToString()) },
                        { "@Observation", (ds.Tables[1].Rows[0]["ObservationText"] == null ? "" : ds.Tables[1].Rows[0]["ObservationText"].ToString()) },
                        { "@Recommendation", (ds.Tables[1].Rows[0]["RecommendationText"] == null ? "" : ds.Tables[1].Rows[0]["RecommendationText"].ToString()) },
                        { "@Decision", (ds.Tables[1].Rows[0]["RecTypeDesc"] == null ? "" : ds.Tables[1].Rows[0]["RecTypeDesc"].ToString()) },
                        { "@Summary", (ds.Tables[1].Rows[0]["Remarks"] == null ? "" : ds.Tables[1].Rows[0]["Remarks"].ToString()) },
                        { "@AppellantUrl", (ds.Tables[1].Rows[0]["AppellantUrls"] == null ? "" : ds.Tables[1].Rows[0]["AppellantUrls"].ToString()) }
                    };
                    DecisionLetterFinal = Data.DecisionFormat;
                    foreach (KeyValuePair<string, string> entry in keyword)
                    {
                        DecisionLetterFinal = DecisionLetterFinal.Replace(entry.Key.ToString(), entry.Value.ToString());
                    }
                }
                Data.DraftDecision = string.IsNullOrWhiteSpace(Data.DraftDecision) ? DecisionLetterFinal : Data.DraftDecision;
                Data.OriginalFormat = DecisionLetterFinal;
            }
            if (ds.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    Data.DraftDecisionnHistory.Add(new DraftDecisionHistoryChairpersonList
                    {
                        RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                        GrievanceId = Convert.ToString(dr["GrievanceId"]),
                        UpdationDateTime = Convert.ToString(dr["UpdationDateTime"]),
                        TimeDiifference = BALCommon.ConvertMinToDHM(Convert.ToString(dr["TimeDiifference"])),
                        UserDetails = Convert.ToString(dr["UserDetails"]),
                        AgreeDisagree = Convert.ToString(dr["AgreeDisagree"]),
                        CreationDateTime = Convert.ToString(dr["CreationDateTime"]),
                        Recommendations = Convert.ToString(dr["Recomendations"]),
                        DecisionType = Convert.ToString(dr["DecisionType"]),
                        DecisionRemarks = Convert.ToString(dr["DecisionRemarks"]),
                        DecisionTypeId = Convert.ToString(dr["DecisionTypeId"]),
                    });
                }

            }
            return Data;
        }
    }

    //---------------------------------------------------------------------------------------------------------------------//

    public class GACAppealAdditionalInput
    {
        public string InputId { get; set; }
        public string InputRequestNo { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        [Required(ErrorMessage = "Enter Query!")]
        [StringLength(2000, ErrorMessage = "Max. 2000 characters allowed!")]
        public string SeekQuery { get; set; }

        public string SeekDate { get; set; }

        public string IsDoc { get; set; }

        public string InputRemarks { get; set; }
        public string InputDate { get; set; }
        public string FilePath { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------//

    public class GACAppealDispose
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [ValidateFile("pdf", MaxFileSize: "4096", ErrorMessage = "Upload a valid {0} file of {1}!")]
        public HttpPostedFileBase UploadPDF { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------//

    public class GACAppealExpertOpinion
    {
        public string InputId { get; set; }
        public string InputRequestNo { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string ExpertUserId { get; set; }
        public List<GACExpertUser> ExpertUserList { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(150, ErrorMessage = "Max. 150 characters allowed!")]
        public string ExpertName { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(150, ErrorMessage = "Max. 150 characters allowed!")]
        public string ExpertEmail { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Max. 10 characters allowed!")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid Mobile No.!")]
        public string ExpertMobile { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(500, ErrorMessage = "Max. 500 characters allowed!")]
        public string AreaOfExpertise { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        [StringLength(2000, ErrorMessage = "Max. 2000 characters allowed!")]
        public string SeekRemarks { get; set; }

        public string SeekDate { get; set; }

        public string IsDoc { get; set; }

        public string InputRemarks { get; set; }
        public string InputDate { get; set; }
        public string FilePath { get; set; }

        public class GACExpertUser
        {
            public string ExpertUserId { get; set; }
            public string ExpertUserNm { get; set; }
            public string AreaOfExpertise { get; set; }
            public string MobileNo { get; set; }
            public string EmailId { get; set; }
            public static List<GACExpertUser> GetExpertUserList(string GACID)
            {
                List<GACExpertUser> List = new List<GACExpertUser>();
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@GACID", GACID),
                    new KeyValuePair<string, string>("@Mode", "EXPERTUSER_LIST")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_Action", SP_Parameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            List.Add(new GACExpertUser
                            {
                                ExpertUserId = Convert.ToString(dr["UserID"]),
                                ExpertUserNm = Convert.ToString(dr["UserName"]),
                                AreaOfExpertise = Convert.ToString(dr["AreaofExpertise"]),
                                MobileNo = Convert.ToString(dr["Mobile"]),
                                EmailId = Convert.ToString(dr["EmailID"]),
                            });
                        }
                    }
                }
                return List;
            }
        }
    }

    //---------------------------------------------------------------------------------------------------------------------//

    public class GACGridViewInbox_DelayResponses
    {
        public List<GACGridViewInbox_DelayResponses_AlertList> AlertList { get; set; }

        public static GACGridViewInbox_DelayResponses GetDelayResponsesData(string GACID, string UserId)
        {
            GACGridViewInbox_DelayResponses Appeal = new GACGridViewInbox_DelayResponses { AlertList = new List<GACGridViewInbox_DelayResponses_AlertList>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACID", GACID),
                new KeyValuePair<string, string>("@UserId", UserId),
                new KeyValuePair<string, string>("@LangCulture", UserSession.LangCulture),
                new KeyValuePair<string, string>("@Mode", "DELAY_RESPONSE")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Appeal.AlertList.Add(new GACGridViewInbox_DelayResponses_AlertList
                        {
                            Flag = Convert.ToString(dr["Flag"]),
                            AlertTypeDesc = Convert.ToString(dr["AlertTypeDesc"]),
                            AppealCount = Convert.ToString(dr["AppealCount"]),
                        });
                    }
                }
            }
            return Appeal;
        }
    }

    public class GACGridViewInbox_DelayResponses_AlertList
    {
        public string Flag { get; set; }
        public string AlertTypeDesc { get; set; }
        public string AppealCount { get; set; }
    }

    public class DecisionLog
    {
        public List<_DecisonLogList> DecisionLogList { get; set; }
        public static DecisionLog GetDelayResponsesData()
        {
            DecisionLog Appeal = new DecisionLog { DecisionLogList = new List<_DecisonLogList>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "List")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_GridViewInbox_DecisionLogList", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Appeal.DecisionLogList.Add(new _DecisonLogList
                        {
                            CreatedDateTime = Convert.ToString(dr["CreatedDateTime"]),
                            CreationIP = Convert.ToString(dr["CreationIP"]),
                            GrievanceID = Convert.ToString(dr["GrievanceID"]),
                            IsWindowOpened = Convert.ToString(dr["IsWindowOpened"]),
                            ParaID = Convert.ToString(dr["ParaID"]),
                            RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                            UserID = Convert.ToString(dr["UserID"]),
                            TimeDifference = BALCommon.ConvertMinToDHM(Convert.ToString(dr["TimeDifference"])),
                        });
                    }
                }
            }
            return Appeal;
        }


    }
    public class _DecisonLogList
    {
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string ParaID { get; set; }
        public string UserID { get; set; }
        public string IsWindowOpened { get; set; }
        public string CreatedDateTime { get; set; }
        public string CreationIP { get; set; }
        public string TimeDifference { get; set; }

    }

}