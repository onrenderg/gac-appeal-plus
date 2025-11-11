using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DigitalNagrik.Areas.Intermediary.Data
{
    public class IntermediaryReply
    {
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        public string IsPartialView { get; set; }
        //New

        public string GrievanceDesc { get; set; }
        public string HeaderText { get; set; }
        public string GrievanceStatus { get; set; }
        //public string ContentClassification { get; set; }
        //public string SubContentClassification { get; set; }
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
        public string GACTitle { get; set; }
        public string GroundTitle { get; set; }
        public string ReliefTitle { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string BriefofComplaint { get; set; }

        //New

        public string InputID { get; set; }
        public string InputSeekDateTime { get; set; }
        public string InputSeekRemarks { get; set; }
        [Required(ErrorMessage = "Enter Reply")]
        [RegularExpression(@"^[a-zA-Z0-9()\s\-/',.]+$", ErrorMessage = "Invalid Characters!")]

        public string IntermediaryReplyText { get; set; }
        public string isSupportingDocument { get; set; }
        public string FilePath { get; set; }
        public string IntermediaryDateTime { get; set; }

        //[Required(ErrorMessage = "Select .pdf document.")]
        //[ValidateFile("pdf", MaxFileSize: "1024", ErrorMessage = "Upload a valid {0} file of {1}!")]
        public HttpPostedFileBase SupportingDocument { get; set; }
    }
}