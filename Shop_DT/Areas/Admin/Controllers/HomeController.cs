using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Shop_DT.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class HomeController : Controller
    {
       
        [Route("/Admin/Index")]
        public IActionResult Index()
        {
            
            return View();
        }
        //[Route("/admin")]
        //public IActionResult Index()
        //{
        //    var taikhoanID = HttpContext.Session.GetString("AccountId");
        //    if (taikhoanID != null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }


        //    return RedirectToAction("Login");
        //}
        
    }
}
