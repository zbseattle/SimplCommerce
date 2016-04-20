using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Shopcuatoi.Core.Domain.Models;
using Microsoft.AspNet.Mvc;
using Shopcuatoi.Orders.Domain.Models;
using System.Security.Claims;

namespace Shopcuatoi.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly UserManager<User> userManager;

        protected BaseController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [NonAction]
        public async Task<User> GetCurrentUserAsync()
        {
            return await userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }

        [NonAction]
        public Guid GetGuestKey()
        {
            if (!Request.Cookies.ContainsKey(nameof(ShoppingCart.GuestKey)))
            {
                return Guid.NewGuid();
            }
            return Guid.Parse(Request.Cookies[nameof(ShoppingCart.GuestKey)].ToString());
        }
    }
}
