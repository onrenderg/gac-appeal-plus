using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Areas.Public.Models
{
    public class FileUpload
    {
        public string Mobile { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string FileType { get; set; }
        [AllowFileSize(FileSize = 1 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 1 MB")]
        public HttpPostedFileBase txtCopyofdecisionPDF { get; set; }
        [DataType(DataType.Upload, ErrorMessage = "Image File is not valid")]
        [AllowFileSize(FileSize = 1 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 1 MB")]
        public HttpPostedFileBase txtImage { get; set; }
        [DataType(DataType.Upload, ErrorMessage = "Voice File is not valid")]
        [AllowFileSize(FileSize = 2 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 2 MB")]
        public HttpPostedFileBase txtVoice { get; set; }
        [DataType(DataType.Upload, ErrorMessage = "Video File is not valid")]
        [AllowFileSize(FileSize = 10 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 10 MB")]
        public HttpPostedFileBase txtVideo { get; set; }

    }
}