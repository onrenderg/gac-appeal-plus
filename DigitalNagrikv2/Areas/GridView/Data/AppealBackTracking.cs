using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.GridView.Data
{
    public class AppealBackTracking_List
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string AppealID { get; set; }
        public string GACId { get; set; }
        public string GACAbbr { get; set; }
        public string AssignedDateTime { get; set; }
        public string ActionId { get; set; }
        public string ActionDesc { get; set; }
        public string LastActionBy { get; set; }
        public string LastActionDateTime { get; set; }
        public string LastActionIP { get; set; }
    }
    public class AppealBackTracking
    {
        public List<AppealBackTracking_List> AppealDetailsList { get; set; }
        public List<SelectListItem> Actions { get; set; }
        public string ActionString { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string AppealID { get; set; }

        public static AppealBackTracking GetAppealDetails(string RegistrationYear, string GrievanceId)
        {
            AppealBackTracking Data = new AppealBackTracking { RegistrationYear = RegistrationYear, GrievanceId = GrievanceId };
            Data.AppealDetailsList = new List<AppealBackTracking_List>();
            Data.Actions = new List<SelectListItem>();
            //List<SelectListItem> List = new List<SelectListItem>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear),
                new KeyValuePair<string, string>("@GrievanceId", GrievanceId),
                new KeyValuePair<string, string>("@Mode", "Get_Details")
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AppealBackTracking_Action", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Data.AppealDetailsList.Add(new AppealBackTracking_List
                        {
                            GrievanceId = (Convert.ToString(dr["GrievanceId"])),
                            ActionDesc = (Convert.ToString(dr["ActionDesc"])),
                            ActionId = (Convert.ToString(dr["ActionId"])),
                            AssignedDateTime = (Convert.ToString(dr["AssignedDateTime"])),
                            GACAbbr = (Convert.ToString(dr["GACAbbr"])),
                            GACId = (Convert.ToString(dr["GACId"])),
                            LastActionBy = (Convert.ToString(dr["LastActionBy"])),
                            LastActionDateTime = (Convert.ToString(dr["LastActionDateTime"])),
                            LastActionIP = (Convert.ToString(dr["LastActionIP"])),
                            RegistrationYear = (Convert.ToString(dr["RegistrationYear"])),
                        });
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        Data.Actions.Add(new SelectListItem { Value = Convert.ToString(dr["ActionId"]), Text = Convert.ToString(dr["ActionDesc"]), });
                    }
                }
            }

            return Data;
        }
    }
}