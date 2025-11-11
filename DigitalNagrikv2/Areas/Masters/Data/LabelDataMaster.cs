using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using DigitalNagrik.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitalNagrik.Areas.Masters.Data
{
    public class LabelDataMaster
    {
        public string ModuleID { get; set; }
        public List<SelectListItem> ModuleList { get; set; }
        public string FormID { get; set; }
        public List<SelectListItem> FormList { get; set; }
        public string LanguageCode { get; set; }
        public List<SelectListItem> LanguageList { get; set; }
        public List<LabelList_Data> LabelList { get; set; }

        public static LabelDataMaster GetData(string Mode,string LanguageCode=null,string FormID = null, string ModuleID = null)
        {
            LabelDataMaster Data = new LabelDataMaster();
            Data.LabelList = new List<LabelList_Data>();
            List<KeyValuePair<string, string>> SP_Parameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@LanguageCode" ,LanguageCode),
                new KeyValuePair<string, string>("@FormID",FormID),
                new KeyValuePair<string, string>("@ModuleID",ModuleID),
                new KeyValuePair<string, string>("@Mode",Mode)
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Masters_LabelData", SP_Parameter);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        Data.LabelList.Add(new LabelList_Data
                        {
                            FormID = Convert.ToString(dr["FormID"]),
                            FormName = Convert.ToString(dr["FormName"]),
                            LabelKey = Convert.ToString(dr["LabelKey"]),
                            LabelValue = Convert.ToString(dr["LabelValue"]),
                            LanguageCode = Convert.ToString(dr["LanguageCode"]),
                            LanguageName = Convert.ToString(dr["LanguageName"]),
                            LastModifiedBy = Convert.ToString(dr["LastModifiedBy"]),
                            LastModifiedIP = Convert.ToString(dr["LastModifiedIP"]),
                            LastModifiedOn = Convert.ToString(dr["LastModifiedOn"]),
                            ModuleID = Convert.ToString(dr["ModuleID"]),
                            ModuleName = Convert.ToString(dr["ModuleName"]),

                        });
                    }

                }
                
            }
            return Data;
        }
    }
    public class LabelList_Data
    {
        [Required(ErrorMessage = "Mandatory Field!")]
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
        public List<SelectListItem> ModuleList { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string FormID { get; set; }
        public string FormName { get; set; }
        public List<SelectListItem> FormList { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string LabelKey { get; set; }
        public string ActionType { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
        public List<SelectListItem> LanguageList { get; set; }
        [Required(ErrorMessage = "Mandatory Field!")]
        public string LabelValue { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        public string LastModifiedIP { get; set; }
    }
}