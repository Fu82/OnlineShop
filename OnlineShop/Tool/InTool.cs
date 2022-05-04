using System;
using System.Data;
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

        //DataTable轉JSON
        public static string DataTableJson(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.Append("\"");
                    sb.Append(dt.Columns[j].ColumnName);
                    sb.Append("\":\"");
                    sb.Append(dt.Rows[i][j].ToString());
                    sb.Append("\",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }

        //判斷字串是否只有英數
        public static bool IsENAndNumber(string str)
        {
            System.Text.RegularExpressions.Regex regEN = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return regEN.IsMatch(str);
        }

        //判斷字串是否只有數字
        public static bool IsNumber(string str)
        {
            System.Text.RegularExpressions.Regex regN = new System.Text.RegularExpressions.Regex(@"^[0-9]*$");
            return regN.IsMatch(str);
        }

        //判斷信箱字串格式
        public static bool IsMail (string str)
        {
            System.Text.RegularExpressions.Regex regM = new System.Text.RegularExpressions.Regex(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$");
            return regM.IsMatch(str);
        }
    }
}
