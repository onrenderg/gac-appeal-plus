using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class CMSDataAddFAQs
    {
        public string FAQID { get; set; }
        [Required(ErrorMessage = "Enter Question")]
        [AllowHtml]
        public string Question { get; set; }
        [Required(ErrorMessage = "Enter Answer")]
        [AllowHtml]
        public string Answer { get; set; }
        public List<QAListTable_List> QAListTable { get; set; }
        [Required(ErrorMessage = "Select is Active")]
        public string isActive { get; set; }
        [Required(ErrorMessage = "Enter Display Order")]
        public string DisplayOrder { get; set; }
        public string Hyperlink { get; set; }
    }
    public class QAListTable_List
    {
        public string FAQID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string isActive { get; set; }
        public string DisplayOrder { get; set; }
        public string Hyperlink { get; set; }


    }
}