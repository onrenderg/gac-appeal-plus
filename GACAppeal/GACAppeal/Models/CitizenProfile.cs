using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;
using Xamarin.Forms;

namespace GACAppeal
{

    public class CitizenProfile
    {
        public string UserId { get; set; }
        public string UserProfilleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string AadhaarVerificationStatus { get; set; }

        public string UserMobile { get; set; }
        public string UserEmail { get; set; }

        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
    }
    public class CitizenProfileDatabase
    {
        private SQLiteConnection conn;
        public CitizenProfileDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<CitizenProfile>();
        }
        public IEnumerable<CitizenProfile> FillCitizenProfile()
        {
            var list = conn.Query<CitizenProfile>("Select * from CitizenProfile");
            App.listCitizenProfile = list;
            return list;
        }
       
        public IEnumerable<CitizenProfile> GetCitizenProfile(String Querryhere, string[] aray)
        {
            var list = conn.Query<CitizenProfile>(Querryhere, aray);
            return list.ToList();
        }
        public string AddCitizenProfile(CitizenProfile service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteCitizenProfile()
        {
            var del = conn.Query<CitizenProfile>("delete from CitizenProfile");
            return "success";
        }

        public string Custom(string query)
        {
            var Custm = conn.Query<CitizenProfile>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<CitizenProfile>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }

    }
}
