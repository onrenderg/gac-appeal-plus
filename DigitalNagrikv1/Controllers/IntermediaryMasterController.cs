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
    public class IntermediaryMasterController : ApiController
    {
#if !DEBUG
        [BasicAuthentication]
#endif
        // GET: api/IntermediaryMaster
        public HttpResponseMessage Get(string IntermediaryType = "0")
        {
            var response = new Generic_Responce();
            try
            {
                if (IntermediaryType != "0")
                {
                    IntermediaryType = AESCryptography.EncryptAES(IntermediaryType);
                }
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();

                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);


                cmd.Parameters.Clear();
                cmd.Parameters.Add("@IntermediaryType", SqlDbType.VarChar).Value = IntermediaryType;
                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "API_getIntermediaryMaster");

                List<IntermediaryMaster> List_Masters = new List<IntermediaryMaster>();
                foreach (DataRow dr in dt.Rows)
                {
                    var item = new IntermediaryMaster();
                    item.IntermediaryId = AESCryptography.EncryptAES(dr["IntermediaryId"].ToString());
                    item.IntermediaryTitle = AESCryptography.EncryptAES(dr["IntermediaryTitle"].ToString());
                    item.URL = AESCryptography.EncryptAES(dr["URL"].ToString());
                    item.IntermediaryType = AESCryptography.EncryptAES(dr["IntermediaryType"].ToString());
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
