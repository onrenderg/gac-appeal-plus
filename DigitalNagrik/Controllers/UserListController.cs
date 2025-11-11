using DigitalNagrik.Filters;
using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using static DigitalNagrik.Models.BALCommon.CustomClientAlerts;

namespace DigitalNagrik.Controllers
{
    public class UserListController : Controller
    {
        [CheckSessions]
        public ActionResult Index()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            UserList obj = new UserList();
            try
            {
                obj = GetUserData("");
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View(obj);
        }
        [CheckSessions]
        public UserList GetUserData(string ReportType)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            UserList Data = new UserList();
            try
            {
                Data.UserListDetails = new List<UserListD>();
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@ReportType", ReportType),
                    new KeyValuePair<string, string>("@func", "GET")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetUserList", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {

                            Data.UserListDetails.Add(new UserListD
                            {
                                UserId = Convert.ToString(dr["UserId"]),
                                UserName = Convert.ToString(dr["UserName"]),
                                Designation = Convert.ToString(dr["Designation"]),
                                Mobile = Convert.ToString(dr["Mobile"]),
                                isActive = Convert.ToString(dr["isActive"]),
                                RoleName = Convert.ToString(dr["RoleName"]),
                                UserRoleID = Convert.ToString(dr["RoleID"]),
                            });
                        }
                    }


                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return Data;
        }
        [CheckSessions]
        public JsonResult ChangeStatusisActive(string UserID, string Command)
        {
            try
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
                TempData["Status"] = "F";
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserID", UserID),
                    new KeyValuePair<string, string>("@Command", Command)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AddUser_Commands", methodparameter);
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
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return Json(new { Status = TempData["Status"] }, JsonRequestBehavior.AllowGet);
        }
        [CheckSessions]
        public ActionResult AddUser()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            UserListD obj = new UserListD();
            try
            {
                obj.RoleTypeList = GetUserRolesType();
                obj.UserRoleList = GetUserRoles("1");
                obj.GACTypeList = GetGACTypeList();
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return View(obj);
        }
        [CheckSessions]
        public List<SelectListItem> GetUserRolesType()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            List<SelectListItem> Data = new List<SelectListItem>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetUserRoleType", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            Data.Add(new SelectListItem { Text = Convert.ToString(dr["RoleTypeName"]), Value = Convert.ToString(dr["RoleTypeID"]) });
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return Data;
        }
        [CheckSessions]
        public JsonResult GetUserRolesData(string UserRoleType)
        {
            _ = new UserListD();
            return Json(new { RoleList = GetUserRoles(UserRoleType), GACList = GetGACTypeList() }, JsonRequestBehavior.AllowGet);
        }
        [CheckSessions]
        public List<SelectListItem> GetUserRoles(string UserRoleType)
        {
            List<SelectListItem> Data = new List<SelectListItem>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserRoleType", UserRoleType)
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetUserRoles", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            Data.Add(new SelectListItem { Text = Convert.ToString(dr["RoleName"]), Value = Convert.ToString(dr["RoleID"]) });
                        }
                    }
                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return Data;
        }
        [CheckSessions]
        public List<SelectListItem> GetGACTypeList()
        {
            List<SelectListItem> Data = new List<SelectListItem>();
            try
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetGACList", methodparameter);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            Data.Add(new SelectListItem { Text = Convert.ToString(dr["GACTitle"]), Value = Convert.ToString(dr["GACId"]) });
                        }
                    }


                }
            }
            catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            return Data;
        }
        [CheckSessions]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult InsertUserData(UserListD Obj)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            UserListD Data = new UserListD();
            Data = Obj;
            try
            {
                string message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("@UserID", Obj.UserId),
                        new KeyValuePair<string, string>("@UserName", Obj.UserName),
                        new KeyValuePair<string, string>("@Designation", Obj.Designation),
                        new KeyValuePair<string, string>("@UserAddress", Obj.UserAddress),
                        new KeyValuePair<string, string>("@Mobile", Obj.Mobile),
                        new KeyValuePair<string, string>("@RoleID", Obj.UserRoleID),
                        new KeyValuePair<string, string>("@RoleTypeId", Obj.RoleTypeID),
                        new KeyValuePair<string, string>("@gacID", Obj.GACIDString),
                        new KeyValuePair<string, string>("@CreatedBy", UserSession.UserID),
                        new KeyValuePair<string, string>("@CreatedIP", CommonRepository.GetIPAddress())
                    };
                    Random rnd = new Random();
                    string s = string.Empty;
                    s = CommonRepository.RandomStringGenerate(6);
                    string s1 = string.Empty;
                    s1 = CommonRepository.RandomStringGenerate(6);
                    string qs = SecureQueryString.SecureQueryString.Encrypt("UserID=" + Obj.UserId);
                    string dm = WebConfigurationManager.AppSettings["domainname"] + "/UserList/UserAccountActivation?qs=" + qs;
                    methodparameter.Add(new KeyValuePair<string, string>("@emaillink", dm));
                    methodparameter.Add(new KeyValuePair<string, string>("@emailverifycode", s.Substring(0, 6))); //
                    methodparameter.Add(new KeyValuePair<string, string>("@mobileverifycode", s1.Substring(0, 6))); // 
                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AuthUser_Add", methodparameter);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        string res = Convert.ToString(ds.Tables[0].Rows[0]["Result"]);
                        if (res.Trim().ToUpper() == "S")
                        {
                            Obj.UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserId"]);
                            string userType = Convert.ToString(ds.Tables[0].Rows[0]["userType"]);
                            if (userType == "EMAIL")
                            {
                                SendSMS smsObj = new SendSMS();
                                string emailSubject = "Grievance Appellate Committee - User Activation";
                                string emailMsg = "Dear " + Obj.UserName + ",<br/> <br/> Your Email ID ( " + Obj.UserId + " ) has been added in Grievance Appellate Committee. To activate this, click on the given link " + dm + "<br/> <br/>Grievance Appellate Committee<br/>";
                                smsObj.SendMailToUser(emailMsg, emailSubject, Obj.UserId.Trim(), "Grievance Appellate Committee", Obj.UserId.Trim());

                                //OTP Mobile
                                string MsgOTP = "Dear User, OTP for logging into https://gac.gov.in is " + s1.Substring(0, 6) + ". Do not share with anyone.";
                                string templateID = "1407167386460654881";
                                SendSMS objmSendSMS = new SendSMS();
                                objmSendSMS.send_SMS(Obj.Mobile, MsgOTP, templateID, "O");

                                //OTP Mobile
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            switch (res.Trim().ToUpper())
                            {
                                case "S":
                                    TempData["Status"] = "S";
                                    break;
                                case "U":
                                    TempData["Status"] = "U";
                                    break;
                                case "F":
                                    TempData["Status"] = "O";
                                    TempData["Status_Msg"] = Convert.ToString(ds.Tables[0].Rows[0]["Msg"]);
                                    TempData["Status_Type"] = AlertTypeFormat.Error;
                                    break;
                                default:
                                    TempData["Status"] = "F";
                                    break;
                            }
                        }
                        if (res.Trim().ToUpper() == "S" || res.Trim().ToUpper() == "U")
                        {
                            string qs1 = SecureQueryString.SecureQueryString.Encrypt("ReportType=Admin");
                            return RedirectToAction("index", new { qs = qs1 });
                        }
                        else if (res.Trim().ToUpper() == "F")
                        {
                            return RedirectToAction("AddUser");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);

            }
            return RedirectToAction("index");
        }
        public ActionResult UserAccountActivation(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            //TempData["Status"] = "F";
            Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
            string UserID = Params["UserID"];
            VerifyOtp Data = new VerifyOtp();
            try
            {
                string s = string.Empty;
                s = CommonRepository.RandomStringGenerate(6);
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserID", UserID),
                    new KeyValuePair<string, string>("@func", "EDIT")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetUserList", methodparameter);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Data.UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserID"]);
                        Data.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                        Data.Mobile = Convert.ToString(ds.Tables[0].Rows[0]["Mobile"]);
                        Data.Seed = Convert.ToString(s);

                        string MobileVerificationCode = Convert.ToString(ds.Tables[1].Rows[0]["MobileVerificationCode"]);

                        SendSMS smsObj = new SendSMS();
                        string msg = "Your GAC Portal OTP for registration is " + MobileVerificationCode.Trim() + ", Validity of this OTP is 30 minutes.";
                        //if (Data.Mobile.ToString() != null || Data.Mobile.ToString() != "")
                        //{
                        //    smsObj.send_SMS(Data.Mobile.ToString().Trim(), msg, "0");
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
            return View(Data);

        }
        [HttpPost]
        public ActionResult checkMobOTP(VerifyOtp Obj)
        {
            _ = new VerifyOtp();

            VerifyOtp Data = Obj;

            if (Obj.Password == null && Obj.ConfPassword == null)
            {
                _ = ModelState.Remove("Password");
                _ = ModelState.Remove("ConfPassword");
            }

            _ = ModelState.Remove("UserName");
            _ = ModelState.Remove("Mobile");




            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserID", Obj.UserId),
                    new KeyValuePair<string, string>("@mobOTP", Obj.mobOTP),
                    new KeyValuePair<string, string>("@HashPassword", Obj.HasPass)
                };


                //methodparameter.Add(new KeyValuePair<string, string>("@func", "EDIT"));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "validate_otp", methodparameter);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["status"]) == "S")
                    {
                        TempData["Status"] = "S";
                        //return View("UserAccountActivation", Data);
                        return RedirectToAction("UserActivationSuccessful");
                    }
                    else
                    {
                        TempData["Status"] = "O";
                        TempData["Status_Msg"] = Convert.ToString(ds.Tables[0].Rows[0]["msg"]);
                        TempData["Status_Type"] = AlertTypeFormat.Error;
                    }
                }
            }
            return View("UserAccountActivation", Data);
        }
        public ActionResult GetUserDetails(string qs)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            try
            {
                ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
                Dictionary<string, string> Params = QueryString.GetDecryptedParameters(qs);
                string UserID = Params["UserID"];
                UserListD Data = new UserListD
                {
                    RoleTypeList = GetUserRolesType(),

                    GACTypeList = GetGACTypeList()
                };

                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserID", UserID),
                    new KeyValuePair<string, string>("@func", "EDIT")
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetUserList", methodparameter);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Data.UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserID"]);
                        Data.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                        Data.Designation = Convert.ToString(ds.Tables[0].Rows[0]["Designation"]);
                        Data.Mobile = Convert.ToString(ds.Tables[0].Rows[0]["Mobile"]);
                        Data.UserAddress = Convert.ToString(ds.Tables[0].Rows[0]["UserAddress"]);
                        Data.UserRoleID = Convert.ToString(ds.Tables[0].Rows[0]["RoleID"]);
                        Data.UserRoleList = GetUserRolesType();
                        Data.RoleTypeID = Convert.ToString(ds.Tables[0].Rows[0]["RoleTypeId"]);
                        Data.UserRoleID = Convert.ToString(ds.Tables[0].Rows[0]["RoleID"]);
                        Data.GACID = Convert.ToString(ds.Tables[0].Rows[0]["GACId"]);
                        Data.GACIDString = Convert.ToString(ds.Tables[0].Rows[0]["GACIDString"]);

                    }
                }
                return View("AddUser", Data);
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return RedirectToAction("index");
        }
        public ActionResult UserActivationSuccessful()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Admin, "1", UserSession.LangCulture);
            return View();
        }
        public ActionResult ChangePassword()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.GACGridView, "1", UserSession.LangCulture);
            return View();
        }
        public ActionResult UpdatePassword(ChangePassword obj)
        {
            try
            {
                ModelState.Remove("CurrentPassword");
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmNewPassword");
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("@UserID", UserSession.UserID),
                    new KeyValuePair<string, string>("@CurrentPassword", obj.HashCurrentPassword),
                    new KeyValuePair<string, string>("@NewPassword", obj.HashNewPassword),
                };
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "UpdatePassword", methodparameter);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    if (dr["Status"].ToString() == "200")
                    {
                        TempData["Status"] = null;
                        TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Successfully Updated", "Password updated successfully.");
                        return RedirectToAction("GAC", "Home");
                    }
                    else
                    {
                        TempData["Status"] = null;
                        TempData["AlertStatus"] = BALCommon.CustomClientAlerts.GenerateAlert(BALCommon.CustomClientAlerts.AlertTypeFormat.Success, "Something went wrong...", dr["Msg"].ToString());
                        return RedirectToAction("ChangePassword", "UserList");
                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            return RedirectToAction("ChangePassword", "UserList");
        }

    }
}