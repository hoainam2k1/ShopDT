using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Shop_DT.HomeView;
using Shop_DT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Shop_DT.Controllers
{
    public class ProductController : Controller
    {
        private readonly db_Shop_DTContext _context;
        public ProductController(db_Shop_DTContext context)
        {
            _context = context;
        }
        [Route("product.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 10;
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreateDate);
                PagedList<Product> models = new PagedList<Product>(lsProduct, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewData["DanhMuc"] = new SelectList(_context.Categories, "Alias", "CatName");
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        
        [Route("/product.html/{keyword}", Name = "SearchProduct")]
        public IActionResult Search(string keyword)
        {
            try
            {
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .Where(x=>x.ProductName.Contains(keyword))
                    .OrderByDescending(x => x.CreateDate);
                
                ViewData["DanhMuc"] = new SelectList(_context.Categories, "Alias", "CatName");
                return View(lsProduct);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("/filter/{Adias}", Name = "ListProduct")]
        public IActionResult list(string adias, int page = 1)
        {
            try
            {
                var pageSize = 10;
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == adias);
                var lsproduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == danhmuc.CatId)
                    .OrderByDescending(x => x.CreateDate);
                PagedList<Product> models = new PagedList<Product>(lsproduct, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhmuc;
                ViewData["DanhMuc"] = new SelectList(_context.Categories, "Alias", "CatName");
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }


        }
        [Route("/{Adias}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {

                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == product.CatId && x.ProductId != id && x.Active == true)
                    .OrderByDescending(x => x.CreateDate)
                    .Take(4)
                    .ToList();
                var lsComment = _context.Comments
                    .AsNoTracking()
                    .Include(x=> x.Customer)
                    .Include(x=>x.Product)
                    .Where(x => x.ProductId == id)
                    .ToList();
                ViewBag.SanPham = lsProduct;
                ViewBag.Comment = lsComment;
                ViewData["DanhMuc"] = new SelectList(_context.Categories, "Alias", "CatName");
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        [Route("api/product/comment")]
        public IActionResult Comment(int productID, string content)
        {

            var taikhoanID = HttpContext.Session.GetString("CustomerId");



            if (taikhoanID == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                try
                {
                    if (ModelState.IsValid)
                    {
                        Comment binhluan = new Comment();
                        binhluan.CustomerId = khachhang.CustomerId;
                        binhluan.ProductId = productID;
                        binhluan.CommentContent = content;
                        binhluan.CommentDate = DateTime.Now;
                        _context.Add(binhluan);
                        _context.SaveChanges();

                        return Json(new { success = true });
                    }

                }
                catch (Exception ex)
                {

                    return Json(new { success = false });
                }


            }


            return Json(new { success = false });



        }
    }
}
