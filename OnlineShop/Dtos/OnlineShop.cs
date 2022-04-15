using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Dtos
{
    public class OnlineShopSelectDto
    {
        public int Id { get; set; }
        public string Acc { get; set; }
        public string Pwd { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int ShopGold { get; set; }
        public byte Level { get; set; }
        public byte Suspension { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
