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
    public class PINController : ApiController
    {
        // GET: api/PIN
        public HttpResponseMessage Get(string UserMobile, string PIN, string Salt)
        {
            var response = new Generic_Responce();
            try
            {
                UserMobile = AESCryptography.DecryptAES(UserMobile);
                PIN = AESCryptography.DecryptAES(PIN);
                Salt = AESCryptography.DecryptAES(Salt);

                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();

                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);


                cmd.Parameters.Clear();
                cmd.Parameters.Add("@UserMobile", SqlDbType.VarChar, 10).Value = UserMobile;
                cmd.Parameters.Add("@PIN", SqlDbType.VarChar).Value = PIN;
                cmd.Parameters.Add("@Salt", SqlDbType.VarChar).Value = Salt;

                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "CheckCitizenPIN");
                if (dt.TableName == "OK")
                {
                    response.status_code = (int)status_code_.Value;
                    response.Message = (string)status_message_.Value;
                    response.developer_message = (string)status_message_.Value;
                }
                else
                {
                    response.developer_message = dt.TableName;
                }
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

        // POST: api/PIN
        public HttpResponseMessage Post(UserPIN userPIN)
        {
            var response = new Generic_Responce();
            try
            {
                userPIN.MobileNo = AESCryptography.DecryptAES(userPIN.MobileNo);
                userPIN.PIN = AESCryptography.DecryptAES(userPIN.PIN);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);
               
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@MobileNo", SqlDbType.Char).Value = userPIN.MobileNo == "" ? (object)DBNull.Value : userPIN.MobileNo;
                cmd.Parameters.Add("@PIN", SqlDbType.Char).Value = userPIN.PIN == "" ? (object)DBNull.Value : userPIN.PIN;
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "SetCitizenPIN");
                if (dt.TableName == "OK")
                {
                    response.status_code = (int)status_code_.Value;
                    response.Message = (string)status_message_.Value;
                    response.developer_message = (string)status_message_.Value;
                }
                else
                {
                    response.developer_message = dt.TableName;
                }
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

    }
}
