using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyNet5ApiAdoTest.Services;
using System.Data;
using static OnlineShop.Pages.Member.BasePage;

namespace OnlineShop.Pages.Member
{
    public class MemberMenuModel : BasePageModel
    {
        private string SQLConnectionString = AppConfigurationService.Configuration.GetConnectionString("OnlineShopDatabase");

        public string MemberID;
        public string accName;
        public string accAddress;
        public string accGold;
        public void OnGet()
        {
            if (!LoginValidate())
            {
                Response.Redirect("/Login");
                return;
            }

            MemberID = HttpContext.Session.GetString("MemberID");

            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            // ÙYÁÏŽìßB¾€&SQLÖ¸Áî
            cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(SQLConnectionString);
            cmd.CommandText = @"EXEC pro_onlineShop_getMemberList @Id";
            cmd.Parameters.AddWithValue("@Id", int.Parse(MemberID));

            //é_†¢ßB¾€
            cmd.Connection.Open();
            da.SelectCommand = cmd;
            da.Fill(dt);

            accName = dt.Rows[0]["f_name"].ToString();
            accAddress = dt.Rows[0]["f_address"].ToString();
            accGold = dt.Rows[0]["f_shopGold"].ToString();

            //êPé]ßB¾€
            cmd.Connection.Close();
        }
    }
}
