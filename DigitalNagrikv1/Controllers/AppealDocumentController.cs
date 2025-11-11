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
    public class AppealDocumentController : ApiController
    {
#if !DEBUG
        [BasicAuthentication]
#endif
        // GET: api/AppealDocument
        public HttpResponseMessage Get(string UserID)
        {
            var response = new Generic_Responce();
            try
            {
                UserID = AESCryptography.DecryptAES(UserID);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();

                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);


                cmd.Parameters.Clear();
                
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@UserID", SqlDbType.VarChar).Value = UserID;
                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "API_getAppealDocument");

                List<AppealDocument> List_Masters = new List<AppealDocument>();
                foreach (DataRow dr in dt.Rows)
                {
                    var item = new AppealDocument();
                    item.RegistrationYear = AESCryptography.EncryptAES(dr["RegistrationYear"].ToString());
                    item.GrievanceId = AESCryptography.EncryptAES(dr["GrievanceId"].ToString());
                    item.FileId = AESCryptography.EncryptAES(dr["FileId"].ToString());
                    item.DocumentTitle = AESCryptography.EncryptAES(dr["DocumentTitle"].ToString());
                    item.FileTypeID = AESCryptography.EncryptAES(dr["FileTypeID"].ToString());
                    item.FilePath = AESCryptography.EncryptAES(dr["FilePath"].ToString());
                    item.FileType = AESCryptography.EncryptAES(dr["FileType"].ToString());
                    item.DocumentType = AESCryptography.EncryptAES(dr["DocumentType"].ToString());
                    item.EvidenceType = AESCryptography.EncryptAES(dr["EvidenceType"].ToString());
                    List_Masters.Add(item);
                }
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
                response.data = List_Masters;
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
        // POST: api/AppealDocument
        public HttpResponseMessage Post(AppealDocument_Post appeal)
        {
            var response = new Generic_Responce();
            try
            {
                appeal.RegistrationYear = AESCryptography.DecryptAES(appeal.RegistrationYear);
                appeal.GrievanceID = AESCryptography.DecryptAES(appeal.GrievanceID);
                appeal.File_ID = AESCryptography.DecryptAES(appeal.File_ID);
                appeal.FilePath = AESCryptography.DecryptAES(appeal.FilePath);
                appeal.DocumentType = AESCryptography.DecryptAES(appeal.DocumentType);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);

                cmd.Parameters.Clear();
                
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@RegistrationYear", SqlDbType.VarChar).Value = appeal.RegistrationYear == "" ? (object)DBNull.Value : appeal.RegistrationYear;
                cmd.Parameters.Add("@GrievanceID", SqlDbType.VarChar).Value = appeal.GrievanceID == "" ? (object)DBNull.Value : appeal.GrievanceID;
                cmd.Parameters.Add("@File_ID", SqlDbType.VarChar).Value = appeal.File_ID == "" ? (object)DBNull.Value : appeal.File_ID;
                cmd.Parameters.Add("@FilePath", SqlDbType.VarChar).Value = appeal.FilePath == "" ? (object)DBNull.Value : appeal.FilePath;
                cmd.Parameters.Add("@DocumentType", SqlDbType.VarChar).Value = appeal.DocumentType == "" ? (object)DBNull.Value : appeal.DocumentType;
                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "API_InsertUpdateAppealDocument");

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