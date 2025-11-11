using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.Intermediary.Data
{
    public class GACApplicationAssigned
    {
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        public string UserID { get; set; }
        public string LinkExpired { get; set; }
        public string OpenInModal { get; set; }
        public string HeaderText { get; set; }
        public string FormName { get; set; }
        public string IsPartialView { get; set; }
        public string ResponseText { get; set; }
        //[Required(ErrorMessage = "Select .pdf !")]
        //[ValidateFile("jpg|jpeg|png", MaxFileSize: "10000", ErrorMessage = "Upload a valid {0} file of {1}!")]
        public HttpPostedFileBase DocumenttoUpload { get; set; }
        public List<GACAppealAssignedList> GACAppealAssigned { get; set; }
        public List<IntermediaryResponseMasterList> IntermediaryResponseMaster { get; set; }
        public List<IntermediarySubResponseMasterList> IntermediarySubResponseMaster { get; set; }

    }
    public class GACAppealAssignedList
    {
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        //New
        public string GrievanceDesc { get; set; }
        public string GrievanceStatus { get; set; }
        //public string ContentClassification { get; set; }
        //public string SubContentClassification { get; set; }
        public string ReasonForAppeal { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryType { get; set; }
        public string IntermediaryGROName { get; set; }
        public string IntermediaryAddress { get; set; }
        public string ReceiptDate { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserMobile { get; set; }
        public string UserEmail { get; set; }
        public string GACTitle { get; set; }
        public string isResponse { get; set; }
        public string GroundTitle { get; set; }
        public string ReliefTitle { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public string BriefofComplaint { get; set; }
        //New
    }
    public class IntermediaryResponseMasterList
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
    public class IntermediarySubResponseMasterList
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
    public class IntermediaryResponseDownloadPDF
    {
        public string GrievanceId { get; set; }
        public string RegistrationYear { get; set; }
        public string GrievanceDesc { get; set; }
        //public string ContentClassification { get; set; }
        //public string SubContentClassification { get; set; }
        public string IntermediaryTitle { get; set; }
        public string IntermediaryType { get; set; }
        public string ReceiptDate { get; set; }
        public string GACTitle { get; set; }
        public string GroundTitle { get; set; }
        public string ReliefTitle { get; set; }
        public string BriefofComplaint { get; set; }
        public string RelieftSoughtSpecification { get; set; }
        public List<IntermediaryResponseDownloadPDFResponse_List> IntermediaryResponseDownloadPDFResponse { get; set; }
        public List<IntermediaryResponseDownloadPDFsubResponse_List> IntermediaryResponseDownloadPDFsubResponse { get; set; }
        public List<IntermediaryResponseDownloadPDFResponseFiles_List> IntermediaryResponseDownloadPDFResponseFiles { get; set; }
        public List<IntermediaryResponseDownloadPDFsubResponseFiles_List> IntermediaryResponseDownloadPDFsubResponseFiles { get; set; }
    }
    public class IntermediaryResponseDownloadPDFResponse_List
    {
        public string GrievanceID { get; set; }
        public string RegistrationYear { get; set; }
        public string ResponseID { get; set; }
        public string ResponseText { get; set; }
        public string ResponseValue { get; set; }
        public string ResponseDetails { get; set; }
        public string ResponseUrls { get; set; }
        public string IntermediaryReplyDateTime { get; set; }

    }
    public class IntermediaryResponseDownloadPDFsubResponse_List
    {
        public string GrievanceID { get; set; }
        public string RegistrationYear { get; set; }
        public string ResponseID { get; set; }
        public string SubResponseID { get; set; }
        public string SubResponseText { get; set; }
        public string SubResponseDetails { get; set; }

    }
    public class IntermediaryResponseDownloadPDFResponseFiles_List
    {

        public string GrievanceID { get; set; }
        public string RegistrationYear { get; set; }
        public string ResponseID { get; set; }
        public string FileID { get; set; }
        public string File { get; set; }

    }
    public class IntermediaryResponseDownloadPDFsubResponseFiles_List
    {
        public string GrievanceID { get; set; }
        public string RegistrationYear { get; set; }
        public string ResponseID { get; set; }
        public string SubResponseID { get; set; }
        public string FileID { get; set; }
        public string File { get; set; }

    }
}