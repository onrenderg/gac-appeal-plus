using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class Dashboard
    {
    }

    public class Dashboard_MeityStats
    {
        public string AppealsReceived { get; set; }
        public string AssignedToGAC { get; set; }
        public string ReferredBack { get; set; }
        public string Rejected { get; set; }
        public string Disposed { get; set; }
        public string InScrutiny { get; set; }

        public string AssignedToGAC_Percent { get; set; }
        public string ReferredBack_Percent { get; set; }
        public string Rejected_Percent { get; set; }
        public string Disposed_Percent { get; set; }
        public string InScrutiny_Percent { get; set; }
        public static Dashboard_MeityStats GetData()
        {
            Dashboard_MeityStats Data = new Dashboard_MeityStats();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "MEITY_STATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Data.AppealsReceived = Convert.ToString(ds.Tables[0].Rows[0]["AppealsReceived"]);
                Data.AssignedToGAC = Convert.ToString(ds.Tables[0].Rows[0]["AssignedToGAC"]);
                Data.ReferredBack = Convert.ToString(ds.Tables[0].Rows[0]["ReferredBack"]);
                Data.Rejected = Convert.ToString(ds.Tables[0].Rows[0]["Rejected"]);
                Data.Disposed = Convert.ToString(ds.Tables[0].Rows[0]["Disposed"]);
                Data.InScrutiny = Convert.ToString(ds.Tables[0].Rows[0]["InScrutiny"]);
                Data.AssignedToGAC_Percent = Convert.ToString(ds.Tables[0].Rows[0]["AssignedToGAC_Percent"]);
                Data.ReferredBack_Percent = Convert.ToString(ds.Tables[0].Rows[0]["ReferredBack_Percent"]);
                Data.Rejected_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Rejected_Percent"]);
                Data.Disposed_Percent = Convert.ToString(ds.Tables[0].Rows[0]["Disposed_Percent"]);
                Data.InScrutiny_Percent = Convert.ToString(ds.Tables[0].Rows[0]["InScrutiny_Percent"]);
            }
            return Data;
        }
    }

    public class Dashboard_MeityStats_AverageDays
    {
        public string TotalNoOfDays { get; set; }
        public string HtmlString { get; set; }
        public List<Dashboard_MeityStats_AverageDays> DataList { get; set; }
    }

    public class Dashboard_MeityStats_AverageDays_Data
    {
        public string DaysParamNm { get; set; }
        public string NoOfDays { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_Meity6Months
    {
        public List<Dashboard_Meity6Months_Data> DataList { get; set; }
        public static Dashboard_Meity6Months GetData()
        {
            Dashboard_Meity6Months Data = new Dashboard_Meity6Months { DataList = new List<Dashboard_Meity6Months_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "MEITY_6MONTHS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_Meity6Months_Data
                    {
                        MonthYearNm = Convert.ToString(dr["MonthYearNm"]),
                        Received = Convert.ToString(dr["Received"]),
                        Resolved = Convert.ToString(dr["Resolved"])
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_Meity6Months_Data
    {
        public string MonthYearNm { get; set; }
        public string Received { get; set; }
        public string Resolved { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_MeityAvgDays
    {
        public List<Dashboard_MeityAvgDays_Data> DataList { get; set; }
        public static Dashboard_MeityAvgDays GetData()
        {
            Dashboard_MeityAvgDays Data = new Dashboard_MeityAvgDays { DataList = new List<Dashboard_MeityAvgDays_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "MEITY_AVGDAYS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_MeityAvgDays_Data
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

    public class Dashboard_MeityAvgDays_Data
    {
        public string GACId { get; set; }
        public string GACName { get; set; }
        public string GACAbbr { get; set; }
        public string AverageDisposalDays { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_MeityGACStats
    {
        public List<Dashboard_MeityGACStats_Data> DataList { get; set; }

        public static Dashboard_MeityGACStats GetData()
        {
            Dashboard_MeityGACStats Data = new Dashboard_MeityGACStats { DataList = new List<Dashboard_MeityGACStats_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "MEITY_GACSTATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_MeityGACStats_Data
                    {
                        GACId = Convert.ToString(dr["GACId"]),
                        GACName = Convert.ToString(dr["GACTitle"]),
                        GACAbbr = Convert.ToString(dr["GACAbbr"]),
                        Assigned = Convert.ToString(dr["Assigned"]),
                        Disposed = Convert.ToString(dr["Disposed"]),
                        Disposed_Percent = Convert.ToString(dr["Disposed_Percent"]),
                        Disposed_DirectionsInterm = Convert.ToString(dr["Disposed_DirectionsInterm"]),
                        Disposed_DirectionsMeity = Convert.ToString(dr["Disposed_DirectionsMeity"]),
                        Pending = Convert.ToString(dr["Pending"]),
                        Pending_Percent = Convert.ToString(dr["Pending_Percent"]),
                        Pending_30days = Convert.ToString(dr["Pending_30days"]),
                        Pending_25_30days = Convert.ToString(dr["Pending_25_30days"]),
                        AverageDisposalDays = Convert.ToString(dr["AverageDisposalDays"]),
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_MeityGACStats_Data
    {
        public string GACId { get; set; }
        public string GACName { get; set; }
        public string GACAbbr { get; set; }
        public string Assigned { get; set; }
        public string Disposed { get; set; }
        public string Disposed_Percent { get; set; }
        public string Disposed_DirectionsInterm { get; set; }
        public string Disposed_DirectionsMeity { get; set; }
        public string Pending { get; set; }
        public string Pending_Percent { get; set; }
        public string Pending_30days { get; set; }
        public string Pending_25_30days { get; set; }
        public string AverageDisposalDays { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_MeityContentClassification
    {
        public List<Dashboard_MeityContentClassification_Data> DataList { get; set; }
        public static Dashboard_MeityContentClassification GetData()
        {
            Dashboard_MeityContentClassification Data = new Dashboard_MeityContentClassification { DataList = new List<Dashboard_MeityContentClassification_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@Mode", "CONTENT_CLASS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Meity_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_MeityContentClassification_Data
                    {
                        ContentClassDesc = Convert.ToString(dr["ContentClassificationDesc"]),
                        ContentSubClassDesc = Convert.ToString(dr["SubContentClassificationDesc"]),
                        NoOfAppeals = Convert.ToString(dr["NoOfAppeals"])
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_MeityContentClassification_Data
    {
        public string ContentClassDesc { get; set; }
        public string ContentSubClassDesc { get; set; }
        public string NoOfAppeals { get; set; }
    }

    //------------------------------------------------------------------------------------------------//
    //------------------------------------------------------------------------------------------------//
    //------------------------------------------------------------------------------------------------//

    public class Dashboard_GACStats
    {
        public string AppealsReceived { get; set; }
        public string AppealsAssigned { get; set; }
        public string AppealsPending { get; set; }
        public string AppealsInbox { get; set; }

        public string AppealsAssigned_Percent { get; set; }
        public string AppealsPending_Percent { get; set; }
        public static Dashboard_GACStats GetData(string GACId)
        {
            Dashboard_GACStats Data = new Dashboard_GACStats();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "GAC_STATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Data.AppealsReceived = Convert.ToString(ds.Tables[0].Rows[0]["AppealsReceived"]);
                Data.AppealsAssigned = Convert.ToString(ds.Tables[0].Rows[0]["AppealsAssigned"]);
                Data.AppealsPending = Convert.ToString(ds.Tables[0].Rows[0]["AppealsPending"]);
                Data.AppealsInbox = Convert.ToString(ds.Tables[0].Rows[0]["AppealsInbox"]);
                Data.AppealsAssigned_Percent = Convert.ToString(ds.Tables[0].Rows[0]["AppealsAssigned_Percent"]);
                Data.AppealsPending_Percent = Convert.ToString(ds.Tables[0].Rows[0]["AppealsPending_Percent"]);
            }
            return Data;
        }
    }

    public class Dashboard_GAC6Months
    {
        public List<Dashboard_GAC6Months_Data> DataList { get; set; }
        public static Dashboard_GAC6Months GetData(string GACId)
        {
            Dashboard_GAC6Months Data = new Dashboard_GAC6Months { DataList = new List<Dashboard_GAC6Months_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "GAC_6MONTHS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_GAC6Months_Data
                    {
                        MonthYearNm = Convert.ToString(dr["MonthYearNm"]),
                        Received = Convert.ToString(dr["Received"]),
                        Resolved = Convert.ToString(dr["Resolved"])
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_GAC6Months_Data
    {
        public string MonthYearNm { get; set; }
        public string Received { get; set; }
        public string Resolved { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

    public class Dashboard_GACContentClassification
    {
        public List<Dashboard_GACContentClassification_Data> DataList { get; set; }
        public static Dashboard_GACContentClassification GetData()
        {
            Dashboard_GACContentClassification Data = new Dashboard_GACContentClassification { DataList = new List<Dashboard_GACContentClassification_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", UserSession.GACID),
                new KeyValuePair<string, string>("@Mode", "CONTENT_CLASS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_GACContentClassification_Data
                    {
                        ContentClassDesc = Convert.ToString(dr["ContentClassificationDesc"]),
                        ContentSubClassDesc = Convert.ToString(dr["SubContentClassificationDesc"]),
                        NoOfAppeals = Convert.ToString(dr["NoOfAppeals"])
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_GACContentClassification_Data
    {
        public string ContentClassDesc { get; set; }
        public string ContentSubClassDesc { get; set; }
        public string NoOfAppeals { get; set; }
    }

    //------------------------------------------------------------------------------------------------//
    public class Dashboard_GACMemberStats
    {
        public List<Dashboard_GACMemberStats_Data> DataList { get; set; }

        public static Dashboard_GACMemberStats GetData(string GACId)
        {
            Dashboard_GACMemberStats Data = new Dashboard_GACMemberStats { DataList = new List<Dashboard_GACMemberStats_Data>() };
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@GACId", GACId),
                new KeyValuePair<string, string>("@Mode", "GAC_MEMBERSTATS")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GAC_Dashboard", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.DataList.Add(new Dashboard_GACMemberStats_Data
                    {
                        MemberId = Convert.ToString(dr["UserID"]),
                        MemberName = Convert.ToString(dr["UserName"]),
                        RoleNm = Convert.ToString(dr["RoleName"]),
                        Assigned = Convert.ToString(dr["totalAssigned"]),
                        Disposed = Convert.ToString(dr["totalDisposed"]),
                        Disposed_Percent = Convert.ToString(dr["totalDisposed_percent"]),
                        Disposed_DirectionsInterm = Convert.ToString(dr["Disposed_DirectionsInterm"]),
                        Disposed_DirectionsMeity = Convert.ToString(dr["Disposed_DirectionsMeity"]),
                        Pending = Convert.ToString(dr["totalPending"]),
                        Pending_Percent = Convert.ToString(dr["totalPending_percent"]),
                        AverageDisposalDays = Convert.ToString(dr["AverageDisposalInMin"]),
                    });
                }
            }
            return Data;
        }
    }

    public class Dashboard_GACMemberStats_Data
    {
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string RoleNm { get; set; }
        public string Assigned { get; set; }
        public string Disposed { get; set; }
        public string Disposed_Percent { get; set; }
        public string Disposed_DirectionsInterm { get; set; }
        public string Disposed_DirectionsMeity { get; set; }
        public string Pending { get; set; }
        public string Pending_Percent { get; set; }
        public string AverageDisposalDays { get; set; }
    }

    //------------------------------------------------------------------------------------------------//

}