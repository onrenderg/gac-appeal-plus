using DigitalNagrik.Areas.GridView.Data;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.Dashboard.Models
{
    public class AdminDashboard
    {
        public string GACId { get; set; }
        public List<SelectListItem> GACList { get; set; }

        public Dashboard_Stats Stats { get; set; }
        public Dashboard_6Months SixMonths { get; set; }
        public Dashboard_GACStats GACStats { get; set; }
        public Dashboard_ReliefSought ReliefSought { get; set; }
        public Dashboard_AppealGround AppealGround { get; set; }
        public Dashboard_AvgDays AvgDays { get; set; }

        public static List<SelectListItem> GetMappedGACList(string UserId)
        {
            List<SelectListItem> GACList = new List<SelectListItem>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@UserId", UserId),
                new KeyValuePair<string, string>("@Mode", "GACLIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GACList.Add(new SelectListItem { Value = Convert.ToString(dr["GACId"]), Text = Convert.ToString(dr["GACTitle"]) });
                }
            }
            return GACList;
        }

        public static Dashboard_Stats GetData(string UserId)
        {
            Dashboard_Stats Data = new Dashboard_Stats();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@UserId", UserId),
                new KeyValuePair<string, string>("@Mode", "STATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Data.AppealsReceived = Convert.ToString(ds.Tables[0].Rows[0]["AppealsReceived"]);
                Data.ResponseIntermediary = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary"]);
                Data.ResponseIntermediary_After72hrs = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary_After72hrs"]);
                Data.ResponseFromIntermediary_NoResponse = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary_NoResponse"]);
                Data.ResponseIntermediary_ResponseTimeExceeded = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary_ResponseTimeExceeded"]);
                Data.Rejected = Convert.ToString(ds.Tables[0].Rows[0]["Rejected"]);
                Data.Disposed = Convert.ToString(ds.Tables[0].Rows[0]["Disposed"]);
                Data.InScrutiny = Convert.ToString(ds.Tables[0].Rows[0]["InScrutiny"]);
                Data.ResponseIntermediary_Percent = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary_Percent"]);
                Data.Rejected_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Rejected_Percent"]);
                Data.Disposed_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Disposed_Percent"]);
                Data.InScrutiny_Percent = Convert.ToString(ds.Tables[0].Rows[0]["InScrutiny_Percent"]);
                Data.reliefGiven = Convert.ToString(ds.Tables[0].Rows[0]["reliefGiven"]);
                Data.noRelief = Convert.ToString(ds.Tables[0].Rows[0]["noRelief"]);
                Data.otherRelief = Convert.ToString(ds.Tables[0].Rows[0]["otherRelief"]);
                Data.TimeBarred = Convert.ToString(ds.Tables[0].Rows[0]["TimeBarred"]);
                Data.SubJudice = Convert.ToString(ds.Tables[0].Rows[0]["SubJudice"]);
                Data.NoComplaint = Convert.ToString(ds.Tables[0].Rows[0]["NoComplaint"]);
                Data.RejOther = Convert.ToString(ds.Tables[0].Rows[0]["RejOther"]);
            }
            return Data;
        }
    }

    public class AdminDashboard_AppealList
    {
        public List<GACGridViewInbox_AppealList> AppealList { get; set; }

        public string FromDt { get; set; }
        public string ToDt { get; set; }
        public string IntermediaryId { get; set; }
        public List<GACIntermediaryVerification_IntermiList> IntermediaryList { get; set; }
        public string IsCompliance { get; set; }
        public string IsIntermediaryResponse { get; set; }

        public string AppealStatus { get; set; }
        public string AssignmentStatus { get; set; }
        public string GACId { get; set; }
        public string Flag { get; set; }

        public static AdminDashboard_AppealList GetAppealList(string GACId, string Flag, string FromDt = "", string ToDt = "", string IntermediaryId = "", string IsCompliance = "", string IsIntermediaryResponse = "", string AssignmentStatus = "N")
        {
            AdminDashboard_AppealList Appeal = new AdminDashboard_AppealList { AppealList = new List<GACGridViewInbox_AppealList>(), GACId = GACId, Flag = Flag };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Flag", Flag),
                new KeyValuePair<string, string>("@FromDt", FromDt),
                new KeyValuePair<string, string>("@ToDt", ToDt),
                new KeyValuePair<string, string>("@IntermediaryId", IntermediaryId),
                new KeyValuePair<string, string>("@IsCompliance", IsCompliance),
                new KeyValuePair<string, string>("@IsIntermediaryResponse", IsIntermediaryResponse),
                new KeyValuePair<string, string>("@AssignmentStatus", AssignmentStatus),
                new KeyValuePair<string, string>("@Mode", "APPEAL_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Appeal.AppealList.Add(new GACGridViewInbox_AppealList
                    {
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
                        IntermediaryReplyTimeLeft = Convert.ToString(dr["IntermediaryReplyTimeLeft"]),
                        GrievanceStatusCd = Convert.ToString(dr["GrievnaceStatus"]),
                        GrievanceActionId = Convert.ToString(dr["ActionId"]),
                        AppellantNm = Convert.ToString(dr["AppellantNm"]),
                        UserEmail = Convert.ToString(dr["UserEmail"]),
                        UserMobile = Convert.ToString(dr["UserMobile"]),
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
                        ComplianceDate = Convert.ToString(dr["ComplianceDate"]),
                        DecisionFilePath = Convert.ToString(dr["DecisionFilePath"]),
                        ComplianceFilePath = Convert.ToString(dr["ComplianceFilePath"]),
                        ComplianceUrl = Convert.ToString(dr["URL"]),
                        GACNm = Convert.ToString(dr["GACNm"]),
                    });
                }
            }
            return Appeal;
        }

        public static DataSet GetAppealListData(string GACId, string Flag, string FromDt = "", string ToDt = "", string IntermediaryId = "", string IsCompliance = "", string IsIntermediaryResponse = "", string AssignmentStatus = "N")
        {
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Flag", Flag),
                new KeyValuePair<string, string>("@FromDt", FromDt),
                new KeyValuePair<string, string>("@ToDt", ToDt),
                new KeyValuePair<string, string>("@IntermediaryId", IntermediaryId),
                new KeyValuePair<string, string>("@IsCompliance", IsCompliance),
                new KeyValuePair<string, string>("@IsIntermediaryResponse", IsIntermediaryResponse),
                new KeyValuePair<string, string>("@AssignmentStatus", AssignmentStatus),
                new KeyValuePair<string, string>("@Mode", "APPEAL_LIST")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            return ds;
        }

    }

    public class Dashboard_Stats
    {
        public string AppealsReceived { get; set; }
        public string UsersRegistered { get; set; }
        public string PendingForAssignment { get; set; }
        public string AssignedToGAC { get; set; }
        public string AppealsReceived_PerDay { get; set; }
        public string ResponseIntermediary { get; set; }
        public string ResponseIntermediary_After72hrs { get; set; }
        public string ResponseFromIntermediary_NoResponse { get; set; }
        public string ResponseIntermediary_ResponseTimeExceeded { get; set; }
        public string InScrutiny { get; set; }
        public string Rejected { get; set; }
        public string Disposed { get; set; }
        public string Compliance { get; set; }

        public string PendingForAssignment_Percent { get; set; }
        public string ResponseIntermediary_Percent { get; set; }
        public string InScrutiny_Percent { get; set; }
        public string Rejected_Percent { get; set; }
        public string Disposed_Percent { get; set; }

        public string reliefGiven { get; set; }
        public string noRelief { get; set; }
        public string otherRelief { get; set; }

        public string TimeBarred { get; set; }
        public string NoComplaint { get; set; }
        public string SubJudice { get; set; }
        public string RejOther { get; set; }

        public static Dashboard_Stats GetData(string GACId)
        {
            Dashboard_Stats Data = new Dashboard_Stats();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "STATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Data.AppealsReceived = Convert.ToString(ds.Tables[0].Rows[0]["AppealsReceived"]);
                Data.UsersRegistered = Convert.ToString(ds.Tables[0].Rows[0]["UsersRegistered"]);
                Data.PendingForAssignment = Convert.ToString(ds.Tables[0].Rows[0]["PendingForAssignment"]);
                Data.PendingForAssignment_Percent = Convert.ToString(ds.Tables[0].Rows[0]["PendingForAssignment_Percent"]);
                Data.AssignedToGAC = Convert.ToString(ds.Tables[0].Rows[0]["AssignedToGAC"]);
                Data.ResponseIntermediary = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary"]);
                Data.ResponseIntermediary_After72hrs = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary_After72hrs"]);
                Data.ResponseFromIntermediary_NoResponse = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary_NoResponse"]);
                Data.ResponseIntermediary_ResponseTimeExceeded = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary_ResponseTimeExceeded"]);
                Data.Rejected = Convert.ToString(ds.Tables[0].Rows[0]["Rejected"]);
                Data.Disposed = Convert.ToString(ds.Tables[0].Rows[0]["Disposed"]);
                Data.InScrutiny = Convert.ToString(ds.Tables[0].Rows[0]["InScrutiny"]);
                Data.ResponseIntermediary_Percent = Convert.ToString(ds.Tables[0].Rows[0]["ResponseIntermediary_Percent"]);
                Data.Rejected_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Rejected_Percent"]);
                Data.Disposed_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Disposed_Percent"]);
                Data.InScrutiny_Percent = Convert.ToString(ds.Tables[0].Rows[0]["InScrutiny_Percent"]);
                Data.reliefGiven = Convert.ToString(ds.Tables[0].Rows[0]["reliefGiven"]);
                Data.noRelief = Convert.ToString(ds.Tables[0].Rows[0]["noRelief"]);
                Data.otherRelief = Convert.ToString(ds.Tables[0].Rows[0]["otherRelief"]);
                Data.TimeBarred = Convert.ToString(ds.Tables[0].Rows[0]["TimeBarred"]);
                Data.SubJudice = Convert.ToString(ds.Tables[0].Rows[0]["SubJudice"]);
                Data.NoComplaint = Convert.ToString(ds.Tables[0].Rows[0]["NoComplaint"]);
                Data.Compliance = Convert.ToString(ds.Tables[0].Rows[0]["Compliance"]);
                Data.RejOther = Convert.ToString(ds.Tables[0].Rows[0]["RejOther"]);
            }
            return Data;
        }
    }

    public class Dashboard_Stats_AverageDays
    {
        public string Title { get; set; }
        public string Flag { get; set; }
        public int TotalNoOfDays { get; set; }
        public string HtmlString { get; set; }
        public List<Dashboard_Stats_AverageDays_Data> DataList { get; set; }

        public static Dashboard_Stats_AverageDays GetData(string GACId, string Flag)
        {
            Dashboard_Stats_AverageDays Data = new Dashboard_Stats_AverageDays { Flag = Flag, DataList = new List<Dashboard_Stats_AverageDays_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Flag", Flag),
                new KeyValuePair<string, string>("@Mode", "STATS_AVGDAYS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (Flag == GACGridViewInbox.MenuStatus.Inbox)
                {
                    Data.TotalNoOfDays = Convert.ToInt32(ds.Tables[0].Rows[0]["NoOfAppeals"]);
                }
                else
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.DataList.Add(new Dashboard_Stats_AverageDays_Data
                        {
                            PeriodDesc = Convert.ToString(dr["PeriodDesc"]),
                            NoOfAppeals = Convert.ToString(dr["NoOfAppeals"]),
                        });
                    }
                    Data.TotalNoOfDays = ds.Tables[1].Rows[0]["NoOfDays"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[1].Rows[0]["NoOfDays"]);
                }
            }
            return Data;
        }
    }

    public class Dashboard_Stats_AverageDays_Data
    {
        public string PeriodDesc { get; set; }
        public string NoOfAppeals { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_6Months
    {
        public List<Dashboard_6Months_Data> DataList { get; set; }
        public static Dashboard_6Months GetData(string GACId)
        {
            Dashboard_6Months Data = new Dashboard_6Months { DataList = new List<Dashboard_6Months_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "6MONTHS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_6Months_Data
                    {
                        MonthYearNm = Convert.ToString(dr["MonthYearNm"]),
                        Received = Convert.ToString(dr["Received"]),
                        Resolved = Convert.ToString(dr["Resolved"]),
                        Rejected = Convert.ToString(dr["Rejected"])
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_6Months_Data
    {
        public string MonthYearNm { get; set; }
        public string Received { get; set; }
        public string Resolved { get; set; }
        public string Rejected { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_AvgDays
    {
        public List<Dashboard_AvgDays_Data> DataList { get; set; }
        public static Dashboard_AvgDays GetData(string GACId)
        {
            Dashboard_AvgDays Data = new Dashboard_AvgDays { DataList = new List<Dashboard_AvgDays_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "AVG_DAYS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_AvgDays_Data
                    {
                        GACId = Convert.ToString(dr["GACId"]),
                        GACName = Convert.ToString(dr["GACTitle"]),
                        GACAbbr = Convert.ToString(dr["GACAbbr"]),
                        AverageDisposalDays = Convert.ToString(dr["AverageDisposalDays"])
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_AvgDays_Data
    {
        public string GACId { get; set; }
        public string GACName { get; set; }
        public string GACAbbr { get; set; }
        public string AverageDisposalDays { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_GACStats
    {
        public List<Dashboard_GACStats_Data> DataList { get; set; }

        public static Dashboard_GACStats GetData(string GACId)
        {
            Dashboard_GACStats Data = new Dashboard_GACStats { DataList = new List<Dashboard_GACStats_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "GAC_STATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_GACStats_Data
                    {
                        GACId = Convert.ToString(dr["GACId"]),
                        GACName = Convert.ToString(dr["GACTitle"]),
                        GACAbbr = Convert.ToString(dr["GACAbbr"]),
                        Assigned = Convert.ToString(dr["Assigned"]),
                        ResponseFromIntermediary = Convert.ToString(dr["ResponseFromIntermediary"]),
                        ResponseFromIntermediary_After72hrs = Convert.ToString(dr["ResponseFromIntermediary_After72hrs"]),
                        ResponseFromIntermediary_NoResponse = Convert.ToString(dr["ResponseFromIntermediary_NoResponse"]),
                        ResponseIntermediary_ResponseTimeExceeded = Convert.ToString(dr["ResponseIntermediary_ResponseTimeExceeded"]),
                        InProcess = Convert.ToString(dr["InProcess"]),
                        InProcess_Percent = Convert.ToString(dr["InProcess_Percent"]),
                        InProcess_MatterSummary = Convert.ToString(dr["InProcess_MatterSummary"]),
                        InProcess_AdditionalInfo = Convert.ToString(dr["InProcess_AdditionalInfo"]),
                        InProcess_Assistance = Convert.ToString(dr["InProcess_Assistance"]),
                        Rejected = Convert.ToString(dr["Rejected"]),
                        Rejected_Percent = Convert.ToString(dr["Rejected_Percent"]),
                        Decided = Convert.ToString(dr["Decided"]),
                        Decided_Percent = Convert.ToString(dr["Decided_Percent"]),
                        AverageDisposalDays = Convert.ToString(dr["AverageDisposalDays"]),
                        OpinionAwaited = Convert.ToString(dr["OpinionAwaited"]),
                        ChairpersonDecision = Convert.ToString(dr["ChairpersonDecision"]),
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_GACStats_Data
    {
        public string GACId { get; set; }
        public string GACName { get; set; }
        public string GACAbbr { get; set; }
        public string Assigned { get; set; }
        public string ResponseFromIntermediary { get; set; }
        public string ResponseFromIntermediary_After72hrs { get; set; }
        public string ResponseFromIntermediary_NoResponse { get; set; }
        public string ResponseIntermediary_ResponseTimeExceeded { get; set; }
        public string InProcess { get; set; }
        public string InProcess_Percent { get; set; }
        public string InProcess_MatterSummary { get; set; }
        public string InProcess_AdditionalInfo { get; set; }
        public string InProcess_Assistance { get; set; }

        public string Rejected { get; set; }
        public string Rejected_Percent { get; set; }

        public string Decided { get; set; }
        public string Decided_Percent { get; set; }
        public string AverageDisposalDays { get; set; }
        public string OpinionAwaited { get; set; }
        public string ChairpersonDecision { get; set; }

    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_ReliefSought
    {
        public List<Dashboard_ReliefSought_Data> DataList { get; set; }
        public static Dashboard_ReliefSought GetData(string GACId)
        {
            Dashboard_ReliefSought Data = new Dashboard_ReliefSought { DataList = new List<Dashboard_ReliefSought_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "RELIEF_WISE")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_ReliefSought_Data
                    {
                        ReliefId = Convert.ToString(dr["ReliefId"]),
                        ReliefTitle = Convert.ToString(dr["ReliefTitle"]),
                        NoOfAppeals = Convert.ToString(dr["NoOfAppeals"]),
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_ReliefSought_Data
    {
        public string ReliefId { get; set; }
        public string ReliefTitle { get; set; }
        public string NoOfAppeals { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_AppealGround
    {
        public List<Dashboard_AppealGround_Data> DataList { get; set; }
        public static Dashboard_AppealGround GetData(string GACId)
        {
            Dashboard_AppealGround Data = new Dashboard_AppealGround { DataList = new List<Dashboard_AppealGround_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "GROUND_WISE")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_AppealGround_Data
                    {
                        GroundId = Convert.ToString(dr["GroundId"]),
                        GroundTitle = Convert.ToString(dr["GroundTitle"]),
                        NoOfAppeals = Convert.ToString(dr["NoOfAppeals"]),
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_AppealGround_Data
    {
        public string GroundId { get; set; }
        public string GroundTitle { get; set; }
        public string NoOfAppeals { get; set; }
    }

    //------------------------------------------------------------------------------------------------//
    //------------------------------------------------------------------------------------------------//


    public class Dashboard_AssignAppeal
    {

        [Required(ErrorMessage = "Mandatory Field!")]
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GrievanceId { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string GACId { get; set; }
        public List<SelectListItem> GACList { get; set; }

        [Required(ErrorMessage = "Mandatory Field!")]
        public string AppealGround { get; set; }
        public List<SelectListItemAppealGround> AppealGroundList { get; set; }

        [StringLength(1000, ErrorMessage = "Max. 1000 characters allowed!")]
        public string Remarks { get; set; }

        public static List<SelectListItem> GetList(string Mode)
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", Mode)
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_AppealAssignment_Action", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List.Add(new SelectListItem { Value = Convert.ToString(dr["Id"]), Text = Convert.ToString(dr["Nm"]) });
                }
            }
            return List;
        }

        public class SelectListItemAppealGround
        {
            public string GroundId { get; set; }
            public string GroundDesc { get; set; }

            public string ITRule { get; set; }
            public string GACId { get; set; }

            public static List<SelectListItemAppealGround> GetGroundList()
            {
                List<SelectListItemAppealGround> List = new List<SelectListItemAppealGround>();
                List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@Mode", "GROUND_LIST")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_AppealAssignment_Action", SP_Parameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        List.Add(new SelectListItemAppealGround { GroundId = Convert.ToString(dr["Id"]), GroundDesc = Convert.ToString(dr["Nm"]), GACId = Convert.ToString(dr["GACId"]), ITRule = Convert.ToString(dr["ITRule"]) });
                    }
                }
                return List;
            }
        }

        public static Dashboard_AssignAppeal GetAppealInfo(string RegistrationYear, string GrievanceId)
        {
            Dashboard_AssignAppeal Data = new Dashboard_AssignAppeal { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "APPEAL_INFO")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_AppealAssignment_Action", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Data.GACId = Convert.ToString(ds.Tables[0].Rows[0]["GACId"]);
                Data.AppealGround = Convert.ToString(ds.Tables[0].Rows[0]["GroundAppealID"]);

            }
            return Data;
        }
    }
}