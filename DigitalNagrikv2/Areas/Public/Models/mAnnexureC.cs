using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class mAnnexureC
    {
        public string Sequence { get; set; }
        public string Section { get; set; }
        public string ExtractITRule { get; set; }
        public string RelatedSubjectEntry { get; set; }
        public string ConcernedMinistry { get; set; }
    }
    public class ListAnnexureC
    { 
    public List<mAnnexureC> list { get; set; }
    }
}