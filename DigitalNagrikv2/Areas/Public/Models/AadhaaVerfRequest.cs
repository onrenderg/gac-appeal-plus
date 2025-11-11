using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class AadhaaVerfRequest
    {
        public string aadhaarNumber { get; set; }
        public string password { get; set; }
        public string txnRequestID { get; set; }
        //public string otpMedium { get; set; }
    }

    public class FinalRequest
    {
        public string serviceId { get; set; }
        public string data { get; set; }
    }
    public class Responseaadhar    {
        public string reqTransactionID { get; set; }
        public string status { get; set; }
        public string otpTransactionID { get; set; }
        public string errorCode { get; set; }
    }
    public class AadhaaOTPRequest
    {
        public string aadhaarNumber { get; set; }
        public string password { get; set; }
        public string txnRequestID { get; set; }
        public string otpTxnID { get; set; }
        public string otp { get; set; }
    }
}