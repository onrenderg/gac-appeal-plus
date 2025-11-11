using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Intermediary.Data
{
    public class mIntermediaryReply
    {
        public List<IntermediaryReplyDetails> IntermediaryReplyList { get; set; }
    }

    public class IntermediaryReplyDetails
    {
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceDesc { get; set; }
        public string GrievanceStatus { get; set; }
        //public string ContentClassification { get; set; }
        //public string SubContentClassification { get; set; }
        public string ReasonForAppeal { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryType { get; set; }
        public string IntermediaryGROName { get; set; }
        public string IntermediaryAddress { get; set; }
        public string ReceiptDate { get; set; }
        public string IntermediaryReply { get; set; }
        public string GACTitle { get; set; }
        public string ReplyPendingTimeInMInutes { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserMobile { get; set; }
        public string UserEmail { get; set; }
    }

}