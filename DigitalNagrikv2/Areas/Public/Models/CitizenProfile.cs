using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class CitizenProfile
    {
        
       
        public string txtFirstName { get; set; }
        public string txtMiddleName { get; set; }
        public string txtLastName { get; set; }
        public string MobileNo { get; set; }
        public string UserId { get; set; }
        public string UserProfilleId { get; set; }
        public string UserEmail { get; set; }
        public string actiontype { get; set; }

        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string AadhaarVerificationStatus { get; set; }
       // public List<IntermediaryDetail> listIntermediaryDetail { get; set; }
    }
    
    public class StateMaster
    {
        public string StateCode { get; set; }
        public string StateName { get; set; }
    }
    public class DistrictMaster
    {
        public string StateCode { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
}