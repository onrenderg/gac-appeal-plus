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
    public class AppealController : ApiController
    {

#if !DEBUG
        [BasicAuthentication]
#endif
        // POST: api/Appeal
        public HttpResponseMessage Post(Appeal_Post appeal)
        {
            var response = new Generic_Responce();
            try
            {
                appeal.UserMobile = AESCryptography.DecryptAES(appeal.UserMobile);
                appeal.UserID = AESCryptography.DecryptAES(appeal.UserID);
                appeal.UserProfileID = AESCryptography.DecryptAES(appeal.UserProfileID);
                appeal.FirstName = AESCryptography.DecryptAES(appeal.FirstName);
                appeal.MiddleName = AESCryptography.DecryptAES(appeal.MiddleName);
                appeal.LastName = AESCryptography.DecryptAES(appeal.LastName);
                appeal.UserEmail = AESCryptography.DecryptAES(appeal.UserEmail);
               
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                
                SqlParameter RegistrationYear_ = new SqlParameter("@RegistrationYear", SqlDbType.Int);
                SqlParameter GrievanceID_ = new SqlParameter("@GrievanceID", SqlDbType.Int);
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);

                cmd.Parameters.Clear();
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;
                RegistrationYear_.Direction = ParameterDirection.Output;
                GrievanceID_.Direction = ParameterDirection.Output;


                cmd.Parameters.Add("@UserMobile", SqlDbType.Char, 10).Value = appeal.UserMobile;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = appeal.UserID;
                cmd.Parameters.Add("@UserProfileID", SqlDbType.Int).Value = appeal.UserProfileID;
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = appeal.FirstName;
                cmd.Parameters.Add("@MiddleName", SqlDbType.VarChar).Value = appeal.MiddleName;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = appeal.LastName;
                cmd.Parameters.Add("@UserEmail", SqlDbType.VarChar).Value = appeal.UserEmail;
                

                cmd.Parameters.Add(RegistrationYear_);
                cmd.Parameters.Add(GrievanceID_);
                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "API_Appeal_Post");
                if (dt.TableName == "OK")
                {
                    response.status_code = (int)status_code_.Value;
                    response.Message = (string)status_message_.Value;
                    response.developer_message = (string)status_message_.Value;
                    List<Appeal_Post_return> listhere = new List<Appeal_Post_return>();
                    var item = new Appeal_Post_return();
                    item.GrievanceID = AESCryptography.EncryptAES(((int)GrievanceID_.Value).ToString());
                    item.RegistrationYear = AESCryptography.EncryptAES(((int)RegistrationYear_.Value).ToString());
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
                if (string.IsNullOrEmpty(response.developer_message))
                {
                    response.developer_message = ex.Message;
                }
                return Request.CreateResponse((HttpStatusCode)response.status_code, response);
            }
        }
        public HttpResponseMessage Put(Appeal appeal)
        {
            var response = new Generic_Responce();
            try
            {
                appeal.UserMobile = AESCryptography.DecryptAES(appeal.UserMobile);
                appeal.RegistrationYear = AESCryptography.DecryptAES(appeal.RegistrationYear);
                appeal.GrievanceId = AESCryptography.DecryptAES(appeal.GrievanceId);
                //appeal.ContentClassificationID = AESCryptography.DecryptAES(appeal.ContentClassificationID);
                //appeal.SubContentClassificationID = AESCryptography.DecryptAES(appeal.SubContentClassificationID);
                appeal.GrievanceDesc = AESCryptography.DecryptAES(appeal.GrievanceDesc);
                appeal.ReasonForAppeal = AESCryptography.DecryptAES(appeal.ReasonForAppeal);
                appeal.IntermediaryId = AESCryptography.DecryptAES(appeal.IntermediaryId);
                appeal.IntermediaryGROName = AESCryptography.DecryptAES(appeal.IntermediaryGROName);
                appeal.EmailOf = AESCryptography.DecryptAES(appeal.EmailOf);
                appeal.IntermediaryGROEmail = AESCryptography.DecryptAES(appeal.IntermediaryGROEmail);
                appeal.IntermediaryAddress = AESCryptography.DecryptAES(appeal.IntermediaryAddress);
                appeal.PlatformTypeId = AESCryptography.DecryptAES(appeal.PlatformTypeId);
                appeal.IntermediaryDate = AESCryptography.DecryptAES(appeal.IntermediaryDate);
                appeal.GrievnaceStatus = AESCryptography.DecryptAES(appeal.GrievnaceStatus);
                appeal.IntermediaryTitle = AESCryptography.DecryptAES(appeal.IntermediaryTitle);
                appeal.IntermediaryURL = AESCryptography.DecryptAES(appeal.IntermediaryURL);
                appeal.languageCode = AESCryptography.DecryptAES(appeal.languageCode);
                appeal.Justification = AESCryptography.DecryptAES(appeal.Justification);
                appeal.ReliefSoughtID = AESCryptography.DecryptAES(appeal.ReliefSoughtID);
                appeal.RelieftSoughtSpecification = AESCryptography.DecryptAES(appeal.RelieftSoughtSpecification);
                appeal.GroundAppealID = AESCryptography.DecryptAES(appeal.GroundAppealID);

                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@UserMobile", SqlDbType.VarChar).Value = appeal.UserMobile == "" ? (object)DBNull.Value : appeal.UserMobile;
                cmd.Parameters.Add("@RegistrationYear", SqlDbType.Int).Value = appeal.RegistrationYear;
                cmd.Parameters.Add("@GrievanceId", SqlDbType.Int).Value = appeal.GrievanceId ?? "0";
                //cmd.Parameters.Add("@ContentClassificationID", SqlDbType.VarChar).Value = appeal.ContentClassificationID == "" ? (object)DBNull.Value : appeal.ContentClassificationID;
                //cmd.Parameters.Add("@SubContentClassificationID", SqlDbType.VarChar).Value = appeal.SubContentClassificationID == "" ? (object)DBNull.Value : appeal.SubContentClassificationID;
                cmd.Parameters.Add("@GrievanceDesc", SqlDbType.VarChar).Value = appeal.GrievanceDesc == "" ? (object)DBNull.Value : appeal.GrievanceDesc;
                cmd.Parameters.Add("@ReasonForAppeal", SqlDbType.VarChar).Value = appeal.ReasonForAppeal == "" ? (object)DBNull.Value : appeal.ReasonForAppeal;
                cmd.Parameters.Add("@IntermediaryId", SqlDbType.VarChar).Value = appeal.IntermediaryId == "" ? (object)DBNull.Value : appeal.IntermediaryId;
                cmd.Parameters.Add("@IntermediaryGROName", SqlDbType.VarChar).Value = appeal.IntermediaryGROName == "" ? (object)DBNull.Value : appeal.IntermediaryGROName;
                cmd.Parameters.Add("@EmailOf", SqlDbType.VarChar).Value = appeal.EmailOf == "" ? (object)DBNull.Value : appeal.EmailOf;
                cmd.Parameters.Add("@IntermediaryGROEmail", SqlDbType.VarChar).Value = appeal.IntermediaryGROEmail == "" ? (object)DBNull.Value : appeal.IntermediaryGROEmail;
                cmd.Parameters.Add("@IntermediaryAddress", SqlDbType.VarChar).Value = appeal.IntermediaryAddress == "" ? (object)DBNull.Value : appeal.IntermediaryAddress;
                cmd.Parameters.Add("@PlatformTypeId", SqlDbType.VarChar).Value = appeal.PlatformTypeId == "" ? (object)DBNull.Value : appeal.PlatformTypeId;
                cmd.Parameters.Add("@IntermediaryDate", SqlDbType.VarChar).Value = appeal.IntermediaryDate == "" ? (object)DBNull.Value : appeal.IntermediaryDate;
                cmd.Parameters.Add("@GrievnaceStatus", SqlDbType.VarChar).Value = appeal.GrievnaceStatus == "" ? (object)DBNull.Value : appeal.GrievnaceStatus;
                cmd.Parameters.Add("@IntermediaryTitle", SqlDbType.VarChar).Value = appeal.IntermediaryTitle == "" ? (object)DBNull.Value : appeal.IntermediaryTitle;
                cmd.Parameters.Add("@IntermediaryURL", SqlDbType.VarChar).Value = appeal.IntermediaryURL == "" ? (object)DBNull.Value : appeal.IntermediaryURL;
                cmd.Parameters.Add("@languageCode", SqlDbType.VarChar).Value = appeal.languageCode == "" ? (object)DBNull.Value : appeal.languageCode;
                cmd.Parameters.Add("@Justification", SqlDbType.VarChar).Value = appeal.Justification == "" ? (object)DBNull.Value : appeal.Justification;
                cmd.Parameters.Add("@ReliefSoughtID", SqlDbType.VarChar).Value = appeal.ReliefSoughtID == "" ? (object)DBNull.Value : appeal.ReliefSoughtID;
                cmd.Parameters.Add("@RelieftSoughtSpecification", SqlDbType.VarChar).Value = appeal.RelieftSoughtSpecification == "" ? (object)DBNull.Value : appeal.RelieftSoughtSpecification;
                cmd.Parameters.Add("@GroundAppealID", SqlDbType.VarChar).Value = appeal.GroundAppealID == "" ? (object)DBNull.Value : appeal.GroundAppealID;
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;


                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "API_insertUpdateAppealChange");
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
