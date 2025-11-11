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
    public class CitizenProfileController : ApiController
    {
        
        // GET: api/CitizenProfile/5
        public HttpResponseMessage Get(string MobileNo)
        {
            var response = new Generic_Responce();
            try
            {
                MobileNo = AESCryptography.DecryptAES(MobileNo);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();

                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 100);


                cmd.Parameters.Clear();
                cmd.Parameters.Add("@UserMobile", SqlDbType.Char,10).Value = MobileNo;

                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "API_CitizenProfile_Get");

                List<CitizenProfile> List_Masters = new List<CitizenProfile>();
                foreach (DataRow dr in dt.Rows)
                {
                    var item = new CitizenProfile();
                    item.UserId = AESCryptography.EncryptAES(dr["UserId"].ToString());
                    item.UserProfilleId = AESCryptography.EncryptAES(dr["UserProfilleId"].ToString());
                    item.FirstName = AESCryptography.EncryptAES(dr["FirstName"].ToString());
                    item.MiddleName = AESCryptography.EncryptAES(dr["MiddleName"].ToString());
                    item.LastName = AESCryptography.EncryptAES(dr["LastName"].ToString());
                    item.UserMobile = AESCryptography.EncryptAES(dr["UserMobile"].ToString());
                    item.UserEmail = AESCryptography.EncryptAES(dr["UserEmail"].ToString());
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

        // POST: api/CitizenProfile
        public HttpResponseMessage Post(CitizenProfile_Post citizenProfile_Post)
        {
            var response = new Generic_Responce();
            try
            {
                citizenProfile_Post.UserMobile = AESCryptography.DecryptAES(citizenProfile_Post.UserMobile);
                citizenProfile_Post.FirstName = AESCryptography.DecryptAES(citizenProfile_Post.FirstName);
                citizenProfile_Post.MiddleName = AESCryptography.DecryptAES(citizenProfile_Post.MiddleName);
                citizenProfile_Post.LastName = AESCryptography.DecryptAES(citizenProfile_Post.LastName);
                citizenProfile_Post.UserEmail = AESCryptography.DecryptAES(citizenProfile_Post.UserEmail);
                citizenProfile_Post.Address = AESCryptography.DecryptAES(citizenProfile_Post.Address);
                citizenProfile_Post.Tehsil = AESCryptography.DecryptAES(citizenProfile_Post.Tehsil);
                citizenProfile_Post.DistrictId = AESCryptography.DecryptAES(citizenProfile_Post.DistrictId);
                citizenProfile_Post.StateId = AESCryptography.DecryptAES(citizenProfile_Post.StateId);
                citizenProfile_Post.PinCode = AESCryptography.DecryptAES(citizenProfile_Post.PinCode);
                citizenProfile_Post.Occupation = AESCryptography.DecryptAES(citizenProfile_Post.Occupation);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);
               
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@UserMobile", SqlDbType.VarChar).Value = citizenProfile_Post.UserMobile == "" ? (object)DBNull.Value : citizenProfile_Post.UserMobile;
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = citizenProfile_Post.FirstName == "" ? (object)DBNull.Value : citizenProfile_Post.FirstName;
                cmd.Parameters.Add("@MiddleName", SqlDbType.VarChar).Value = citizenProfile_Post.MiddleName == "" ? (object)DBNull.Value : citizenProfile_Post.MiddleName;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = citizenProfile_Post.LastName == "" ? (object)DBNull.Value : citizenProfile_Post.LastName;
                cmd.Parameters.Add("@UserEmail", SqlDbType.VarChar).Value = citizenProfile_Post.UserEmail == "" ? (object)DBNull.Value : citizenProfile_Post.UserEmail;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = citizenProfile_Post.Address == "" ? (object)DBNull.Value : citizenProfile_Post.Address;
                cmd.Parameters.Add("@Tehsil", SqlDbType.VarChar).Value = citizenProfile_Post.Tehsil == "" ? (object)DBNull.Value : citizenProfile_Post.Tehsil;
                cmd.Parameters.Add("@DistrictId", SqlDbType.Int).Value = citizenProfile_Post.DistrictId == "" ? (object)DBNull.Value : citizenProfile_Post.DistrictId;
                cmd.Parameters.Add("@StateId", SqlDbType.Int).Value = citizenProfile_Post.StateId == "" ? (object)DBNull.Value : citizenProfile_Post.StateId;
                cmd.Parameters.Add("@PinCode", SqlDbType.Char).Value = citizenProfile_Post.PinCode == "" ? (object)DBNull.Value : citizenProfile_Post.PinCode;
                cmd.Parameters.Add("@Occupation", SqlDbType.VarChar).Value = citizenProfile_Post.Occupation == "" ? (object)DBNull.Value : citizenProfile_Post.Occupation;
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "InsertCitizenProfile");

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
