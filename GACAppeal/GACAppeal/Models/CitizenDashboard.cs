using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;
using Xamarin.Forms;

namespace GACAppeal
{

    public class CitizenDashboard
    {
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserMobile { get; set; }
        public string UserEmail { get; set; }
        public string StatusId { get; set; }
        public string StatusTitle { get; set; }
        public string GrievanceDesc { get; set; }
        public string ReasonForAppeal { get; set; }
        public string IntermediaryURL { get; set; }
        public string UserId { get; set; }
        public string UserProfileId { get; set; }
        public string CaseHistoryFilePath { get; set; }
        public string CaseHistoryFileType { get; set; }
        public string IntermediaryId { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryGROName { get; set; }
        public string EmailOf { get; set; }
        public string IntermediaryGROEmail { get; set; }
        public string IntermediaryAddress { get; set; }
        public string IntermediaryDate { get; set; }
        public string GrievnaceStatus { get; set; }
        public string ReceiptDate { get; set; }
        public string LastUpdatedOn { get; set; }
        public string Justification { get; set; }
        public string ReliefSoughtID { get; set; }
        public string ReliefTitle { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string GroundAppealID { get; set; }
        public string GroundTitle { get; set; }
        public string ReceiptDateTime { get; set; }
        public string LastResponseTime { get; set; }
        public string GroundAppealLawText { get; set; }
        public string EntryFieldLabel { get; set; }
        public string SpecificationLabel { get; set; }

        public string Keyword { get; set; }
        public string BriefofComplaint { get; set; }
        public string dateofComplaint { get; set; }
        public string dateofDecision { get; set; }
        public string ComplianceURL { get; set; }
        public string DecisionFilePath { get; set; }

        public string AppealID { get; set; }

        public string cnt_total { get; set; }
        public string cnt_draft { get; set; }
        public string cnt_submitted { get; set; }
        public string cnt_rejected { get; set; }
        public string cnt_queryraised { get; set; }
        public string cnt_disposed { get; set; }
        public string ImageToDisplay { get; set; }

        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
    }
    public class CitizenDashboardDatabase
    {
        private SQLiteConnection conn;
        public CitizenDashboardDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<CitizenDashboard>();
        }
        public IEnumerable<CitizenDashboard> FillCitizenDashboard()
        {
            var list = conn.Query<CitizenDashboard>("Select * from CitizenDashboard");
            App.listCitizenDashboard = list;
            return list;
        }
        public IEnumerable<CitizenDashboard> FillCitizenDashboardByID(string id)
        {
            var list = conn.Query<CitizenDashboard>("Select * from CitizenDashboard where GrievanceId = ?", new string[1] { id});
            return list;
        }

        public IEnumerable<CitizenDashboard> GetCitizenDashboard(String Querryhere, string[] aray)
        {
            var list = conn.Query<CitizenDashboard>(Querryhere, aray);
            return list.ToList();
        }
        public string AddCitizenDashboard(CitizenDashboard service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteCitizenDashboard()
        {
            var del = conn.Query<CitizenDashboard>("delete from CitizenDashboard");
            return "success";
        }

        public string Custom(string query)
        {
            var Custm = conn.Query<CitizenDashboard>(query);
            return "success";
        }
        public string CustomForUpdate(string query, string[] aray)
        {
            try
            {
                var Custm = conn.Query<CitizenDashboard>(query, aray);
                return "success";
            }
            catch { return "failed"; }
        }

    }
}
