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
    public class CitizenDashboardController : ApiController
    {
#if !DEBUG
        [BasicAuthentication]
#endif
        // GET: api/DistrictMaster
        public HttpResponseMessage Get(string UserMobile)
        {
            var response = new Generic_Responce();
            try
            {
                UserMobile = AESCryptography.DecryptAES(UserMobile);
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();

                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);


                cmd.Parameters.Clear();
                cmd.Parameters.Add("@UserMobile", SqlDbType.VarChar).Value = UserMobile;

                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "API_getCitizenDashboardList");

                List<CitizenDashboard> List_Masters = new List<CitizenDashboard>();
                foreach (DataRow dr in dt.Rows)
                {
                    var item = new CitizenDashboard();
                    item.UserMobile = AESCryptography.EncryptAES(dr["UserMobile"].ToString());
                    item.languageCode = AESCryptography.EncryptAES(dr["languageCode"].ToString());
                    item.languageDescription = AESCryptography.EncryptAES(dr["languageDescription"].ToString());
                    item.languageCode = AESCryptography.EncryptAES(dr["languageCode"].ToString());
                    item.RegistrationYear = AESCryptography.EncryptAES(dr["RegistrationYear"].ToString());
                    item.GrievanceId = AESCryptography.EncryptAES(dr["GrievanceId"].ToString());
                    item.EmailOf = AESCryptography.EncryptAES(dr["EmailOf"].ToString());
                    item.ReliefSoughtID = AESCryptography.EncryptAES(dr["ReliefSoughtID"].ToString());
                    item.ReliefTitle = AESCryptography.EncryptAES(dr["ReliefTitle"].ToString());
                    item.RelieftSoughtSpecification = AESCryptography.EncryptAES(dr["RelieftSoughtSpecification"].ToString());
                    item.GroundAppealID = AESCryptography.EncryptAES(dr["GroundAppealID"].ToString());
                    item.GroundTitle = AESCryptography.EncryptAES(dr["GroundTitle"].ToString());
                    item.Justification = AESCryptography.EncryptAES(dr["Justification"].ToString());
                    
                   
                    item.GrievanceDesc = AESCryptography.EncryptAES(dr["GrievanceDesc"].ToString());
                    item.ReasonForAppeal = AESCryptography.EncryptAES(dr["ReasonForAppeal"].ToString());
                    item.IntermediaryURL = AESCryptography.EncryptAES(dr["IntermediaryURL"].ToString());
                    item.UserId = AESCryptography.EncryptAES(dr["UserId"].ToString());
                    item.UserProfileId = AESCryptography.EncryptAES(dr["UserProfileId"].ToString());
                    item.CaseHistoryFilePath = AESCryptography.EncryptAES(dr["CaseHistoryFilePath"].ToString());
                    item.CaseHistoryFileType = AESCryptography.EncryptAES(dr["CaseHistoryFileType"].ToString());
                    item.IntermediaryId = AESCryptography.EncryptAES(dr["IntermediaryId"].ToString());
                    item.IntermediaryTitle = AESCryptography.EncryptAES(dr["IntermediaryTitle"].ToString());
                    item.IntermediaryGROName = AESCryptography.EncryptAES(dr["IntermediaryGROName"].ToString());
                    item.IntermediaryGROEmail = AESCryptography.EncryptAES(dr["IntermediaryGROEmail"].ToString());
                    item.IntermediaryAddress = AESCryptography.EncryptAES(dr["IntermediaryAddress"].ToString());
                    item.PlatformTypeId = AESCryptography.EncryptAES(dr["PlatformTypeId"].ToString());
                    item.PlatformTypeTitle = AESCryptography.EncryptAES(dr["PlatformTypeTitle"].ToString());
                    item.IntermediaryDate = AESCryptography.EncryptAES(dr["IntermediaryDate"].ToString());
                    item.GrievnaceStatus = AESCryptography.EncryptAES(dr["GrievnaceStatus"].ToString());
                    item.StatusTitle = AESCryptography.EncryptAES(dr["StatusTitle"].ToString());
                    item.ReceiptDate = AESCryptography.EncryptAES(dr["ReceiptDate"].ToString());
                    item.LastUpdatedOn = AESCryptography.EncryptAES(dr["LastUpdatedOn"].ToString());
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
    }
}
