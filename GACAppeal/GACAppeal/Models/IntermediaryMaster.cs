using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;
using Xamarin.Forms;

namespace GACAppeal
{

    public class IntermediaryMaster
    {
        public string IntermediaryId { get; set; }
        public string IntermediaryTitle { get; set; }
        public string URL { get; set; }
        public string GOName { get; set; }
        public string Address { get; set; }
        public string GOEmail { get; set; }
        public string HelpLink { get; set; }

        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
    }
    public class IntermediaryMasterDatabase
    {
        private SQLiteConnection conn;
        public IntermediaryMasterDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<IntermediaryMaster>();
        }
        public IEnumerable<IntermediaryMaster> FillIntermediaryType()
        {
            var list = conn.Query<IntermediaryMaster>("Select * from IntermediaryMaster");
            return list;
        }

        public IEnumerable<IntermediaryMaster> FillIntermediaryByTitle(string _IntermediaryTitle)
        {
            var list = conn.Query<IntermediaryMaster>($"Select * from IntermediaryMaster where IntermediaryTitle like '%{_IntermediaryTitle}%' order by IntermediaryTitle LIMIT 10");
            return list;
        }

        public IEnumerable<IntermediaryMaster> FillIntermediaryByURL(string _URL)
        {
            var list = conn.Query<IntermediaryMaster>($"Select * from IntermediaryMaster where URL like '%{_URL}%' order by URL LIMIT 10");
            return list;
        }

        public IEnumerable<IntermediaryMaster> GetIntermediaryMaster(String Querryhere, string[] aray)
        {
            var list = conn.Query<IntermediaryMaster>(Querryhere, aray);
            return list.ToList();
        }
        public string AddIntermediaryMaster(IntermediaryMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteIntermediaryMaster(string _IntermediaryType)
        {
            if (_IntermediaryType == "0")
            {
                var del = conn.Query<IntermediaryMaster>("delete from IntermediaryMaster");
            }
            else
            {
                var del = conn.Query<IntermediaryMaster>("delete from IntermediaryMaster where IntermediaryType = ?", new string[1] { _IntermediaryType});
            }
            
            return "success";
        }

        public string Custom(string query)
        {
            var Custm = conn.Query<IntermediaryMaster>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<IntermediaryMaster>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }

    }
}
