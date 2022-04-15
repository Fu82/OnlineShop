using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineShopController : ControllerBase
    {
        private readonly OnlineShopContext _onlineShopContext;

        public OnlineShopController(OnlineShopContext onlineShopContext)
        {
            _onlineShopContext = onlineShopContext;
        }

        // GET: api/<OnlineShopController>
        [HttpGet]
        public IEnumerable<OnlineShopSelectDto> Get()
        {
            var result = _onlineShopContext.TMember.Select(a => new OnlineShopSelectDto
            {
                Id = a.FId,
                Acc = a.FAcc,
                Pwd = a.FPwd,
                Phone = a.FPhone,
                Mail = a.FMail,
                Name = a.FName,
                Address = a.FAddress,
                ShopGold = a.FShopGold,
                Level = a.FLevel,
                Suspension = a.FSuspension,
                CreateDate = a.FCreateDate,
                UpdateDate = a.FUpdateDate
            });

            return result;
        }

        // GET api/<OnlineShopController>/5
        [HttpGet("{id}")]
        public OnlineShopSelectDto Get(int id)
        {
            var result = (from a in _onlineShopContext.TMember
                         where a.FId == id
                         select new OnlineShopSelectDto
                         {
                             Id = a.FId,
                             Acc = a.FAcc,
                             Pwd = a.FPwd,
                             Phone = a.FPhone,
                             Mail = a.FMail,
                             Name = a.FName,
                             Address = a.FAddress,
                             ShopGold = a.FShopGold,
                             Level = a.FLevel,
                             Suspension = a.FSuspension,
                             CreateDate = a.FCreateDate,
                             UpdateDate = a.FUpdateDate

                         }).SingleOrDefault();

            return result;
        }

        // POST api/<OnlineShopController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<OnlineShopController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OnlineShopController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
