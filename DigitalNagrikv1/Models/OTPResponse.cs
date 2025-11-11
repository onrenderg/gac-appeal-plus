using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    //Input
    public class _GenerateOTP
    {
        public string MobileNo { get; set; }
        
    }
    //Output
    public class GenerateOTP_
    {
        public int otp_id { get; set; }
        public int otp_value { get; set; }
        public string SMS_Response { get; set; }

    }
    //PUT: INPUT

    public class _ValidateOTP
    {
        public string MobileNo { get; set; }
        public string MobileNoOTPID { get; set; }
        public string txtMobileOTP { get; set; }
    }
    public class _ValidateOTP_Return
    {
        public string UserID { get; set; }
        public string UserProfileID { get; set; }
    }
}