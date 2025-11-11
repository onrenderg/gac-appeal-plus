using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;
using Xamarin.Forms;

namespace GACAppeal
{
    public class Groundkeywords
    {
        public string Keyword { get; set; }

        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
    }
    public class GroundkeywordsDatabase
    {
        private SQLiteConnection conn;
        public GroundkeywordsDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<Groundkeywords>();
        }
        public IEnumerable<Groundkeywords> FillGroundkeywords(string _keyword)
        {
            List<Groundkeywords> list = new List<Groundkeywords>();
            try
            {
                list = conn.Query<Groundkeywords>($"Select * from Groundkeywords where Keyword like '{_keyword.Trim()}%'");
            }
            catch (Exception ex)
            {
                list = conn.Query<Groundkeywords>($"Select * from Groundkeywords");
            }
            return list;
        }
        public IEnumerable<Groundkeywords> GetGroundkeywords(String Querryhere, string[] aray)
        {
            var list = conn.Query<Groundkeywords>(Querryhere, aray);
            return list.ToList();
        }
        public string AddGroundkeywords(Groundkeywords service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteGroundkeywords()
        {
            var del = conn.Query<Groundkeywords>("delete from Groundkeywords");
            return "success";
        }
        
        public string Custom(string query)
        {
            var Custm = conn.Query<Groundkeywords>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<Groundkeywords>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }
        
    }
}
