using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Areas.Public.Models
{
    public class UserOTPVerfication
    {
        [Required(ErrorMessage = "Please enter Mobile OTP ")]
        public string txtMobileOTP { get; set; }

        [Required(ErrorMessage = "Please enter captcha ")]
        public string Captcha { get; set; }

        public string MobileNo { get; set; }
        public string Emailid { get; set; }
        public string OTPID { get; set; }
        public string IntermediaryDate { get; set; }
        public string OTPSent { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceID { get; set; }
        public string EmailorMobile { get; set; }
        public string Msg { get; set; }
        public string OTP { get; set; }
        

    }
}