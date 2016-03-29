using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HvCommerce.Core.Domain.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using HvCommerce.Infrastructure.Domain.IRepositories;
using HvCommerce.Orders.ApplicationServices;
using HvCommerce.Orders.Domain.Models;
using HvCommerce.Web.ViewModels.Manage;

namespace HvCommerce.Web.Controllers
{
    public class ShoppingCartController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly IRepositoryWithTypedId<ShoppingCartItem, long> _shoppingCartRepository;

        public ShoppingCartController(UserManager<User> userManager,
            IRepositoryWithTypedId<ShoppingCartItem, long> shoppingCartRepository)
        {
            _userManager = userManager;
            _shoppingCartRepository = shoppingCartRepository;
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
            var shoppingCarts = await _shoppingCartRepository.Query()
                .Where(x => x.CreatedById == currentUser.Id).ToListAsync();
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
                CreatedOn = DateTime.UtcNow,
                ProductVariation = new ProductVariation()
                {
                    IsAllowOrder = true,
                    DisplayOrder = 1,
                    IsPublished = true,
                    IsDeleted = false
                }
            };

            _shoppingCartRepository.Add(shoppingCart);
            _shoppingCartRepository.SaveChange();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove([FromBody] long id)
        {
            var shoppingCart = _shoppingCartRepository.Get(id);
            if (shoppingCart == null)
            {
                return new HttpStatusCodeResult(400);
            }

            _shoppingCartRepository.Remove(shoppingCart);
            _shoppingCartRepository.SaveChange();
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
