using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop_DT.Extension;
using Shop_DT.HomeView;
using Shop_DT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ShopDT.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly db_Shop_DTContext _context;
        public INotyfService _notyfService { get; }
        public ShoppingCartController(db_Shop_DTContext context, INotyfService notifyService)
        {
            _context = context;
            _notyfService = notifyService;
        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> gioHang = GioHang;
            try
            {
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    if (amount.HasValue)
                    {
                        item.amount += amount.Value;
                    }
                    else
                    {
                        item.amount++;
                    }
                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(product => product.ProductId == productID);

                    // double totalPrice = hh.Price.Value * amount;
                    CartItem newItem = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh,

                        //TotalMoney = totalPrice
                    };

                    gioHang.Add(newItem);
                }

                _notyfService.Success("Thêm thành công");
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true, redirect = "./product.html" });
            }
            catch
            {
                return Json(new { success = false });
            }

        }
        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            List<CartItem> gioHang = GioHang;
            try
            {
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    if (amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    else
                    {
                        item.amount++;
                    }
                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(product => product.ProductId == productID);

                    // double totalPrice = hh.Price.Value * amount;
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh,

                        //TotalMoney = totalPrice
                    };

                    gioHang.Add(item);
                }

                _notyfService.Success("Thêm thành công");
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

        }
        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

        }
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            
            var lsGioHang = GioHang; 
            return View(GioHang);
        }
    }
}
