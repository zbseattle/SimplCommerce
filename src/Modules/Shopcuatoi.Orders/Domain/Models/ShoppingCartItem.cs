using System;
using Shopcuatoi.Core.Domain.Models;
using Shopcuatoi.Infrastructure.Domain.Models;

namespace Shopcuatoi.Orders.Domain.Models
{
    public class ShoppingCartItem : Entity
    {
        public long ProductId { get; set; }

        public long? ProductVariationId { get; set; }

        public long ShoppingCartId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual Product Product { get; set; }

        public virtual ProductVariation ProductVariation { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }
    }
}