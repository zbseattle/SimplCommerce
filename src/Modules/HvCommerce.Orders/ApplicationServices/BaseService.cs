using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HvCommerce.Core.Infrastructure.EntityFramework;
using HvCommerce.Infrastructure.Domain.IRepositories;
using HvCommerce.Infrastructure.Domain.Models;

namespace HvCommerce.Orders.ApplicationServices
{
    public class BaseService<TEntity> : BaseServiceWithTypeId<TEntity, long> where TEntity : EntityWithTypedId<long>
    {
        public BaseService(IRepositoryWithTypedId<TEntity, long> repository) : base(repository)
        {
        }
    }
}
