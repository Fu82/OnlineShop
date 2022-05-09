using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using OnlineShop.DTOs;
using OnlineShop.Models;
using OnlineShop.Tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly OnlineShopContext _OnlineShopContext;
        public LoginController(OnlineShopContext onlineShopContext)
        {
            _OnlineShopContext = onlineShopContext;
        }

        //SQL連線字串 SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        /// <summary>產生亂數字串</summary>
        /// <param name="Number">字元數</param>
        /// <returns></returns>
        //public string CreateRandomCode(int Number)
        //{
        //    string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
        //    string[] allCharArray = allChar.Split(',');
        //    string randomCode = "";
        //    int temp = -1;

        //    Random rand = new Random();
        //    for (int i = 0; i < Number; i++)
        //    {
        //        if (temp != -1)
        //        {
        //            rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
        //        }
        //        int t = rand.Next(36);
        //        if (temp != -1 && temp == t)
        //        {
        //            return CreateRandomCode(Number);
        //        }
        //        temp = t;
        //        randomCode += allCharArray[t];
        //    }
        //    return randomCode;
        //}

        [HttpPost]
        public string Login(MemberSelectDto value)
        {
            //查詢伺服器狀態是否正常
            if (ModelState.IsValid == false)
            {
                /*****/
                return "輸入參數有誤";
            }

            string loginErrorStr = "";//記錄錯誤訊息

            //帳號資料驗證
            if (value.Account == "" || (string.IsNullOrEmpty(value.Account)))
            {
                loginErrorStr += "【 帳號不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    loginErrorStr += "【 🚫帳號只能為英數 】\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    loginErrorStr += "【 🚫帳號長度應介於8～20個數字之間 】\n";
                }
            };

            //密碼資料驗證
            if (value.Pwd == "" || (string.IsNullOrEmpty(value.Pwd)))
            {
                loginErrorStr += "【 密碼不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Pwd))
                {
                    loginErrorStr += "【 🚫密碼只能為英數 】\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    loginErrorStr += "【 🚫密碼長度應介於8～16個數字之間 】\n";
                }
            }

            //錯誤訊息不為空
            if (loginErrorStr != "")
            {
                return loginErrorStr;
            }
            else
            {
                SqlCommand cmd = null;
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter();

                try
                {
                    // 資料庫連線
                    cmd = new SqlCommand();
                    cmd.Connection = new SqlConnection(SQLConnectionString);

                    cmd.CommandText = @"EXEC pro_onlineShop_getMember @f_acc, @f_pwd";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@f_pwd", Tool.InTool.PwdToMD5(value.Pwd));

                    //開啟連線
                    cmd.Connection.Open();

                    if (cmd.ExecuteScalar() == null)
                    {
                        return "登入失敗";
                    }
                    else
                    {
                        da.SelectCommand = cmd;
                        da.Fill(dt);

                        //添加角色權限
                        var claims = new List<Claim>
                        {
                           new Claim(ClaimTypes.Name, value.Account), //存使用者名稱,
                        };


                        HttpContext.Session.SetString("SessionID", dt.Rows[0]["f_id"].ToString());

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        //Random a = new Random();
                        //int x;
                        //x = a.Next(1, 11);

                        //Dictionary<string, int> dic = new Dictionary<string, int>();
                        //dic.Add("key", x);

                        //string aa = dic["key"].ToString();

                        Random a = new Random();
                        int x;
                        x = a.Next(1, 11);

                        ConcurrentDictionary<string, int> dic = new ConcurrentDictionary<string, int>();
                        dic.TryAdd("key", x);

                        string aa = dic["key"].ToString();

                        //判斷是否重複登入
                        if (User.Identity.IsAuthenticated)
                        {
                            return "請先登出再進行登入";
                        }
                        else
                        {
                            return "登入成功" + "驗證碼:" + aa;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
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

        [HttpDelete("Logout")]
        public void logout()
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
