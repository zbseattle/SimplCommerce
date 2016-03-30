using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopcuatoi.Orders.ApplicationServices
{
    public interface IShoppingCartService
    {
        void ChangeGuestKeyToUser(Guid guestKey, long userId);
    }
}
