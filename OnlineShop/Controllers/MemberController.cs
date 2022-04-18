using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using OnlineShop.DTOs;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(value.Pwd));//MD5 加密傳密碼進去
                var strResult = BitConverter.ToString(result);

                TMember insert = new TMember
                {
                    FAcc = value.Account,
                    FPwd = strResult.Replace("-", ""),
                    FPhone = value.Phone,
                    FMail = value.Mail,
                };
                _OnlineShopContext.Add(insert);
                _OnlineShopContext.SaveChanges();
                return "新增成功";
            }
        }

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
