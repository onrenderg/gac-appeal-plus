using System;
using SQLite;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;

namespace GACAppeal.Models
{
	public class FormLabels
	{
        public string ModuleID { get; set; }
        public string FormID { get; set; }
        public string LabelID { get; set; }
        public string LabelText { get; set; }
        public string LanguageCode { get; set; }
    }
    public class FormLabelsDatabase
    {
        private SQLiteConnection conn;
        public FormLabelsDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<FormLabels>();
        }
        public IEnumerable<FormLabels> FillFormLabels()
        {
            var list = conn.Query<FormLabels>($"Select * from FormLabels");
            return list;
        }
        public string FillFormLabels(string _LabelID)
        {
            string myLableText = "";
            var list = conn.Query<FormLabels>($"Select * from FormLabels where LanguageCode = '{Preferences.Get("LanguageCode", "en-IN")}' and LabelID = '{_LabelID}'");
            if (list.Any())
            {
                myLableText = list.First().LabelText;
            }
            return myLableText;
        }

        public IEnumerable<FormLabels> GetFormLabels(String Querryhere, string[] aray)
        {
            var list = conn.Query<FormLabels>(Querryhere, aray);
            return list.ToList();
        }
        public string AddFormLabels(FormLabels service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteFormLabels()
        {
            var del = conn.Query<FormLabels>("delete from FormLabels");
            return "success";
        }

        public string Custom(string query)
        {
            var Custm = conn.Query<FormLabels>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<FormLabels>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }

    }
}
