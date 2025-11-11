using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class NewUserRegistration
    {
        //[Required(ErrorMessage = "Please enter mobile or email")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered mobile is not valid.")]
        public string txtmobile { get; set; }
        [Required(ErrorMessage = "Please enter captacha ")]
        public string Captcha { get; set; }


        public string EmailorMobile { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,12}|[0-9]{1,3})(\]?)$", ErrorMessage = "Entered valid email")]
        public string txtEmail { get; set; }
    }
}