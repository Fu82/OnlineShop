using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShop.Tool;
using System;

namespace OnlineShop.Pages
{
    public class indexModel : PageModel
    {
        public void OnGet()
        {


            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||  //判斷Session[Account]是否為空
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//判斷DB SessionId與瀏覽器 SessionId是否一樣
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)//判斷是否過期
            {
                Response.Redirect("/Login");
            }
        }
    }
}
