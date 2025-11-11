using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Models
{
    public class Generic_Responce
    {
        public int status_code { get; set; } = 500;
        public string Message { get; set; } = "Something went Wrong\nPlease try again later";
        public string developer_message { get; set; } = "Web Service is in Exception";
        public object error_list { get; set; }
        public object data { get; set; }
    }
}