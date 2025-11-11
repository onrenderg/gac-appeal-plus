using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class GACAppealsAssigned
    {
        public List<GACAppealsAssignedDetails> GACAppealsAssignedList { get; set; }
        public List<GACAppealsAssignedMarkedforDispoal> GACAppealsMarkedForDispoal { get; set; }
        public string StatusMsg { get; set; }
        public string FIleExt { get; set; }

    }

    public class GACAppealsAssignedDetails
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
        public string GACTitle { get; set; }
        public string AppealCommunicationLogStatus { get; set; }
        public string AppealCommunicationName { get; set; }
    }

    public class GACAppealAssignedAction
    {
        public string GrievanceId { get; set; }
        public string GrievanceDesc { get; set; }
        public string RegistrationYear { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryType { get; set; }
        public string ContentClassification { get; set; }
        public string SubContentClassification { get; set; }
        public string RemarksMeity { get; set; }
        public string GACTitle { get; set; }
        public string ReceiptDate { get; set; }
        public List<SelectListItem> GAC_ActionList { get; set; }
        public List<SelectListItem> GAC_AsssignedForwardList { get; set; }
        [Required(ErrorMessage = "Select Action!")]
        public string ActionID { get; set; }

        [Required(ErrorMessage = "Select Forwarded To!")]
        public string ForwardedToID { get; set; }
        [Required(ErrorMessage = "Enter Name!")]
        public string EMName { get; set; }
        [Required(ErrorMessage = "Enter Email ID!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Characters in Email Address!")]
        public string EMEmailID { get; set; }
        [Required(ErrorMessage = "Enter Mobile No!")]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Please enter a valid mobile number!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Please enter a valid mobile number!")]
        public string EMMobileNo { get; set; }
        [Required(ErrorMessage = "Enter Area of Expertise!")]
        public string EMAreaofExpertise { get; set; }

        [Required(ErrorMessage = "Enter Remarks!")]
        public string Remarks { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Enter Mail Content!")]
        public string MailContent { get; set; }
    }

    public class GACAppealsAssignedMarkedforDispoal
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
        public string GACTitle { get; set; }
        public string DateOfDisposal { get; set; }
        public string ActionID { get; set; }
        public string ActionDesc { get; set; }
        public string isLetterUploaded { get; set; }
        public string isThisUserMarkDisposal { get; set; }
    }

    public class GACAppealsGenerateLetter
    {
        //public List<SelectListItem> AnnexureTypeList { get; set; }
        public string ActionID { get; set; }
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        [AllowHtml]
        public string LetterText { get; set; }
    }
    public class GACAppealsUploadLetter
    {
        //public List<SelectListItem> AnnexureTypeList { get; set; }      
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }

        [Required(ErrorMessage = "Select .pdf document.")]
        //[ValidateFile("pdf", MaxFileSize: "1024", ErrorMessage = "Upload a valid {0} file of {1}!")]
        public HttpPostedFileBase UploadPDF { get; set; }
    }

    public class GACAppealsUsersForEmail
    {
        public GACAppealsUsersForEmail()
        {
            GACAppealsUsers = new List<GACAppealsUsersForEmail>();
        }
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public List<GACAppealsUsersForEmail> GACAppealsUsers { get; set; }

    }

}