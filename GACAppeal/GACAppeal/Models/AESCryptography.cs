using System;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials;
namespace GACAppeal
{
    public class AESCryptography
    {
        public static string EncryptAES(string plainText)
        {
            try
            {
                string key = Preferences.Get("EncKey", "xxxxxxxxxxxxxxxx"); ;
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(Encrypt(plainBytes, getRijndaelManaged(key)));
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string DecryptAES(string encryptedText)
        {
            try
            {
                string key = Preferences.Get("EncKey", "xxxxxxxxxxxxxxxx"); ;
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                return Encoding.UTF8.GetString(Decrypt(encryptedBytes, getRijndaelManaged(key)));
            }
            catch (Exception)
            {
                return "";
            }
        }
        private static RijndaelManaged getRijndaelManaged(string secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
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