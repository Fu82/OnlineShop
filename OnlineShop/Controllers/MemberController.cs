using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using OnlineShop.DTOs;
using OnlineShop.Models;
using OnlineShop.Tool;
using System;
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
            duplicateAccount = 101
        }
        private enum PutACCountErrorCode //更新帳號
        {
            //<summary >
            //帳號刪除成功
            //</summary >
            PutOK = 0,
            //<summary >
            //此帳號不可更改
            //</summary >
            DontPut = 100,
            //<summary >
            //尚未建立權限
            //</summary >
            LvIsNull = 101

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
                addMemberErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
                int SQLReturnCode = int.Parse(addMemberErrorStr);

                switch (SQLReturnCode)
                {
                    case (int)addACCountErrorCode.duplicateAccount:
                        return "此帳號已存在";

                    case (int)addACCountErrorCode.AddOK:
                        return "帳號新增成功";
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

        ////更新帳號
        //[HttpPut("PutAcc")]
        //public string PutAcc([FromQuery] int id, [FromBody] MemberSelectDto value)
        //{
        //    string addMemberErrorStr = "";//記錄錯誤訊息

        //    //查詢資料庫狀態是否正常
        //    if (ModelState.IsValid == false)
        //    {
        //        return "參數異常";
        //    }

        //    if (!string.IsNullOrEmpty(addMemberErrorStr))
        //    {
        //        return addMemberErrorStr;
        //    }


        //    SqlCommand cmd = null;
        //    //DataTable dt = new DataTable();
        //    try
        //    {
        //        // 資料庫連線
        //        cmd = new SqlCommand();
        //        cmd.Connection = new SqlConnection(SQLConnectionString);

        //        cmd.CommandText = @"EXEC pro_onlineShopBack_putAccount @Id, @Level";

        //        cmd.Parameters.AddWithValue("@Id", id);
        //        cmd.Parameters.AddWithValue("@Level", value.Level);

        //        //開啟連線
        //        cmd.Connection.Open();
        //        addMemberErrorStr = cmd.ExecuteScalar().ToString();//執行Transact-SQL
        //        int SQLReturnCode = int.Parse(addMemberErrorStr);

        //        switch (SQLReturnCode)
        //        {
        //            case (int)PutACCountErrorCode.DontPut:
        //                return "此帳號不可做更改";
        //            case (int)PutACCountErrorCode.PutOK:
        //                return "帳號更新成功";
        //            default:
        //                return "失敗";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Parameters.Clear();
        //            cmd.Connection.Close();
        //        }
        //    }
        //}

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
    }
}
