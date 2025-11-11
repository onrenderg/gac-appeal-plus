using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class FormLabels
    {
        public class Module
        {

            public const string Appellant = "1";
            public const string Intermediary = "2";
            public const string Admin = "3";
            public const string SubjectExpert = "4";
            public const string GACGridView = "5";
            public const string GACDashboard = "6";
            public const string Masters = "7";
            public const string Reports = "8";
        }
        public class FormMasters
        {
            public const string MasterIntermediary = "1";
            public const string MasterGroundforAppeal = "2";
            public const string MasterReliefSought = "3";
            public const string MasterExpertUsers = "4";

        }

        public static Dictionary<string, string> GetFormLabels(string ModuleID, string FormID, string LangCulture)
        {
            Dictionary<string, string> labelDictionary = new Dictionary<string, string>();
            var methodparameter = new List<KeyValuePair<string, string>>();
            methodparameter.Add(new KeyValuePair<string, string>("@ModuleID", ModuleID));
            methodparameter.Add(new KeyValuePair<string, string>("@FormID", FormID));
            methodparameter.Add(new KeyValuePair<string, string>("@LanguageCode", LangCulture));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "getFormLabels", methodparameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    for (var row = 0; row < ds.Tables[0].Rows.Count; row++)
                    {
                        labelDictionary.Add(ds.Tables[0].Rows[row]["LabelID"].ToString(), ds.Tables[0].Rows[row]["LabelText"].ToString());
                    }
                    return labelDictionary;
                }
            }

            return labelDictionary;
        }

        public static string GetSingleLabels(string ModuleID, string FormID, string LangCulture, string LabelKey)
        {
            string LabelText = string.Empty;
            var methodparameter = new List<KeyValuePair<string, string>>();
            methodparameter.Add(new KeyValuePair<string, string>("@ModuleID", ModuleID));
            methodparameter.Add(new KeyValuePair<string, string>("@FormID", FormID));
            methodparameter.Add(new KeyValuePair<string, string>("@LabelKey", LabelKey));
            methodparameter.Add(new KeyValuePair<string, string>("@LanguageCode", LangCulture));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "getFormLabels", methodparameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    LabelText = Convert.ToString(ds.Tables[0].Rows[0]["LabelText"]);
                }
            }
            return LabelText;
        }
    }

    public class mLanguageMaster
    {
        public string languageCode { get; set; }
        public string languageDescription { get; set; }
        public string languageDescriptionlocal { get; set; }
    }

}