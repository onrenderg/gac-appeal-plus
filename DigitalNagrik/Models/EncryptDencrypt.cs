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
        
        public static string Encrypt(string PlainText, string secretKey)
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
     
    }
}