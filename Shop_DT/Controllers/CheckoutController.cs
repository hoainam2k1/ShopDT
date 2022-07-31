using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shop_DT.Extension;
using Shop_DT.Helpper;
using Shop_DT.HomeView;
using Shop_DT.Models;
using Shop_DT.Service;
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
        public SendMailService _sendMailService { get; }
        public CheckoutController(db_Shop_DTContext context, INotyfService notifyService, SendMailService sendMailService)
        {
            _context = context;
            _notyfService = notifyService;
            _sendMailService = sendMailService;
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

            ViewData["Payment"] = new SelectList(_context.Payments, "PaymentId", "PaymentName", model.PaymentID);
            ViewBag.GioHang = cart;

            return View(model);
        }
        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            if (muaHang.PaymentID == 4)
            {
                if (ModelState.IsValid)
                {
                    return RedirectToAction("AddOrder", muaHang);
                }
                var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");

                ViewBag.GioHang = cart;
                return View(muaHang);
            }
            else
            {
                List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
                //request params need to request to MoMo system
                string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
                string partnerCode = "MOMOOJOI20210710";
                string accessKey = "iPXneGmrJH0G8FOP";
                string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
                string orderInfo = "Thanh toán bằng ví điện tử MoMo";
                string returnUrl = "https://localhost:44348/Checkout/AddOrder";
                string notifyurl = "http://ba1adf48beba.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

                string amount = cart.Sum(x => x.TotalMoney).ToString();
                string orderid = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyurl + "&extraData=" +
                    extraData;

                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);

                //build body json request
                JObject message = new JObject
                {
                    { "partnerCode", partnerCode },
                    { "accessKey", accessKey },
                    { "requestId", requestId },
                    { "amount", amount },
                    { "orderId", orderid },
                    { "orderInfo", orderInfo },
                    { "returnUrl", returnUrl },
                    { "notifyUrl", notifyurl },
                    { "extraData", extraData },
                    { "requestType", "captureMoMoWallet" },
                    { "signature", signature }
                };

                string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
                try
                {
                    JObject jmessage = JObject.Parse(responseFromMomo);


                    return Redirect(jmessage.GetValue("payUrl").ToString());
                }
                catch
                {
                    return RedirectToAction("Checkout", "Order");
                }
            }

        }
        public IActionResult AddOrder(MuaHangVM muaHang)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();


            var mailcontent = new MailContent();

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
            mailcontent.To = khachhang.Email;
            mailcontent.Subject = "Chi tiết đơn hàng";
            
                Order donhang = new Order();
                donhang.CustomerId = model.CustomerId;
                donhang.OrderDate = DateTime.Now;
                donhang.TransactStatusId = 1;
                donhang.PaymentId = muaHang.PaymentID;
                donhang.PaymentDate = DateTime.Now;
                donhang.Deleted = false;
                donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
                _context.Add(donhang);
                _context.SaveChanges();
                mailcontent.Body = "Đơn hàng của bạn đã được đặt thành công!<br />Tổng đơn hàng: " + donhang.TotalMoney.Value.ToString("#,##0 VNĐ") + " bao gồm:";

                foreach (var item in cart)
                {

                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = donhang.OrderId;
                    orderDetail.ProductId = item.product.ProductId;
                    orderDetail.Quantity = item.amount;
                    orderDetail.Total = donhang.TotalMoney;
                    int sum = Convert.ToInt32(item.amount) * Convert.ToInt32(item.product.Price);
                    mailcontent.Body += "<br />Sản phẩm: " + item.product.ProductName + " số lượng: " + item.amount + " Thành tiền: " + sum.ToString("#,##0 VNĐ");
                    _context.Add(orderDetail);
                    _context.SaveChanges();
                }


                var send = _sendMailService.SendMail(mailcontent);
                HttpContext.Session.Remove("GioHang");
                _notyfService.Success("Đơn hàng đặt thành công");
                return RedirectToAction("Success", "Checkout");

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
