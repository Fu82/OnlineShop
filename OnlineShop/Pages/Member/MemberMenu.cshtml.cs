using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;

namespace OnlineShop.Pages.Member
{
    public class MemberMenuModel : PageModel
    {
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        public string Session01;
        public string accName;
        public string accAddress;
        public string accGold;
        public void OnGet()
        {
            Session01 = HttpContext.Session.GetString("SessionID");

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // ÙYÁÏŽìßB¾€&SQLÖ¸Áî
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"EXEC pro_onlineShop_getMemberList @Id";
            cmd.Parameters.AddWithValue("@Id", int.Parse(Session01));

            //é_†¢ßB¾€
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            accName = dt.Rows[0]["f_name"].ToString();
            accAddress = dt.Rows[0]["f_address"].ToString();
            accGold = dt.Rows[0]["f_shopGold"].ToString();

            //êPé]ßB¾€
            cmd.Connection.Close();
            //HttpContext.Session.Remove("SessionID");
            //Session01 = HttpContext.Session.GetString("SessionID");


        }
    }
}
