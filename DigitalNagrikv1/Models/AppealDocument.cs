using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class AppealDocument
    {
        
        public string RegistrationYear { get; set; }
        public string GrievanceId { get; set; }
        public string FileId { get; set; }
        public string DocumentTitle { get; set; }
        public string FileTypeID { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string DocumentType { get; set; }
        public string EvidenceType { get; set; }
    }
    public class AppealDocument_Post
    {
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string File_ID { get; set; }
        public string FilePath { get; set; }
        public string DocumentType { get; set; }
    }
    public class AppealDocument_Delete
    {
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string File_ID { get; set; }

    }
}