using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class LoginviaPassword
    {
        [Required(ErrorMessage = "Enter User Id")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Only 5 to 100 characters allowed in Email Address!")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Invalid Characters in Email Address!")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Enter Captcha")]
        public string Captcha { get; set; }
        public string HashPassword { get; set; }
        public string Seed { get; set; }
    }
}