using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using OnlineShop.Models;
using OnlineShop.Tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MemberController : ControllerBase
    {

        //SQL連線字串 SQLConnectionString
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        //已註解
        #region GetAccount EF舊寫法用所需
        //private readonly OnlineShopContext _OnlineShopContext;
        //public MemberController(OnlineShopContext onlineShopContext)
        //{
        //    _OnlineShopContext = onlineShopContext;
        //}
        #endregion

        //已註解
        #region  GetAccount舊寫法EF
        //[HttpGet]
        //public IEnumerable<MemberSelectDto> Get()
        //#region 帳號相關列舉(Enum)
        //private enum addACCountErrorCode //新增帳號
        //{
        //    var result = _OnlineShopContext.TMember
        //        .Select(a => new MemberSelectDto
        //        {
        //            Id = a.FId,
        //            Account = a.FAcc,
        //            Pwd = a.FPwd,
        //            Phone = a.FPhone,
        //            Mail = a.FMail
        //        });

        //    return result;
        //}
        #endregion

        #region 帳號相關列舉(Enum)
        private enum addACCountErrorCode //新增帳號
        {
            //<summary >
            //帳號新增成功
            //</summary >
            AddOK = 0,

            //<summary >
            //帳號重複
            //</summary >
            duplicateAccount = 100
        }
        private enum PutAccErrorCode //更新會員帳號
        {
            //<summary >
            //會員帳號更新成功
            //</summary >
            PutOK = 0
        }
        private enum AuthAccErrorCode //驗證會員帳號
        {
            //<summary >
            //會員帳號更新成功
            //</summary >
            AuthOK = 0
        }
        private enum putKeyErrorCode
        {
            //<summary >
            //
            //</summary >
            PutKeyOK = 0,

            //<summary >
            //帳號不存在
            //</summary >
            AccIsNull = 101
        }
        private enum putMemberPwdErrorCode //忘記密碼會員驗證
        {
            //<summary >
            //密碼變更成功
            //</summary >
            PutOK = 0,

            //<summary >
            //新密碼與確認密碼不相同
            //</summary >
            confirmError = 100,

            //<summary >
            //帳號不存在
            //</summary >
            AccIsNull = 101
        }
        #endregion


        //增加帳號
        [HttpPost("AddAcc")]
        public string AddAcc([FromBody] MemberSelectDto value)
        {
            //後端驗證
            //如字串字數特殊字元驗證

            string addMemberErrorStr = ""; //記錄錯誤訊息

            //帳號資料驗證
            if (string.IsNullOrEmpty(value.Account)) //空字串判斷and Null值判斷皆用IsNullOrEmpty
            {
                addMemberErrorStr += "【 🚫帳號不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    addMemberErrorStr += "【 🔒帳號只能為英數 】";
                }
                if (value.Account.Length > 20 || value.Account.Length < 8)
                {
                    addMemberErrorStr += "【 🔒帳號長度應介於8～20個數字之間 】\n";
                }
            }

            //密碼資料驗證
            if (string.IsNullOrEmpty(value.Pwd))
            {
                addMemberErrorStr += "【 🚫密碼不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Pwd))
                {
                    addMemberErrorStr += "【 ㊙️密碼只能為英數 】\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    addMemberErrorStr += "【 ㊙️密碼長度應介於8～16個數字之間 】\n";
                }
            }

            //手機資料驗證
            if (string.IsNullOrEmpty(value.Phone))
            {
                addMemberErrorStr += "【 🚫手機不可為空 】\n";
            }
            else
            {
                if (!InTool.IsNumber(value.Phone))
                {
                    addMemberErrorStr += "【 📞手機只能為數字 】\n";
                }
                if (value.Phone.Length < 10)
                {
                    addMemberErrorStr += "【 📞手機格式錯誤 】\n";
                }
            }

            //信箱資料驗證
            if (string.IsNullOrEmpty(value.Mail))
            {
                addMemberErrorStr += "【 🚫信箱不可為空 】\n";
            }
            else
            {
                if (!InTool.IsMail(value.Mail))
                {
                    addMemberErrorStr += "【 📧信箱格式錯誤 】\n";
                }
            }

            if (!string.IsNullOrEmpty(addMemberErrorStr))
            {
                return addMemberErrorStr;
            }

            SqlCommand cmd = null;

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
                addMemberErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addMemberErrorStr);

                InCode.TimeCode timeCode = new InCode.TimeCode();
                timeCode.KeyCode = InCode.VerifyKey();
                timeCode.ValidTime = DateTime.Now.AddMinutes(10);

                InCode.dic.TryAdd(value.Account, timeCode);

                switch (SQLReturnCode)
                {
                    case (int)addACCountErrorCode.duplicateAccount:
                        return "此帳號已存在";

                    case (int)addACCountErrorCode.AddOK:
                        return "帳號新增成功  " + "驗證碼： " + InCode.dic[value.Account].KeyCode;

                    default:
                        return "失敗";
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


        //驗證帳號
        [HttpPut("VerifyMember")]
        //public IEnumerable<AccountSelectDto> Get()
        public string VerifyMember([FromBody] MemberSelectDto value)
        {

            string AuthMemberErrorStr = "";//記錄錯誤訊息

            //查詢伺服器狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "輸入參數有誤";
            }

            //帳號資料驗證
            if (value.Account == "" || (string.IsNullOrEmpty(value.Account)))
            {
                AuthMemberErrorStr += "【 🚫帳號不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    AuthMemberErrorStr += "【 🔒帳號只能為英數 】\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    AuthMemberErrorStr += "【 🔒帳號長度應介於8～20個數字之間 】\n";
                }
            }

            //密碼資料驗證
            if (value.Pwd == "" || (string.IsNullOrEmpty(value.Pwd)))
            {
                AuthMemberErrorStr += "【 🚫密碼不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Pwd))
                {
                    AuthMemberErrorStr += "【 ㊙️密碼只能為英數 】\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    AuthMemberErrorStr += "【 ㊙️密碼長度應介於8～16個數字之間 】\n";
                }
            }

            //驗證碼資料驗證
            if (value.Code == "")
            {
                AuthMemberErrorStr += "【 🚫驗證碼不可為空 】\n";
            }
            else
            {
                if (!InTool.IsNumber(value.Code))
                {
                    AuthMemberErrorStr += "【 🔑驗證碼只能為數字 】\n";
                }
            }

            //錯誤訊息不為空
            if (AuthMemberErrorStr != "")
            {
                return AuthMemberErrorStr;
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

                    cmd.CommandText = @"EXEC pro_onlineShop_putMemberAuth @f_acc, @f_pwd";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@f_pwd", Tool.InTool.PwdToMD5(value.Pwd));

                    //開啟連線
                    cmd.Connection.Open();
                    AuthMemberErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                    int SQLReturnCode = int.Parse(AuthMemberErrorStr);


                    switch (SQLReturnCode)
                    {
                        case (int)AuthAccErrorCode.AuthOK:

                            if (value.Code != InCode.dic[value.Account].KeyCode)
                            {
                                return "【 🔑驗證碼錯誤 】";
                            }
                            else if (InCode.dic[value.Account].ValidTime < DateTime.Now)
                            {
                                InCode.dic.TryRemove(value.Account, out _);
                                return "【 🔑驗證碼失效 】";
                            }
                            else
                            {
                                InCode.dic.TryRemove(value.Account, out _);
                                return "驗證成功";
                            }

                        default:
                            return "失敗";
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
        }


        //取得會員資料
        [HttpGet("GetMember")]
        //public IEnumerable<AccountSelectDto> Get()
        public string GetMember([FromQuery] int id)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // 資料庫連線&SQL指令
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);
                cmd.CommandText = @"EXEC pro_onlineShop_getMemberList @f_Id";
                cmd.Parameters.AddWithValue("@f_Id", id);

                //開啟連線
                cmd.Connection.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                //關閉連線
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    cmd.Connection.Close();
                }
            }

            //DataTable轉Json;
            var result = Tool.InTool.DataTableJson(dt);

            return result;
        }


        //編輯資料
        [HttpPut("PutMember")]
        public string PutAcc([FromQuery] int id, [FromBody] MemberSelectDto value)
        {
            //登入&身分檢查
            if (!loginValidate())
            {
                return "已從另一地點登入,轉跳至登入頁面";
            }

            string putMemberErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            if (!string.IsNullOrEmpty(putMemberErrorStr))
            {
                return putMemberErrorStr;
            }

            SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShop_putMemberList @f_id, @f_name, @f_address";

                cmd.Parameters.AddWithValue("@f_id", id);
                cmd.Parameters.AddWithValue("@f_name", value.Name);
                cmd.Parameters.AddWithValue("@f_address", value.Address);

                //開啟連線
                cmd.Connection.Open();
                putMemberErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(putMemberErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)PutAccErrorCode.PutOK:
                        return "帳號更新成功";

                    default:
                        return "失敗";
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


        //忘記密碼
        [HttpPost("PostForgetPwd")]
        public string PostMemberPwd([FromBody] MemberSelectDto value)
        {
            string postMemberPwdErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //帳號資料驗證
            if (value.Account == "" || (string.IsNullOrEmpty(value.Account)))
            {
                postMemberPwdErrorStr += "【 🚫帳號不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    postMemberPwdErrorStr += "【 🔒帳號只能為英數 】\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    postMemberPwdErrorStr += "【 🔒帳號長度應介於8～20個數字之間 】\n";
                }
            }

            //錯誤訊息不為空
            if (postMemberPwdErrorStr != "")
            {
                return postMemberPwdErrorStr;
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

                    cmd.CommandText = @"EXEC pro_onlineShop_getForgetMember @f_acc";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);

                    //開啟連線
                    cmd.Connection.Open();
                    postMemberPwdErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                    int SQLReturnCode = int.Parse(postMemberPwdErrorStr);

                    InCode.TimeCode timeCode = new InCode.TimeCode();
                    timeCode.KeyCode = InCode.VerifyKey();
                    timeCode.ValidTime = DateTime.Now.AddMinutes(10);

                    InCode.dic.TryAdd(value.Account, timeCode);

                    switch (SQLReturnCode)
                    {
                        case (int)putKeyErrorCode.AccIsNull:
                            return "此帳號不存在";

                        case (int)putKeyErrorCode.PutKeyOK:
                            return "帳號正確  " + "驗證碼：" + InCode.dic[value.Account].KeyCode;

                        default:
                            return "失敗";
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
        }


        //驗證新密碼
        [HttpPut("VerifyForgetPwd")]
        public string PutMemberPwd([FromBody] PutPwdDto value)
        {
            string putMemberPwdErrorStr = "";//記錄錯誤訊息

            //查詢伺服器狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "輸入參數有誤";
            }

            //帳號資料驗證
            if (value.Account == "" || (string.IsNullOrEmpty(value.Account)))
            {
                putMemberPwdErrorStr += "【 🚫帳號不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    putMemberPwdErrorStr += "【 🔒帳號只能為英數 】\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    putMemberPwdErrorStr += "【 🔒帳號長度應介於8～20個數字之間 】\n";
                }
            }

            //密碼資料驗證
            if (string.IsNullOrEmpty(value.newPwd) || string.IsNullOrEmpty(value.cfmNewPwd))//空字串判斷and Null值判斷皆用IsNullOrEmpty
            {
                putMemberPwdErrorStr += "【 🚫新密碼或確認密碼不可為空 】\n";
            }
            else
            {
                if (value.newPwd != value.cfmNewPwd)//空字串判斷and Null值判斷皆用IsNullOrEmpty
                {
                    putMemberPwdErrorStr += "【 ㊙️新密碼與確認新密碼需相同 】\n";
                }

                if (!InTool.IsENAndNumber(value.newPwd) || !InTool.IsENAndNumber(value.cfmNewPwd))
                {
                    putMemberPwdErrorStr += "【 ㊙️密碼只能為英數 】\n";
                }
                if (value.newPwd.Length > 16 || value.newPwd.Length < 8)
                {
                    putMemberPwdErrorStr += "【 ㊙️密碼長度應介於8～16個數字之間 】\n";
                }
            }

            //驗證碼資料驗證
            if (value.Code == "")
            {
                putMemberPwdErrorStr += "【 🚫驗證碼不可為空 】\n";
            }
            else
            {
                if (!InTool.IsNumber(value.Code))
                {
                    putMemberPwdErrorStr += "【 🔑驗證碼只能為數字 】\n";
                }
            }

            //錯誤訊息不為空
            if (putMemberPwdErrorStr != "")
            {
                return putMemberPwdErrorStr;
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

                    cmd.CommandText = @"EXEC pro_onlineShop_putForgetMemberPwd @f_acc, @newPwd, @cfmNewPwd";

                    cmd.Parameters.AddWithValue("@f_acc", value.Account);
                    cmd.Parameters.AddWithValue("@newPwd", Tool.InTool.PwdToMD5(value.newPwd));
                    cmd.Parameters.AddWithValue("@cfmNewPwd", Tool.InTool.PwdToMD5(value.cfmNewPwd));

                    //開啟連線
                    cmd.Connection.Open();
                    putMemberPwdErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                    int SQLReturnCode = int.Parse(putMemberPwdErrorStr);

                    switch (SQLReturnCode)
                    {
                        case (int)putMemberPwdErrorCode.confirmError:
                            return "新密碼與確認新密碼不相同";

                        case (int)putMemberPwdErrorCode.AccIsNull:
                            return "此帳號不存在";

                        case (int)putMemberPwdErrorCode.PutOK:

                            if (value.Code != InCode.dic[value.Account].KeyCode)
                            {
                                return "【 🔑驗證碼錯誤 】";
                            }
                            else if (InCode.dic[value.Account].ValidTime < DateTime.Now)
                            {
                                InCode.dic.TryRemove(value.Account, out _);
                                return "【 🔑驗證碼失效 】";
                            }
                            else
                            {
                                InCode.dic.TryRemove(value.Account, out _);
                                return "密碼修改成功";
                            }

                        default:
                            return "失敗";
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
        }


        //登入檢查
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
