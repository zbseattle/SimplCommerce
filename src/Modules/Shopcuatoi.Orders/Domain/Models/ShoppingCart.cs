using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shopcuatoi.Core.Domain.Models;
using Shopcuatoi.Infrastructure.Domain.Models;

namespace Shopcuatoi.Orders.Domain.Models
{
    public class ShoppingCart : Entity
    {
        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? CreatedById { get; set; }

        public virtual User CreatedBy { get; set; }

        public Guid? GuestKey { get; set; }

        public string DeliveryAddress { get; set; }

        public bool PaymentStatus { get; set; }

        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } 
    }
}
