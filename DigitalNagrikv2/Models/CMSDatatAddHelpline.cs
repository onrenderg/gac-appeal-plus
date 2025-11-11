using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class CMSDatatAddHelpline
    {
        public CMSDatatAddHelpline()
        {
            DeviceNameList = new List<SelectListItem>();
            ContentTypeList = new List<SelectListItem>();
        }
        public string HelplineID { get; set; }

        [Required(ErrorMessage = "Select Content Type!")]
        //[StringLength(100, MinimumLength = 0, ErrorMessage = "Only 100 characters allowed!")]
        public string ContentType { get; set; }
        [Required(ErrorMessage = "Select Content Type")]
        public List<SelectListItem> ContentTypeList { get; set; }
        [Required(ErrorMessage = "Select Content For")]
        public string DeviceNameID { get; set; }
        public string DeviceNameName { get; set; }
        [Required(ErrorMessage = "Select Content For")]
        public List<SelectListItem> DeviceNameList { get; set; }
        [Required(ErrorMessage = "Document Name")]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "Only 1000 characters allowed!")]
        public string DocumentName { get; set; }
        [Required(ErrorMessage = "Select .pdf !")]
        //[ValidateFile("jpg|jpeg|png", MaxFileSize: "10000", ErrorMessage = "Upload a valid {0} file of {1}!")]
        public HttpPostedFileBase DocumenttoUpload { get; set; }
        [Required(ErrorMessage = "Enter Display Order")]
        public string DisplayOrder { get; set; }
        public string Enabled { get; set; }
        [Required(ErrorMessage = "Enter Link Address")]
        public string LinkAddress { get; set; }
        public string DocID { get; set; }
        public List<DocumnetListTable_List> DocumnetListTable { get; set; }
        public class DocumnetListTable_List
        {
            public string HelpLineID { get; set; }
            public string DocumentType { get; set; }
            public string DocumentName_link { get; set; }
            public string PDFlink_URLLink { get; set; }
            public string DeviceName { get; set; }
            public string IsActive { get; set; }
            public string DisplayOrder { get; set; }
            public string DocID { get; set; }
        }
    }
}