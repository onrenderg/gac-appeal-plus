using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DigitalNagrik.Models
{
    public class DBAccess
    {
        private SqlCommand cmd = new SqlCommand();
        private SqlDataAdapter da = new SqlDataAdapter();
        public DataTable getDBData(SqlCommand cmdparameters, string spname)
        {
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString());
            cmd = cmdparameters;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spname;

            cmd.Connection = sqlConnection;
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                dt.TableName = "OK";
                return dt;
            }
            catch (Exception e)
            {
                dt.TableName = e.Message;
                
                return dt;
            }
            finally
            {
                // sqlConnection.Close()
                sqlConnection.Dispose();
            }
        }
    }
}