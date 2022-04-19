using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.DTOs
{
    public class MemberSelectDto
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Pwd { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int ShopGold { get; set; }
        public byte Level { get; set; }
        public byte Suspension { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
