using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Areas.Public.Models
{
    public class CitizenDashboard
    {
        public string MobileNo { get; set; }
        public string ListHeaderText { get; set; }
        public CitizenProfile citizenProfile { get; set; }
        public List<mIntermediaryDetailPart> listIntermediaryDetail { get; set; }
        public CitizenDashboardCount citizenDashboardCount { get; set; }
        public string GrievanceStatus { get; set; }
        //public string AadhaarNo { get; set; }
        //public string AadhaarOTP { get; set; }
        //public string OTPFunc { get; set; }
        //public string otpTxnID { get; set; }
    }
    public class CitizenDashboardCount
    {
        public string Total { get; set; }
        public string Draft { get; set; }
        public string Submitted { get; set; }
        public string Rejected { get; set; }
        public string Referback { get; set; }
        public string Disposed { get; set; }
    }
}