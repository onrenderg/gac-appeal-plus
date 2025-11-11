using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class mGACAppeal
    {
        public List<AppealDetails> GACAppealList { get; set; }
    }
    public class AppealDetails
    {
        public string GrievanceId { get; set; }
        public string GrievanceDesc { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceStatus { get; set; }
        public string ContentClassification { get; set; }
        public string SubContentClassification { get; set; }
        public string ReasonForAppeal { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryType { get; set; }
        public string IntermediaryGROName { get; set; }
        public string IntermediaryAddress { get; set; }
        public string ReceiptDate { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserMobile { get; set; }
        public string UserEmail { get; set; }
        public string ActionRemarks { get; set; }
        public string AssignedTOGACDetails { get; set; }
        public string AssignmentDateTime { get; set; }
        public string GACTitle { get; set; }
        public string AssignOrNotStatus { get; set; }
    }
    public class AppealAction
    {
        public string GrievanceId { get; set; }
        public string GrievanceDesc { get; set; }
        public string RegistrationYear { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryType { get; set; }
        public string ContentClassification { get; set; }
        public string SubContentClassification { get; set; }
        public List<SelectListItem> GAC_ActionList { get; set; }

        [Required(ErrorMessage = "Select Action!")]
        public string ActionID { get; set; }
        public string ActionName { get; set; }

        [Required(ErrorMessage = "Enter Remarks!")]
        public string Remarks { get; set; }
        public List<SelectListItem> GACUsersForAssignment { get; set; }

        [Required(ErrorMessage = "Select GAC!")]
        public string GACId { get; set; }
        public string GACTitle { get; set; }
        public string GACAbbr { get; set; }
        public string GACDetail { get; set; }

        public string ReceiptDate { get; set; }

    }
    public class AppealTrackDetModl
    {
        public List<AppealTrackDet> AppealTrackDetlst { get; set; }
    }
    public class AppealTrackDet
    {
        public string RegYear { get; set; }
        public string GrieveID { get; set; }
        public string GrievanceDesc { get; set; }
        public string ActionID { get; set; }
        public string ActionTitle { get; set; }
        public string Remarks { get; set; }
        public string ActionBy { get; set; }
        public string ActionDate { get; set; }
        public string ActionTime { get; set; }
        public string ActionIp { get; set; }

        public string Actionlvl { get; set; }
        public string GacAbbr { get; set; }
        public string GacTitle { get; set; }
        public string AssignedTo { get; set; }
        public string RejectReason { get; set; }
        public string RejectSummary { get; set; }

    }
    public class mGACAppealSearch
    {
        public mGACAppealSearch()
        {
            RegistrationYearList = new List<SelectListItem>();
            GACList = new List<SelectListItem>();
            ContentClassificationList = new List<SelectListItem>();
            SubContentClassificationList = new List<SelectListItem>();
            IntermediaryList = new List<SelectListItem>();
            GACAppealList = new List<AppealDetails>();
        }

        [Required(ErrorMessage = "Select Registration Year!")]
        public string RegistrationYearID { get; set; }
        public string RegistrationYear { get; set; }
        public List<SelectListItem> RegistrationYearList { get; set; }

        [Required(ErrorMessage = "Select GAC!")]
        public string GACID { get; set; }
        public string GACTitle { get; set; }
        public List<SelectListItem> GACList { get; set; }
        public string ContentClassificationID { get; set; }
        public string ContentClassificationTitle { get; set; }
        public List<SelectListItem> ContentClassificationList { get; set; }
        public string SubContentClassificationID { get; set; }
        public string SubContentClassificationTitle { get; set; }
        public List<SelectListItem> SubContentClassificationList { get; set; }
        public string IntermediaryID { get; set; }
        public string IntermediaryTitle { get; set; }
        public List<SelectListItem> IntermediaryList { get; set; }
        public string SearchText { get; set; }
        public List<AppealDetails> GACAppealList { get; set; }

        public string Interval { get; set; }
        public List<SelectListItem> IntervalList { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }

}