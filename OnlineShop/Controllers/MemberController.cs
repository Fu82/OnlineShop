using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using OnlineShop.DTOs;
using OnlineShop.Models;
using OnlineShop.Tool;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {

        private readonly OnlineShopContext _OnlineShopContext;
        public MemberController(OnlineShopContext onlineShopContext)
        {
            _OnlineShopContext = onlineShopContext;
        }

        //SQL連線字串 SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        // GET: api/<MemberController>
        [HttpGet]
        public IEnumerable<MemberSelectDto> Get()
        {
            var result = _OnlineShopContext.TMember
                .Select(a => new MemberSelectDto
                {
                    Id = a.FId,
                    Account = a.FAcc,
                    Pwd = a.FPwd,
                    Phone = a.FPhone,
                    Mail = a.FMail
                });

            return result;
        }

        // GET api/<MemberController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MemberController>
        [HttpPost]
        public string Post([FromBody] MemberSelectDto value)
        {
            //後端驗證

            string addMemberError = "";

            if (value.Account == "" && value.Pwd == "" && value.Phone == "" && value.Mail == "")
            {
                addMemberError += "【 🚫欄位必填 】\n";
            }
            
            if (value.Account != "")
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    addMemberError += "【 🚫帳號只能為英數 】";
                }
                if(value.Account.Length > 20 || value.Account.Length < 8)
                {
                    addMemberError += "【 🚫帳號長度應介於8～20個數字之間 】\n";
                }
            }
            else
            {
                addMemberError += "【 🚫帳號未填 】\n";
            };

            if (value.Pwd != "")
            {
                if (!InTool.IsENAndNumber(value.Pwd))
                {
                    addMemberError += "【 🚫密碼只能為英數 】\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    addMemberError += "【 🚫密碼長度應介於8～16個數字之間 】\n";
                }
            }
            else
            {
                addMemberError += "【 🚫密碼未填 】\n";
            };

            if (value.Phone != "")
            {
                if (!InTool.IsNumber(value.Phone))
                {
                    addMemberError += "【 🚫手機只能為數字 】\n";
                }
                if (value.Phone.Length < 10)
                {
                    addMemberError += "【 🚫手機格式錯誤 】\n";
                }
            }
            else
            {
                addMemberError += "【 🚫手機未填 】\n";
            };

            if (value.Mail != "")
            {
                if (!InTool.IsMail(value.Mail))
                {
                    addMemberError += "【 🚫信箱格式錯誤 】\n";
                }
            }
            else
            {
                addMemberError += "【 🚫信箱未填 】\n";
            };

            if (addMemberError != "")
            {
                return addMemberError;
            }
            else
            {
                SqlCommand cmd = null;
                //DataTable dt = new DataTable();

                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(SQLConnectionString);

                    //帳號重複驗證寫在SP中
                    cmd.CommandText = @"EXEC pro_onlineShop_addMember @f_acc, @f_pwd, @f_phone, @f_mail";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@f_pwd", Tool.InTool.PwdToMD5(value.Pwd));
                    cmd.Parameters.AddWithValue("@f_phone", value.Phone);
                    cmd.Parameters.AddWithValue("@f_mail", value.Mail);

                    //開啟連線
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery(); //執行Transact-SQL
                    cmd.Connection.Close();
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
                }
                return "新增成功";
            }
        }

        #region 舊寫法MD5
        //using (var md5 = MD5.Create())
        //{
        //    var result = md5.ComputeHash(Encoding.ASCII.GetBytes(value.Pwd));//MD5 加密傳密碼進去

        //    var strResult = BitConverter.ToString(result);

        //    var user = (from a in _OnlineShopContext.TMember
        //                where a.FAcc == value.Account
        //                && a.FPwd == strResult.Replace("-", "")
        //                select a).SingleOrDefault();

        //    if (user == null)
        //    {
        //        return "帳號密碼錯誤";
        //    }
        //    else
        //    {
        //        //這邊等等寫驗證
        //        var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.FAcc),
        //    };
        //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        //        return "OK";
        //    }
        //}
        #endregion

        ////Mail判斷(測試)
        //public static bool IsMaill(string value)
        //{
        //    try
        //    {
        //        var addr = new System.Net.Mail.MailAddress(value);
        //        return addr.Address.ToUpper() == value.ToUpper();
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        // PUT api/<MemberController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MemberController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }   
    }
}
