using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Data;
using NICServiceAdaptor;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Policy;

namespace DigitalNagrik.Models
{
    public class SendSMS
    {
        public string send_SMS(string mobileNo, string msgString, string template_id, string SMSType)
        {
            string result = "";
            string userID = "";
            string pwd = "";
            string senderID = "";
            string entity_id = "";
            string SendSMSYN = "";
            try
            {
                if (SMSType == "S")
                {
                    userID = ConfigurationManager.AppSettings["userIDSMS"];
                }
                else
                {
                    userID = ConfigurationManager.AppSettings["userIDOTP"];
                }
                pwd = ConfigurationManager.AppSettings["pwdSMS"];
                senderID = ConfigurationManager.AppSettings["senderIDSMS"];
                entity_id = ConfigurationManager.AppSettings["entity_id"];
                SendSMSYN = ConfigurationManager.AppSettings["SendSMSYN"];
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                result = "Invalid Input Parameters";
                return result;
            }

            if (SendSMSYN == "N")
            {
                return "";
            }
            try
            {
                if (Regex.IsMatch(mobileNo, @"\d{10}"))
                {
                    if (mobileNo.Trim().Length == 10)
                        mobileNo = "91" + mobileNo;
                    Check_SSL_Certificate();
                    Stream dataStream;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://smsgw.sms.gov.in/failsafe/HttpLink?");
                    request.ProtocolVersion = HttpVersion.Version10;
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";
                    request.Method = "POST";

                    try
                    {
                        var unicode = "";
                        if (IsUnicode(msgString))
                        {
                            unicode = "&msgType=UC";
                            msgString = ConvertStringToHex(msgString, System.Text.Encoding.BigEndianUnicode);
                        }
                        string query = "username=" + HttpUtility.UrlEncode(userID) + "&pin=" + HttpUtility.UrlEncode(pwd) + "&message=" + HttpUtility.UrlEncode(msgString) + "&mnumber=" + HttpUtility.UrlEncode(mobileNo) + "&signature=" + HttpUtility.UrlEncode(senderID) + "&dlt_entity_id=" + HttpUtility.UrlEncode(entity_id) + "&dlt_template_id=" + HttpUtility.UrlEncode(template_id) + unicode;

                        byte[] byteArray = Encoding.ASCII.GetBytes(query);
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = byteArray.Length;
                        dataStream = request.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();
                        WebResponse response = request.GetResponse();
                        string Status = ((HttpWebResponse)response).StatusDescription;
                        dataStream = response.GetResponseStream();
                        string str = string.Empty;
                        using (StreamReader sr = new StreamReader(dataStream))
                        {
                            int intC;
                            while ((intC = sr.Read()) != -1)
                            {
                                char c = (char)intC;
                                StringBuilder sb = new StringBuilder();
                                if (c == '\n')
                                {
                                    break;
                                }
                                if (sb.Length >= 5000)
                                {
                                    throw new Exception("input too long");
                                }
                                sb.Append(c);
                                str = sb.ToString();
                            }
                        }
                        //reader.Close();
                        dataStream.Close();
                        result = Status.ToString();
                    }
                    catch (Exception ex)
                    {
                        result = "Msg Not Sent: " + ex.Message.ToString();
                        MyExceptionHandler.LogError(ex);
                    }
                }
                else
                    result = "Invalid Mobile Number";
            }
            catch (Exception ex)
            {
                result = "Please Try Later";
                MyExceptionHandler.LogError(ex);
            }

            return result;
        }
        public static void Check_SSL_Certificate()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidate);
        }

        public static bool RemoteCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors))
                return false;
            else if (((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch))
            {
                Zone z = null/* TODO Change to default(_) if this is not a reference type */;
                z = Zone.CreateFromUrl(((HttpWebRequest)sender).RequestUri.ToString());

                if ((z.SecurityZone == System.Security.SecurityZone.Intranet | z.SecurityZone == System.Security.SecurityZone.MyComputer))
                    return true;

                return false;
            }
            else
                return true;
        }
        public bool IsUnicode(string input)
        {
            var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
            var unicodBytesCount = Encoding.UTF8.GetByteCount(input);
            return asciiBytesCount != unicodBytesCount;
        }
        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);

            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);

            foreach (byte b in stringBytes)

                sbBytes.AppendFormat("{0:X2}", b);

            return sbBytes.ToString();
        }


        #region Send email
        public string SendMailToUser(string MyEmailContent, string EmailSubject, string eMAILID, string OrganisationName, string UserID)
        {
            try
            {
              return  SendMailToUser_NIC(MyEmailContent, EmailSubject, "mangal83.rajput@gmail.com", OrganisationName, UserID);
                //SendMailToUser_NIC(MyEmailContent, EmailSubject, "ajay.chahal@nic.in", OrganisationName, UserID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SendMailToActualUser(string MyEmailContent, string EmailSubject, string eMAILID, string OrganisationName, string UserID)
        {
            try
            {
              return  SendMailToUser_NIC(MyEmailContent, EmailSubject, "mangal83.rajput@gmail.com", OrganisationName, UserID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SendMailToUser_NIC(string MyEmailContent, string EmailSubject, string eMAILID, string OrganisationName, string UserID)
        {
            string emailStatus = string.Empty; 
            if (!string.IsNullOrWhiteSpace(MyEmailContent) && !string.IsNullOrWhiteSpace(EmailSubject) && !string.IsNullOrWhiteSpace(eMAILID))
            {
                try
                {
                    if (eMAILID != "")
                    {
                        MailAddress mailfrom = new MailAddress("noreply-gac@gov.in", "Grievance Appellate Committee");
                        MailAddress mailto = new MailAddress(eMAILID);
                        string mailmsg = string.Empty;
                        mailmsg = mailmsg + "<HTML>";
                        mailmsg = mailmsg + "<HEAD>";
                        mailmsg = mailmsg + "<TITLE>" + EmailSubject + "</TITLE>";
                        mailmsg = mailmsg + "</HEAD>";
                        mailmsg = mailmsg + "<BODY>";
                        mailmsg = mailmsg + "<TABLE cellpadding=\"4\">";
                        mailmsg = mailmsg + "<TR><TD><FONT  color=\"black\"  SIZE=\"3\">";
                        mailmsg = mailmsg + MyEmailContent + "<br/><br/>";
                        mailmsg = mailmsg + "<b>Note: This is a system generated email, please do not reply to it.</b>" + "<br/>";
                        mailmsg = mailmsg + "</FONT></TD></TR></TABLE><br/><br/>";
                        mailmsg = mailmsg + "</BODY>";
                        mailmsg = mailmsg + "</HTML>";
                        MailMessage Email = new MailMessage(mailfrom, mailto)
                        {
                            BodyEncoding = Encoding.Default,
                            Subject = EmailSubject,
                            IsBodyHtml = true,
                            DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                            Body = mailmsg
                        };
                        string SMTP_HOST_NAME = Convert.ToString(ConfigurationManager.AppSettings["SMTP_HOST_NAME"]);
                        string SMTP_AUTH_USER = SecureQueryString.SecureQueryString.Decrypt(Convert.ToString(ConfigurationManager.AppSettings["SMTP_AUTH_USER"]));
                        string SMTP_AUTH_PWD = SecureQueryString.SecureQueryString.Decrypt(Convert.ToString(ConfigurationManager.AppSettings["SMTP_AUTH_PWD"]));
                        ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        SmtpClient mailClient = new SmtpClient
                        {
                            Host = SMTP_HOST_NAME,
                            Port = 25,
                            //Port = 587/465,
                            UseDefaultCredentials = false,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Credentials = new NetworkCredential(SMTP_AUTH_USER, SMTP_AUTH_PWD)
                        };

                        mailClient.Send(Email);
                        Email.Dispose();

                        //SmtpClient mailClient = new SmtpClient
                        //{
                        //    Host = "relay.nic.in",
                        //    Port = 25,
                        //    UseDefaultCredentials = false,
                        //    EnableSsl = false
                        //};
                        //mailClient.Send(Email);
                        emailStatus = "OK";
                        var methodparameter = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("@UserID", UserID == null ? "" : UserID),
                            new KeyValuePair<string, string>("@UserType", "NIC"),
                            new KeyValuePair<string, string>("@eMailId", eMAILID),
                            new KeyValuePair<string, string>("@MailSubject", EmailSubject),
                            new KeyValuePair<string, string>("@MailContent", MyEmailContent),
                            new KeyValuePair<string, string>("@Status", "OK")
                        };
                        string result = ServiceAdaptor.GetStringFromService("DigitalNagrik", "DigitalNagrikConnStr", "InsertMSSql", "eMail_InserteMailDescription", methodparameter);
                    }
                }
                catch (Exception ex)
                {
                    emailStatus = "ERROR";
                    var methodparameter = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("@UserID", UserID == null ? "" : UserID),
                        new KeyValuePair<string, string>("@UserType", "NIC"),
                        new KeyValuePair<string, string>("@eMailId", eMAILID),
                        new KeyValuePair<string, string>("@MailSubject", EmailSubject),
                        new KeyValuePair<string, string>("@MailContent", MyEmailContent),
                        new KeyValuePair<string, string>("@Status", ex.Message)
                    };
                    string result = ServiceAdaptor.GetStringFromService("DigitalNagrik", "DigitalNagrikConnStr", "InsertMSSql", "eMail_InserteMailDescription", methodparameter);
                }
            }
            return emailStatus; 
        }

        #endregion

        #region Send Attachment in email
        public void SendAttachmentMailToUser(string MyEmailContent, string EmailSubject, string ReceiverId, byte[] AttachmentPdf, string FileName, string OrganisationName, string UserID)
        {
            try
            {

                SendAttachmentMailToUser_NIC(MyEmailContent, EmailSubject, "mangal83.rajput@gmail.com", AttachmentPdf, FileName, UserID);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendAttachmentMailToActualUser(string MyEmailContent, string EmailSubject, string ReceiverId, byte[] AttachmentPdf, string FileName, string OrganisationName, string UserID)
        {
            try
            {

                SendAttachmentMailToUser_NIC(MyEmailContent, EmailSubject, ReceiverId, AttachmentPdf, FileName, UserID);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendAttachmentMailToUser_NIC(string MyEmailContent, string EmailSubject, string ReceiverId, byte[] AttachmentPdf, string FileName, string UserID)
        {
            if (!string.IsNullOrWhiteSpace(MyEmailContent) && !string.IsNullOrWhiteSpace(EmailSubject) && !string.IsNullOrWhiteSpace(ReceiverId) && !string.IsNullOrWhiteSpace(FileName))
            {
                try
                {
                    if (ReceiverId != "")
                    {
                        MailAddress mailfrom = new MailAddress("noreply-gac@gov.in", "Grievance Appellate Committee");
                        MailAddress mailto = new MailAddress(ReceiverId);
                        string mailmsg = string.Empty;
                        mailmsg = mailmsg + "<HTML>";
                        mailmsg = mailmsg + "<HEAD>";
                        mailmsg = mailmsg + "<TITLE>" + EmailSubject + "</TITLE>";
                        mailmsg = mailmsg + "</HEAD>";
                        mailmsg = mailmsg + "<BODY>";
                        mailmsg = mailmsg + "<TABLE cellpadding=\"4\">";
                        mailmsg = mailmsg + "<TR><TD><FONT  color=\"black\"  SIZE=\"3\">";
                        mailmsg = mailmsg + MyEmailContent + "<br/><br/>";
                        mailmsg = mailmsg + "<b>Note: This is a system generated email, please do not reply to it.</b>" + "<br/>";
                        mailmsg = mailmsg + "</FONT></TD></TR></TABLE><br/><br/>";
                        mailmsg = mailmsg + "</BODY>";
                        mailmsg = mailmsg + "</HTML>";

                        MailMessage Email = new MailMessage(mailfrom, mailto)
                        {
                            BodyEncoding = Encoding.Default,
                            Subject = EmailSubject,
                            Priority = MailPriority.High,
                            IsBodyHtml = true,
                            DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                            Body = mailmsg,
                        };
                        Attachment attachment = new Attachment(new MemoryStream(AttachmentPdf), FileName + ".pdf");
                        Email.Attachments.Add(attachment);
                        string SMTP_HOST_NAME = Convert.ToString(ConfigurationManager.AppSettings["SMTP_HOST_NAME"]);
                        string SMTP_AUTH_USER = SecureQueryString.SecureQueryString.Decrypt(Convert.ToString(ConfigurationManager.AppSettings["SMTP_AUTH_USER"]));
                        string SMTP_AUTH_PWD = SecureQueryString.SecureQueryString.Decrypt(Convert.ToString(ConfigurationManager.AppSettings["SMTP_AUTH_PWD"]));
                        ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        SmtpClient mailClient = new SmtpClient
                        {
                            Host = SMTP_HOST_NAME,
                            Port = 25,
                            //Port = 587/465,
                            UseDefaultCredentials = false,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Credentials = new NetworkCredential(SMTP_AUTH_USER, SMTP_AUTH_PWD)
                        };
                        mailClient.Send(Email);
                        Email.Dispose();

                        //SmtpClient mailClient = new SmtpClient
                        //{
                        //    Host = "relay.nic.in",
                        //    Port = 25,
                        //    UseDefaultCredentials = false,
                        //    EnableSsl = false
                        //};
                        //mailClient.Send(Email);

                        var methodparameter = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("@UserID", UserID == null ? "" : UserID),
                            new KeyValuePair<string, string>("@UserType", "NIC"),
                            new KeyValuePair<string, string>("@eMailId", ReceiverId),
                            new KeyValuePair<string, string>("@MailSubject", EmailSubject),
                            new KeyValuePair<string, string>("@MailContent", MyEmailContent),
                            new KeyValuePair<string, string>("@Status", "OK")
                        };
                        //string result = ServiceAdaptor.GetStringFromService("DigitalNagrik", "DigitalNagrikConnStr", "InsertMSSql", "eMail_InserteMailDescription", methodparameter);
                        string result = ServiceAdaptor.GetStringFromService("DigitalNagrik", "DigitalNagrikConnStr", "InsertMSSql", "eMail_InserteMailDescription", methodparameter);

                    }
                }
                catch (Exception ex)
                {
                    var ExceptionMsg = ex.Message;
                    var methodparameter = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("@UserID", UserID == null ? "" : UserID),
                            new KeyValuePair<string, string>("@UserType", "NIC"),
                            new KeyValuePair<string, string>("@eMailId", ReceiverId),
                            new KeyValuePair<string, string>("@MailSubject", EmailSubject),
                            new KeyValuePair<string, string>("@MailContent", MyEmailContent),
                            new KeyValuePair<string, string>("@Status", ExceptionMsg)
                        };
                    //string result = ServiceAdaptor.GetStringFromService("HPPSC", "DigitalNagrikConnStr", "InsertMSSql", "eMail_InserteMailDescription", methodparameter);
                    string result = ServiceAdaptor.GetStringFromService("DigitalNagrik", "DigitalNagrikConnStr", "InsertMSSql", "eMail_InserteMailDescription", methodparameter);
                }
            }
        }

        #endregion
        public static string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i <= hash.Length - 1; i++)
                result.Append(hash[i].ToString("X2"));
            return result.ToString();
        }
        private static EmailConfig GetEmailConfiguration(string stateCode, string organisationID)
        {
            EmailConfig Obj = new EmailConfig();
            var methodparameter = new List<KeyValuePair<string, string>>();
            methodparameter.Add(new KeyValuePair<string, string>("@stateCode", stateCode));
            methodparameter.Add(new KeyValuePair<string, string>("@organisationID", organisationID));
            methodparameter.Add(new KeyValuePair<string, string>("@tenantID", "DigitalNagrikConnStr"));
            DataSet ds = ServiceAdaptor.GetDataSetFromService("DigitalNagrik", "DigitalNagrikConnStr", "SelectMSSql", "email_GetEmailCredentials", methodparameter);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Obj.MailAddress = Convert.ToString(ds.Tables[0].Rows[0]["MailAddress"]);
                    Obj.DisplayName = Convert.ToString(ds.Tables[0].Rows[0]["DisplayName"]);
                    Obj.NetworkUserName = Convert.ToString(ds.Tables[0].Rows[0]["NetworkUserName"]);
                    Obj.NetworkPassword = Convert.ToString(ds.Tables[0].Rows[0]["NetworkPassword"]);
                    Obj.SMTPHost = Convert.ToString(ds.Tables[0].Rows[0]["SMTPHost"]);
                    Obj.SMTPPort = Convert.ToInt32(ds.Tables[0].Rows[0]["SMTPPort"]);
                    Obj.SMSService = Convert.ToString(ds.Tables[0].Rows[0]["SMSService"]);
                }
            }
            return Obj;
        }
    }

    class MyPolicy1 : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }

    public class EmailConfig
    {
        public string MailAddress { get; set; }
        public string DisplayName { get; set; }
        public string NetworkUserName { get; set; }
        public string NetworkPassword { get; set; }
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string SMSService { get; set; }
    }

    public class SMSServiceType
    {
        /// <summary>uses genpmis mail service</summary>
        public const string Genpmis = "genpmis";
        /// <summary>uses nic relay mail service</summary>
        public const string NIC = "nic";
        /// <summary>uses Current Service Commission mail service</summary>
        public const string ServiceCommission = "commission";
    }
}