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
    public class AnnexureCMasterController : ApiController
    {
#if !DEBUG
        [BasicAuthentication]
#endif
        // GET: api/AnnexureCMaster
        public HttpResponseMessage Get(string GroundAppealID = "0")
        {
            var response = new Generic_Responce();
            try
            {
                if (GroundAppealID != "0")
                {
                    GroundAppealID = AESCryptography.DecryptAES(GroundAppealID);
                }
                DBAccess objDBAccess = new DBAccess();
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();

                SqlParameter status_code_ = new SqlParameter("@status_code", SqlDbType.Int);
                SqlParameter status_message_ = new SqlParameter("@status_message", SqlDbType.VarChar, 100);


                cmd.Parameters.Clear();
                cmd.Parameters.Add("@GroundAppealID", SqlDbType.VarChar).Value = GroundAppealID;

                status_code_.Direction = ParameterDirection.Output;
                status_message_.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(status_code_);
                cmd.Parameters.Add(status_message_);

                dt = objDBAccess.getDBData(cmd, "getAnnexureCMaster");

                List<AnnexureCMaster> List_Masters = new List<AnnexureCMaster>();
                foreach (DataRow dr in dt.Rows)
                {
                    var item = new AnnexureCMaster();
                    item.Sequence = AESCryptography.EncryptAES(dr["Sequence"].ToString());
                    item.Section = AESCryptography.EncryptAES(dr["Section"].ToString());
                    item.ExtractITRule = AESCryptography.EncryptAES(dr["ExtractITRule"].ToString());
                    item.RelatedSubjectEntry = AESCryptography.EncryptAES(dr["RelatedSubjectEntry"].ToString());
                    item.ConcernedMinistry = AESCryptography.EncryptAES(dr["ConcernedMinistry"].ToString());
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
