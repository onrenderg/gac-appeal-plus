using System;
using SQLite;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace GACAppeal.Models
{
	public class RefreshMasters
	{
        public string master_name { get; set; }
        public string last_updated { get; set; }
        public string last_updated_mobile { get; set; }
        public string mandatory { get; set; }
    }
    public class RefreshMastersDatabase
    {
        private SQLiteConnection conn;
        public RefreshMastersDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<RefreshMasters>();
        }
        public List<RefreshMasters> FillRefreshMasters()
        {
            var list = conn.Query<RefreshMasters>("Select * from RefreshMasters where last_updated <> last_updated_mobile");
            App.list_RefreshMasters = list;
            return list;
        }
        public bool UpdateMaster(string _mastername)
        {
            bool requireUpdate = false;
            var list = conn.Query<RefreshMasters>("Select * from RefreshMasters where master_name = ? and last_updated <> last_updated_mobile", new string[1] { _mastername});
            if (list.Any())
            {
                requireUpdate = true;
            }
            return requireUpdate;
        }

        public IEnumerable<RefreshMasters> GetRefreshMasters(String Querryhere, string[] aray)
        {
            var list = conn.Query<RefreshMasters>(Querryhere, aray);
            return list.ToList();
        }
        public string AddRefreshMasters(RefreshMasters service)
        {
            var list = conn.Query<RefreshMasters>("Select * from RefreshMasters where master_name = ?", new string[1] { service.master_name});
            if (list.Any())
            {
                var Custm = conn.Query<RefreshMasters>("Update RefreshMasters set last_updated = ? where master_name = ?", new string[2] { service.last_updated,service.master_name});
            }
            else
            {
                service.last_updated_mobile = "1900-01-01 01:01:01";
                conn.Insert(service);
            }
            
            return "success";
        }
        public string UpdateRefreshMasters(string master_name)
        {
            var Custm = conn.Query<RefreshMasters>("Update RefreshMasters set last_updated_mobile = last_updated where master_name = ?", new string[1] { master_name});
            return "success";
        }
        public string DeleteRefreshMasters()
        {
            var del = conn.Query<RefreshMasters>("delete from RefreshMasters");
            return "success";
        }

        public string Custom(string query)
        {
            var Custm = conn.Query<RefreshMasters>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<RefreshMasters>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }

    }
}
