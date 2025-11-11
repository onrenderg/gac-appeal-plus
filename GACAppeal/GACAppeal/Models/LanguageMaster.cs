using System;
using SQLite;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace GACAppeal.Models
{
	public class LanguageMaster
	{
        public string LanguageCode { get; set; }
        public string languageDescription { get; set; }
        public string languageDescriptionlocal { get; set; }
    }
    public class LanguageMasterDatabase
    {
        private SQLiteConnection conn;
        public LanguageMasterDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<LanguageMaster>();
        }
        public List<LanguageMaster> FillLanguageMaster()
        {
            var list = conn.Query<LanguageMaster>("Select * from LanguageMaster");
            return list;
        }


        public IEnumerable<LanguageMaster> GetLanguageMaster(String Querryhere, string[] aray)
        {
            var list = conn.Query<LanguageMaster>(Querryhere, aray);
            return list.ToList();
        }
        public string AddLanguageMaster(LanguageMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteLanguageMaster()
        {
            var del = conn.Query<LanguageMaster>("delete from LanguageMaster");
            return "success";
        }

        public string Custom(string query)
        {
            var Custm = conn.Query<LanguageMaster>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<LanguageMaster>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }

    }
}
