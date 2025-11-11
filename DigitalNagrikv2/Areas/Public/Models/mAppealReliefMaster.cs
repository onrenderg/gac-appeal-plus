using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DigitalNagrik.Areas.Public.Models
{
    public class mAppealReliefMaster
    {
        public string ReliefId { get; set; }
        public string ReliefTitle { get; set; }
        public string SpecificationLabel { get; set; }
    }
}