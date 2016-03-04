using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HvCommerce.Infrastructure.Domain.Models;

namespace HvCommerce.Orders.ApplicationServices
{
    public interface IBaseServiceWithTypeId<TEntity, in TId> where TEntity : EntityWithTypedId<TId>
    {
        void Add(TEntity entity);

        TEntity Get(TId id);

        IQueryable<TEntity> Query();

        void Remove(TEntity entity);
    }
}
