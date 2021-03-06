using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
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
            //查詢伺服器狀態是否正常
            if (ModelState.IsValid == false)
            {
                /*****/
                return "輸入參數有誤";
            }

            if (User.Identity.IsAuthenticated)
            {
                return "請先登出再進行登入";
            }

            string loginErrorStr = "";//記錄錯誤訊息

            //帳號資料驗證
            if (value.Account == "" || (string.IsNullOrEmpty(value.Account)))
            {
                loginErrorStr += "【 🚫帳號不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    loginErrorStr += "【 🔒帳號只能為英數 】\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    loginErrorStr += "【 🔒帳號長度應介於8～20個數字之間 】\n";
                }
            };

            //密碼資料驗證
            if (value.Pwd == "" || (string.IsNullOrEmpty(value.Pwd)))
            {
                loginErrorStr += "【 🚫密碼不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Pwd))
                {
                    loginErrorStr += "【 ㊙️密碼只能為英數 】\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    loginErrorStr += "【 ㊙️密碼長度應介於8～16個數字之間 】\n";
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
                        return "登入失敗"; //登入失敗
                    }
                    else //登入成功
                    {
                        da.SelectCommand = cmd;
                        da.Fill(dt);

                        //Session傳遞
                        HttpContext.Session.SetString("Account", value.Account);
                        HttpContext.Session.SetString("MemberID", cmd.ExecuteScalar().ToString());

                        //資料庫中 Account為空 or 存的sessionId與現在的不符
                        if (!string.IsNullOrWhiteSpace(dt.Rows[0]["f_sessionId"].ToString()) &&
                            dt.Rows[0]["f_sessionId"].ToString() != HttpContext.Session.Id)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = @"UPDATE t_member WITH(ROWLOCK) SET f_sessionId = @sessionId WHERE f_acc = @f_acc
                                                SELECT f_sessionId FROM t_member WHERE f_acc = @f_acc ";
                            cmd.Parameters.AddWithValue("@sessionId", HttpContext.Session.Id);
                            cmd.Parameters.AddWithValue("@f_acc", value.Account);

                            SessionDB.SessionInfo SessionInfo = new SessionDB.SessionInfo();
                            SessionInfo.SId = cmd.ExecuteScalar().ToString();//updata完後的sessionId
                            SessionInfo.ValidTime = DateTime.Now.AddMinutes(30);//失效時間 =(現在時間在加30分鐘)


                            SessionDB.sessionDB.AddOrUpdate(HttpContext.Session.GetString("Account"),
                                                            SessionInfo,
                                                           (key, oldValue) => oldValue = SessionInfo);

                            return "重複登入";  //重複登入
                        }
                        else
                        {
                            //登入成功 紀錄session 在DB中
                            cmd.Parameters.Clear();
                            cmd.CommandText = @"UPDATE t_member WITH(ROWLOCK) SET f_sessionId = @sessionId WHERE f_acc = @f_acc 
                                                SELECT f_sessionId FROM t_member WHERE f_acc = @f_acc ";
                            cmd.Parameters.AddWithValue("@sessionId", HttpContext.Session.Id);
                            cmd.Parameters.AddWithValue("@f_acc", value.Account);

                            SessionDB.SessionInfo SessionInfo = new SessionDB.SessionInfo();
                            SessionInfo.SId = cmd.ExecuteScalar().ToString(); //updata完後的sessionId
                            SessionInfo.ValidTime = DateTime.Now.AddMinutes(30);//失效時間 =(現在時間在加30分鐘)

                            SessionDB.sessionDB.AddOrUpdate(HttpContext.Session.GetString("Account"),
                                                            SessionInfo,
                                                           (key, oldValue) => oldValue = SessionInfo);

                            return "登入成功";
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

        //登出
        [HttpDelete("Logout")]
        public void logout()
        {
            //HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //登入&身分檢查
            if (!loginValidate())
            {
                return;
            }


            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {

                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                //清空DB的sessionId
                cmd.CommandText = @"UPDATE t_member WITH(ROWLOCK) SET f_sessionId = '' WHERE f_acc = @f_acc ";
                cmd.Parameters.AddWithValue("@f_acc", HttpContext.Session.GetString("Account"));
                //開啟連線
                cmd.Connection.Open();
                cmd.ExecuteScalar();
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Parameters.Clear();
                }
            }

            //清空Dictionary & Session[Account]中的值

            SessionDB.sessionDB.TryRemove(HttpContext.Session.GetString("Account"), out _);
            HttpContext.Session.Clear();
        }
        [HttpGet("NoLogin")]
        public string noLogin()
        {
            return "未登入";
        }
        private bool loginValidate()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||                        //判斷Session[Account]是否為空
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//判斷DB SessionId與瀏覽器 SessionId是否一樣
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)       //判斷是否過期
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
