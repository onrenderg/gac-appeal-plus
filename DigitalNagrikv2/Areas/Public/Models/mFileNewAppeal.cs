using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DigitalNagrik.Areas.Public.Models
{
    public class mFileNewAppeal
    {
        [Required(ErrorMessage = "Please select option have you already approached the concerned Intermediary ")]
        public string rbtnApproached { get; set; }

        [Required(ErrorMessage = "Please select option is the grievance sub judice ")]
        public string rbtnsubjudice { get; set; }

        [Required(ErrorMessage = "Please enter date")]
        public string txtdatecomm { get; set; }
        public string minDate { get; set; }
        public string maxDate { get; set; }
        public List<IntermediaryDetail> listIntermediaryDetail { get; set; }
    }
}