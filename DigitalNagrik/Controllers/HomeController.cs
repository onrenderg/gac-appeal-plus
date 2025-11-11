using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalNagrik.Models;
using System.Data;
using NICServiceAdaptor;
using Rotativa;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Web.Security;
using DigitalNagrik.Areas.Public.Models;
using DigitalNagrik.Areas.Public.Controllers;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Security;
using System.Net;
using System.Security.Cryptography;
using ChaChaEncryption;

namespace DigitalNagrik.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Default()
        {
            GenerateKeySalt();
            return View();
        }
        [HttpGet]
        public ActionResult getSSMIOfficerList()
        {
            //Maintain language after session out
            string language = string.Empty;
            if (UserSession.LangCulture != null)
            {
                language = UserSession.LangCulture;
                Session.Clear();
                UserSession.LangCulture = language;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
            Response.Cache.SetNoStore();
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            //SSMIOfficerList objSSMIOfficerList = new SSMIOfficerList();
            List<SSMIOfficer> objSSMIOfficerList = new List<SSMIOfficer>();
            SSMIOfficer objSSMIOfficer = new SSMIOfficer();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@isActive", "Y"));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "getSSMIOfficerList", methodParameter);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in dt.Rows)
                        {
                            objSSMIOfficer = new SSMIOfficer();
                            objSSMIOfficer.SSMI_ID = dr1["SSMI_ID"].ToString();
                            objSSMIOfficer.SSMIDesc = dr1["SSMIDesc"].ToString();
                            objSSMIOfficer.SSMIEmail = dr1["SSMIEmail"].ToString();
                            objSSMIOfficerList.Add(objSSMIOfficer);
                        }

                    }
                }
            }

            return PartialView("_SSMIOfficerList", objSSMIOfficerList);
        }

        [HttpGet]
        public ActionResult Index()
        {
            //Maintain language after session out
            string language = string.Empty;
            if (UserSession.LangCulture != null)
            {
                language = UserSession.LangCulture;
                Session.Clear();
                UserSession.LangCulture = language;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
            Response.Cache.SetNoStore();
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonLogin obj = new CommonLogin();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            obj.Seed = Guid.NewGuid().ToString();
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(CommonLogin objCommonLogin)
        {


            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            ModelState["Captcha"].Errors.Clear();
            ModelState["AppellantMobile"].Errors.Clear();

            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            objCommonLogin.Submitted = count.Submitted;
            objCommonLogin.Disposed = count.Disposed;

            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();

            if (!objCommonLogin.PolicyCheck)
            {
                objCommonLogin.Captcha = "";
                ModelState.Remove("Captcha");
                ModelState.AddModelError("PolicyCheck", "Please accept Privacy Policy and Terms of Service");
                return View(objCommonLogin);
            }

            if (!ValidateCaptcha(objCommonLogin.Captcha))
            {
                objCommonLogin.Captcha = "";
                ModelState.Remove("Captcha");
                ModelState.AddModelError("Captcha", "Invalid Captcha");
                return View(objCommonLogin);
            }
            objCommonLogin.Captcha = "";
            ModelState.Remove("Captcha");
            if (string.IsNullOrEmpty(objCommonLogin.AppellantMobile))
            {
                ModelState.AddModelError("txtMobile", "Please enter valid credentials");
                return View(objCommonLogin);
            }
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objCommonLogin.AppellantMobile));
            methodParameter.Add(new KeyValuePair<string, string>("@EmailorMobile", "M"));
            methodParameter.Add(new KeyValuePair<string, string>("@Func", "LoginCitizen"));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "GetResendOTP_new", methodParameter);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows[0]["OTPID"].ToString() == "0" && dt.Rows[0]["OTP"].ToString() == "0")
                    {
                        ModelState.AddModelError("AppellantErrorMessage", dt.Rows[0]["Msg"].ToString());
                        return View(objCommonLogin);
                    }
                    else
                    {

                        Session["OTPID"] = dt.Rows[0]["OTPID"].ToString();
                        Session["CitizenEmailMobile"] = objCommonLogin.AppellantMobile;
                        SendSMS objmSendSMS = new SendSMS();
                        objmSendSMS.send_SMS(objCommonLogin.AppellantMobile, dt.Rows[0]["Msg"].ToString(), dt.Rows[0]["TemplateID"].ToString(), "O");
                        objCommonLogin.statusMessage = "LoadOTPVerification";
                        Session["ActionFromIndex"] = "A";
                        return RedirectToAction("OTPVerificationNew");
                    }
                }
                else
                {
                    ModelState.AddModelError("AppellantErrorMessage", "Unable to send OTP");

                    return View(objCommonLogin);
                }
            }
            else
            {
                ModelState.AddModelError("AppellantErrorMessage", "Unable to send OTP");

                return View(objCommonLogin);
            }
            return View(objCommonLogin);
        }

        public ActionResult Intermediary()
        {
            //Maintain language after session out
            string language = string.Empty;
            if (UserSession.LangCulture != null)
            {
                language = UserSession.LangCulture;
                Session.Clear();
                UserSession.LangCulture = language;
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
            Response.Cache.SetNoStore();
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonLogin obj = new CommonLogin();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            obj.Seed = Guid.NewGuid().ToString();
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            obj.Seed = Guid.NewGuid().ToString();
            GenerateKeySalt();
            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Intermediary(CommonLogin objCommonLogin)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            ModelState["IntCaptcha"].Errors.Clear();
            ModelState["InterEmailID"].Errors.Clear();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            objCommonLogin.Submitted = count.Submitted;
            objCommonLogin.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            if (!ValidateCaptcha(objCommonLogin.IntCaptcha))
            {
                objCommonLogin.IntCaptcha = "";
                ModelState.Remove("IntCaptcha");
                ModelState.AddModelError("IntCaptcha", "Invalid Captcha");
                return View(objCommonLogin);

            }
            objCommonLogin.IntCaptcha = "";
            ModelState.Remove("IntCaptcha");
            bool hasValidUser = false;
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserID", objCommonLogin.InterEmailID));
            var AuthUserDS = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AuthUserIntermediary", methodParameter);
            if (AuthUserDS != null)
            {
                if (AuthUserDS.Tables.Count > 0)
                {
                    if (AuthUserDS.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = AuthUserDS.Tables[0].Rows[0];
                        if (dr["Msg"].ToString() == "Valid User")
                        {
                            hasValidUser = true;
                        }

                    }
                }
            }
            if (hasValidUser == false)
            {
                objCommonLogin.InterEmailID = "";
                ModelState.Remove("InterEmailID");
                objCommonLogin.Password = "";
                ModelState.Remove("Password");
                ModelState.AddModelError("IntErrorMessage", "Please enter valid credentials");
                return View(objCommonLogin);
            }
            if (objCommonLogin.IntLoginMethod == "O")
            {
                try
                {
                    string Result = string.Empty;
                    methodParameter.Clear();
                    methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objCommonLogin.InterEmailID));
                    methodParameter.Add(new KeyValuePair<string, string>("@EmailorMobile", "E"));
                    methodParameter.Add(new KeyValuePair<string, string>("@Func", "InterLogin"));
                    var ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetResendOTP_new", methodParameter);
                    if (ds != null)
                    {
                        Session["OTPID"] = ds.Tables[0].Rows[0]["OTPID"].ToString();
                        Session["CitizenEmailMobile"] = objCommonLogin.InterEmailID;
                        SendSMS smsObj = new SendSMS();
                        string emailSubject = "Grievance Appellate Committee : Login Verification OTP";
                        string emailMsg = ds.Tables[0].Rows[0]["Msg"].ToString();
                        //string emailResponse = smsObj.SendMailToUser(emailMsg, emailSubject, objCommonLogin.InterEmailID, "Grievance Appellate Committee ", objCommonLogin.InterEmailID);
                        string emailResponse = "OK";
                        Session["ActionFromIndex"] = "I";

                        if (emailResponse.Equals("OK"))
                        {
                            return RedirectToAction("OTPVerificationNew", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("IntErrorMessage", "Currently we are facing problem in sending email, you are requested to try again later!");
                            return View(objCommonLogin);
                        }
                    }
                }
                catch (Exception ex) { MyExceptionHandler.LogError(ex); }
                ModelState.AddModelError("IntErrorMessage", "Error occured while sending PIN");
            }
            else
            {
                objCommonLogin.Password = "";
                ModelState.Remove("Password");
                methodParameter = new List<KeyValuePair<string, string>>();
                methodParameter.Add(new KeyValuePair<string, string>("@UserID", objCommonLogin.InterEmailID));
                methodParameter.Add(new KeyValuePair<string, string>("@Seed", objCommonLogin.Seed));
                methodParameter.Add(new KeyValuePair<string, string>("@HashPassword", objCommonLogin.HashPwd));
                var ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CheckIntermediaryPassword", methodParameter);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows[0]["LoginMethod"].ToString() == "P")
                            {
                                if (ds.Tables[0].Rows[0]["Msg"].ToString() == "ValidPassword")
                                {
                                    DataRow dr = AuthUserDS.Tables[0].Rows[0];
                                    TempData["Status"] = null;
                                    UserSession.isSSOLogin = "N";
                                    UserSession.RoleID = "98";
                                    UserSession.UserID = dr["UserID"].ToString();
                                    UserSession.IntermediaryId = dr["IntermediaryId"].ToString();
                                    UserSession.IntermediaryTitle = dr["IntermediaryTitle"].ToString();
                                    if (AuthUserDS.Tables[1] != null)
                                        if (AuthUserDS.Tables[1].Rows.Count > 0)
                                        {
                                            IntermediaryForSession il;
                                            List<IntermediaryForSession> iList = new List<IntermediaryForSession>();
                                            foreach (DataRow dr1 in AuthUserDS.Tables[1].Rows)
                                            {
                                                il = new IntermediaryForSession();
                                                il.IntermediaryTitle = dr1["IntermediaryTitle"].ToString();
                                                il.UserName = dr1["UserName"].ToString();
                                                il.Designation = dr1["Designation"].ToString();
                                                il.URL = dr1["URL"].ToString();
                                                il.HelpLink = dr1["HelpLink"].ToString();
                                                il.Address = dr1["Address"].ToString();
                                                iList.Add(il);
                                            }
                                            UserSession.IntermediaryList = iList;
                                        }

                                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, UserSession.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, UserSession.RoleID, "/");
                                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                    //cookie.Secure = true;
                                    //cookie.HttpOnly = true;
                                    Response.AppendCookie(cookie);
                                    string ID = InsertLoginStatus(objCommonLogin.InterEmailID, "Success", "Successful Login", "Intermediary");
                                    if (Convert.ToInt32(ID) > 0)
                                    {
                                        Session["LogoutID"] = ID;
                                        return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                                    }
                                    else
                                    {
                                        TempData["Status"] = "F";
                                        return RedirectToAction("Intermediary", "Home");
                                    }
                                    //////////////////////////
                                }
                                else
                                {
                                    objCommonLogin.InterEmailID = "";
                                    ModelState.Remove("InterEmailID");
                                    objCommonLogin.Password = "";
                                    ModelState.Remove("Password");
                                    ModelState.AddModelError("IntErrorMessage", "Please enter valid credentials");
                                }
                            }
                            else
                            {
                                objCommonLogin.InterEmailID = "";
                                ModelState.Remove("InterEmailID");
                                objCommonLogin.Password = "";
                                ModelState.Remove("Password");
                                ModelState.AddModelError("IntErrorMessage", "Your account is not configured for password based login. Kinldy login using OTP");
                            }
                        }
                    }
                }
            }
            return View(objCommonLogin);
        }


        public ActionResult GAC()
        {
            //Maintain language after session out
            string language = string.Empty;
            if (UserSession.LangCulture != null)
            {
                language = UserSession.LangCulture;
                Session.Clear();
                UserSession.LangCulture = language;
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
            Response.Cache.SetNoStore();
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonLogin obj = new CommonLogin();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            obj.Seed = Guid.NewGuid().ToString();
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            return View(obj);
        }
        public ActionResult GACLogin()
        {
            //Session.Abandon();
            //Session.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
            Response.Cache.SetNoStore();
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonLogin obj = new CommonLogin();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            obj.Seed = Guid.NewGuid().ToString();
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            GenerateKeySalt();
            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GACLogin(CommonLogin objCommonLogin)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            ModelState["GACCaptcha"].Errors.Clear();
            ModelState["GACEmailID"].Errors.Clear();
            ModelState["Password"].Errors.Clear();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            objCommonLogin.Submitted = count.Submitted;
            objCommonLogin.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            if (!ValidateCaptcha(objCommonLogin.GACCaptcha))
            {
                objCommonLogin.GACCaptcha = "";
                ModelState.Remove("GACCaptcha");
                ModelState.AddModelError("GACCaptcha", "Invalid Captcha");
                return View(objCommonLogin);
            }
            objCommonLogin.GACCaptcha = "";
            ModelState.Remove("GACCaptcha");
            Session["ActionFromIndex"] = "G";
            try
            {
                TempData["Status"] = "F";
                //Session.Clear();
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@UserID", objCommonLogin.GACEmailID));
                methodparameter.Add(new KeyValuePair<string, string>("@Password", objCommonLogin.Password));
                methodparameter.Add(new KeyValuePair<string, string>("@Seed", objCommonLogin.Seed));
                methodparameter.Add(new KeyValuePair<string, string>("@HashPassword", objCommonLogin.HashPwd));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AuthUser", methodparameter);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            if (dr["Msg"].ToString() == "Valid User" && dr["Mobile"].ToString() != "")
                            {

                                var methodParameter = new List<KeyValuePair<string, string>>();
                                methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", dr["Mobile"].ToString()));
                                methodParameter.Add(new KeyValuePair<string, string>("@EmailorMobile", "M"));
                                methodParameter.Add(new KeyValuePair<string, string>("@Func", "GACLogin"));
                                ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetResendOTP_new", methodParameter);
                                if (ds != null)
                                {
                                    DataTable dt = ds.Tables[0];
                                    Session["OTPID"] = ds.Tables[0].Rows[0]["OTPID"].ToString();
                                    Session["CitizenEmailMobile"] = dr["Mobile"].ToString();
                                    Session["GACEmailID"] = objCommonLogin.GACEmailID;

                                    SendSMS objmSendSMS = new SendSMS();
                                    objmSendSMS.send_SMS(dr["Mobile"].ToString(), dt.Rows[0]["Msg"].ToString(), dt.Rows[0]["TemplateID"].ToString(), "O");
                                    return RedirectToAction("OTPVerificationNew", "Home");
                                }
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
            }
            objCommonLogin.GACEmailID = "";
            ModelState.Remove("GACEmailID");
            objCommonLogin.Password = "";
            ModelState.Remove("Password");
            ModelState.AddModelError("GACErrorMessage", "Please enter valid credentials");
            return View(objCommonLogin);
        }
        public ActionResult ExpertMember()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonLogin obj = new CommonLogin();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            obj.Seed = Guid.NewGuid().ToString();
            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            GenerateKeySalt();
            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ExpertMember(CommonLogin objCommonLogin)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            ModelState["MExpCaptcha"].Errors.Clear();
            ModelState["ExpertMobile"].Errors.Clear();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            objCommonLogin.Submitted = count.Submitted;
            objCommonLogin.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            if (!ValidateCaptcha(objCommonLogin.MExpCaptcha))
            {
                objCommonLogin.MExpCaptcha = "";
                ModelState.Remove("MExpCaptcha");
                ModelState.AddModelError("MExpCaptcha", "Invalid Captcha");
                return View(objCommonLogin);

            }
            objCommonLogin.MExpCaptcha = "";
            ModelState.Remove("MExpCaptcha");
            Session["ActionFromIndex"] = "E";
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@MobileNo", objCommonLogin.ExpertMobile));
            var ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CheckMemberExpert", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows[0]["Userexits"].ToString() == "Y")
                    {
                        methodParameter.Clear();
                        methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objCommonLogin.ExpertMobile));
                        methodParameter.Add(new KeyValuePair<string, string>("@EmailorMobile", "M"));
                        methodParameter.Add(new KeyValuePair<string, string>("@Func", "ExpertLogin"));
                        ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "GetResendOTP_new", methodParameter);
                        if (ds != null)
                        {
                            dt = ds.Tables[0];
                            Session["OTPID"] = ds.Tables[0].Rows[0]["OTPID"].ToString();
                            Session["CitizenEmailMobile"] = objCommonLogin.ExpertMobile;

                            SendSMS objmSendSMS = new SendSMS();
                            objmSendSMS.send_SMS(objCommonLogin.ExpertMobile, dt.Rows[0]["Msg"].ToString(), dt.Rows[0]["TemplateID"].ToString(), "O");

                            return RedirectToAction("OTPVerificationNew", "Home");
                        }
                    }
                    else
                    {
                        objCommonLogin.ExpertMobile = "";
                        ModelState.Remove("ExpertMobile");
                        ModelState.AddModelError("ExpertMemberMessage", "Please enter valid credentials");
                    }
                }
            }
            return View(objCommonLogin);
        }
        public ActionResult OTPVerificationNew(string actionfrom)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(1));
            Response.Cache.SetNoStore();
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            if (Session["ActionFromIndex"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["CitizenEmailMobile"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            CommonLogin objUserOTPVerfication = new CommonLogin();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            objUserOTPVerfication.Submitted = count.Submitted;
            objUserOTPVerfication.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();

            objUserOTPVerfication.OTPID = Session["OTPID"].ToString();
            if (Session["ActionFromIndex"].ToString() == "A")
            {
                ViewData["SuccessMessage"] = "OTP Sent Successfully";
                objUserOTPVerfication.OTPMobile = Session["CitizenEmailMobile"].ToString();
            }
            else if (Session["ActionFromIndex"].ToString() == "I")
            {
                objUserOTPVerfication.InterEmailID = Session["CitizenEmailMobile"].ToString();
            }
            else if (Session["ActionFromIndex"].ToString() == "G")
            {
                objUserOTPVerfication.OTPMobile = Session["CitizenEmailMobile"].ToString();
                objUserOTPVerfication.GACEmailID = Session["GACEmailID"].ToString();
            }
            else if (Session["ActionFromIndex"].ToString() == "E")
            {
                objUserOTPVerfication.OTPMobile = Session["CitizenEmailMobile"].ToString();
            }
            objUserOTPVerfication.EncryptedID = Session["CitizenEmailMobile"].ToString();
            GenerateKeySalt();
            return View(objUserOTPVerfication);
        }

        public void GenerateKeySalt()
        {
            StringBuilder randomText = new StringBuilder();
            randomText.Append(CommonRepository.RandomStringGenerateNumbers(16));
            Session["EncKeySalt"] = Convert.ToString(randomText);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult OTPVerificationNew(CommonLogin objUserOTPVerfication)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            if (Session["ActionFromIndex"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            objUserOTPVerfication.OTPEnc = Decrypt(objUserOTPVerfication.OTPEnc.ToString(), Session["EncKeySalt"].ToString());
            objUserOTPVerfication.EncryptedID = Decrypt(objUserOTPVerfication.EncryptedID.ToString(), Session["EncKeySalt"].ToString());
            objUserOTPVerfication.OTP = "";
            ModelState.Remove("OTP");
            ModelState.Remove("EncryptedID");
            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            objUserOTPVerfication.Submitted = count.Submitted;
            objUserOTPVerfication.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            if (!ValidateCaptcha(objUserOTPVerfication.Captcha))
            {
                objUserOTPVerfication.Captcha = "";
                ModelState.Remove("Captcha");
                ModelState.AddModelError("", "Invalid Captcha");
                return View(objUserOTPVerfication);
            }
            objUserOTPVerfication.Captcha = "";
            ModelState.Remove("Captcha");
            if (Session["ActionFromIndex"].ToString() == "A")
            {
                var methodParameter = new List<KeyValuePair<string, string>>();
                // methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.OTPMobile.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.EncryptedID.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@OTPID", objUserOTPVerfication.OTPID.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@OTP", objUserOTPVerfication.OTPEnc.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@func", "Citizen"));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "CheckEmailMobileOTPAll", methodParameter);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];

                            if (dt.Rows[0]["UserVerfied"].ToString() == "Y")
                            {

                                Session["UserVerfied"] = "Y";
                                Session["CitizenEmailMobile"] = objUserOTPVerfication.EncryptedID;
                                if (dt.Rows[0]["AadhaarVerificationStatus"].ToString() == "N")
                                {
                                    return RedirectToAction("AadhaarVerification", "Home", new { Message = "LoginSuccess" });
                                }
                                else
                                {
                                    objcm.CreateUser(Session["CitizenEmailMobile"].ToString());
                                    string ID = InsertLoginStatus(Session["CitizenEmailMobile"].ToString(), "Success", "Successful Login", "Appellant");
                                    Session["LogoutID"] = ID;
                                    UserSession.RoleID = "99";
                                    return RedirectToAction("CitizenDashboard", "Registration", new { Message = "LoginSuccess", Area = "Public" });
                                }
                            }
                            else
                            {

                                ModelState.AddModelError("", "Mobile OTP Not Verified");
                                return View("OTPVerificationNew", objUserOTPVerfication);
                            }

                        }

                    }
                }
            }
            else if (Session["ActionFromIndex"].ToString() == "I")
            {
                string Result = "";
                try
                {
                    var methodParameter = new List<KeyValuePair<string, string>>();
                    //methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.InterEmailID.ToString()));
                    methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.EncryptedID.ToString()));
                    methodParameter.Add(new KeyValuePair<string, string>("@OTPID", objUserOTPVerfication.OTPID.ToString()));
                    methodParameter.Add(new KeyValuePair<string, string>("@OTP", objUserOTPVerfication.OTPEnc.ToString()));
                    methodParameter.Add(new KeyValuePair<string, string>("@func", "Inter"));

                    DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "CheckEmailMobileOTPAll", methodParameter);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["UserVerfied"].ToString() == "Y")
                        {
                            TempData["Status"] = "F";
                            string language = string.Empty;
                            if (UserSession.LangCulture != null)
                            {
                                language = UserSession.LangCulture;
                                Session.Clear();
                                UserSession.LangCulture = language;
                            }
                            methodParameter = new List<KeyValuePair<string, string>>();
                            methodParameter.Add(new KeyValuePair<string, string>("@UserID", objUserOTPVerfication.EncryptedID));
                            ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AuthUserIntermediary", methodParameter);
                            if (ds != null)
                            {
                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow dr = ds.Tables[0].Rows[0];
                                        if (dr["Msg"].ToString() == "Not Authorized")
                                        {

                                            ModelState.AddModelError("", "Email OTP Not Verified");
                                        }
                                        else if (dr["Msg"].ToString() == "Valid User")
                                        {
                                            TempData["Status"] = null;
                                            UserSession.isSSOLogin = "N";
                                            UserSession.UserID = dr["UserID"].ToString();
                                            //UserSession.UserName = dr["UserName"].ToString();
                                            //UserSession.Designation = dr["Designation"].ToString();
                                            UserSession.IntermediaryId = dr["IntermediaryId"].ToString();
                                            UserSession.IntermediaryTitle = dr["IntermediaryTitle"].ToString();
                                            //UserSession.URL = dr["URL"].ToString();
                                            //UserSession.HelpLink = dr["HelpLink"].ToString();
                                            //UserSession.Address = dr["Address"].ToString();
                                            if (ds.Tables[1] != null)
                                                if (ds.Tables[1].Rows.Count > 0)
                                                {
                                                    IntermediaryForSession il;
                                                    List<IntermediaryForSession> iList = new List<IntermediaryForSession>();
                                                    foreach (DataRow dr1 in ds.Tables[1].Rows)
                                                    {
                                                        il = new IntermediaryForSession();
                                                        il.IntermediaryTitle = dr1["IntermediaryTitle"].ToString();
                                                        il.UserName = dr1["UserName"].ToString();
                                                        il.Designation = dr1["Designation"].ToString();
                                                        il.URL = dr1["URL"].ToString();
                                                        il.HelpLink = dr1["HelpLink"].ToString();
                                                        il.Address = dr1["Address"].ToString();
                                                        iList.Add(il);
                                                    }
                                                    UserSession.IntermediaryList = iList;
                                                }

                                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, UserSession.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, UserSession.RoleID, "/");
                                            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                            //cookie.Secure = true;
                                            //cookie.HttpOnly = true;
                                            Response.AppendCookie(cookie);
                                            string ID = InsertLoginStatus(objUserOTPVerfication.EncryptedID, "Success", "Successful Login", "Intermediary");
                                            if (Convert.ToInt32(ID) > 0)
                                            {
                                                Session["LogoutID"] = ID;
                                                UserSession.RoleID = "98";
                                                return RedirectToAction("Index", "IntermediaryDashboard", new { Area = "Intermediary" });
                                            }
                                            else
                                            {
                                                TempData["Status"] = "F";
                                                return RedirectToAction("Intermediary", "Home");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        TempData["Status"] = "F";
                                        return RedirectToAction("Intermediary", "Home");
                                    }

                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("OTP", "Invalid OTP");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyExceptionHandler.LogError(ex);
                    throw ex;
                }
                //return Json(new { Result }, JsonRequestBehavior.AllowGet);
            }
            else if (Session["ActionFromIndex"].ToString() == "G")
            {
                var methodParameter = new List<KeyValuePair<string, string>>();
                //methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.OTPMobile.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.EncryptedID.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@OTPID", objUserOTPVerfication.OTPID.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@OTP", objUserOTPVerfication.OTPEnc.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@func", "GACLogin"));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "CheckEmailMobileOTPAll", methodParameter);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt.Rows[0]["UserVerfied"].ToString() == "Y")
                            {
                                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                                methodparameter.Add(new KeyValuePair<string, string>("@UserID", Session["GACEmailID"].ToString()));
                                ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AuthUserAfterOTP", methodparameter);
                                if (ds != null)
                                {
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (ds.Tables[0].Rows.Count > 0)
                                        {
                                            DataRow dr = ds.Tables[0].Rows[0];
                                            if (dr["Msg"].ToString() == "Valid User")
                                            {
                                                TempData["Status"] = null;
                                                UserSession.isSSOLogin = "N";
                                                UserSession.UserID = dr["Userid"].ToString();
                                                UserSession.UserName = dr["UserName"].ToString();
                                                UserSession.EmailId = dr["EmailId"].ToString();
                                                UserSession.Mobile = dr["Mobile"].ToString();
                                                UserSession.Designation = dr["Designation"].ToString();
                                                UserSession.RoleID = dr["UserRole"].ToString();
                                                UserSession.RoleName = dr["RoleName"].ToString();
                                                UserSession.GACID = dr["GACID"].ToString();
                                                UserSession.GACName = dr["GACName"].ToString();
                                                UserSession.RoleTypeId = dr["RoleTypeId"].ToString();
                                                UserSession.RoleTypeName = dr["RoleTypeName"].ToString();
                                                UserSession.UserType = dr["UserType"].ToString();
                                                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, UserSession.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, UserSession.RoleID, "/");
                                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                                //cookie.Secure = true;
                                                //cookie.HttpOnly = true;
                                                Response.AppendCookie(cookie);
                                                string ID = InsertLoginStatus(UserSession.UserID, "Success", "Successful Login", "GAC");
                                                if (Convert.ToInt32(ID) > 0)
                                                {
                                                    Session["LogoutID"] = ID;
                                                    if (UserSession.RoleID == "1")// Admin
                                                    {
                                                        return RedirectToAction("Index", "AdminDashboard", new { Area = "Dashboard" });
                                                    }
                                                    else if (UserSession.RoleID == "2")// Secretariat
                                                    {
                                                        return RedirectToAction("Index", "AdminDashboard", new { Area = "Dashboard" });
                                                    }
                                                    else
                                                    {
                                                        return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
                                                    }

                                                }

                                            }

                                        }

                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "OTP Not Verified");
                                return View(objUserOTPVerfication);
                            }

                        }

                    }
                }
            }
            else if (Session["ActionFromIndex"].ToString() == "E")
            {
                var methodParameter = new List<KeyValuePair<string, string>>();
                // methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.OTPMobile.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", objUserOTPVerfication.EncryptedID.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@OTPID", objUserOTPVerfication.OTPID.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@OTP", objUserOTPVerfication.OTPEnc.ToString()));
                methodParameter.Add(new KeyValuePair<string, string>("@func", "MemberExpert"));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "CheckEmailMobileOTPAll", methodParameter);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt.Rows[0]["UserVerfied"].ToString() == "Y")
                        {
                            string ID = InsertLoginStatus(Session["CitizenEmailMobile"].ToString(), "Success", "Successful Login", "ExpertMember");
                            UserSession.UserID = objUserOTPVerfication.EncryptedID;
                            UserSession.Mobile = objUserOTPVerfication.EncryptedID;
                            UserSession.UserName = dt.Rows[0]["UserName"].ToString();
                            //ModelState.AddModelError("", "OTP  Verified");
                            UserSession.RoleID = "97";
                            return RedirectToAction("Index", "SubjectExpert", new { Area = "SubjectExpert" });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Mobile OTP Not Verified");
                            return View(objUserOTPVerfication);
                        }

                    }

                }
            }

            return View(objUserOTPVerfication);

        }


        public ActionResult AadhaarVerification()
        {
            if (Session["UserVerfied"] == null)
            {
                return RedirectToAction("Index");
            }

            if (Session["UserVerfied"].ToString() != "Y")
            {
                return RedirectToAction("Index");
            }

            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            if (Session["CitizenEmailMobile"] == null)
            {
                return RedirectToAction("Index");
            }
            CommonFunctions objcm = new CommonFunctions();
            mHome count = new mHome();
            count = objcm.DashboardCount();
            CommonLogin obj = new CommonLogin();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            obj.OTPFunc = "A";
            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AadhaarVerification(CommonLogin obj)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            if (Session["CitizenEmailMobile"] == null)
            {
                return RedirectToAction("Index");
            }
            string OTPFunc = "A";
            ModelState["AadhaarOTP"].Errors.Clear();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();

            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            if (!ValidateCaptcha(obj.Captcha))
            {
                obj.Captcha = "";
                ModelState.Remove("Captcha");
                ModelState.AddModelError("Captcha", "Invalid Captcha");
                return View(obj);

            }
            obj.Captcha = "";
            ModelState.Remove("Captcha");
            obj.AadhaarNo = "";
            ModelState.Remove("AadhaarNo");
                    
            ChaChaPoly objChaChaPoly = new ChaChaPoly();
            string Enckey = "df0025ae-77d8-4806-aa72-ee7610b00bf5";
            string AadhaarAuthentication = "https://authenticate.epramaan.gov.in/authwebservice/requestauth/v3";
            string AadhaarOTPSend = "https://authenticate.epramaan.gov.in/authwebservice/requestaadhaarotpauth/v3";
            string AadhaarOTPVerify = "https://authenticate.epramaan.gov.in/authwebservice/verifyaadhaarotpauth/v2";



            //string AadhaarAuthentication = "https://epstg.meripehchaan.gov.in/authwebservice/requestauth/v3";
            //string AadhaarOTPSend = "https://epstg.meripehchaan.gov.in/authwebservice/requestaadhaarotpauth/v3";
            //string AadhaarOTPVerify = "https://epstg.meripehchaan.gov.in/authwebservice/verifyaadhaarotpauth/v2";
            //string Enckey = "88f81129-9f33-40d6-94de-6ef6451819aa";

            if (obj.OTPFunc == "A")
            {
                try
                {

                    obj.AadhaarNo = Decrypt(obj.AadhaarNoEnc.ToString(), Session["EncKeySalt"].ToString());
                    AadhaaVerfRequest reqaadhaar = new AadhaaVerfRequest();
                    reqaadhaar.name = obj.AadhaarName;
                    reqaadhaar.aadhaarNumber = obj.AadhaarNo;
                    reqaadhaar.txnRequestID = Guid.NewGuid().ToString().Trim();
                    reqaadhaar.demoAuth = "y";
                    reqaadhaar.bioAuth = "n";
                    reqaadhaar.password = "Welcome@123";
                    reqaadhaar.userConsent = "y";



                    string data = Newtonsoft.Json.JsonConvert.SerializeObject(reqaadhaar);
                    //string jsonStringenc = EncryptDencrypt.Encrypt(data, "df0025ae-77d8-4806-aa72-ee7610b00bf5");
                    string jsonStringenc = objChaChaPoly.Encrypt(data, Enckey, reqaadhaar.txnRequestID);
                    FinalRequest objFinalRequest = new FinalRequest();
                    objFinalRequest.serviceId = "10003";
                    objFinalRequest.data = jsonStringenc;
                    objFinalRequest.reqTxnId = reqaadhaar.txnRequestID;
                    string finaldata = Newtonsoft.Json.JsonConvert.SerializeObject(objFinalRequest);
                    WebClient wc = new WebClient();
                    string result;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    //result = wc.UploadString("https://authenticate.epramaan.gov.in/authwebservice/requestauth", finaldata);
                    result = wc.UploadString(AadhaarAuthentication, finaldata);


                    string result_decrypt = objChaChaPoly.Decrypt(result, Enckey, reqaadhaar.txnRequestID);


                    Responseaadhar objResponseFirst = Newtonsoft.Json.JsonConvert.DeserializeObject<Responseaadhar>(result_decrypt);
                    objcm.AadharVerificationLog("requestauth", result_decrypt);
                    if ((objResponseFirst.status == null ? "" : objResponseFirst.status) == "true")
                    {


                        //////////////////////////////////////////
                        // after verification send Aadhaar OTP
                        try
                        {
                            AadhaaVerfOTPRequest reqaadhaarOTP = new AadhaaVerfOTPRequest();
                            reqaadhaarOTP.password = "Welcome@123";
                            reqaadhaarOTP.aadhaarNumber = obj.AadhaarNo;
                            reqaadhaarOTP.txnRequestID = Guid.NewGuid().ToString().Trim();
                            reqaadhaarOTP.otpMedium = "00";
                            reqaadhaarOTP.userConsent = "y";
                            string dataOTP = Newtonsoft.Json.JsonConvert.SerializeObject(reqaadhaarOTP);
                            //string jsonStringOTPenc = EncryptDencrypt.Encrypt(dataOTP, Enckey);
                            string jsonStringOTPenc = objChaChaPoly.Encrypt(dataOTP, Enckey, reqaadhaarOTP.txnRequestID);

                            objFinalRequest = new FinalRequest();
                            objFinalRequest.serviceId = "10003";
                            objFinalRequest.data = jsonStringOTPenc;
                            objFinalRequest.reqTxnId = reqaadhaarOTP.txnRequestID;
                            string finaldataOTP = Newtonsoft.Json.JsonConvert.SerializeObject(objFinalRequest);
                            wc = new WebClient();
                            wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                            //result = wc.UploadString("https://authenticate.epramaan.gov.in/authwebservice/requestaadhaarotpauth/v3", finaldataOTP);
                            result = wc.UploadString(AadhaarOTPSend, finaldataOTP);
                            result_decrypt = objChaChaPoly.Decrypt(result, Enckey, reqaadhaarOTP.txnRequestID);
                            //Responseaadhar objResponseFirstOTP = Newtonsoft.Json.JsonConvert.DeserializeObject<Responseaadhar>(result_decrypt);
                            Responseaadhar objResponseFirstOTP = Newtonsoft.Json.JsonConvert.DeserializeObject<Responseaadhar>(result_decrypt);
                            objcm.AadharVerificationLog("requestaadhaarotpauth", result);
                            if ((objResponseFirstOTP.status == null ? "" : objResponseFirstOTP.status) == "true")
                            {
                                ModelState.Remove("OTPFunc");
                                OTPFunc = "B";
                                obj.otpTxnID = objResponseFirstOTP.otpTransactionID;
                                TempData["SuccessMessage"] = "OTP sent successfully";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Presently Aadhaar verification service is facing some technical issue, Please try after some time";
                            }
                        }
                        catch (Exception ex)
                        {

                            string exmsg = ex.Message;
                            TempData["ErrorMessage"] = "Presently Aadhaar verification service is facing some technical issue, Please try after some time";
                        }
                        //ModelState.Remove("OTPFunc");
                        //OTPFunc = "B";
                        //obj.otpTxnID = objResponseFirstOTP.otpTransactionID;
                        //TempData["SuccessMessage"] = "OTP sent successfully";
                    }
                    else
                    {
                        //TempData["ErrorMessage"] = "Presently Aadhaar verification service is facing some technical issue, Please try after some time";
                        TempData["ErrorMessage"] = "Aadhaar Details not matched";
                    }
                }
                catch (Exception ex)
                {
                    string exmessage = ex.Message;
                    TempData["ErrorMessage"] = "Presently Aadhaar verification service is facing some technical issue, Please try after some time";
                }

            }
            else
            {

                OTPFunc = "B";
                try
                {

                    obj.AadhaarNo = Decrypt(obj.AadhaarNoEnc.ToString(), Session["EncKeySalt"].ToString());
                    obj.AadhaarOTP = Decrypt(obj.AadhaarOTPEnc.ToString(), Session["EncKeySalt"].ToString());
                    AadhaaOTPRequest reqOTP = new AadhaaOTPRequest();
                    reqOTP.aadhaarNumber = obj.AadhaarNo;
                    reqOTP.txnRequestID = Guid.NewGuid().ToString().Trim();
                    reqOTP.password = "Welcome@123";

                    reqOTP.otpTxnID = obj.otpTxnID;
                    reqOTP.otp = obj.AadhaarOTP;
                    // reqaadhaar.otpMedium = "Y";

                    string data = Newtonsoft.Json.JsonConvert.SerializeObject(reqOTP);
                    //string jsonStringenc = EncryptDencrypt.Encrypt(data, "df0025ae-77d8-4806-aa72-ee7610b00bf5");
                    string jsonStringenc = objChaChaPoly.Encrypt(data, Enckey, reqOTP.txnRequestID);
                    FinalRequest objFinalRequest = new FinalRequest();
                    objFinalRequest.serviceId = "10003";
                    objFinalRequest.data = jsonStringenc;
                    objFinalRequest.reqTxnId = reqOTP.txnRequestID;
                    string finaldata = Newtonsoft.Json.JsonConvert.SerializeObject(objFinalRequest);

                    WebClient wc = new WebClient();
                    string result;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    //result = wc.UploadString("https://authenticate.epramaan.gov.in/authwebservice/verifyaadhaarotpauth", finaldata);
                    result = wc.UploadString(AadhaarOTPVerify, finaldata);
                    string result_decrypt = objChaChaPoly.Decrypt(result, Enckey, reqOTP.txnRequestID);
                    objcm.AadharVerificationLog("verifyaadhaarotpauth", result_decrypt);
                    Responseaadhar objResponseFirst = Newtonsoft.Json.JsonConvert.DeserializeObject<Responseaadhar>(result_decrypt);
                    if ((objResponseFirst.status == null ? "" : objResponseFirst.status) == "true")
                    {
                        objcm.CreateUser(Session["CitizenEmailMobile"].ToString());
                        var namearr = obj.AadhaarName.Trim().Split(' ');
                        var FirstName = "";
                        var MiddleName = "";
                        var LastName = "";
                        if (namearr.Length == 1)
                        {
                            FirstName = obj.AadhaarName.ToUpper();
                        }
                        else if (namearr.Length == 2)
                        {
                            FirstName = namearr[0].ToString().ToUpper();
                            LastName = namearr[1].ToString().ToUpper();
                        }
                        else
                        {
                            FirstName = namearr[0].ToString().ToUpper();
                            MiddleName = namearr[1].ToString().ToUpper();
                            LastName = namearr[2].ToString().ToUpper();

                        }
                        objcm.UpdateAadhaarVerfication(Session["UserID"].ToString(), FirstName, MiddleName, LastName);
                        TempData["SuccessMessage"] = "Aadhaar Verified Sucessfully";
                        Session["UserVerfied"] = null;
                        UserSession.RoleID = "99";
                        return RedirectToAction("CitizenDashboard", "Registration", new { Message = "LoginSuccess", Area = "Public" });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please enter correct OTP or retry aadhaar verification";
                    }
                }
                catch (Exception)
                {

                    TempData["ErrorMessage"] = "Presently Aadhaar verification service is facing some technical issue, Please try after some time";
                }

            }

            obj.OTPFunc = OTPFunc;
            return View(obj);
        }

        public ActionResult RightsofUsers()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonLogin obj = new CommonLogin();
            mHome count = new mHome();
            CommonFunctions objcm = new CommonFunctions();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            ViewData["ITRuleProvision"] = fillITRuleProvisionForHomePage();
            return View(obj);
        }
        public ActionResult PressRelease()
        {
            return View();
        }



        public ActionResult ExistMobileorEmailforChangeMob()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonLogin obj = new CommonLogin();
            CommonFunctions objcm = new CommonFunctions();
            mHome count = new mHome();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ExistMobileorEmailforChangeMob(CommonLogin obj)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonFunctions objcm = new CommonFunctions();
            mHome count = new mHome();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            if (!ValidateCaptcha(obj.Captcha))
            {
                obj.AppellantMobile = "";
                obj.AppellantEmailID = "";
                obj.Captcha = "";
                ModelState.Remove("Captcha");
                ModelState.Clear();
                ModelState.AddModelError("Captcha", "Invalid Captcha");

                return View(obj);
            }
            obj.Captcha = "";
            ModelState.Remove("Captcha");
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@Mobile", obj.AppellantMobile));
            methodParameter.Add(new KeyValuePair<string, string>("@Email", obj.AppellantEmailID));
            var ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "CheckCitizenforNewMobile", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows[0]["statusCode"].ToString() == "Y")
                    {
                        Session["CitizenEmailMobile"] = obj.AppellantEmailID;
                        Session["OTPID"] = dt.Rows[0]["MobileOTPID"].ToString();
                        Session["UserID"] = dt.Rows[0]["UserID"].ToString();
                        SendSMS smsObj = new SendSMS();
                        string emailSubject = "Grievance Appellate Committee : Change Mobile Number Verification OTP";
                        string emailMsg = dt.Rows[0]["Msg"].ToString();
                        smsObj.SendMailToActualUser(emailMsg, emailSubject, obj.AppellantEmailID, "Grievance Appellate Committee ", obj.AppellantEmailID);
                        return RedirectToAction("OTPVerficationforNewMobile", "Home");
                    }
                    else
                    {
                        obj.AppellantMobile = "";
                        obj.AppellantEmailID = "";
                        obj.Captcha = "";
                        ModelState.Clear();
                        ModelState.AddModelError("validationMessage", "Please enter valid credentials");
                        return View(obj);
                    }
                }
            }
            ModelState.AddModelError("validationMessage", "Please enter valid credentials");
            return View(obj);
        }
        [HttpGet]
        public ActionResult OTPVerficationforNewMobile()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            if (Session["CitizenEmailMobile"] == null)
            {
                return RedirectToAction("Index");
            }
            CommonLogin obj = new CommonLogin();
            CommonFunctions objcm = new CommonFunctions();
            mHome count = new mHome();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            obj.AppellantEmailID = Session["CitizenEmailMobile"].ToString();
            obj.OTPID = Session["OTPID"].ToString();
            GenerateKeySalt();
            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult OTPVerficationforNewMobile(CommonLogin obj)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            if (Session["CitizenEmailMobile"] == null)
            {
                return RedirectToAction("Index");
            }
            obj.OTP = "";
            ModelState.Remove("OTP");

            obj.OTPEnc = Decrypt(obj.OTPEnc.ToString(), Session["EncKeySalt"].ToString());
            CommonFunctions objcm = new CommonFunctions();
            mHome count = new mHome();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            if (!ValidateCaptcha(obj.Captcha))
            {
                obj.Captcha = "";
                ModelState.Remove("Captcha");
                ModelState.AddModelError("Captcha", "Invalid Captcha");
                return View(obj);
            }
            ModelState.Remove("Captcha");
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@Email", obj.AppellantEmailID));
            methodParameter.Add(new KeyValuePair<string, string>("@OTPID", obj.OTPID));
            methodParameter.Add(new KeyValuePair<string, string>("@OTP", obj.OTPEnc));
            var ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "CheckEmailOTPforNewMobile", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows[0]["OTPVerfied"].ToString() == "Y")
                    {

                        return RedirectToAction("UpdateNewMobile", "Home");
                    }
                }
            }
            ModelState.AddModelError("validationMessage", "Invalid OTP");
            return View(obj);
        }

        [HttpGet]
        public ActionResult UpdateNewMobile()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            if (Session["CitizenEmailMobile"] == null)
            {
                return RedirectToAction("Index");
            }
            CommonLogin obj = new CommonLogin();
            CommonFunctions objcm = new CommonFunctions();
            mHome count = new mHome();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            obj.AppellantEmailID = Session["CitizenEmailMobile"].ToString();

            return View(obj);
        }
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateNewMobile(CommonLogin obj)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            CommonFunctions objcm = new CommonFunctions();
            mHome count = new mHome();
            count = objcm.DashboardCount();
            obj.Submitted = count.Submitted;
            obj.Disposed = count.Disposed;
            ViewData["LanguageMaster"] = objcm.fillLanguageMaster();
            if (!ValidateCaptcha(obj.Captcha))
            {
                obj.Captcha = "";
                ModelState.Remove("Captcha");
                ModelState.AddModelError("Captcha", "Invalid Captcha");

            }
            obj.Captcha = "";
            ModelState.Remove("Captcha");
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserID", Session["UserID"].ToString()));
            methodParameter.Add(new KeyValuePair<string, string>("@NewMobile", obj.AppellantMobile));
            var ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "UpdateCitizenMobile", methodParameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows[0]["statusCode"].ToString() == "Y")
                    {
                        TempData["SuccessMessage"] = dt.Rows[0]["statusMessage"].ToString();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("validationMessage", dt.Rows[0]["statusMessage"].ToString());
                    }

                }
            }
            ModelState.AddModelError("validationMessage", "Mobile number not updated, Please try again later");
            return View(obj);
        }
        public JsonResult getResendOTP(string EmailorMobile, string func)
        {
            var objUserOTPVerfication = new UserOTPVerfication();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@UserEmailMobile", Session["CitizenEmailMobile"].ToString()));
            methodParameter.Add(new KeyValuePair<string, string>("@EmailorMobile", EmailorMobile));
            methodParameter.Add(new KeyValuePair<string, string>("@Func", func));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "GetResendOTP_new", methodParameter);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                objUserOTPVerfication.OTPID = dt.Rows[0]["OTPID"].ToString();
                //objUserOTPVerfication.OTP = dt.Rows[0]["OTP"].ToString();
                if (dt.Rows[0]["OTPID"].ToString() == "0")
                {
                    objUserOTPVerfication.Msg = dt.Rows[0]["Msg"].ToString();
                }



                Session["OTPID"] = objUserOTPVerfication.OTPID;
                // SEND SMS
                if (EmailorMobile == "M")
                {
                    if (dt.Rows[0]["OTPID"].ToString() == "0" && dt.Rows[0]["OTP"].ToString() == "0")
                    {
                        return Json(objUserOTPVerfication);
                    }
                    else
                    {

                        SendSMS objmSendSMS = new SendSMS();
                        objmSendSMS.send_SMS(Session["CitizenEmailMobile"].ToString(), dt.Rows[0]["Msg"].ToString(), dt.Rows[0]["TemplateID"].ToString(), "O");
                    }
                }
                else
                {

                    SendSMS smsObj = new SendSMS();
                    string emailSubject = "Grievance Appellate Committee : Login Verification OTP";
                    string emailMsg = dt.Rows[0]["Msg"].ToString();
                    if (func == "InterLogin")
                    {
                        smsObj.SendMailToUser(emailMsg, emailSubject, Session["CitizenEmailMobile"].ToString(), "Grievance Appellate Committee ", Session["CitizenEmailMobile"].ToString());
                    }
                    else if (func == "NewMobile")
                    {
                        emailSubject = "Grievance Appellate Committee : Change Mobile Number Verification OTP";
                        smsObj.SendMailToActualUser(emailMsg, emailSubject, Session["CitizenEmailMobile"].ToString(), "Grievance Appellate Committee ", Session["CitizenEmailMobile"].ToString());
                    }
                    else
                    {
                        smsObj.SendMailToActualUser(emailMsg, emailSubject, Session["CitizenEmailMobile"].ToString(), "Grievance Appellate Committee ", Session["CitizenEmailMobile"].ToString());
                    }

                }

            }
            return Json(objUserOTPVerfication);
        }


        public List<IntermediaryMaster> fillIntermediaryMaster()
        {
            var TempList = new List<IntermediaryMaster>();
            var methodParameter = new List<KeyValuePair<string, string>>();
            methodParameter.Add(new KeyValuePair<string, string>("@IntermediaryTitle", ""));
            methodParameter.Add(new KeyValuePair<string, string>("@URL", ""));
            methodParameter.Add(new KeyValuePair<string, string>("@func", "All"));

            var ds = ServiceAdaptor.GetDataSetFromService("HPPSC", HttpContext.Request.Url.Host.ToString(), "SelectMSSql", "getIntermediaryMaster", methodParameter);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TempList.Add(new IntermediaryMaster
                        {
                            IntermediaryId = Convert.ToString(dr["IntermediaryId"]),
                            IntermediaryTitle = Convert.ToString(dr["IntermediaryTitle"]),
                            URL = Convert.ToString(dr["URL"]),
                            GOEmail = Convert.ToString(dr["GOEmail"]),
                            Address = Convert.ToString(dr["Address"]),
                            GOName = Convert.ToString(dr["GOName"]),
                            IsActive = Convert.ToString(dr["IsActive"]),
                            HelpLink = Convert.ToString(dr["HelpLink"])
                        });
                    }

                }
            }
            return TempList;
        }


        public List<ITRuleProvisionForHomePage> fillITRuleProvisionForHomePage()
        {

            var methodParameter = new List<KeyValuePair<string, string>>();
            List<ITRuleProvisionForHomePage> tmpList = new List<ITRuleProvisionForHomePage>();
            DataSet ds = ServiceAdaptor.GetDataSetFromService("HPPSC", "", "SelectMSSql", "getITRuleProvisionForHomePage", methodParameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable evdt = ds.Tables[0];
                    foreach (DataRow r in evdt.Rows)
                    {
                        tmpList.Add(new ITRuleProvisionForHomePage
                        {
                            itrule = Convert.ToString(r["itrule"]),
                            extractOfRule = Convert.ToString(r["extractOfRule"])
                        });
                    }




                }
            }
            return tmpList;
        }

        public ActionResult About()
        {
            //CMSDataAddDocument obj = new CMSDataAddDocument();
            //CMSDataController ControllerObj = new CMSDataController();
            //try
            //{
            //    obj.DocumnetListTable = ControllerObj.GetDocumnetListTable_List("0");
            //}
            //catch (Exception ex) { MyExceptionHandler.LogError(ex); }
            //turn View(obj);
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult GACContent()
        {
            return View();
        }
        public ActionResult GACWorkflow()
        {
            return View();
        }
        public ActionResult FileAppeal()
        {
            return View();
        }
        public ActionResult ViewStatus()
        {
            return View();
        }
        public ActionResult GroundofAppeal()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Error()
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            return View();
        }

        #region captchaRegion
        public ActionResult CaptchaImageNew(bool noisy = true)
        {
            FileContentResult img = null;
            if (Request.UrlReferrer != null)
            {
                StringBuilder randomText = new StringBuilder();
                randomText.Append(CommonRepository.RandomStringGenerate(6));
                Session["RegisterCaptcha"] = "";
                Session["RegisterCaptcha"] = Convert.ToString(randomText);
                using (var mem = new MemoryStream())
                using (var bmp = new Bitmap(100, 30))
                using (var gfx = Graphics.FromImage((System.Drawing.Image)bmp))
                {
                    gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    gfx.SmoothingMode = SmoothingMode.AntiAlias;
                    gfx.FillRectangle(Brushes.Aquamarine, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));
                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 100, 30);
                    HatchBrush hBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightGray, Color.White);
                    gfx.FillRectangle(hBrush, rect);
                    hBrush = new HatchBrush(HatchStyle.LargeConfetti, Color.Green, Color.DarkBlue);
                    float fontSize = 16;
                    System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Strikeout);
                    float x = 10;
                    float y = 1;
                    PointF fPoint = new PointF(x, y);
                    gfx.DrawString(randomText.ToString(), font, hBrush, fPoint);
                    //gfx.DrawString("9999", font, hBrush, fPoint);
                    bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                    img = this.File(mem.GetBuffer(), "image/Jpeg");
                }
            }
            else
            {
                Session["RegisterCaptcha"] = "";
                return RedirectToAction("Error", "Home");
            }

            return img;
        }

        public JsonResult CheckCaptcha(string CaptchaCD)
        {
            return Json(!(ValidateCaptcha(CaptchaCD)) ? false : true, JsonRequestBehavior.AllowGet);
        }
        bool ValidateCaptcha(string CaptchaText)
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(CaptchaText))
            {
                if (Session["RegisterCaptcha"] != null)
                {
                    if (CaptchaText.Trim() == Session["RegisterCaptcha"].ToString())
                    {
                        Session["RegisterCaptcha"] = "";
                        result = true;
                    }
                }
            }
            return result;
        }
        #endregion
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult LoginViaPassword(LoginviaPassword obj)
        {
            ViewData["LabelDictionary"] = FormLabels.GetFormLabels(FormLabels.Module.Appellant, "1", UserSession.LangCulture);
            try
            {
                TempData["Status"] = "F";
                string language = string.Empty;
                if (UserSession.LangCulture != null)
                {
                    language = UserSession.LangCulture;
                    Session.Clear();
                    UserSession.LangCulture = language;
                }
                List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@UserID", obj.EmailID));
                methodparameter.Add(new KeyValuePair<string, string>("@Password", obj.Password));
                methodparameter.Add(new KeyValuePair<string, string>("@Seed", obj.Seed));
                methodparameter.Add(new KeyValuePair<string, string>("@HashPassword", obj.HashPassword));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AuthUser", methodparameter);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            if (dr["Msg"].ToString() == "Not Authorized")
                            {
                                TempData["Status"] = "F";
                                return RedirectToAction("Login", "Home");
                            }
                            else if (dr["Msg"].ToString() == "Valid User")
                            {
                                TempData["Status"] = null;
                                UserSession.isSSOLogin = "N";
                                UserSession.UserID = dr["Userid"].ToString();
                                UserSession.UserName = dr["UserName"].ToString();
                                UserSession.EmailId = dr["EmailId"].ToString();
                                UserSession.Mobile = dr["Mobile"].ToString();
                                UserSession.Designation = dr["Designation"].ToString();
                                UserSession.RoleID = dr["UserRole"].ToString();
                                UserSession.RoleName = dr["RoleName"].ToString();
                                UserSession.GACID = dr["GACID"].ToString();
                                UserSession.GACName = dr["GACName"].ToString();
                                UserSession.RoleTypeId = dr["RoleTypeId"].ToString();
                                UserSession.RoleTypeName = dr["RoleTypeName"].ToString();
                                UserSession.UserType = dr["UserType"].ToString();
                                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, UserSession.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, UserSession.RoleID, "/");
                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                //cookie.Secure = true;
                                //cookie.HttpOnly = true;
                                Response.AppendCookie(cookie);
                                string ID = InsertLoginStatus(obj.EmailID, "Success", "Successful Login", "GAC");
                                if (Convert.ToInt32(ID) > 0)
                                {
                                    Session["LogoutID"] = ID;
                                    if (UserSession.RoleID == "1")// Admin
                                    {
                                        return RedirectToAction("Index", "AdminDashboard", new { Area = "Dashboard" });
                                    }
                                    else if (UserSession.RoleID == "2")// Secretariat
                                    {
                                        return RedirectToAction("Index", "AdminDashboard", new { Area = "Dashboard" });
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
                                    }

                                }
                                else
                                {
                                    TempData["Status"] = "F";
                                    return RedirectToAction("Login", "Home");
                                }
                            }
                        }
                        else
                        {
                            TempData["Status"] = "F";
                            return RedirectToAction("Login", "Home");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return RedirectToAction("Login", "Home");
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        private string InsertLoginStatus(string UserID, string AuthenticationStatus, string AuthenticationMessage, string UserType)
        {
            try
            {
                string ID = "0";
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@UserID", UserID));
                methodparameter.Add(new KeyValuePair<string, string>("@AuthenticationStatus", AuthenticationStatus));
                methodparameter.Add(new KeyValuePair<string, string>("@AuthenticationMessage", AuthenticationMessage));
                methodparameter.Add(new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()));
                methodparameter.Add(new KeyValuePair<string, string>("@UserType", UserType));
                DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "InsertLoginStatus", methodparameter);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ID = ds.Tables[0].Rows[0]["ID"].ToString();
                }

                return ID;
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return "0";
            }
        }
        public string Encrypt(String plainText, String SecretKey)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            try
            {
                return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(SecretKey)));
            }
            catch
            {
                return "";
            }
        }

        public string Decrypt(String encryptedText, String SecretKey)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(SecretKey)));
            }
            catch
            {
                return "";
            }
        }

        public byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }
    }
}
//UserSession.UserRoleID
