using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Areas.Public.Models
{
    public class mPinLogin
    {

        [Required(ErrorMessage = "Please Enter Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Mobile Number")]
        public string txtMobile { get; set; }
        
        [Required(ErrorMessage = "Please enter captcha ")]
       // [RegularExpression("[^0-9]", ErrorMessage = "Captcha must be numeric")]
        public string Captcha { get; set; }

        public string Seed { get; set; }
        public string HashPwd { get; set; }

        [Required(ErrorMessage = "Invalid OTP")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Invalid OTP")]        
        public string OTP { get; set; }
        public string OTPID { get; set; }
        public string statusMessage { get; set; }

        public string Submitted { get; set; }
        public string Disposed { get; set; }
    }
}