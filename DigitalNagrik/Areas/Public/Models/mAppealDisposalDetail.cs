using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class mAppealDisposalDetail
    {
        public byte[] LetterCopy = new byte[] { };
        public string Remarks { get; set; }
        public string DateOfDisposal { get; set; }
        public string DecisionDate { get; set; }
    }
}