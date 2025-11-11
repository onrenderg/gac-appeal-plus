using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class FAQs
    {
        public List<FAQ> FAQList { get; set; }


    }
    public class FAQ
    {
        public FAQ(int sr, string ques, string ans,string hyperlink)
        {
            SrNo = sr;
            Question = ques;
            Answer = ans;
            Hyperlink = hyperlink;
        }
        public int SrNo { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Hyperlink { get; set; }

    }
}