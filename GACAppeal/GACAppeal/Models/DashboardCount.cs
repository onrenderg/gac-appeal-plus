using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;
using Xamarin.Forms;

namespace GACAppeal
{
    public class DashboardCount
    {
        public string Submitted { get; set; }
        public string Disposed { get; set; }

        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
    }
    public class DashboardCountDatabase
    {
        private SQLiteConnection conn;
        public DashboardCountDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<DashboardCount>();
        }
        public IEnumerable<DashboardCount> FillDashboardCount()
        {
            var list = conn.Query<DashboardCount>("Select * from DashboardCount");
            App.list_DashboardCount = list;
            return list;
        }
        public IEnumerable<DashboardCount> GetDashboardCount(String Querryhere, string[] aray)
        {
            var list = conn.Query<DashboardCount>(Querryhere, aray);
            return list.ToList();
        }
        public string AddDashboardCount(DashboardCount service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteDashboardCount()
        {
            var del = conn.Query<DashboardCount>("delete from DashboardCount");
            return "success";
        }

        public string Custom(string query)
        {
            var Custm = conn.Query<DashboardCount>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<DashboardCount>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }

    }
}
