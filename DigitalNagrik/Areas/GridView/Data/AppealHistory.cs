using Microsoft.Security.Application;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.GridView.Data
{
    public class AppealHistory
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string MatterSummary { get; set; }
        public string ObservationText { get; set; }
        public string DraftDecision { get; set; }
        public string RejectSummary { get; set; }
        public string IsIntermediaryReply { get; set; }
        public string IntermediaryTime { get; set; }
        public string DecisionFilePath { get; set; }
        public string RejectReason { get; set; }
        public string ComplianceDate { get; set; }
        public string ComplianceRemarks { get; set; }
        public string ComplianceSupportingDocument { get; set; }
        public string ComplianceURL { get; set; }
        public string AssignedToGAC { get; set; }

        public List<AppealHistory_List> HistoryList { get; set; }
        public List<GACAppealAdditionalInput> AdditionalInputList { get; set; }
        public List<GACAppealExpertOpinion> ExpertOpinionList { get; set; }

        public static AppealHistory GetAppealHistory(string RegistrationYear, string GrievanceId)
        {
            AppealHistory Appeal = new AppealHistory() { HistoryList = new List<AppealHistory_List>(), AdditionalInputList = new List<GACAppealAdditionalInput>(), ExpertOpinionList = new List<GACAppealExpertOpinion>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "TRACK_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_AppealHistory", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Appeal.HistoryList.Add(new AppealHistory_List
                        {
                            ActionId = Convert.ToString(dr["ActionId"]),
                            ActionDateTime = Convert.ToString(dr["ActionDateTime"]),
                            ActionBy = Convert.ToString(dr["ActionBy"]),
                            ActionByUserName = Convert.ToString(dr["ActionByUserName"]),
                            Remarks = Convert.ToString(dr["Remarks"]),
                            IsAgree = Convert.ToString(dr["IsAgree"]),
                            FromGACId = Convert.ToString(dr["FromGACId"]),
                            SuggestedGACId = Convert.ToString(dr["SuggestedGACId"]),
                            ToGACId = Convert.ToString(dr["ToGACId"]),
                            ExpertInputId = Convert.ToString(dr["ExpertInputId"]),
                            AdditionInputId = Convert.ToString(dr["AdditionInputId"]),
                            ExpertInputId_Reply = Convert.ToString(dr["ExpertInputId_Reply"]),
                            AdditionInputId_Reply = Convert.ToString(dr["AdditionInputId_Reply"]),
                            DraftDecisionLog = Convert.ToString(dr["DraftDecision"]),
                        });
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    Appeal.IsIntermediaryReply = Convert.ToString(ds.Tables[1].Rows[0]["IsIntermediaryReply"]);
                    Appeal.AssignedToGAC = Convert.ToString(ds.Tables[1].Rows[0]["AssignedToGAC"]);
                    Appeal.MatterSummary = Sanitizer.GetSafeHtmlFragment( Convert.ToString(ds.Tables[1].Rows[0]["MatterSummary"]));
                    Appeal.DraftDecision = Sanitizer.GetSafeHtmlFragment(Convert.ToString(ds.Tables[1].Rows[0]["DraftDecision"]));
                    Appeal.RejectSummary = Sanitizer.GetSafeHtmlFragment(Convert.ToString(ds.Tables[1].Rows[0]["RejectSummary"]));
                    Appeal.IntermediaryTime = Convert.ToString(ds.Tables[1].Rows[0]["IntermediaryTime"]);
                    Appeal.DecisionFilePath = Sanitizer.GetSafeHtmlFragment(Convert.ToString(ds.Tables[1].Rows[0]["DecisionFilePath"]));
                    Appeal.RejectReason = Convert.ToString(ds.Tables[1].Rows[0]["NoAdmitTypeDesc"]);
                    Appeal.ObservationText = Convert.ToString(ds.Tables[1].Rows[0]["ObservationText"]);
                    Appeal.ComplianceDate = Convert.ToString(ds.Tables[1].Rows[0]["ComplianceDate"]);
                    Appeal.ComplianceRemarks = Convert.ToString(ds.Tables[1].Rows[0]["Remarks"]);
                    Appeal.ComplianceSupportingDocument = Convert.ToString(ds.Tables[1].Rows[0]["SupportingDocument"]);
                    Appeal.ComplianceURL = Convert.ToString(ds.Tables[1].Rows[0]["URL"]);
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

    public class AppealHistory_List
    {
        public string ActionId { get; set; }
        public string Remarks { get; set; }
        public string ActionBy { get; set; }
        public string ActionByUserName { get; set; }
        public string ActionDateTime { get; set; }
        public string IsAgree { get; set; }
        public string FromGACId { get; set; }
        public string SuggestedGACId { get; set; }
        public string ToGACId { get; set; }
        public string ExpertInputId { get; set; }
        public string AdditionInputId { get; set; }
        public string ExpertInputId_Reply { get; set; }
        public string AdditionInputId_Reply { get; set; }
        public string DraftDecisionLog { get; set; }
    }
}