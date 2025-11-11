using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using NICServiceAdaptor;
using DigitalNagrik.Models;
using DigitalNagrik.Areas.Intermediary.Data;

namespace DigitalNagrik.Areas.Intermediary.Controllers
{
    public class IntermediaryReplyController : Controller
    {
        // GET: Intermediary/IntermediaryReply
        public ActionResult Index()
        {
            mIntermediaryReply obj = new mIntermediaryReply();
            obj = GetGACAppealIntermediaryReplyList();
            return View(obj);
        }
        public mIntermediaryReply GetGACAppealIntermediaryReplyList()
        {
            mIntermediaryReply Data = new mIntermediaryReply();
            Data.IntermediaryReplyList = new List<IntermediaryReplyDetails>();
            var methodparameter = new List<KeyValuePair<string, string>>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AppealGACIntermediaryReplyList", methodparameter);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Data.IntermediaryReplyList.Add(new IntermediaryReplyDetails
                    {
                        GrievanceId = Convert.ToString(dr["GrievanceId"]),
                        GrievanceDesc = Convert.ToString(dr["GrievanceDesc"]),
                        RegistrationYear = Convert.ToString(dr["RegistrationYear"]),
                        GrievanceStatus = Convert.ToString(dr["GrievnaceStatus"]),
                        //ContentClassification = Convert.ToString(dr["ContentClassificationTitle"]),
                        //SubContentClassification = Convert.ToString(dr["SubContentClassificationTitle"]),
                        ReasonForAppeal = Convert.ToString(dr["ReasonForAppeal"]),
                        IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                        IntermediaryType = Convert.ToString(dr["IntermediaryType"]),
                        IntermediaryGROName = Convert.ToString(dr["IntermediaryGROName"]),
                        IntermediaryAddress = Convert.ToString(dr["IntermediaryAddress"]),
                        ReceiptDate = Convert.ToString(dr["ReceiptDate"]),
                        GACTitle = Convert.ToString(dr["GACTitle"]),
                        UserID = Convert.ToString(dr["UserId"]),
                        UserName = Convert.ToString(dr["UserName"]),
                        UserMobile = Convert.ToString(dr["UserMobile"]),
                        UserEmail = Convert.ToString(dr["UserEmail"]),
                        ReplyPendingTimeInMInutes = BALCommon.ConvertMinToDHM(Convert.ToString(dr["ReplyPendingTimeInMInutes"])),                    
                    });
                }
            }
            return Data;
        }

    }
}