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
    public class DeleteAppealDocumentController : ApiController
    {
#if !DEBUG
        [BasicAuthentication]
#endif        
        // POST: api/DeleteAppealDocument
        public HttpResponseMessage Post(AppealDocument_Delete appealDocument_Delete)
        {
            var response = new Generic_Responce();
            try
            {
                appealDocument_Delete.RegistrationYear = AESCryptography.DecryptAES(appealDocument_Delete.RegistrationYear);
                appealDocument_Delete.GrievanceID = AESCryptography.DecryptAES(appealDocument_Delete.GrievanceID);
                appealDocument_Delete.File_ID = AESCryptography.DecryptAES(appealDocument_Delete.File_ID);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);

                cmd.Parameters.Clear();

                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RegistrationYear", SqlDbType.VarChar).Value = appealDocument_Delete.RegistrationYear == "" ? (object)DBNull.Value : appealDocument_Delete.RegistrationYear;
                cmd.Parameters.Add("@GrievanceID", SqlDbType.VarChar).Value = appealDocument_Delete.GrievanceID == "" ? (object)DBNull.Value : appealDocument_Delete.GrievanceID;
                cmd.Parameters.Add("@File_ID", SqlDbType.VarChar).Value = appealDocument_Delete.File_ID == "" ? (object)DBNull.Value : appealDocument_Delete.File_ID;
                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "API_DeleteAppealDocument");

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