using DigitalNagrik.Areas.Intermediary.Data;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.Intermediary.Controllers
{
    public class IntermediaryResponseQuestionController : Controller
    {
        // GET: Intermediary/IntermediaryResponseQuestion
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IntermediaryQuestionsView()
        {
            IntermediaryQuestions obj = new IntermediaryQuestions();
            obj.IntermediaryReposnseList = new List<IntermediaryResponse>();
            obj.IntermediarySubReposnseList = new List<IntermediarySubResponse>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                {
                    //methodparameter.Add(new KeyValuePair<string, string>("@GrievanceId", GrievanceId));
                    //methodparameter.Add(new KeyValuePair<string, string>("@RegistrationYear", RegistrationYear));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_Questions]", methodparameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.IntermediaryReposnseList.Add(new IntermediaryResponse
                            {
                                ChangeOn = Convert.ToString(dr["ChangeOn"]),
                                HasFiles = Convert.ToString(dr["HasFiles"]),
                                IsActive = Convert.ToString(dr["IsActive"]),
                                IsMultipleSubResponse = Convert.ToString(dr["IsMultipleSubResponse"]),
                                IsSubResponse = Convert.ToString(dr["IsSubResponse"]),
                                MaxFileSizeMb = Convert.ToString(dr["MaxFileSizeMb"]),
                                MaxNoOfFiles = Convert.ToString(dr["MaxNoOfFiles"]),
                                MinNoOfFiles = Convert.ToString(dr["MinNoOfFiles"]),
                                ResponseID = Convert.ToString(dr["ResponseID"]),
                                ResponseText = Convert.ToString(dr["ResponseText"])
                            });
                        }

                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            obj.IntermediarySubReposnseList.Add(new IntermediarySubResponse
                            {
                                MinNoOfFiles = Convert.ToString(dr["MinNoOfFiles"]),
                                MaxNoOfFiles = Convert.ToString(dr["MaxNoOfFiles"]),
                                MaxFileSizeMb = Convert.ToString(dr["MaxFileSizeMb"]),
                                ResponseID = Convert.ToString(dr["ResponseID"]),
                                SubResponseID = Convert.ToString(dr["SubResponseID"]),
                                SubResponseText = Convert.ToString(dr["SubResponseText"]),
                                IsActive = Convert.ToString(dr["IsActive"]),
                                HasFiles = Convert.ToString(dr["HasFiles"])

                            });
                        }

                    }
                }
            }

            catch (Exception ex)
            {

            }
            return View("IntermediaryQuestionsView", obj);
        }
        public ActionResult AddUpdateResponseShowPopUp(string ActionType, string ResponseID)
        {
            IntermediaryResponse obj = new IntermediaryResponse();
            obj = AddUpdateResponseShowPopUpData(ResponseID);
            return PartialView("PartialViews/_AddUpdateResponsePV", obj);
        }
        public IntermediaryResponse AddUpdateResponseShowPopUpData(string ResponseID)
        {
            IntermediaryResponse obj = new IntermediaryResponse();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@ResponseID", ResponseID));
                //if (UserSession.MemberID != null || UserSession.MemberID != "")
                //{
                //    methodparameter.Add(new KeyValuePair<string, string>("@MemberID", UserSession.UserID));
                //}
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_Questions", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.ChangeOn = Convert.ToString(ds.Tables[0].Rows[0]["ChangeOn"]);
                        obj.HasFiles = Convert.ToString(ds.Tables[0].Rows[0]["HasFiles"]);
                        obj.IsActive = Convert.ToString(ds.Tables[0].Rows[0]["IsActive"]);
                        obj.IsMultipleSubResponse = Convert.ToString(ds.Tables[0].Rows[0]["IsMultipleSubResponse"]);
                        obj.IsSubResponse = Convert.ToString(ds.Tables[0].Rows[0]["IsSubResponse"]);
                        obj.MaxFileSizeMb = Convert.ToString(ds.Tables[0].Rows[0]["MaxFileSizeMb"]);
                        obj.MaxNoOfFiles = Convert.ToString(ds.Tables[0].Rows[0]["MaxNoOfFiles"]);
                        obj.MinNoOfFiles = Convert.ToString(ds.Tables[0].Rows[0]["MinNoOfFiles"]);
                        obj.ResponseID = Convert.ToString(ds.Tables[0].Rows[0]["ResponseID"]);
                        obj.ResponseText = Convert.ToString(ds.Tables[0].Rows[0]["ResponseText"]);
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        public ActionResult AddUpdateSubResponseShowPopUp(string ActionType, string ResponseID, string SubResponseID)
        {
            IntermediarySubResponse obj = new IntermediarySubResponse();
            obj = AddUpdateSubResponseShowPopUpData(ResponseID, SubResponseID);
            return PartialView("PartialViews/_AddUpdateSubResponsePV", obj);
        }
        public IntermediarySubResponse AddUpdateSubResponseShowPopUpData(string ResponseID, string SubResponseID)
        {
            IntermediarySubResponse obj = new IntermediarySubResponse();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@ResponseID", ResponseID));
                methodparameter.Add(new KeyValuePair<string, string>("@SubResponseID", SubResponseID));
                //if (UserSession.MemberID != null || UserSession.MemberID != "")
                //{
                //    methodparameter.Add(new KeyValuePair<string, string>("@MemberID", UserSession.UserID));
                //}
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Intermediary_Questions", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        obj.HasFiles = Convert.ToString(ds.Tables[1].Rows[0]["HasFiles"]);
                        obj.IsActive = Convert.ToString(ds.Tables[1].Rows[0]["IsActive"]);
                        obj.MaxFileSizeMb = Convert.ToString(ds.Tables[1].Rows[0]["MaxFileSizeMb"]);
                        obj.MaxNoOfFiles = Convert.ToString(ds.Tables[1].Rows[0]["MaxNoOfFiles"]);
                        obj.MinNoOfFiles = Convert.ToString(ds.Tables[1].Rows[0]["MinNoOfFiles"]);
                        obj.SubResponseText = Convert.ToString(ds.Tables[1].Rows[0]["SubResponseText"]);
                        obj.ResponseID = Convert.ToString(ds.Tables[1].Rows[0]["ResponseID"]);
                        obj.SubResponseID = Convert.ToString(ds.Tables[1].Rows[0]["SubResponseID"]);
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        public ActionResult SaveIntermediaryResponseQuestion(IntermediaryResponse obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    var methodparameter = new List<KeyValuePair<string, string>>();
                    methodparameter.Add(new KeyValuePair<string, string>("@Func", "SaveR"));
                    methodparameter.Add(new KeyValuePair<string, string>("@ResponseText", obj.ResponseText));
                    if (obj.IsActive != null && obj.IsActive == "on")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "Y"));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "N"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@IsSubResponse", obj.IsSubResponse));
                    methodparameter.Add(new KeyValuePair<string, string>("@IsMultipleSubResponse", obj.IsMultipleSubResponse));
                    methodparameter.Add(new KeyValuePair<string, string>("@MaxFileSizeMb", obj.MaxFileSizeMb));
                    methodparameter.Add(new KeyValuePair<string, string>("@MaxNoOfFiles", obj.MaxNoOfFiles));
                    methodparameter.Add(new KeyValuePair<string, string>("@MinNoOfFiles", obj.MinNoOfFiles));
                    methodparameter.Add(new KeyValuePair<string, string>("@ChangeOn", obj.ChangeOn));
                    methodparameter.Add(new KeyValuePair<string, string>("@HasFiles", obj.HasFiles));
                    if (obj.ResponseID != null && obj.ResponseID != "")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "U"));
                        methodparameter.Add(new KeyValuePair<string, string>("@ResponseID", obj.ResponseID));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "I"));
                    }

                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_QuestionsInsUpd]", methodparameter);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            switch (res.Trim().ToUpper())
                            {
                                case "S":
                                    TempData["Status"] = "S";
                                    break;
                                default:
                                    TempData["Status"] = "F";
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("IntermediaryQuestionsView");
        }
        public ActionResult SaveIntermediarySubResponseQuestion(IntermediarySubResponse obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    var methodparameter = new List<KeyValuePair<string, string>>();
                    methodparameter.Add(new KeyValuePair<string, string>("@Func", "SaveSR"));
                    methodparameter.Add(new KeyValuePair<string, string>("@ResponseID", obj.ResponseID));
                    methodparameter.Add(new KeyValuePair<string, string>("@SubResponseText", obj.SubResponseText));
                    if (obj.IsActive != null && obj.IsActive == "on")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "Y"));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@isActive", "N"));
                    }
                    methodparameter.Add(new KeyValuePair<string, string>("@MaxFileSizeMb", obj.MaxFileSizeMb));
                    methodparameter.Add(new KeyValuePair<string, string>("@MaxNoOfFiles", obj.MaxNoOfFiles));
                    methodparameter.Add(new KeyValuePair<string, string>("@MinNoOfFiles", obj.MinNoOfFiles));
                    methodparameter.Add(new KeyValuePair<string, string>("@HasFiles", obj.HasFiles));
                    if (obj.SubResponseID != null && obj.SubResponseID != "")
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "U"));
                        methodparameter.Add(new KeyValuePair<string, string>("@SubResponseID", obj.SubResponseID));
                    }
                    else
                    {
                        methodparameter.Add(new KeyValuePair<string, string>("@Action", "I"));
                    }

                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[Intermediary_QuestionsInsUpd]", methodparameter);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            switch (res.Trim().ToUpper())
                            {
                                case "S":
                                    TempData["Status"] = "S";
                                    break;
                                default:
                                    TempData["Status"] = "F";
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return RedirectToAction("IntermediaryQuestionsView");
        }
    }
}