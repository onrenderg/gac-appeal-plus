using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DigitalNagrik.Models
{
    public class mHome
    {

        [Required(ErrorMessage = "Enter Email ID or Mobile No!")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Only 10 to 100 characters allowed in  User ID!")]
        public string UserID { get; set; }

        //[RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Please enter a valid mobile number!")]
        //[StringLength(10, MinimumLength = 10, ErrorMessage = "Please enter a valid mobile number!")]

        public string Captcha { get; set; }
        public string OTP { get; set; }

        public string Submitted { get; set; }
        public string Disposed { get; set; }

    }
    public class mHomeCitizenDetails
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public string MobileNo { get; set; }

    }
}