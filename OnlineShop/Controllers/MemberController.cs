﻿using Microsoft.AspNetCore.Authorization;
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
        public static ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();

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
        private enum GetForgetMemberErrorCode //忘記密碼會員驗證
        {
            //<summary >
            //會員帳號正確
            //</summary >
            GetMemberOK = 0
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
                    addMemberErrorStr += "【 🚫帳號只能為英數 】";
                }
                if (value.Account.Length > 20 || value.Account.Length < 8)
                {
                    addMemberErrorStr += "【 🚫帳號長度應介於8～20個數字之間 】\n";
                }
            }

            //密碼資料驗證
            if (string.IsNullOrEmpty(value.Pwd))
            {
                addMemberErrorStr += "[密碼不可為空]\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Pwd))
                {
                    addMemberErrorStr += "【 🚫密碼只能為英數 】\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    addMemberErrorStr += "【 🚫密碼長度應介於8～16個數字之間 】\n";
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
                    addMemberErrorStr += "【 🚫手機只能為數字 】\n";
                }
                if (value.Phone.Length < 10)
                {
                    addMemberErrorStr += "【 🚫手機格式錯誤 】\n";
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
                    addMemberErrorStr += "【 🚫信箱格式錯誤 】\n";
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

                dic.GetOrAdd("code", VerifyKey());

                switch (SQLReturnCode)
                {
                    case (int)addACCountErrorCode.duplicateAccount:
                        return "此帳號已存在";

                    case (int)addACCountErrorCode.AddOK:
                        return "帳號新增成功  " + "驗證碼： " + dic["code"];

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

        //取得會員資料
        [HttpGet("GetMember")]
        //public IEnumerable<AccountSelectDto> Get()
        public string GetMember([FromQuery] int id )
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // 資料庫連線&SQL指令
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"EXEC pro_onlineShop_getMemberList @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            //開啟連線
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //關閉連線
            cmd.Connection.Close();

            //DataTable轉Json;
            var result = Tool.InTool.DataTableJson(dt);

            return result;
        }

        //驗證帳號
        [HttpPut("VerifyMember")]
        //public IEnumerable<AccountSelectDto> Get()
        public string VerifyMember([FromBody] MemberSelectDto value)
        {
            //查詢伺服器狀態是否正常
            if (ModelState.IsValid == false)
            {
                /*****/
                return "輸入參數有誤";
            }

            string AuthMemberErrorStr = "";//記錄錯誤訊息

            //帳號資料驗證
            if (value.Account == "" || (string.IsNullOrEmpty(value.Account)))
            {
                AuthMemberErrorStr += "【 帳號不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    AuthMemberErrorStr += "【 🚫帳號只能為英數 】\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    AuthMemberErrorStr += "【 🚫帳號長度應介於8～20個數字之間 】\n";
                }
            }

            //密碼資料驗證
            if (value.Pwd == "" || (string.IsNullOrEmpty(value.Pwd)))
            {
                AuthMemberErrorStr += "【 密碼不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Pwd))
                {
                    AuthMemberErrorStr += "【 🚫密碼只能為英數 】\n";
                }
                if (value.Pwd.Length > 16 || value.Pwd.Length < 8)
                {
                    AuthMemberErrorStr += "【 🚫密碼長度應介於8～16個數字之間 】\n";
                }
            }

            //驗證碼資料驗證
            if (value.Code == "")
            {
                AuthMemberErrorStr += "【 驗證碼不可為空 】\n";
            }
            else
            {
                if (!InTool.IsNumber(value.Code))
                {
                    AuthMemberErrorStr += "【 🚫驗證碼只能為數字 】\n";
                }
            }

            if (value.Code != dic["code"])
            {
                AuthMemberErrorStr += "【 驗證碼錯誤 】\n";
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
                            return "驗證成功"
                                ;
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

        //編輯資料
        [HttpPut("PutMember")]
        public string PutAcc([FromQuery] int id, [FromBody] MemberSelectDto value)
        {
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
        [HttpGet("GetForgetPwd")]
        public string GetMemberPwd([FromBody] MemberSelectDto value)
        {
            string getMemberPwdErrorStr = "";//記錄錯誤訊息

            //查詢資料庫狀態是否正常
            if (ModelState.IsValid == false)
            {
                return "參數異常";
            }

            //帳號資料驗證
            if (value.Account == "" || (string.IsNullOrEmpty(value.Account)))
            {
                getMemberPwdErrorStr += "【 帳號不可為空 】\n";
            }
            else
            {
                if (!InTool.IsENAndNumber(value.Account))
                {
                    getMemberPwdErrorStr += "【 🚫帳號只能為英數 】\n";
                }
                if (value.Account.Length > 20 || value.Account.Length < 3)
                {
                    getMemberPwdErrorStr += "【 🚫帳號長度應介於8～20個數字之間 】\n";
                }
            };

            //錯誤訊息不為空
            if (getMemberPwdErrorStr != "")
            {
                return getMemberPwdErrorStr;
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

                    dic.GetOrAdd("code", VerifyKey());

                    //開啟連線
                    cmd.Connection.Open();
                    getMemberPwdErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                    int SQLReturnCode = int.Parse(getMemberPwdErrorStr);

                    switch (SQLReturnCode)
                    {
                        case (int)GetForgetMemberErrorCode.GetMemberOK:
                            return "帳號正確  " + "驗證碼：" + dic["code"];

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
    }
}
