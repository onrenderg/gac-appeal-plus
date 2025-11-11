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
    public class StateController : ApiController
    {
#if !DEBUG
        [BasicAuthentication]
#endif
        // GET: api/StateMaster
        public HttpResponseMessage Get()
        {
            var response = new Generic_Responce();
            try
            {

                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();

                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 200);


                cmd.Parameters.Clear();

                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "getStateMaster");

                List<StateMaster> List_Masters = new List<StateMaster>();
                foreach (DataRow dr in dt.Rows)
                {
                    var item = new StateMaster();
                    item.StateCode = AESCryptography.EncryptAES(dr["StateCode"].ToString());
                    item.StateName = AESCryptography.EncryptAES(dr["StateName"].ToString());
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
