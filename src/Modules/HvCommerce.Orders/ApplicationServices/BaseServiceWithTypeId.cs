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
    public class BaseServiceWithTypeId<TEntity, TId> : IBaseServiceWithTypeId<TEntity, TId> where TEntity : EntityWithTypedId<TId>
    {
        protected readonly IRepositoryWithTypedId<TEntity, TId> repository;

        public BaseServiceWithTypeId(IRepositoryWithTypedId<TEntity, TId> repository)
        {
            this.repository = repository;
        }

        public void Add(TEntity entity)
        {
            repository.Add(entity);
            repository.SaveChange();
        }

        public TEntity Get(TId id)
        {
            return repository.Get(id);
        }

        public IQueryable<TEntity> Query()
        {
            return repository.Query();
        }

        public void Remove(TEntity entity)
        {
            repository.Remove(entity);
            repository.SaveChange();
        }
    }
}
