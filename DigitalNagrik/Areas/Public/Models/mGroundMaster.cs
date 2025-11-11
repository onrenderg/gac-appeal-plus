using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DigitalNagrik.Areas.Public.Models
{
    public class mGroundMaster
    {
        public int GroundId { get; set; }
        public string GroundTitle { get; set; }
        public string CorrespondingITRule { get; set; }
        public string EntryRequired { get; set; }
        public string EntryFieldLabel { get; set; }
        public int listorder { get; set; }
        public string ExtractITRule { get; set; }
    }
}