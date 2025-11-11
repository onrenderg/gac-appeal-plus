using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class Helpline
    {
        public Helpline()
        {
            HelpSectionList = new List<SelectListItem>();
            ContentTypeList = new List<SelectListItem>();
        }

        public string ContentTypeID { get; set; }
        public List<SelectListItem> ContentTypeList { get; set; }
        public string HelpSectionID { get; set; }
        public List<SelectListItem> HelpSectionList { get; set; }
        public List<DocumnetListTable_List> DocumnetListTable { get; set; }

        public class DocumnetListTable_List
        {
            public string HelpLineID { get; set; }
            public string DeviceName { get; set; }
            public string ContentType { get; set; }
            public string Name { get; set; }
            public string OrderofAppearance { get; set; }
            public string URL { get; set; }
            public string DocID { get; set; }
        }
    }
}