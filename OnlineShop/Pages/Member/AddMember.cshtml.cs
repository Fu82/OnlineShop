using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;

namespace OnlineShop.Pages
{
    public class AddMemberModel : PageModel
    {
        private static string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");
        public void OnGet()
        {
        }
    }
}
