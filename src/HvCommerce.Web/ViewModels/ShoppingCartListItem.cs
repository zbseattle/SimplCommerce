using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HvCommerce.Web.Areas.Client.ViewModels
{
    public class ShoppingCartListItem
    {
        public long Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice => Quantity*Price;
    }
}
