using Microsoft.AspNet.Identity.EntityFramework;
using Shopcuatoi.Infrastructure.Domain.Models;

namespace Shopcuatoi.Core.Domain.Models
{
    public class Role : IdentityRole<long>, IEntityWithTypedId<long>
    {
    }
}