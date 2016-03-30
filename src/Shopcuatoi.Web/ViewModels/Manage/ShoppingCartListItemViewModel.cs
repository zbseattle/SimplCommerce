using System;

namespace HvCommerce.Web.ViewModels.Manage
{
    public class ShoppingCartListItemViewModel
    {
        public long Id { get; set; }

        public string CreatedOn { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice => Quantity*Price;
    }
}
