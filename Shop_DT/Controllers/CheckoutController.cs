using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_DT.Extension;
using Shop_DT.Helpper;
using Shop_DT.HomeView;
using Shop_DT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Shop_DT.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly db_Shop_DTContext _context;
        public INotyfService _notyfService { get; }
        public CheckoutController(db_Shop_DTContext context, INotyfService notifyService)
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

        [Route("checkout.html")]
        public IActionResult Index(string returnUrl = null)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            ViewData["lsTinhThanh"] = new SelectList(_context.Locations, "LocationId", "Name");
            ViewBag.GioHang = cart;
            var tpayment = _context.Payments.AsNoTracking().ToList();
            ViewBag.Payment = tpayment;
            return View(model);
        }
        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                khachhang.Address = muaHang.Address;
                //khachhang.LocationId = muaHang.TinhThanh;
                model.Address = khachhang.Address;
                //model.TinhThanh = khachhang.Location.LocationId;

                _context.Update(khachhang);
                _context.SaveChanges();

            }
            try
            {
                if (ModelState.IsValid)
                {
                    Order donhang = new Order();
                    donhang.CustomerId = model.CustomerId;
                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusId = 1;

                    donhang.Deleted = false;
                    donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
                    _context.Add(donhang);
                    _context.SaveChanges();


                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.ProductId = item.product.ProductId;
                        orderDetail.Quantity = item.amount;
                        orderDetail.Total = donhang.TotalMoney;
                        _context.Add(orderDetail);
                        _context.SaveChanges();
                    }

                    HttpContext.Session.Remove("GioHang");
                    _notyfService.Success("Đơn hàng đặt thành công");
                    return RedirectToAction("Success", "Checkout");
                }
              
            }
            catch (Exception ex)
            {
                ViewData["lsTinhThanh"] = new SelectList(_context.Locations, "LocationID", "Name");
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewBag.GioHang = cart;
            return View(model);
        }
        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders.Where(x => x.CustomerId == Convert.ToInt32(taikhoanID)).OrderByDescending(x => x.OrderId);
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachhang.FullName;
                successVM.Phone = khachhang.Phone;
                successVM.Address = khachhang.Address;
                return View(successVM);
            }
            catch
            {
                return View();
            }
        }
    }

}
