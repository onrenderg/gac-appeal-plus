using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class Appeal
    {
        public string UserMobile { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        //public string ContentClassificationID { get; set; }
        //public string SubContentClassificationID { get; set; }
        public string GrievanceDesc { get; set; }
        public string ReasonForAppeal { get; set; }
        public string IntermediaryId { get; set; }
        public string IntermediaryGROName { get; set; }
        public string EmailOf { get; set; }
        public string IntermediaryGROEmail { get; set; }
        public string IntermediaryAddress { get; set; }
        public string PlatformTypeId { get; set; }
        public string IntermediaryDate { get; set; }
        public string GrievnaceStatus { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryURL { get; set; }
        public string languageCode { get; set; }
        public string Justification { get; set; }
        public string ReliefSoughtID { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string GroundAppealID { get; set; }
    }
    public class Appeal_Post
    {
        public string UserMobile { get; set; }
        public string UserID { get; set; }
        public string UserProfileID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
    }
    public class Appeal_Post_return
    {
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
    }
    public class Appeal_FinalSubmit
    {
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string UserMobile { get; set; }
        public string IPAddress { get; set; }
    }
}