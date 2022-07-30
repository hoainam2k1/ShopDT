using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_DT.HomeView;
using Shop_DT.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_DT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly db_Shop_DTContext _context;
        public HomeController(ILogger<HomeController> logger,db_Shop_DTContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewVM model = new HomeViewVM();
            var lsProducts = _context.Products.AsNoTracking()
                .Where(x => x.Active == true)
                .OrderByDescending(x => x.CreateDate)
                .ToList();

            List<ProductHomeVM> lsProductView = new List<ProductHomeVM>();
            var lsCats = _context.Categories
                .AsNoTracking()
                .Where(x => x.Published == true)
                .ToList();
            foreach (var item in lsCats)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.category = item;
                productHome.lsProducts = lsProducts.Where(x => x.CatId == item.CatId).ToList();
                lsProductView.Add(productHome);
            }
            model.Products = lsProductView;
            ViewBag.AllProducts = lsProducts;
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Alias", "CatName");
            return View(model);
        }
        [Route("contact.html", Name = "Contact")]
        public IActionResult Contact()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Alias", "CatName");
            return View();
        }
        [Route("about.html", Name = "About")]
        public IActionResult About()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Alias", "CatName");
            return View();
        }
        [Route("privacy.html", Name = "Shopping")]
        public IActionResult Privacy()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Alias", "CatName");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
