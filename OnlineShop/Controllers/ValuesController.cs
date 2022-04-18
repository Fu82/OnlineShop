using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("index")]
        public string Index()
        {
            string strConn = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
            return strConn;
        }
    }
}
