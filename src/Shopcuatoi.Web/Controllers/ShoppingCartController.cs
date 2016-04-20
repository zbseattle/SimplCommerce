using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using HvCommerce.Web.ViewModels.Manage;
using Microsoft.AspNet.Http;
using Shopcuatoi.Core.Domain.Models;
using Shopcuatoi.Infrastructure.Domain.IRepositories;
using Shopcuatoi.Orders.ApplicationServices;
using Shopcuatoi.Orders.Domain.Models;
using Shopcuatoi.Web.ViewModels;

namespace Shopcuatoi.Web.Controllers
{
    public class ShoppingCartController : BaseController
    {
        private readonly IRepository<ShoppingCart> shoppingCartRepository;
        private readonly IRepository<ShoppingCartItem> shoppingCartItemRepository;
        private readonly IShoppingCartService shoppingCartService;

        public ShoppingCartController(UserManager<User> userManager,
            IRepository<ShoppingCart> shoppingCartRepository,
            IShoppingCartService shoppingCartService,
            IRepository<ShoppingCartItem> shoppingCartItemRepository) : base(userManager)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.shoppingCartItemRepository = shoppingCartItemRepository;
            this.shoppingCartService = shoppingCartService;
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
                    .Where(x => x.CreatedById == currentUser.Id)
                    .Include(x => x.ShoppingCartItems)
                    .SelectMany(x => x.ShoppingCartItems).Distinct().ToListAsync();
            }
            else
            {
                var guestKey = GetGuestKey();
                shoppingCarts = await shoppingCartRepository.Query()
                    .Where(x => x.GuestKey.HasValue && x.GuestKey.Value == guestKey)
                    .Include(x => x.ShoppingCartItems)
                    .SelectMany(x => x.ShoppingCartItems).Distinct().ToListAsync();
            }

            var shoppingCartListItems = shoppingCarts.Select(x =>
            new ShoppingCartListItemViewModel()
            {
                Id = x.Id,
                Price = x.Product.Price,
                Quantity = x.Quantity,
                CreatedOn = x.CreatedOn.ToString(CultureInfo.InvariantCulture),
                ProductName = x.Product.Name
            });

            return Json(shoppingCartListItems);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(long productId)
        {
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                shoppingCartService.AddToCartByUser(productId, user.Id);
            }
            else
            {
                var guestKey = GetGuestKey();
                shoppingCartService.AddToCartByGuestKey(productId, guestKey);
                Response.Cookies.Append(nameof(ShoppingCart.GuestKey), guestKey.ToString(), new CookieOptions()
                {
                    Expires = DateTime.MaxValue
                });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove([FromBody] long itemId)
        {
            var shoppingCartItem = shoppingCartItemRepository.Get(itemId);
            if (shoppingCartItem == null)
            {
                return new HttpStatusCodeResult(400);
            }

            shoppingCartItemRepository.Remove(shoppingCartItem);
            shoppingCartItemRepository.SaveChange();
            return Json(true);
        }

        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] ShoppingCartItemViewModel shoppingCartItem)
        {
            var entityShoppingCartItem = shoppingCartItemRepository.Get(shoppingCartItem.ItemId);
            if (entityShoppingCartItem == null)
            {
                return new HttpStatusCodeResult(400);
            }
            entityShoppingCartItem.Quantity = shoppingCartItem.Quantity;
            shoppingCartItemRepository.Update(entityShoppingCartItem);
            shoppingCartItemRepository.SaveChange();
            return Json(true);
        }
    }
}
