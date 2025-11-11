using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DigitalNagrik.Models
{
    public class BALCommon
    {
        public class CustomClientAlerts
        {
            public class AlertTypeFormat { public static readonly string Success = "success", Error = "error", Info = "info"; }
            private static readonly Dictionary<string, string> _AlertIconClasses = new Dictionary<string, string>() { { "success", "bx-check-circle" }, { "error", "bx-message-x" }, { "info", "bx-info-circle" } };
            private static readonly Dictionary<string, string> _AlertColorClass = new Dictionary<string, string>() { { "success", "text-success" }, { "error", "text-danger" }, { "info", "text-info" } };

            public string AlertType { get; set; }
            public string AlertIcon { get; set; }
            public string AlertColor { get; set; }
            public string AlertTitle { get; set; }
            public string AlertMessage { get; set; }

            public static CustomClientAlerts GenerateAlert(string AlertTypeFormat, string AlertTitle, string AlertMessage)
            {
                CustomClientAlerts Alert = new CustomClientAlerts
                {
                    AlertTitle = AlertTitle,
                    AlertMessage = AlertMessage,
                    AlertType = AlertTypeFormat,
                    AlertIcon = _AlertIconClasses[AlertTypeFormat],
                    AlertColor = _AlertColorClass[AlertTypeFormat],
                };
                return Alert;
            }

            public static CustomClientAlerts GenerateErrorAlert()
            {
                CustomClientAlerts Alert = new CustomClientAlerts
                {
                    AlertTitle = "Couldn't Process Request",
                    AlertMessage = "Failed to process your request at the moment. Please try again later. Contact your administrator if problem persists.",
                    AlertType = AlertTypeFormat.Error,
                    AlertIcon = _AlertIconClasses[AlertTypeFormat.Error],
                    AlertColor = _AlertColorClass[AlertTypeFormat.Error],
                };
                return Alert;
            }
        }
        public static string Trim(string Item)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Item))
                    return Item.Trim();
            }
            catch (Exception ex) { }
            return Item;
        }
        public static string RenderRazorViewToString(string viewName, ControllerContext c_context, ViewDataDictionary viewData, TempDataDictionary tempData, object model = null)
        {
            using (var sw = new StringWriter())
            {
                try
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(c_context, viewName);
                    viewData.Model = model;
                    var viewContext = new ViewContext(c_context, viewResult.View, viewData, tempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(c_context, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
                catch (Exception ex) { MyExceptionHandler.HandleException(ex); }
                return string.Empty;
            }
        }
        private static string GetMacAddress()
        {
            try
            {
                string macAddresses = string.Empty;

                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        macAddresses += nic.GetPhysicalAddress().ToString();
                        break;
                    }
                }

                return macAddresses;
            }
            catch
            {
                return "";

            }
        }
        public static byte[] GetFTPFileBytes(string FilePath, string FileType)
        {
            string ftp = ConfigurationManager.AppSettings["FTPServerAddress"];
            string ftpUserName = ConfigurationManager.AppSettings["FTPUserID"];
            string ftpPassword = ConfigurationManager.AppSettings["FTPUserPassword"];
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + FilePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                var tm = FilePath.Split('0');
                string contentType = "application/" + FileType.Replace(".", "").ToLower().ToString();
                string fileNameDisplayedToUser = FilePath.Split('/')[2];
                byte[] bytes = new byte[] { };
                using (MemoryStream stream = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(stream);
                    bytes = stream.ToArray();
                }
                ftpStream.Close();
                return bytes;
            }
            catch (WebException ex) { }
            return new byte[] { };
        }

        public static DataTable UpdateExcelDataTable(DataTable dataTable, string[] ColumnNamesDB, string[] ColumnNamesNew, string[] ColumnsToRemove)
        {
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i <= ColumnNamesDB.Count() - 1; i++)
                    dataTable.Columns[ColumnNamesDB[i]].ColumnName = ColumnNamesNew[i];
                if (ColumnsToRemove.Count() > 0)
                {
                    for (int i = 0; i <= ColumnsToRemove.Count() - 1; i++)
                        dataTable.Columns.Remove(ColumnsToRemove[i]);
                }
            }
            return dataTable;
        }

        private static string GetBrowserID()
        {
            try

            {
                return HttpContext.Current.Request.Browser.Browser.ToString() + HttpContext.Current.Request.Browser.Type.ToString();
            }

            catch
            {
                return "";
            }
        }

        public static string GetIPAddress()
        {
            //return HttpContext.Current.Request.UserHostAddress;
            try
            {

                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        return addresses[0];
                    }
                }
                return context.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch
            {
                return "";
            }

        }
        public static string ConvertMinToDHM(string Minutes)
        {
            if (double.TryParse(Minutes, out double Mins))
            {
                double Day = Math.Floor(Mins / (60 * 24));
                double Hour = Math.Floor((Mins - (Day * 60 * 24)) / 60);
                double Min = Math.Round(Mins % 60);
                return (Day != 0 ? "<span>" + Day + "d </span>" : "") + (Hour != 0 ? "<span>" + Hour + "h </span>" : "") + (Min != 0 || (Day == 0 && Hour == 0 && Min == 0) ? "<span>" + Min + "m </span>" : "");
            }
            return "<span>0m</span>";
        }

        public static string GetSystemID()
        {
            try
            {
                //return "DESKTOP-" + GetMacAddress() + GetIPAddress() + GetBrowserID();
                return "DESKTOP-" + GetIPAddress() + GetBrowserID();
            }
            catch
            {
                return "";
            }
        }
        public static string EncryptAES(string plainText, string key)
        {
            try
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(Encrypt(plainBytes, getRijndaelManaged(key)));
            }
            catch
            {
                return "";
            }
        }

        public static string DecryptAES(string encryptedText, string key)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(Decrypt(encryptedBytes, getRijndaelManaged(key)));
            }
            catch
            {
                return "";
            }
        }

        private static RijndaelManaged getRijndaelManaged(string secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
        public static string GetQSDecryptedValue(string Key, Dictionary<string, string> Parameters)
        {
            string Value = string.Empty;
            try
            {
                if (Parameters.ContainsKey(Key))
                {
                    Value = Parameters[Key];
                }
            }
            catch { }
            return Value;
        }


        public static string ParichayAPI_HMAC(string InputString)
        {
            try
            {
                string json, Result1 = string.Empty;
                json = "{\"HmacString\":\"" + InputString + "\"}";
                // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + ConfigurationManager.AppSettings["VMIP"].ToString() + ":" + ConfigurationManager.AppSettings["SSOAPIPort"].ToString() + "/hmac");
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 1;
                request.ContentType = "application/json";
                request.Method = "POST";
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.AllowAutoRedirect = true;
                request.MaximumAutomaticRedirections = 10;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                HMACResponse dtResult = new HMACResponse();
                string result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
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
                        result = sb.ToString();
                    }
                }
                dtResult = (HMACResponse)Newtonsoft.Json.JsonConvert.DeserializeObject(result, (typeof(HMACResponse)));               
                if (dtResult.status.Equals("success"))
                Result1 = dtResult.data.signature;
                httpResponse.Close();
                return Result1;
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return string.Empty;
            }
        }

        public static string ParichayAPI_Encryption(string InputString)
        {
            try
            {
                string json, Result1 = string.Empty;

                json = "{\"AESString\":\"" + InputString + "\"}";

                // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + ConfigurationManager.AppSettings["VMIP"].ToString() + ":" + ConfigurationManager.AppSettings["SSOAPIPort"].ToString() + "/encryption");
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 1;
                request.ContentType = "application/json";
                request.Method = "POST";
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.AllowAutoRedirect = true;
                request.MaximumAutomaticRedirections = 10;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                HMACResponse dtResult = new HMACResponse();
                string result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    int usrSleepTime = Int32.Parse("60");
                    if (usrSleepTime >= 60 && usrSleepTime <= 120)
                    {
                        Thread.Sleep(usrSleepTime);
                    }
                    else
                    {
                        throw new Exception("Invalid sleep duration");
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
                            throw new Exception("input too long");
                        }
                        sb.Append(c);
                        result = sb.ToString();
                    }
                }
                dtResult = (HMACResponse)Newtonsoft.Json.JsonConvert.DeserializeObject(result, (typeof(HMACResponse)));               
                if (dtResult.status.Equals("success"))
                    Result1 = dtResult.data.signature;
                httpResponse.Close();
                return Result1;
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return string.Empty;
            }
        }

        public static Signature ParichayAPI_decryption(string InputString)
        {
            try
            {
                string json;
                json = "{\"EncryptedString\":\"" + InputString + "\"}";
                // System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + ConfigurationManager.AppSettings["VMIP"].ToString() + ":" + ConfigurationManager.AppSettings["SSOAPIPort"].ToString() + "/decryption");
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 1;
                request.ContentType = "application/json";
                request.Method = "POST";
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.AllowAutoRedirect = true;
                request.MaximumAutomaticRedirections = 10;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                Root dtResult = new Root();
                Signature dtResult1 = new Signature();
                string result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
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
                        result = sb.ToString();
                    }
                }
                dtResult = (Root)Newtonsoft.Json.JsonConvert.DeserializeObject(result, (typeof(Root)));

                if (dtResult.status.Equals("success"))
                    dtResult1 = dtResult.data.signature;
                httpResponse.Close();
                return dtResult1;
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                return null;
            }
        }

    }
    public class HMACResponse
    {
        public output data { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
    public class output
    {
        public string signature { get; set; }
    }

    public class Signature
    {
        public string authId { get; set; }
        public string authRole { get; set; }
        public string browserId { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string departmentName { get; set; }
        public string designation { get; set; }
        public string email { get; set; }
        public string employeeCode { get; set; }
        public string expiresAt { get; set; }
        public string firstName { get; set; }
        public string fullName { get; set; }
        public string handShakingId { get; set; }
        public string ip { get; set; }
        public string lastName { get; set; }
        public string localTokenId { get; set; }
        public string location { get; set; }
        public List<string> mailAlternateAddress { get; set; }
        public List<string> mailEquivalentAddress { get; set; }
        public string mobileNo { get; set; }
        public string nicaccountexpdate { get; set; }
        public string parichayId { get; set; }
        public string role { get; set; }
        public string sessionId { get; set; }
        public string state { get; set; }
        public string status { get; set; }
        public string subservice { get; set; }
        public string ua { get; set; }
        public string userName { get; set; }
        public string user_id { get; set; }
        public string zimOtp { get; set; }
    }

    public class Datasign
    {
        public Signature signature { get; set; }
    }
    public class Root
    {
        public string status { get; set; }
        public string message { get; set; }
        public Datasign data { get; set; }
    }
}