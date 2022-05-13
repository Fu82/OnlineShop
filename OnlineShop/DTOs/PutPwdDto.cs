using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Controllers
{
    public class PutPwdDto
    {
        public string Account { get; set; }
        public string newPwd { get; set; }
        public string cfmNewPwd { get; set; }

        public string Code { get; set; }
    }
}
