using System;
using System.Security.Cryptography;
using System.Text;

namespace OnlineShop.Tool
{
    public class InTool
    {
        //MD5加密
        public static string PwdToMD5(string pwd)
        {
            var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.ASCII.GetBytes(pwd));//MD5 加密傳密碼進去
            var strResult = BitConverter.ToString(result);
            var md5pwd = strResult.Replace("-", "");
            return md5pwd;
        }
    }
}
