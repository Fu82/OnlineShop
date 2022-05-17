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
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Account")) ||  //�Д�Session[Account]�Ƿ���
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].SId != HttpContext.Session.Id ||//�Д�DB SessionId�c�g�[�� SessionId�Ƿ�һ��
                SessionDB.sessionDB[HttpContext.Session.GetString("Account")].ValidTime < DateTime.Now)//�Д��Ƿ��^��
            {
                Response.Redirect("/Index");
            }
        }
    }
}
