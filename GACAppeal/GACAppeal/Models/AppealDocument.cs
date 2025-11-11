using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;
using Xamarin.Forms;

namespace GACAppeal
{
    public class AppealDocument
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }

        public string FileId { get; set; }
        public string EvidenceTitle { get; set; }
        public string FileTypeID { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string DocumentType { get; set; }

        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
    }
    public class AppealDocumentDatabase
    {
        private SQLiteConnection conn;
        public AppealDocumentDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<AppealDocument>();
        }
        public IEnumerable<AppealDocument> FillAppealDocument(string _RegistrationYear, string _GrievanceID)
        {
            var list = conn.Query<AppealDocument>("Select * from AppealDocument where RegistrationYear = ? and GrievanceId = ?", new string[2] {_RegistrationYear,_GrievanceID });
            return list;
        }
        public IEnumerable<AppealDocument> GetAppealDocument(String Querryhere, string[] aray)
        {
            var list = conn.Query<AppealDocument>(Querryhere, aray);
            return list.ToList();
        }
        public string AddAppealDocument(AppealDocument service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteAppealDocument()
        {
            var del = conn.Query<AppealDocument>("delete from AppealDocument");
            return "success";
        }
        
        public string Custom(string query)
        {
            var Custm = conn.Query<AppealDocument>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<AppealDocument>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }
        
    }
}
