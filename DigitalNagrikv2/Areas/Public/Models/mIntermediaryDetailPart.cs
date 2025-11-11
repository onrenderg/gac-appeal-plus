using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Areas.Public.Models
{
    public class mIntermediaryDetailPart
    {
        [Required(ErrorMessage = "Please enter mobile")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Mobile Number")]
        public string MobileNo { get; set; }
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Enter valid name")]
        [Required(ErrorMessage = "Please enter first name ")]
        public string txtFirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Enter valid name")]
        public string txtMiddleName { get; set; }
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Enter valid name")]
        public string txtLastName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,12}|[0-9]{1,3})(\]?)$", ErrorMessage = "Entered valid email")]
        public string txtEmail { get; set; }

       // [Required(ErrorMessage = "Please select intermediary ")]
        public string txtIntermediary { get; set; }
        [Url(ErrorMessage = "Please enter valid respondent intermediary URL")]
        [RegularExpression(@"^((http|https|HTTP|HTTPS):\/\/.)[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-zA-Z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)$", ErrorMessage = "Please enter valid respondent intermediary URL")]
        public string txturlofintermediary { get; set; }
        //[Required(ErrorMessage = "Please Enter Email ")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please Enter Valid Email ")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,12}|[0-9]{1,3})(\]?)$", ErrorMessage = "Entered valid email")]
        public string GROIntermediaryEmail { get; set; }

        //[Required(ErrorMessage = "Please enter Justification for invoking the ground for the appeal")]
       [StringLength(4000)]
        public string txtJustification { get; set; }

        //[Required(ErrorMessage = "Please select Ground for the appeal")]
        public string ddlGroundAppeal { get; set; }

        //[Required(ErrorMessage = "Please Enter Ground Appeal Specification ")]
        public string GroundAppealLawText { get; set; }
        //[Required(ErrorMessage = "Please select relief sought ")]
        public string ddlReliefSought { get; set; }
        public string txtdateofDecision { get; set; }
        public string txtDateofComplaint { get; set; }
        public string BriefofComplaint { get; set; }
        public string Keyword { get; set; }
        public string ReliefSoughtTitle { get; set; }
        //[Required(ErrorMessage = "Please Enter Relief Specification ")]
        public string ReliefSoughtSpecification { get; set; }
        public string GroundTitle { get; set; }
        public string GroundTitleText { get; set; }

        public string txtCorrespondenceAddress { get; set; }
        public string IntermediaryId { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string HeaderText { get; set; } 
        public string GrievnaceStatus { get; set; }
        public string StatusTitle { get; set; }
        public string ReceiptDate { get; set; }
        public string LastUpdatedOn { get; set; }
        public string ReceiptDateTime { get; set; }
        public string LastResponseTime { get; set; }
        public string actionDraftProceed { get; set; }
        public string CitizenName { get; set; }
        public string EntryFieldLabel { get; set; }
        public string SpecificationLabel { get; set; }
        public string ComplianceURL { get; set; }
        public string DecisionFilePath { get; set; }
        public string SubmitDate { get; set; }

        
        [RegularExpression(@"^[a-zA-Z0-9.#()*&,{}\-\/ ]+$", ErrorMessage = "Ticket / Docket Number is not valid")]
        public string TicketDocketNumber { get; set; }

    }
}