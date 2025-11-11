using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class UploadDocument
    {

        public string UserEmailMobile { get; set; }
        public string EmailorMobile { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string FileType { get; set; }
        public string DocumentType { get; set; }
        [Required(ErrorMessage = "Please select file")]
        public HttpPostedFileBase fileUpload { get; set; }
        [Required(ErrorMessage = "Please enter URL")]
        public string IURL { get; set; }
    }
}