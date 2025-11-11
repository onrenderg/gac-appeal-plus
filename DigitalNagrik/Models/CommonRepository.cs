using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace DigitalNagrik.Models
{
    public class CommonRepository
    {
        //-----------------------------------Decrypt QS------------------------------------//
        public static Dictionary<string, string> GetQSDecryptedParameters(string qs)
        {
            Dictionary<string, string> DecryptedParameters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(qs))
            {
                try
                {
                    string[] arrMsgs = SecureQueryString.SecureQueryString.Decrypt(qs).Split('&');
                    string[] arrIndMsg = { };
                    for (int i = 0; i <= arrMsgs.Length - 1; i++)
                    {
                        arrIndMsg = arrMsgs[i].Split('=');
                        DecryptedParameters.Add(arrIndMsg[0], arrIndMsg[1]);
                    }
                }
                catch { }
            }
            return DecryptedParameters;
        }
        public static bool IsValidUrl(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }
        public static string RandomStringGenerate(int length)
        {
            const string valid = "abcdefghijkmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }
        public static string GenerateCSRFToken()
        {
            string FieldValue = String.Empty;
            string Token = CommonRepository.RandomStringGenerate(40);
            FieldValue = "<input id='CustomCSRFToken' type='hidden' name='CustomCSRFToken' value='" + Token + "' />";
            HttpContext.Current.Session["_CustomCSRFToken"] = Convert.ToString(Token);
            return FieldValue;
        }
        public static string RandomStringGenerateNumbers(int length)
        {
            const string valid = "0123456789";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }
        public static string GetQSDecryptedValue(string Key, Dictionary<string, string> Parameters)
        {
            string Value = string.Empty;
            try
            {
                if (Parameters.ContainsKey(Key))
                {
                    Value = Parameters[Key];
                }
            }
            catch { }
            return Value;
        }
        //-----------------------------------Decrypt QS------------------------------------//

        public static bool CheckSessionVariables(string variables)
        {
            bool Result = true;
            if (!string.IsNullOrWhiteSpace(variables))
            {
                string[] varArray = variables.Split(',').ToArray();
                for (int i = 0; i < varArray.Length; i++)
                {
                    var value = HttpContext.Current.Session[varArray[i]];
                    if (value == null)
                    {
                        Result = false;
                        break;
                    }
                }
            }
            return Result;
        }

        public static string GetIPAddress()
        {
            //return HttpContext.Current.Request.UserHostAddress;
            try
            {

                System.Web.HttpContext context = System.Web.HttpContext.Current;
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        return addresses[0];
                    }
                }
                return context.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch
            {
                return "";
            }

        }

        public static string ComputeSha256HashCaseSense(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        public static string EncryptStringSymmetric(string InputText)
        {
            // We are now going to create an instance of the
            string Password = Getpass();
            // Rihndael class.

            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            // First we need to turn the input strings into a byte array.

            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);

            // We are using salt to make it harder to guess our key

            // using a dictionary attack.

            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

            // The (Secret Key) will be generated from the specified

            // password and salt.

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

            // Create a encryptor from the existing SecretKey bytes.

            // We use 32 bytes for the secret key

            // (the default Rijndael key length is 256 bit = 32 bytes) and

            // then 16 bytes for the IV (initialization vector),

            // (the default Rijndael IV length is 128 bit = 16 bytes)

            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            // Create a MemoryStream that is going to hold the encrypted bytes

            MemoryStream memoryStream = new MemoryStream();

            // Create a CryptoStream through which we are going to be processing our data.

            // CryptoStreamMode.Write means that we are going to be writing data

            // to the stream and the output will be written in the MemoryStream

            // we have provided. (always use write mode for encryption)

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

            // Start the encryption process.

            cryptoStream.Write(PlainText, 0, PlainText.Length);

            // Finish encrypting.

            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memoryStream into a byte array.

            byte[] CipherBytes = memoryStream.ToArray();

            // Close both streams.

            memoryStream.Close();

            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.

            // A common mistake would be to use an Encoding class for that.

            // It does not work, because not all byte values can be

            // represented by characters. We are going to be using Base64 encoding

            // That is designed exactly for what we are trying to do.

            string EncryptedData = Convert.ToBase64String(CipherBytes);

            // Return encrypted string.

            return EncryptedData;
        }

        public static string DecryptString(string InputText)
        {
            try
            {
                RijndaelManaged RijndaelCipher = new RijndaelManaged();
                string Password = Getpass();
                byte[] EncryptedData = Convert.FromBase64String(InputText);

                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

                // Create a decryptor from the existing SecretKey bytes.

                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                MemoryStream memoryStream = new MemoryStream(EncryptedData);

                // Create a CryptoStream. (always use Read mode for decryption).

                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

                // Since at this point we don’t know what the size of decrypted data

                // will be, allocate the buffer long enough to hold EncryptedData;

                // DecryptedData is never longer than EncryptedData.

                byte[] PlainText = new byte[EncryptedData.Length];

                // Start decrypting.

                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

                memoryStream.Close();

                cryptoStream.Close();

                // Convert decrypted data into a string.

                string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

                // Return decrypted string.

                return DecryptedData;
            }
            catch (Exception ex)
            {
                return "";  //EventLog.WriteEntry(exc.Source, "Database Error From Exception Log!", EventLogEntryType.Error, 65535);
            }
        }

        //--Start Added on 14-01-2021 by mangal
        public static string Encrypt(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged()));
        }
        public static string Decrypt(string encryptedText)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged()));
        }
        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }
        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
        private static RijndaelManaged GetRijndaelManaged()
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes("$NIV%&04042020Key%");
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }
        //--End Added on 14-01-2021 by mangal
        private static string Getpass()
        {
            return "..........";
        }

        //private static string GetConnection(string connectionId)
        //{
        //    if (connectionId != "HPPSCConnStr")
        //    {
        //        string connection = ConfigurationManager.ConnectionStrings["HPPSCConnStr"].ConnectionString;
        //        var dTable = new DataTable();
        //        SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
        //        SqlConnection sqlConnection = new SqlConnection(connection);
        //        SqlCommand sqlCommand = new SqlCommand("mas_getOrganisation", sqlConnection);
        //        sqlCommand.Parameters.AddWithValue("@organisationDomain", connectionId);
        //        sqlCommand.CommandType = CommandType.StoredProcedure;
        //        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
        //        sqlDataAdapter.SelectCommand = sqlCommand;
        //        sqlDataAdapter.Fill(dTable);
        //        if (dTable.Rows.Count > 0)
        //        {
        //            builder["Data Source"] = dTable.Rows[0]["dbIP"];
        //            builder["integrated Security"] = false;
        //            builder["Initial Catalog"] = dTable.Rows[0]["dbCatalog"];
        //            builder["User ID"] = dTable.Rows[0]["dbUser"];
        //            builder["Password"] = dTable.Rows[0]["dbPassword"];
        //        }
        //        return builder.ConnectionString;
        //    }
        //    return ConfigurationManager.ConnectionStrings[connectionId].ConnectionString;
        //}

        //public static string BulkInsert(DataTable data, string procedureName)
        //{
        //    // bulk send data to database
        //    try
        //    {
        //        var conn = new SqlConnection(GetConnection(CurrentSession.domainName));
        //        var cmd = new SqlCommand(procedureName, conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        SqlParameter param = cmd.Parameters.AddWithValue("@data", data);
        //        param.SqlDbType = SqlDbType.Structured;
        //        conn.Open();
        //        int resultCount = cmd.ExecuteNonQuery();
        //        conn.Close();
        //        if (resultCount > 0)
        //            return "True";
        //        else
        //            return "FAILED";
        //    }
        //    catch (Exception ex)
        //    {
        //        MyExceptionHandler.LogError(ex);
        //        return "ERROR";
        //    }
        //}

        ///// <summary>
        ///// Select bulk data directly from database
        ///// </summary>      
        //public static DataSet GetBulkDataSet(string procedureName, List<KeyValuePair<string, string>> methodParameter)
        //{      
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        var conn = new SqlConnection(GetConnection(CurrentSession.domainName));
        //        var cmd = new SqlCommand(procedureName, conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        if (methodParameter != null)
        //        {
        //            foreach (var element in methodParameter)
        //            {
        //                cmd.Parameters.Add(new SqlParameter(parameterName: element.Key, value: element.Value.ToString()));
        //            }
        //        }

        //        conn.Open();
        //        SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //        adp.Fill(ds);
        //        cmd.Parameters.Clear();
        //        conn.Close();
        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        MyExceptionHandler.LogError(ex);
        //        return ds;
        //    }
        //}

        //public static DataSet GetBulkDataSet(string procedureName, DataTable data)
        //{
        //    // bulk send data to database
        //    DataSet ds;
        //    try
        //    {
        //        ds = new DataSet();
        //        var conn = new SqlConnection(GetConnection(CurrentSession.domainName));
        //        var cmd = new SqlCommand(procedureName, conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        SqlParameter param = cmd.Parameters.AddWithValue("@data", data);
        //        param.SqlDbType = SqlDbType.Structured;
        //        conn.Open();
        //        SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //        adp.Fill(ds);
        //        cmd.Parameters.Clear();
        //        conn.Close();
        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        MyExceptionHandler.LogError(ex);
        //        return null;
        //    }
        //}

        #region ImageData
        public const int exifOrientationID = 0x0112;

        public static byte[] GetImageBytes(byte[] inputArray)
        {
            Image img = ByteArrayToImage(inputArray);
            if (img.PropertyIdList.Contains(exifOrientationID))
            {
                var orientation = (int)img.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        break;
                    case 2:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        img.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        img.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        img.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                img.RemovePropertyItem(274);
            }
            return ImageToByteArray(img);
        }
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(byteArrayIn))
                {
                    System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
                    Image img = (Image)converter.ConvertFrom(byteArrayIn);

                    return img;
                }
            }
            catch (Exception ex)
            {
                MyExceptionHandler.LogError(ex);
                throw ex;
            }
        }
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            ImageConverter imgCon = new ImageConverter();
            return (byte[])imgCon.ConvertTo(imageIn, typeof(byte[]));
        }
        #endregion


    }
    public class QueryString
    {
        public static Dictionary<string, string> GetDecryptedParameters(string qs)
        {
            Dictionary<string, string> DecryptedParameters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(qs))
            {
                try
                {
                    string[] arrMsgs = SecureQueryString.SecureQueryString.Decrypt(qs).Split('&');
                    string[] arrIndMsg = { };
                    for (int i = 0; i <= arrMsgs.Length - 1; i++)
                    {
                        arrIndMsg = arrMsgs[i].Split('=');
                        DecryptedParameters.Add(arrIndMsg[0], arrIndMsg[1]);
                    }
                }
                catch { }
            }
            return DecryptedParameters;
        }
    }
}