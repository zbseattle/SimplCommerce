using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HvCommerce.Core.Domain.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using HvCommerce.Orders.ApplicationServices;
using HvCommerce.Orders.Domain.Models;
using HvCommerce.Web.Areas.Client.ViewModels;
using Microsoft.AspNet.Authorization;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace HvCommerce.Web.Areas.Client.Controllers
{
    public class ShoppingCartController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(UserManager<User> userManager,
            IShoppingCartService shoppingCartService)
        {
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
        }

        //
        // GET: /ShoppingCart/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ListAjax([DataSourceRequest] DataSourceRequest request)
        {
            var user = await GetCurrentUserAsync();
            var shoppingCartItems = _shoppingCartService.FindByUserId(user.Id);
            var gridData = shoppingCartItems.ToDataSourceResult(
                request,
                x => new ShoppingCartListItem()
                {
                    Id = x.Id,
                    ProductName = x.Product.Name,
                    CreatedOn = x.CreatedOn,
                    Quantity = x.Quantity,
                    Price = x.Product.Price
                });
            return Json(gridData);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(long productId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var shoppingCart = new ShoppingCartItem()
            {
                ProductId = productId,
                CreatedById = user.Id,
                ProductVariationId = 0,
                Quantity = 1,
                ProductVariation = new ProductVariation()
                {
                    Price = 100,
                    OldPrice = 50,
                    IsAllowOrder = true,
                    DisplayOrder = 1,
                    IsPublished = true,
                    IsDeleted = false
                }
            };

            _shoppingCartService.Add(shoppingCart);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            var shoppingCart = _shoppingCartService.Get(id);
            if (shoppingCart == null)
            {
                return new HttpStatusCodeResult(400);
            }

            _shoppingCartService.Remove(shoppingCart);
            return Json(true);
        }

        #region Helpers

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }

        #endregion
    }
}
