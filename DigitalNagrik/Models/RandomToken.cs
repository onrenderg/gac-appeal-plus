using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DigitalNagrik.Models
{
    public class RandomToken
    {
        public static string RandomTokenGeneration()
        {
            HttpContext.Current.Session["_RandomToken"] = RandomString(50);
            return Convert.ToString(HttpContext.Current.Session["_RandomToken"]);
        }

        public static string ValidateRandomToken(string clientToken)
        {
            if (clientToken.Equals(HttpContext.Current.Session["_RandomToken"]))
            {
                HttpContext.Current.Session["_RandomToken"] = RandomString(10);
                return Convert.ToString('Y');
            }
            else
            {
                return Convert.ToString('N');
            }
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                //return Convert.ToHexString(hashBytes); // .NET 5 +

                // Convert the byte array to hexadecimal string prior to .NET 5
                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}