using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class mAppealComplianceDetail
    {
        public byte[] SupportingDocument = new byte[] { };
        public string ComplianceDate { get; set; }
        public string Remarks { get; set; }
    }
}