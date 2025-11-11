using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    
    public class CitizenDashboard
    {
        
        public string UserMobile { get; set; }
        public string languageCode { get; set; }
        public string languageDescription { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string EmailOf { get; set; }
        public string ReliefSoughtID { get; set; }
        public string ReliefTitle { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string GroundAppealID { get; set; }
        public string GroundTitle { get; set; }
        public string Justification { get; set; }
       
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
        public string IntermediaryGROEmail { get; set; }
        public string IntermediaryAddress { get; set; }
        public string PlatformTypeId { get; set; }
        public string PlatformTypeTitle { get; set; }
        public string IntermediaryDate { get; set; }
        public string GrievnaceStatus { get; set; }
        public string StatusTitle { get; set; }
        public string ReceiptDate { get; set; }
        public string LastUpdatedOn { get; set; }

    }
}