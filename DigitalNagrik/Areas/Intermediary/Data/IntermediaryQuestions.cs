using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalNagrik.Areas.Intermediary.Data
{
    public class IntermediaryQuestions
    {
        public List<IntermediaryResponse> IntermediaryReposnseList { get; set; }
        public List<IntermediarySubResponse> IntermediarySubReposnseList { get; set; }

    }

    public class IntermediaryResponse
    {
        public string ResponseID { get; set; }
        public string ResponseText { get; set; }
        public string IsSubResponse { get; set; }
        public string ChangeOn { get; set; }
        public string IsMultipleSubResponse { get; set; }
        public string HasFiles { get; set; }
        public string MinNoOfFiles { get; set; }
        public string MaxNoOfFiles { get; set; }
        public string MaxFileSizeMb { get; set; }
        public string IsActive { get; set; }
                            
    }
    public class IntermediarySubResponse
    {
        public string ResponseID { get; set; }
        public string SubResponseID { get; set; }
        public string SubResponseText { get; set; }
        public string HasFiles { get; set; }
        public string MinNoOfFiles { get; set; }
        public string MaxNoOfFiles { get; set; }
        public string MaxFileSizeMb { get; set; }
        public string IsActive { get; set; }
    }
}