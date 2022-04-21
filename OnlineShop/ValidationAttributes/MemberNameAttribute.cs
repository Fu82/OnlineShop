using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.ValidationAttributes
{
    public class MemberNameAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            OnlineShopContext _onlineShopContext = (OnlineShopContext)validationContext.GetService(typeof(OnlineShopContext));

            var Account = (string)value;

            var findAccount = from a in _onlineShopContext.TMember
                              where a.FAcc == Account
                              select a;

            if(findAccount.FirstOrDefault() != null)
            {
                return new ValidationResult("已存在相同的代辦事務");
            }

            return ValidationResult.Success;
        }
    }
}
