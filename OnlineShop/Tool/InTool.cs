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

        //是否只有英數
        public static bool IsENAndNumber(string str)
        {
            System.Text.RegularExpressions.Regex regEN = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return regEN.IsMatch(str);
        }

        //是否只有數字
        public static bool IsNumber(string str)
        {
            System.Text.RegularExpressions.Regex regN = new System.Text.RegularExpressions.Regex(@"^[0-9]*$");
            return regN.IsMatch(str);
        }

        //是否只有數字
        public static bool IsMail (string str)
        {
            System.Text.RegularExpressions.Regex regM = new System.Text.RegularExpressions.Regex(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$");
            return regM.IsMatch(str);
        }
    }
}
