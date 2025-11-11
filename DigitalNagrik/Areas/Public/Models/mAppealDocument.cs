using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class mAppealDocument
    {
        public string MobileNo { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string FileId { get; set; }
        public string DocumentTitle { get; set; }
        public string FileTypeID { get; set; }
        public string FilePath { get; set; }

        public string FileType { get; set; }

        public string DocumentType { get; set; }
        public string EvidenceType { get; set; }

        public string GrievnaceStatus { get; set; }
        public string StatusTitle { get; set; }
        public string ReceiptDate { get; set; }
        public string LastUpdatedOn { get; set; }
        public string actionDraftProceed { get; set; }
        public string UploadLimit { get; set; }

        public string IURL { get; set; }
        public HttpPostedFileBase fileUpload { get; set; }
    }
}