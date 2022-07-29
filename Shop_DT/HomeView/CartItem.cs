using Shop_DT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Shop_DT.HomeView
{
    public class CartItem
    {
        
        public Product product { get; set; }
        public int amount { get; set; }
        //public double TotalMoney { get; set; }
        public double TotalMoney => amount * product.Price.Value;
    }
}
