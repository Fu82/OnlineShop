using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Tool
{
    public class InCode
    {
        /// <summary>
        /// 產生4位亂數字串
        /// </summary>
        public static string VerifyKey()
        {
            string key = "";
            Random r = new Random();

            int num1 = r.Next(0, 9);
            int num2 = r.Next(0, 9);
            int num3 = r.Next(0, 9);
            int num4 = r.Next(0, 9);

            int[] numbers = new int[4] { num1, num2, num3, num4 };
            for (int i = 0; i < numbers.Length; i++)
            {
                key += numbers[i].ToString();
            }
            return key;
        }

        /// <summary>
        /// 存取4位數至記憶體
        /// </summary>
        public static ConcurrentDictionary<string, TimeCode> dic = new ConcurrentDictionary<string, TimeCode>();

        public class TimeCode
        {
            public string KeyCode { get; set; } = string.Empty;

            public DateTime ValidTime { get; set; } = DateTime.Now.AddMinutes(10);
        }
    }
}
