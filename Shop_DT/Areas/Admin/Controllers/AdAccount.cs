using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_DT.Extension;
using Shop_DT.Helpper;
using Shop_DT.HomeView;
using Shop_DT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop_DT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdAccount : Controller
    {
        private readonly db_Shop_DTContext _context;
        public INotyfService _notyfService { get; }
        public AdAccount(db_Shop_DTContext context, INotyfService notifyService)
        {
            _context = context;
            _notyfService = notifyService;
        }
        public IActionResult Profile()
        {
            return View();
        }
        [Route("/Admin/")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var admin = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (admin != null)
                    return Json(data: "Số điện thoại : " + Phone + " Đã được sử dụng ");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var admin = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (admin != null)
                    return Json(data: "Email : " + Email + " Đã được sử dụng ");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/Admin/")]
        public async Task<IActionResult> Login(AdminVM qtv)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(qtv.UserName);
                    if (!isEmail) return View(qtv);
                    var admin = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == qtv.UserName);
                    if (admin == null) return RedirectToAction("DangKyTaiKhoan");
                    //string pass = (qtv.Password + admin.Salt.Trim()).ToMD5();
                    string pass = qtv.Password;
                    if (admin.Password != pass)
                    {
                        _notyfService.Success("Sai thông tin đăng nhập");
                        return View(qtv);
                    }
                    if (admin.Active == false) return RedirectToAction("ThongBao", "Account");

                    HttpContext.Session.SetString("AccountId", admin.AccountId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("AccountId");
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, admin.FullName),
                        new Claim("AccountId", admin.AccountId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);

                    admin.LastLogin = DateTime.Now;
                    _context.Update(admin);
                    _context.SaveChanges();
                    _notyfService.Success("Đăng nhập thành công!");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Login", "AdAccount");
            }
            return View(qtv);
        }
        [HttpGet]
        [Route("/admin/dang-xuat.html", Name = "LogoutAd")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AccountId");
            return RedirectToAction("Index", "Home");
        }
    }
}
