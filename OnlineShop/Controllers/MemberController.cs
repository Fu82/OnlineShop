using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using OnlineShop.DTOs;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            SqlCommand cmd = null;
            //DataTable dt = new DataTable();

            try
            {
                // 資料庫連線
                cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(SQLConnectionString);

                cmd.CommandText = @"EXEC pro_onlineShop_addMember @f_acc, @f_pwd, @f_phone, @f_mail";
                cmd.Parameters.AddWithValue("@f_acc", value.Account);
                cmd.Parameters.AddWithValue("@f_pwd", Tool.InTool.PwdToMD5(value.Pwd));
                cmd.Parameters.AddWithValue("@f_phone", value.Phone);
                cmd.Parameters.AddWithValue("@f_mail", value.Mail);

                //SqlDataAdapter da = new SqlDataAdapter();

                //開啟連線
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
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

            return "新增成功";
        }

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
