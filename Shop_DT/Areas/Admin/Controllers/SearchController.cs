using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_DT.Models;
using System.Collections.Generic;
using System.Linq;
namespace Shop_DT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly db_Shop_DTContext _context;
        public SearchController(db_Shop_DTContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword))
                .OrderByDescending(x => x.ProductName)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("ListProductsSearchPartial", ls);
            }
        }
        [HttpPost]
        public IActionResult FindCustomer(string keyword)
        {
            List<Customer> ls = new List<Customer>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListCustomerSearchPartial", null);
            }
            ls = _context.Customers
                .AsNoTracking()
                .Where(x => x.FullName.Contains(keyword))
                .OrderByDescending(x => x.FullName)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListCustomerSearchPartial", null);
            }
            else
            {
                return PartialView("ListCustomerSearchPartial", ls);
            }
        }
        [HttpPost]
        public IActionResult FindAccount(string keyword)
        {
            List<Account> ls = new List<Account>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListAccountSearchPartial", null);
            }
            ls = _context.Accounts
                .AsNoTracking()
                .Include(a => a.Role)
                .Where(x => x.FullName.Contains(keyword))
                .OrderByDescending(x => x.FullName)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListAccountSearchPartial", null);
            }
            else
            {
                return PartialView("ListAccountSearchPartial", ls);
            }
        }
        [HttpPost]
        public IActionResult FindCategory(string keyword)
        {
            List<Category> ls = new List<Category>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListCategorySearchPartial", null);
            }
            ls = _context.Categories
                .AsNoTracking()
                .Where(x => x.CatName.Contains(keyword))
                .OrderByDescending(x => x.CatName)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListCategorySearchPartial", null);
            }
            else
            {
                return PartialView("ListCategorySearchPartial", ls);
            }
        }
        [HttpPost]
        public IActionResult FindOrder(string keyword)
        {
            List<Order> ls = new List<Order>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListOrderSearchPartial", null);
            }
            ls = _context.Orders
                .AsNoTracking()
                .Where(x => x.Customer.FullName.Contains(keyword))
                .OrderByDescending(x => x.OrderDetails)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListOrderSearchPartial", null);
            }
            else
            {
                return PartialView("ListOrderSearchPartial", ls);
            }
        }
    }
}