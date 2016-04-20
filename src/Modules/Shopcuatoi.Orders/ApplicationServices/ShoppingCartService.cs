using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shopcuatoi.Infrastructure.Domain.IRepositories;
using Shopcuatoi.Orders.Domain.Models;
using System.Data.Entity;

namespace Shopcuatoi.Orders.ApplicationServices
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCartItem> shoppingCartItemRepository;
        private readonly IRepository<ShoppingCart> shoppingCartRepository;

        public ShoppingCartService(IRepository<ShoppingCartItem> shoppingCartItemRepository,
            IRepository<ShoppingCart> shoppingCartRepository)
        {
            this.shoppingCartItemRepository = shoppingCartItemRepository;
            this.shoppingCartRepository = shoppingCartRepository;
        }

        public void ChangeGuestKeyToUser(Guid guestKey, long userId)
        {
            var shoppingCart = shoppingCartRepository.Query()
                .FirstOrDefault(x => x.GuestKey.HasValue && x.GuestKey.Value == guestKey);

            if (shoppingCart != null)
            {
                shoppingCart.GuestKey = null;
                shoppingCart.CreatedById = userId;
                shoppingCartRepository.SaveChange();
            }
        }

        private void UpdateExistedShoppingCart(ShoppingCart shoppingCart, long productId)
        {
            shoppingCart.UpdatedOn = DateTime.Now;
            var existedShoppingCartItem = shoppingCart.ShoppingCartItems.FirstOrDefault(x => x.ProductId == productId);
            if (existedShoppingCartItem != null)
            {
                existedShoppingCartItem.Quantity += 1;
            }
            else
            {
                shoppingCart.ShoppingCartItems.Add(new ShoppingCartItem()
                {
                    ProductId = productId,
                    Quantity = 1,
                    ShoppingCartId = shoppingCart.Id,
                    CreatedOn = DateTime.Now
                });
            }
            shoppingCartRepository.Update(shoppingCart);
        }

        private void AddNewShoppingCart(ShoppingCart shoppingCart, long productId)
        {
            shoppingCart.CreatedOn = DateTime.Now;
            shoppingCart.PaymentStatus = false;
            if (shoppingCart.ShoppingCartItems == null)
            {
                shoppingCart.ShoppingCartItems = new List<ShoppingCartItem>();
            }
            shoppingCart.ShoppingCartItems.Add(new ShoppingCartItem()
            {
                ProductId = productId,
                Quantity = 1,
                CreatedOn = DateTime.Now
            });
            shoppingCartRepository.Add(shoppingCart);
        }

        public void AddToCartByUser(long productId, long userId)
        {
            var shoppingCart = shoppingCartRepository.Query()
                .Include(x => x.ShoppingCartItems)
                .FirstOrDefault(x => x.CreatedById == userId);
            if (shoppingCart != null)
            {
                UpdateExistedShoppingCart(shoppingCart, productId);
            }
            else
            {
                shoppingCart = new ShoppingCart()
                {
                    CreatedById = userId
                };
                AddNewShoppingCart(shoppingCart, productId);
            }

            shoppingCartRepository.SaveChange();
        }

        public void AddToCartByGuestKey(long productId, Guid guestKey)
        {
            var shoppingCart = shoppingCartRepository.Query()
                .Include(x => x.ShoppingCartItems)
                .FirstOrDefault(x => x.GuestKey == guestKey);
            if (shoppingCart != null)
            {
                UpdateExistedShoppingCart(shoppingCart, productId);
            }
            else
            {
                shoppingCart = new ShoppingCart()
                {
                    GuestKey = guestKey
                };
                AddNewShoppingCart(shoppingCart, productId);
            }

            shoppingCartRepository.SaveChange();
        }

    }
}
