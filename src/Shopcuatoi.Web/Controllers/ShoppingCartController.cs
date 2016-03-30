using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using HvCommerce.Web.ViewModels.Manage;
using Microsoft.AspNet.Http;
using Shopcuatoi.Core.Domain.Models;
using Shopcuatoi.Infrastructure.Domain.IRepositories;
using Shopcuatoi.Orders.Domain.Models;

namespace Shopcuatoi.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<ShoppingCartItem> shoppingCartRepository;

        public ShoppingCartController(UserManager<User> userManager,
            IRepository<ShoppingCartItem> shoppingCartRepository)
        {
            this.userManager = userManager;
            this.shoppingCartRepository = shoppingCartRepository;
        }

        //
        // GET: /ShoppingCart/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var currentUser = await GetCurrentUserAsync();
            List<ShoppingCartItem> shoppingCarts;
            if (currentUser != null)
            {
                shoppingCarts = await shoppingCartRepository.Query()
                    .Where(x => x.CreatedById == currentUser.Id).ToListAsync();
            }
            else
            {
                var guestKey = GetGuestKey();
                shoppingCarts = await shoppingCartRepository.Query()
                    .Where(x => x.GuestKey.HasValue && x.GuestKey.Value == guestKey).ToListAsync();
            }

            var shoppingCartListItems = shoppingCarts.Select(x =>
            new ShoppingCartListItemViewModel()
            {
                Id = x.Id,
                Price = x.Product.Price,
                Quantity = x.Quantity,
                CreatedOn = x.CreatedOn.ToString(),
                ProductName = x.Product.Name
            });

            return Json(shoppingCartListItems);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(long productId)
        {
            var guestKey = GetGuestKey();

            var shoppingCart = new ShoppingCartItem()
            {
                ProductId = productId,
                Quantity = 1,
                CreatedOn = DateTime.UtcNow
            };

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                shoppingCart.CreatedById = user.Id;
            }
            else
            {
                shoppingCart.GuestKey = guestKey;
                Response.Cookies.Append(nameof(ShoppingCartItem.GuestKey), guestKey.ToString(), new CookieOptions()
                {
                    Expires = DateTime.MaxValue
                });
            }

            shoppingCartRepository.Add(shoppingCart);
            shoppingCartRepository.SaveChange();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove([FromBody] long id)
        {
            var shoppingCart = shoppingCartRepository.Get(id);
            if (shoppingCart == null)
            {
                return new HttpStatusCodeResult(400);
            }

            shoppingCartRepository.Remove(shoppingCart);
            shoppingCartRepository.SaveChange();
            return Json(true);
        }

        #region Helpers

        private async Task<User> GetCurrentUserAsync()
        {
            return await userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }

        private Guid GetGuestKey()
        {
            if (!Request.Cookies.ContainsKey(nameof(ShoppingCartItem.GuestKey)))
            {
                return Guid.NewGuid();
            }
            return Guid.Parse(Request.Cookies[nameof(ShoppingCartItem.GuestKey)].ToString());
        }

        #endregion
    }
}
