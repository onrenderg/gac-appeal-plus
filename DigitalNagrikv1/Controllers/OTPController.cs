using DigitalNagrik.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DigitalNagrik.Controllers
{
    public class OTPController : ApiController
    {
#if !DEBUG
        [BasicAuthentication]
#endif
        // POST: api/OTP
        public HttpResponseMessage Post(_GenerateOTP _generateOTP)
        {
            var response = new Generic_Responce();
            try
            {
                _generateOTP.MobileNo = AESCryptography.DecryptAES(_generateOTP.MobileNo);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);
                SqlParameter otp_id_ = new SqlParameter("@otp_id", SqlDbType.Int);
                SqlParameter otp_value_ = new SqlParameter("@otp_value", SqlDbType.Int);

                SqlParameter message_ = new SqlParameter("@otp_message", SqlDbType.VarChar, 300);
                SqlParameter entityid_ = new SqlParameter("@otp_entityid", SqlDbType.VarChar, 50);
                SqlParameter tmpid_ = new SqlParameter("@otp_tmpid", SqlDbType.VarChar, 50);
                SqlParameter username_ = new SqlParameter("@otp_username", SqlDbType.VarChar, 50);
                SqlParameter password_ = new SqlParameter("@otp_password", SqlDbType.VarChar, 50);
                SqlParameter senderid_ = new SqlParameter("@otp_senderid", SqlDbType.VarChar, 50);

                cmd.Parameters.Clear();
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                otp_id_.Direction = ParameterDirection.Output;
                otp_value_.Direction = ParameterDirection.Output;
                message_.Direction = ParameterDirection.Output;
                entityid_.Direction = ParameterDirection.Output;
                tmpid_.Direction = ParameterDirection.Output;
                username_.Direction = ParameterDirection.Output;
                password_.Direction = ParameterDirection.Output;
                senderid_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@UserMobile", SqlDbType.Char).Value = _generateOTP.MobileNo == "" ? (object)DBNull.Value : _generateOTP.MobileNo;
                
                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                cmd.Parameters.Add(otp_id_);
                cmd.Parameters.Add(otp_value_);
                cmd.Parameters.Add(message_);
                cmd.Parameters.Add(entityid_);
                cmd.Parameters.Add(tmpid_);
                cmd.Parameters.Add(username_);
                cmd.Parameters.Add(password_);
                cmd.Parameters.Add(senderid_);

                dt = objDBAccess.getDBData(cmd, "API_OTP_Post");

                var dataClass = new GenerateOTP_();
               
                if (dt.TableName == "OK")
                {
                    response.status_code = (int)status_code_.Value;
                    response.Message = (string)status_message_.Value;
                    response.developer_message = (string)status_message_.Value;
                    dataClass.otp_id = (int)otp_id_.Value;
#if DEBUG
                    {
                        dataClass.otp_value = (int)otp_value_.Value;
                    }
#endif
                }
                else
                {
                    response.developer_message = dt.TableName;
                }
                dataClass.SMS_Response = "No SMS Send";
                if (response.status_code == 201)
                {
                    dataClass.SMS_Response = sendSingleSMS(_generateOTP.MobileNo, (string)message_.Value ?? "", (string)tmpid_.Value, (string)username_.Value, (string)password_.Value, (string)senderid_.Value, (string)entityid_.Value);
                }
                response.data = dataClass;
                return Request.CreateResponse((HttpStatusCode)response.status_code, response);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(response.developer_message))
                {
                    response.developer_message = ex.Message;
                }
                return Request.CreateResponse((HttpStatusCode)response.status_code, response);
            }
        }
        // PUT: api/OTP
        public HttpResponseMessage Put(_ValidateOTP _validateOTP)
        {
            var response = new Generic_Responce();
            try
            {
                _validateOTP.MobileNo = (AESCryptography.DecryptAES(_validateOTP.MobileNo));
                _validateOTP.MobileNoOTPID = (AESCryptography.DecryptAES(_validateOTP.MobileNoOTPID));
                _validateOTP.txtMobileOTP = AESCryptography.DecryptAES(_validateOTP.txtMobileOTP);

                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter UserID_ = new SqlParameter("@UserID", SqlDbType.Int);
                SqlParameter UserProfileID_ = new SqlParameter("@UserProfileID", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 100);

                cmd.Parameters.Clear();
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;
                UserID_.Direction = ParameterDirection.Output;
                UserProfileID_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@UserMobile", SqlDbType.VarChar).Value = _validateOTP.MobileNo;
                cmd.Parameters.Add("@OTP_ID", SqlDbType.VarChar).Value = _validateOTP.MobileNoOTPID;
                cmd.Parameters.Add("@OTP", SqlDbType.VarChar).Value = _validateOTP.txtMobileOTP;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);
                cmd.Parameters.Add(UserID_);
                cmd.Parameters.Add(UserProfileID_);

                dt = objDBAccess.getDBData(cmd, "API_OTP_Put");
                if (dt.TableName == "OK")
                {
                    response.status_code = (int)status_code_.Value;
                    response.Message = (string)status_message_.Value;
                    response.developer_message = (string)status_message_.Value;
                    List<_ValidateOTP_Return> listhere = new List<_ValidateOTP_Return>();
                    var item = new _ValidateOTP_Return();
                    item.UserID = AESCryptography.EncryptAES(((int)UserID_.Value).ToString());
                    item.UserProfileID = AESCryptography.EncryptAES(((int)UserProfileID_.Value).ToString());
                    listhere.Add(item);
                    response.data = listhere;
                }
                else
                {
                    response.developer_message = dt.TableName;
                }
                return Request.CreateResponse((HttpStatusCode)response.status_code, response);
            }
            catch (Exception ex)
            {
                response.developer_message = ex.Message;
                return Request.CreateResponse((HttpStatusCode)response.status_code, response);
            }
        }

        protected string sendSingleSMS(string mobileNo, string message, string tmpid, string username, string password, string senderid, string entityID)
        {
            string Status = "";
            try
            {
                Stream dataStream;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://smsgw.sms.gov.in/failsafe/HttpLink");
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 1;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";
                request.Method = "POST";
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                string query =
                $"username={HttpUtility.UrlEncode(username)}" +
                $"&pin={HttpUtility.UrlEncode(password)}" +
                $"&message={HttpUtility.UrlEncode(message)}" +
                $"&mnumber={HttpUtility.UrlEncode(mobileNo)}" +
                $"&signature={HttpUtility.UrlEncode(senderid)}" +
                $"&dlt_entity_id={HttpUtility.UrlEncode(entityID)}" +
                $"&dlt_template_id={HttpUtility.UrlEncode(tmpid)}";

                byte[] byteArray = Encoding.ASCII.GetBytes(query);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                Status = ((HttpWebResponse)response).StatusDescription;
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                //WriteErrorLogToFile("Function Name sendSMS:------template_id : " + tmpid + " send_SMS ------Mobile -- " + mobileNo + "+responseFromServer" + responseFromServer);
                return responseFromServer;
            }
            catch (Exception ex)
            {
                Status = "Service Exception: " + ex.Message;
                //WriteErrorLogToFile("Function Name sendSMS:------template_id : " + tmpid + " send_SMS ------Mobile -- " + mobileNo + "+Message" + ex.Message.ToString() + "-----------------stacktrace-----------------" + ex.StackTrace.ToString());
            }
            return Status;
        }
    }
}

