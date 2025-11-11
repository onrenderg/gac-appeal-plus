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
    public class DistrictController : ApiController
    {
#if !DEBUG
        [BasicAuthentication]
#endif
        // GET: api/DistrictMaster
        public HttpResponseMessage Get(string StateCode = "0")
        {
            var response = new Generic_Responce();
            try
            {
                if (StateCode != "0")
                {
                    StateCode = AESCryptography.DecryptAES(StateCode);
                }
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();

                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 100);


                cmd.Parameters.Clear();
                cmd.Parameters.Add("@StateCode", SqlDbType.Int).Value = StateCode;

                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "getDistrictMaster");

                List<DistrictMaster> List_Masters = new List<DistrictMaster>();
                foreach (DataRow dr in dt.Rows)
                {
                    var item = new DistrictMaster();
                    item.StateCode = AESCryptography.EncryptAES(dr["StateCode"].ToString());
                    item.DistrictCode = AESCryptography.EncryptAES(dr["DistrictCode"].ToString());
                    item.DistrictName = AESCryptography.EncryptAES(dr["DistrictName"].ToString());
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
