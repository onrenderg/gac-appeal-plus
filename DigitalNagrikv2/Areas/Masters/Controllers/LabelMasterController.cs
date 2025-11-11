using DigitalNagrik.Areas.Masters.Data;
using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace DigitalNagrik.Areas.Masters.Controllers
{
    public class LabelMasterController : Controller
    {
        // GET: Masters/LabelMaster
        [CheckSessions]
        public ActionResult Index()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, "1", UserSession.LangCulture);
            return View();
        }
        [CheckSessions]
        public ActionResult Parameters()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, "1", UserSession.LangCulture);
            LabelDataMaster Data = new LabelDataMaster();
            Data.ModuleList = ModuleMaster();
            Data.FormList = FormMaster();
            Data.LanguageList = LanguageMaster();
            return View("PartialViews/_Parameters", Data);
        }
        [CheckSessions]
        public ActionResult LabelList(string LanguageCode = null, string FormID = null, string ModuleID = null)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, "1", UserSession.LangCulture);
            LabelDataMaster Data = LabelDataMaster.GetData("LIST",LanguageCode,FormID,ModuleID);
            Data.ModuleList = ModuleMaster();
            Data.FormList = FormMaster();
            Data.LanguageList = LanguageMaster();
            return View("PartialViews/_LabelsList", Data);
        }
        [CheckSessions]
        public ActionResult AddUpdateLabelShowPopUp(string ModuleID, string FormID, string LanguageCode, string LabelKey, string ActionType)
        {
            LabelList_Data obj = new LabelList_Data();
            obj = AddUpdateLabelShowPopUpData(ModuleID, FormID, LanguageCode, LabelKey, ActionType);
            //if (ActionType != "N")
            //{

            //}         
            return PartialView("PartialViews/_AddUpdateLabels", obj);
        }

        public List<SelectListItem> LanguageMaster()
        {
            LabelList_Data obj = new LabelList_Data();
            obj.LanguageList = new List<SelectListItem>();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                //if (UserSession.MemberID != null || UserSession.MemberID != "")
                //{
                //    methodparameter.Add(new KeyValuePair<string, string>("@MemberID", UserSession.UserID));
                //}
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "getLangMaster", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.LanguageList.Add(new SelectListItem { Value = Convert.ToString(dr["LanguageCode"]), Text = Convert.ToString(dr["LanguageName"]), });
                        }
                    }

                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj.LanguageList;
        }
        public List<SelectListItem> ModuleMaster()
        {
            LabelList_Data obj = new LabelList_Data();
            obj.ModuleList = new List<SelectListItem>();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                //if (UserSession.MemberID != null || UserSession.MemberID != "")
                //{
                //    methodparameter.Add(new KeyValuePair<string, string>("@MemberID", UserSession.UserID));
                //}
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "getModuleMaster", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.ModuleList.Add(new SelectListItem { Value = Convert.ToString(dr["ModuleID"]), Text = Convert.ToString(dr["ModuleName"]), });
                        }
                    }

                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj.ModuleList;
        }
        public JsonResult GetFormMaster(string ModuleID)
        {
            return Json(FormMaster(ModuleID), JsonRequestBehavior.AllowGet);
        }
        public List<SelectListItem> FormMaster(string ModuleID="")
        {
            LabelList_Data obj = new LabelList_Data();
            obj.FormList = new List<SelectListItem>();
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                if (ModuleID != null || ModuleID != "")
                {
                    methodparameter.Add(new KeyValuePair<string, string>("@ModuleID", ModuleID));
                }
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "getFormMaster", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            obj.FormList.Add(new SelectListItem { Value = Convert.ToString(dr["FormID"]), Text = Convert.ToString(dr["FormName"]), });
                        }
                    }

                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj.FormList;
        }
        public LabelList_Data AddUpdateLabelShowPopUpData(string ModuleID, string FormID, string LanguageCode, string LabelKey, string ActionType)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Masters, FormLabels.FormMasters.MasterReliefSought, UserSession.LangCulture);
            LabelList_Data obj = new LabelList_Data();
            obj.ModuleList = ModuleMaster();
            obj.FormList = FormMaster(ModuleID);
            obj.LanguageList = LanguageMaster();
            obj.ActionType = ActionType;
            try
            {
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@ModuleID", ModuleID));
                methodparameter.Add(new KeyValuePair<string, string>("@FormID", FormID));
                methodparameter.Add(new KeyValuePair<string, string>("@LanguageCode", LanguageCode));
                methodparameter.Add(new KeyValuePair<string, string>("@LabelKey", LabelKey));
                methodparameter.Add(new KeyValuePair<string, string>("@Mode", "GETLABEL"));
                //if (UserSession.MemberID != null || UserSession.MemberID != "")
                //{
                //    methodparameter.Add(new KeyValuePair<string, string>("@MemberID", UserSession.UserID));
                //}
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "Masters_LabelData", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj.ModuleID = Convert.ToString(ds.Tables[0].Rows[0]["ModuleID"]);
                        obj.ModuleName = Convert.ToString(ds.Tables[0].Rows[0]["ModuleName"]);
                        obj.FormID = Convert.ToString(ds.Tables[0].Rows[0]["FormID"]);
                        obj.FormName = Convert.ToString(ds.Tables[0].Rows[0]["FormName"]);
                        obj.LabelKey = Convert.ToString(ds.Tables[0].Rows[0]["LabelKey"]);
                        obj.LabelValue = Convert.ToString(ds.Tables[0].Rows[0]["LabelValue"]);
                        obj.LanguageCode = Convert.ToString(ds.Tables[0].Rows[0]["LanguageCode"]);
                        obj.LanguageName = Convert.ToString(ds.Tables[0].Rows[0]["LanguageName"]);
                        obj.LastModifiedBy = Convert.ToString(ds.Tables[0].Rows[0]["LastModifiedBy"]);
                        obj.LastModifiedIP = Convert.ToString(ds.Tables[0].Rows[0]["LastModifiedIP"]);
                        obj.LastModifiedOn = Convert.ToString(ds.Tables[0].Rows[0]["LastModifiedOn"]);
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return obj;
        }
        [HttpPost]
        [CheckSessions]
        [ValidateAntiForgeryToken]
        public ActionResult SaveLabel(LabelList_Data obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Status"] = "F";
                    var methodparameter = new List<KeyValuePair<string, string>>();
                    methodparameter.Add(new KeyValuePair<string, string>("@ModuleID", obj.ModuleID));
                    methodparameter.Add(new KeyValuePair<string, string>("@FormID", obj.FormID));
                    methodparameter.Add(new KeyValuePair<string, string>("@LabelKey", obj.LabelKey));
                    methodparameter.Add(new KeyValuePair<string, string>("@LanguageCode", obj.LanguageCode));
                    methodparameter.Add(new KeyValuePair<string, string>("@LabelValue", obj.LabelValue));
                    methodparameter.Add(new KeyValuePair<string, string>("@ActionType", obj.ActionType));
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "[LabelMaster_SaveLabel]", methodparameter);
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
                                case "E":
                                    TempData["Status"] = "O";
                                    TempData["Status_Type"] = BALCommon.CustomClientAlerts.AlertTypeFormat.Error;
                                    TempData["Status_Msg"] = ds.Tables[0].Rows[0]["msg"];
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
            return RedirectToAction("Index", "LabelMaster", new { Area = "Masters" });
        }

    }
}