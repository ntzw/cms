using System;
using System.Security.Cryptography;
using System.Text;

namespace Helper
{
    public class Md5Helper
    {
        public static string GetMD5_32(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var t = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            var sb = new StringBuilder(32);
            for (var i = 0; i < t.Length; i++) sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }

        //16位加密
        public static string GetMd5_16(string s)
        {
            var md5 = new MD5CryptoServiceProvider();
            var t2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(s)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
    }
}