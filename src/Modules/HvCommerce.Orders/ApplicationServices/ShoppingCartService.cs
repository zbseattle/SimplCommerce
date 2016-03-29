using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using HvCommerce.Infrastructure.Domain.IRepositories;
using HvCommerce.Orders.Domain.Models;

namespace HvCommerce.Orders.ApplicationServices
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepositoryWithTypedId<ShoppingCartItem, long> _repository;

        public ShoppingCartService(IRepositoryWithTypedId<ShoppingCartItem, long> repository)
        {
            _repository = repository;
        }

        public IEnumerable<ShoppingCartItem> FindByUserId(long userId)
        {
            return _repository.Query().Include(x => x.Product).Where(x => x.CreatedById == userId);
        }  
    }
}
