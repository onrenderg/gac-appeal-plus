using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;


namespace DigitalNagrik.Models
{
    public class CMSDataAddDocument
    {
        public CMSDataAddDocument()
        {
            DocumentSelectionList = new List<SelectListItem>();
            DocumentSortByList = new List<SelectListItem>();
            ContentTypeList = new List<SelectListItem>();
        }
        public string DocID { get; set; }

        [Required(ErrorMessage = "Select Content Type!")]
        //[StringLength(100, MinimumLength = 0, ErrorMessage = "Only 100 characters allowed!")]
        public string ContentType { get; set; }
        [Required(ErrorMessage = "Select Content Type")]
        public List<SelectListItem> ContentTypeList { get; set; }

        public string DocumentSelectionID { get; set; }
        public string DocumentSelectionNm { get; set; }
        public List<SelectListItem> DocumentSelectionList { get; set; }

        public string DocumentSortByListID { get; set; }
        public string DocumentSortByListNm { get; set; }
        public List<SelectListItem> DocumentSortByList { get; set; }

        [Required(ErrorMessage = "Document Name")]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "Only 1000 characters allowed!")]
        public string DocumentName { get; set; }

        [Required(ErrorMessage = "Select .pdf !")]
        //[ValidateFile("jpg|jpeg|png", MaxFileSize: "10000", ErrorMessage = "Upload a valid {0} file of {1}!")]
        public HttpPostedFileBase DocumenttoUpload { get; set; }

        [Required(ErrorMessage = "Enter Document Validity")]
        public string DocumentValidity { get; set; }
        [Required(ErrorMessage = "Enter Document Date")]
        public string DocumentDate { get; set; }
 
        public string ShowNewInfographics { get; set; }
        public string OpeninNewTab { get; set; }
        [Required(ErrorMessage = "Enter Link Address")]
        public string LinkAddress { get; set; }
        public string DisplayPref { get; set; }

        public List<DocumnetListTable_List> DocumnetListTable { get; set; }

        public class DocumnetListTable_List
        {
            public string DocID { get; set; }
            public string DocumentType { get; set; }
            public string DocumentName_link { get; set; }
            public string PDFlink_URLLink { get; set; }
            public string Validity { get; set; }
            public string ShownewInfographics { get; set; }
            public string inNewTab { get; set; }
            public string IsActive { get; set; }
            public string DocSectionID { get; set; }
            public string DocOrLinkDate { get; set; }
            public string DisplayPref { get; set; }

        }
    }
}