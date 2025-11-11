using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Areas.Public.Models
{
    public class DocumentFileTypeMaster
    {
        public string AppealDocumentType { get; set; }
        public string FileTypeId { get; set; }
        public string FileTypeTitle { get; set; }
        public string FileExtension { get; set; }
        public string MaxFileSize { get; set; }
        public string UploadLimit { get; set; }

    }
}