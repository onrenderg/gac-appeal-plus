using DigitalNagrik.Models;
using NICServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DigitalNagrik.Controllers
{
    public class SSOLoginController : Controller
    {
        // GET: SSOLogin
        [AllowAnonymous]
        public ActionResult Index(string String)
        {
            //-----------------------------------------------------------------
            UserLogin Login = new UserLogin();
            try
            {
                if (String != null)
                {
                    TempData["Status"] =null;
                    string RES = String;
                    if (RES == "No")
                    {
                        return RedirectToAction("GAC", "Home");
                    }
                    else
                    {
                        string SSOServiceURL = "http://" + ConfigurationManager.AppSettings["VMIP"].ToString() + ":" + ConfigurationManager.AppSettings["SSOAPIPort"].ToString() + "/handshake/" + RES + "/GACePortal";

                        // Usage
                        if (!CommonRepository.IsValidUrl(SSOServiceURL))    
                        {
                            TempData["Status"] = "F";
                            return RedirectToAction("GAC", "Home");
                        }

                        //HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create("http://" + ConfigurationManager.AppSettings["VMIP"].ToString() + ":" + ConfigurationManager.AppSettings["SSOAPIPort"].ToString() + "/handshake/" + RES + "/GACePortal");
                        HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(SSOServiceURL);
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                        webRequest.KeepAlive = false;
                        webRequest.Method = "GET";
                        webRequest.ProtocolVersion = HttpVersion.Version10;
                        webRequest.AllowAutoRedirect = true;
                        webRequest.MaximumAutomaticRedirections = 10;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        WebResponse response1 = webRequest.GetResponse();
                        string ES = string.Empty;
                        using (var streamReader = new StreamReader(response1.GetResponseStream(), Encoding.UTF8))
                        {
                            int usrSleepTime = Int32.Parse("60");
                            if (usrSleepTime >= 60 && usrSleepTime <= 120)
                            {
                                Thread.Sleep(usrSleepTime);
                            }
                            else
                            {
                                throw new Exception("Invalid Sleep Duration");
                            }
                            int intC;
                            StringBuilder sb = new StringBuilder();
                            while ((intC = streamReader.Read()) != -1)
                            {
                                char c = (char)intC;
                                if (c == '\n')
                                {
                                    break;
                                }
                                if (sb.Length >= 4000)
                                {
                                    throw new Exception("Input too Long");
                                }
                                sb.Append(c);
                                ES = sb.ToString();
                            }
                        }
                        if (ES.ToLower().Equals("false"))
                        {
                            TempData["Status"] = "F";
                            return RedirectToAction("GAC", "Home");
                        }
                        else
                        {
                            //return RedirectToAction("InitializeParams.aspx?string=" + ES);
                            Signature Signature = BALCommon.ParichayAPI_decryption(ES);
                            string UserId = Signature.userName;
                            string status = Signature.status;
                            Session.Clear();
                            List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>();
                            methodparameter.Add(new KeyValuePair<string, string>("@UserID", UserId));
                            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AuthSSOUser", methodparameter);
                            if (ds != null)
                            {
                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        DataRow dr = ds.Tables[0].Rows[0];
                                        TempData["Status"] = null;
                                        UserSession.isSSOLogin = "Y";
                                        UserSession.UserID =Convert.ToString(dr["Userid"]);
                                        UserSession.UserName = Convert.ToString(dr["UserName"]);
                                        UserSession.EmailId = Convert.ToString(dr["EmailId"]);
                                        UserSession.Mobile = Convert.ToString(dr["Mobile"]);
                                        UserSession.Designation = Convert.ToString(dr["Designation"]);
                                        UserSession.RoleID = Convert.ToString(dr["UserRole"]);
                                        UserSession.RoleName = Convert.ToString(dr["RoleName"]);
                                        UserSession.GACID = Convert.ToString(dr["GACID"]);
                                        UserSession.GACName = Convert.ToString(dr["GACName"]);
                                        UserSession.RoleTypeId = Convert.ToString(dr["RoleTypeId"]);
                                        UserSession.RoleTypeName = Convert.ToString(dr["RoleTypeName"]);
                                        UserSession.UserType = Convert.ToString(dr["UserType"]);
                                        UserSession.sessionId = Signature.sessionId;
                                        UserSession.service = "GACePortal"; // 'Cred.Item("service")
                                        UserSession.localTokenId = Signature.localTokenId;
                                        UserSession.browserId = Signature.browserId;
                                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, UserSession.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), false, UserSession.RoleID, "/");
                                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                        //cookie.Secure = true;
                                        //cookie.HttpOnly = true;
                                        Response.AppendCookie(cookie);
                                        string ID = InsertLoginStatus(UserSession.EmailId, "Success", "Successful Login");
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
                                                //return RedirectToAction("Index", "AdminDashboard", new { Area = "Dashboard" });
                                                return RedirectToAction("Index", "GACGridViewInbox", new { Area = "GridView" });
                                            }

                                        }
                                        else
                                        {
                                            TempData["Status"] = "F";
                                            return RedirectToAction("GAC", "Home");
                                        }
                                    }
                                    else
                                    {
                                        TempData["Status"] = "F";
                                        return RedirectToAction("GAC", "Home");
                                    }

                                }
                            }

                        }
                    }
                }
                else
                {
                    TempData["Status"] = "F";
                    return RedirectToAction("GAC", "Home");
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return RedirectToAction("GAC", "Home");
            }
            return RedirectToAction("GAC", "Home");
        }      
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                return true;
            return true;
        }
        public static bool RemoteCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors)
                return false;
            else if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch) == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                //Zone z;
                //z = Zone.CreateFromUrl((HttpWebRequest)sender.RequestUri.ToString());
                //if ((z.SecurityZone == System.Security.SecurityZone.Intranet | z.SecurityZone == System.Security.SecurityZone.MyComputer))
                return true;
                //return false;
            }
            else
                return true;
        }
        public static void Check_SSL_Certificate()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertificateValidate);
        }
        public string InsertLoginStatus(string UserID, string AuthenticationStatus, string AuthenticationMessage)
        {
            try
            {
                string ID = "0";
                var methodparameter = new List<KeyValuePair<string, string>>();
                methodparameter.Add(new KeyValuePair<string, string>("@UserID", UserID));
                methodparameter.Add(new KeyValuePair<string, string>("@AuthenticationStatus", AuthenticationStatus));
                methodparameter.Add(new KeyValuePair<string, string>("@AuthenticationMessage", AuthenticationMessage));
                methodparameter.Add(new KeyValuePair<string, string>("@IPAddress", BALCommon.GetIPAddress()));
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
     
        [AllowAnonymous]
        public ActionResult ParichayCall()
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.Cache.SetExpires(DateTime.Now - new TimeSpan(1, 0, 0));
                Response.Cache.SetLastModified(DateTime.Now);
                Response.Cache.SetAllowResponseInBrowserHistory(false);
                string HMAC = BALCommon.ParichayAPI_HMAC("Parichay1622544184996https://parichay.staging.nic.in/pnv1/api/loginGACePortal");
                string ENC = BALCommon.ParichayAPI_Encryption(Session.SessionID);
                return Redirect("https://parichay.staging.nic.in/pnv1/api/login?service=GACePortal&tid=1622544184996&cs=" + HMAC + "&string=" + ENC);
            }
            // Response.Write(BALCommon.ParichayAPI_HMAC("123"))
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return RedirectToAction("Index", "Home");
            }
        }
        //Citizen Authenticate
        public ActionResult CitizenValidate(string UserID)
        {
            List<KeyValuePair<string, string>> methodparameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("@UserID", UserID)
            };
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "AuthCitizenUser", methodparameter);
            if (ds != null)
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["Msg"].ToString() == "Valid User")
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        UserSession.UserID = dr["UserID"].ToString();
                        UserSession.UserName = dr["UserName"].ToString();
                        UserSession.EmailId = dr["EmailID"].ToString();
                        UserSession.Mobile = dr["Mobile"].ToString();
                        UserSession.RoleID = "99";
                        UserSession.RoleName = "Citizen";
                        string ID = InsertLoginStatus(UserID, "success", "Successful Login");
                        if (Convert.ToInt32(ID) > 0)
                        {
                            Session["LogoutID"] = ID;
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, UserSession.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), false, UserSession.RoleID, "/");
                            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                            //cookie.Secure = true;
                            //cookie.HttpOnly = true;
                            Response.AppendCookie(cookie);
                        }
                        return RedirectToAction("Index", "DashboardCitizen");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        //Citizen Authenticate
    }
}