using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Please enter a valid login ID.")]
        public string UserID { get; set; }
    }
}