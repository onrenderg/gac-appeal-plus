using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DigitalNagrik.Models;

namespace DigitalNagrik.Controllers
{
    public class FinalAppealController : ApiController
    {

#if !DEBUG
        [BasicAuthentication]
#endif
        public HttpResponseMessage Post(Appeal_FinalSubmit appeal)
        {
            var response = new Generic_Responce();
            try
            {
                appeal.RegistrationYear = AESCryptography.DecryptAES(appeal.RegistrationYear);
                appeal.GrievanceID = AESCryptography.DecryptAES(appeal.GrievanceID);
                appeal.UserMobile = AESCryptography.DecryptAES(appeal.UserMobile);
                appeal.IPAddress = AESCryptography.DecryptAES(appeal.IPAddress);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@RegistrationYear", SqlDbType.VarChar).Value = appeal.RegistrationYear == "" ? (object)DBNull.Value : appeal.RegistrationYear;
                cmd.Parameters.Add("@GrievanceID", SqlDbType.VarChar).Value = appeal.GrievanceID == "" ? (object)DBNull.Value : appeal.GrievanceID;
                cmd.Parameters.Add("@UserMobile", SqlDbType.VarChar).Value = appeal.UserMobile == "" ? (object)DBNull.Value : appeal.UserMobile;
                cmd.Parameters.Add("@IPAddress", SqlDbType.VarChar).Value = appeal.IPAddress == "" ? (object)DBNull.Value : appeal.IPAddress;
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "FinalSubmitAppeal");
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
