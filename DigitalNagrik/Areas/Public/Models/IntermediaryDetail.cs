using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Areas.Public.Models
{
    public class IntermediaryDetail
    {
        public string MobileNo { get; set; }
       
        [Required(ErrorMessage = "Please enter intermediary ")]
        public string txtIntermediary { get; set; }

        //[Required(ErrorMessage = "Please enter platform type ")]
        public string ddlplatform { get; set; }
        [Required(ErrorMessage = "Please enter URL of website/media/image/app ")]
        public string txturlofintermediary { get; set; }

        //[Required(ErrorMessage = "Please Enter Name of Grievance Officer of Intermediary ")]
        public string txtNameofGROName { get; set; }

       // [Required(ErrorMessage = "Please Enter Email of Grievance Officer ")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please Enter Email of Grievance Officer ")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,12}|[0-9]{1,3})(\]?)$", ErrorMessage = "Entered valid email")]
        public string txtNameofGROEmail { get; set; }

        [Required(ErrorMessage = "Please Enter Intermediary Decision Date ")]
        public string txtIntermediaryDecisionDate { get; set; }
        //[Required(ErrorMessage = "Please select Category of Content ")]
        //public string ddlCategoryofContent { get; set; }
        //[Required(ErrorMessage = "Please select Sub Category of Content ")]
        //public string ddlsubCategoryofContent { get; set; }
        
        public string txtCorrespondenceAddress { get; set; }
        //[Required(ErrorMessage = "Please select Copy of decision")]
        [AllowFileSize(FileSize = 1 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 1 MB")]
        public HttpPostedFileBase txtCopyofdecisionPDF { get; set; }
        //[Required(ErrorMessage = "Please enter Brief description ")]
        public string txtBriefdescription { get; set; }

        //[Required(ErrorMessage = "Please enter Reason for filling ")]
        public string txtReasonforfilling { get; set; }
        [DataType(DataType.ImageUrl, ErrorMessage = "URL is not valid")]
        public string txtURLimage { get; set; }
        [DataType(DataType.Url, ErrorMessage = "URL is not valid")]
        public string txtURLVoiceNote { get; set; }
        [DataType(DataType.Url, ErrorMessage = "URL is not valid")]
        public string txtURLVideo { get; set; }
        [DataType(DataType.Upload, ErrorMessage = "Image File is not valid")]
        [AllowFileSize(FileSize = 1 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 1 MB")]
        public HttpPostedFileBase txtImage { get; set; }
        [DataType(DataType.Upload, ErrorMessage = "Voice File is not valid")]
        [AllowFileSize(FileSize = 2 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 2 MB")]
        public HttpPostedFileBase txtVoice { get; set; }
        [DataType(DataType.Upload, ErrorMessage = "Video File is not valid")]
        [AllowFileSize(FileSize = 10 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 10 MB")]
        public HttpPostedFileBase txtVideo { get; set; }

        [Required(ErrorMessage = "Please select self declaration ")]
        public bool declaration { get; set; }

        [Required(ErrorMessage = "Please select language ")]
        public string languageCode { get; set; }

        public string languageDescription { get; set; }

        public string actionDraftProceed { get; set; }
        public string hasImage { get; set; }
        public string hasVoice { get; set; }
        public string hasAudio { get; set; }

    
        public string IntermediaryId { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string CaseHistoryFilePath { get; set; }
        public string CaseHistoryFileType { get; set; }
        public string GrievnaceStatus { get; set; }
        public string StatusTitle { get; set; }
        public string ReceiptDate { get; set; }
        public string LastUpdatedOn { get; set; }
        public string CitizenName { get; set; }
        public string EmailID { get; set; }
        public string PinCode { get; set; }
        public string Address { get; set; }
        public string Tehsil { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string IntermediaryTypeDesc { get; set; }
        public string PlatformTypeTitle { get; set; }
        //public string ContentClassificationTitle { get; set; }
        //public string SubContentClassificationTitle { get; set; }

        public List<mAppealDocument> appealDocumentList  {get; set;}
        public List<mAppealAction> listAppealAction { get; set; }



    }
   
    public class IntermediaryMaster
    {
        public string IntermediaryId { get; set; }
        public string IntermediaryTitle { get; set; }
        public string URL { get; set; }
        public string Address { get; set; }
        public string GOEmail { get; set; }
        public string GOName { get; set; }
        public string IsActive { get; set; }
        public string HelpLink { get; set; }
    }
   
}