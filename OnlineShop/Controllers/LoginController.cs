using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using OnlineShop.DTOs;
using OnlineShop.Models;
using OnlineShop.Tool;
using System;
using System.Data;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly OnlineShopContext _OnlineShopContext;
        public LoginController(OnlineShopContext onlineShopContext)
        {
            _OnlineShopContext = onlineShopContext;
        }

        //SQL連線字串 SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        [HttpPost]
        public string Login(MemberSelectDto value)
        {
            //後端驗證

            string loginError = "";

            if (value.Account == "" && value.Pwd == "")
            {
                loginError += "【 🚫欄位必填 】\n";
            }
            
            if (value.Account != "")
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    loginError += "【 🚫帳號只能為英數 】";
                }
                if(value.Account.Length > 20 || value.Account.Length < 8)
                {
                    loginError += "【 🚫帳號長度應介於8～20個數字之間 】\n";
                }
            }
            else
            {
                loginError += "【 🚫帳號未填 】\n";
            };

            if (value.Pwd != "")
            {
                if (!InTool.IsENAndNumber(value.Pwd))
                {
                    loginError += "【 🚫密碼只能為英數 】\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    loginError += "【 🚫密碼長度應介於8～16個數字之間 】\n";
                }
            }
            else
            {
                loginError += "【 🚫密碼未填 】\n";
            };

            if (loginError != "")
            {
                return loginError;
            }
            else
            {
                SqlCommand cmd = null;
                DataTable dt = new DataTable();

                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShop_getMember @f_acc, @f_pwd";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@f_pwd", Tool.InTool.PwdToMD5(value.Pwd));

                    SqlDataAdapter da = new SqlDataAdapter();

                    //開啟連線
                    cmd.Connection.Open();

                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    cmd.Connection.Close();

                    if (dt.Rows.Count == 0)
                    {
                        return "帳號密碼錯誤";
                    }
                    else
                    {
                        return "登入成功";
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    return "123";
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Parameters.Clear();
                        cmd.Connection.Close();
                    }
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
        }

        [HttpDelete]
        public void Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        [HttpGet("NoLogin")]
        public string noLogin()
        {
            return "未登入";
        }
    }
}
