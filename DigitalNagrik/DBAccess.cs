using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace DigitalNagrik
{
    public class DBAccess
    {
        private SqlCommand cmd;
        private SqlDataAdapter da = new SqlDataAdapter();
        private SqlConnection sqlConnection;
        public DataTable SELECTData(List<KeyValuePair<string, string>> cmdparameters, string spname)
        {

            // '''''''''''''''''''''''''''''''''       
            string conname = "DigitalNagrik";

            // '''''''''''''''''''''''''''''''''
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[conname].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            cmd = new SqlCommand();

            foreach (var param in cmdparameters)
            {
                //cmd.Parameters.Add(new SqlParameter(param.Key, param.Value));
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }


            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spname;

            cmd.Connection = sqlConnection;
            cmd.CommandTimeout = 240;
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                dt.TableName = "Selected";
                sqlConnection.Close();
                return dt;
            }
            catch (Exception e)
            {
                dt.TableName = "Failed";
                return dt;
            }
            finally
            {
                sqlConnection.Dispose();
            }
        }
        public DataTable INSERTUpdateData(List<KeyValuePair<string, dynamic>> cmdparameters, string spname)
        {
            // '''''''''''''''''''''''''''''''''
            string conname = "DigitalNagrik";

            // '''''''''''''''''''''''''''''''''
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[conname].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);


            cmd = new SqlCommand();

            foreach (var param in cmdparameters)
            {
                cmd.Parameters.Add(new SqlParameter(param.Key, param.Value));
                //cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spname;
            cmd.Connection = sqlConnection;
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                dt.TableName = "Selected";
                return dt;
            }
            catch (Exception e)
            {
                dt.TableName = "Failed";
                return dt;
            }
            finally
            {
                sqlConnection.Dispose();
            }
        }
        public DataSet SELECTDataSet(List<KeyValuePair<string, string>> cmdparameters, string spname)
        {
            // '''''''''''''''''''''''''''''''''
            string conname = "DigitalNagrik";

            // '''''''''''''''''''''''''''''''''
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[conname].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            cmd = new SqlCommand();

            foreach (var param in cmdparameters)
            {
                //cmd.Parameters.Add(new SqlParameter(param.Key, param.Value));
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spname;
            cmd.Connection = sqlConnection;
            cmd.CommandTimeout = 240;
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
                return ds;
            }
            catch (Exception e)
            {
                return ds;
            }
            finally
            {
                sqlConnection.Dispose();
            }
        }
    }
}
