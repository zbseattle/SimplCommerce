using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Shopcuatoi.Core.ApplicationServices;
using Shopcuatoi.Core.Domain.Models;
using Shopcuatoi.Infrastructure.Domain.IRepositories;
using Shopcuatoi.Web.ViewModels;
using Shopcuatoi.Web.ViewModels.Catalog;
using Microsoft.AspNet.Mvc;
using Shopcuatoi.Orders.ApplicationServices;
using Shopcuatoi.Orders.Domain.Models;

namespace Shopcuatoi.Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IMediaService mediaService;
        private readonly IRepository<Product> productRepository;
        private readonly IShoppingCartService shoppingCartService;

        public ProductController(IRepository<Product> productRepository, 
            IMediaService mediaService, 
            IRepository<Category> categoryRepository,
            IShoppingCartService shoppingCartService,
            UserManager<User> userManager
            ) : base(userManager)
        {
            this.productRepository = productRepository;
            this.mediaService = mediaService;
            this.categoryRepository = categoryRepository;
            this.shoppingCartService = shoppingCartService;
        }

        public IActionResult ProductsByCategory(string catSeoTitle)
        {
            var category = categoryRepository.Query().FirstOrDefault(x => x.SeoTitle == catSeoTitle);
            if (category == null)
            {
                return Redirect("~/Error/FindNotFound");
            }

            var model = new ProductsByCategory
            {
                CategoryName = category.Name
            };

            var products = productRepository.Query()
                .Where(x => x.Categories.Any(c => c.CategoryId == category.Id) && x.IsPublished)
                .Select(x => new ProductListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    SeoTitle = x.SeoTitle,
                    Price = x.Price,
                    OldPrice = x.OldPrice,
                    ThumbnailImage = x.ThumbnailImage
                }).ToList();

            foreach (var product in products)
            {
                product.ThumbnailUrl = mediaService.GetThumbnailUrl(product.ThumbnailImage);
            }

            model.Products = products;

            return View(model);
        }

        public IActionResult ProductDetail(string seoTitle)
        {
            var product = productRepository.Query()
                .Include(x => x.Medias)
                .Include(x => x.Variations)
                .FirstOrDefault(x => x.SeoTitle == seoTitle && x.IsPublished);
            if (product == null)
            {
                return Redirect("~/Error/FindNotFound");
            }

            var model = new ProductDetail
            {
                Id = product.Id,
                Name = product.Name,
                OldPrice = product.OldPrice,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Specification = product.Specification
            };

            MapProductVariantToProductVm(product, model);

            foreach (var mediaViewModel in product.Medias.Select(productMedia => new MediaViewModel
            {
                Url = mediaService.GetMediaUrl(productMedia.Media),
                ThumbnailUrl = mediaService.GetThumbnailUrl(productMedia.Media)
            }))
            {
                model.Images.Add(mediaViewModel);
            }

            return View(model);
        }

        public async Task<IActionResult> LoadProductModal(long productId)
        {
            var product = productRepository.Query()
                .Include(x => x.Medias)
                .Include(x => x.Variations)
                .FirstOrDefault(x => x.Id == productId && x.IsPublished);
            if (product == null)
            {
                return Redirect("~/Error/FindNotFound");
            }

            var guestKey = GetGuestKey();
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                shoppingCartService.AddToCartByUser(productId, user.Id);
            }
            else
            {
                shoppingCartService.AddToCartByGuestKey(productId, guestKey);
                Response.Cookies.Append(nameof(ShoppingCart.GuestKey), guestKey.ToString(), new CookieOptions()
                {
                    Expires = DateTime.MaxValue
                });
            }

            return PartialView("Partials/_ProductModalPartial", product);
        }

        private static void MapProductVariantToProductVm(Product product, ProductDetail model)
        {
            foreach (var variation in product.Variations)
            {
                var variationVm = new ProductDetailVariation
                {
                    Id = variation.Id,
                    PriceOffset = variation.PriceOffset
                };

                foreach (var combination in variation.AttributeCombinations)
                {
                    variationVm.Attributes.Add(new ProductDetailVariationAttribute
                    {
                        AttributeId = combination.AttributeId,
                        AttributeName = combination.Attribute.Name,
                        Value = combination.Value
                    });
                }

                model.Variations.Add(variationVm);
            }
        }
    }
}