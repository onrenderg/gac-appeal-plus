using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class CitizenProfile
    {

        public string UserId { get; set; }
        public string UserProfilleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    
        public string UserMobile { get; set; }
        public string UserEmail { get; set; }
       
    }
    public class CitizenProfile_Post
    {
        public string UserMobile { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public string Address { get; set; }
        public string Tehsil { get; set; }
        public string DistrictId { get; set; }
        public string StateId { get; set; }
        public string PinCode { get; set; }
        public string Occupation { get; set; }
    }
}