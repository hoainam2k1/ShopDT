using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop_DT.Extension;
using Shop_DT.HomeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Shop_DT.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}
