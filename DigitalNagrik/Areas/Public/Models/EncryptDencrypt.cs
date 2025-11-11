using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
namespace DigitalNagrik.Areas.Public.Models
{
    public class EncryptDencrypt
    {
        static string ComputeSha256Hash(string rawData)
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
        public  static string Encrypt(string PlainText, string secretKey)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.ECB;
            byte[] keyArr = generateAES256Key(secretKey);
            byte[] KeyArrBytes32Value = new byte[16];
            Array.Copy(keyArr, KeyArrBytes32Value, 16);
            aes.Key = KeyArrBytes32Value;
            ICryptoTransform encrypto = aes.CreateEncryptor();
            byte[] plainTextByte = ASCIIEncoding.UTF8.GetBytes(PlainText);
            byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
            return Convert.ToBase64String(CipherText);
        }

        public static byte[] generateAES256Key(string seed)
        {
            SHA256 sha256 = SHA256CryptoServiceProvider.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(seed));
        }
        public static String EncryptAES(String plainText)
        {
            try
            {
                string key = ComputeSha256Hash("df0025ae-77d8-4806-aa72-ee7610b00bf5");
                //string key = "df0025ae-77d8-4806-aa72-ee7610b00bf5";
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(Encrypt(plainBytes, getRijndaelManaged(key)));
            }
            catch (Exception ex)
            {               
                    return plainText;                
            }

        }

        public static String DecryptAES(String encryptedText)
        {
            try
            {
               string key = ComputeSha256Hash("df0025ae-77d8-4806-aa72-ee7610b00bf5");
               
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(Decrypt(encryptedBytes, getRijndaelManaged(key)));
            }
            catch (Exception)
            {
                {
                    return encryptedText;
                }                
            }

        }

        private static RijndaelManaged getRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                KeySize = 256,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
    }
}