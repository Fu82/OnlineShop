using System;

namespace OnlineShop.Controllers
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Num { get; set; }
        public int Category { get; set; }
        public int SubCategory { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public int Price { get; set; }
        public int Status { get; set; }
        public string Content { get; set; }
        public int Stock { get; set; }
        public int Popularity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
