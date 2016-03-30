using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shopcuatoi.Infrastructure.Domain.IRepositories;
using Shopcuatoi.Orders.Domain.Models;

namespace Shopcuatoi.Orders.ApplicationServices
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCartItem> shoppingCartRepository;

        public ShoppingCartService(IRepository<ShoppingCartItem> shoppingCartRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
        }

        public void ChangeGuestKeyToUser(Guid guestKey, long userId)
        {
            var shoppingCartItems = shoppingCartRepository.Query()
                .Where(x => x.GuestKey.HasValue && x.GuestKey.Value == guestKey);

            foreach (var shoppingCartItem in shoppingCartItems)
            {
                shoppingCartItem.GuestKey = null;
                shoppingCartItem.CreatedById = userId;
            }

            shoppingCartRepository.SaveChange();

        }
    }
}
