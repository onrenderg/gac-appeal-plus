using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Models
{
    public class CommonLogin
    {
        public string EncryptedID { get; set; }
        public string OTPMobile { get; set; }

        [Required(ErrorMessage = "Please accept Privacy Policy and Terms of Service")] 
        [MustBeTrue(ErrorMessage = "Please accept Privacy Policy and Terms of Service")]
        public bool  PolicyCheck { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Mobile Number")]
        public string AppellantMobile { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Mobile Number")]
        public string ExpertMobile { get; set; }

        [Required(ErrorMessage = "Enter Email ID")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Only 5 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Email Address!")]
        public string AppellantEmailID { get; set; }

        [Required(ErrorMessage = "Enter Email ID")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Only 5 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Email Address!")]
        public string InterEmailID { get; set; }

        [Required(ErrorMessage = "Enter Email ID")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Only 5 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Email Address!")]
        public string GACEmailID { get; set; }

        [Required(ErrorMessage = "Please enter captcha ")]
        public string Captcha { get; set; }
        [Required(ErrorMessage = "Please enter captcha ")]
        public string IntCaptcha { get; set; }

        [Required(ErrorMessage = "Please enter captcha ")]
        public string GACCaptcha { get; set; }

        [Required(ErrorMessage = "Please enter captcha ")]
        public string MExpCaptcha { get; set; }
        public string Seed { get; set; }
        public string HashPwd { get; set; }

        [Required(ErrorMessage = "Invalid OTP")]
        //[RegularExpression("", ErrorMessage = "Invalid OTP")]
        public string OTP { get; set; }
        public string OTPEnc { get; set; }
        public string OTPID { get; set; }
        public string statusMessage { get; set; }
        public string Submitted { get; set; }
        public string Disposed { get; set; }
        // new Fields
        public string hdActionAppellant { get; set; }
        public string hdActionInter { get; set; }
        public string hdActionGACUser { get; set; }
        public string hdExpertMember { get; set; }

        [Required(ErrorMessage = "Please enter valid aadhaar number ")]
        public string AadhaarNo { get; set; }
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Please enter valid name as in aadhaar")]
        [Required(ErrorMessage = "Please enter valid name as in aadhaar ")]
        public string AadhaarName { get; set; }
        [Required(ErrorMessage = "Invalid OTP")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Invalid OTP")]
        public string AadhaarOTP { get; set; }

        public string AadhaarNoEnc { get; set; }

        public string AadhaarOTPEnc { get; set; }
        public string OTPFunc { get; set; }
        public string otpTxnID { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select OTP / Password ")]
        public string IntLoginMethod { get; set; }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class MustBeTrueAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                return value != null && value is bool && (bool)value;
            }
        }
    }
}