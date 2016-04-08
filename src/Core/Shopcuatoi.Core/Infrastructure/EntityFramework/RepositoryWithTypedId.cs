using System.Linq;
using Microsoft.Data.Entity;
using Shopcuatoi.Infrastructure.Domain.IRepositories;
using Shopcuatoi.Infrastructure.Domain.Models;

namespace Shopcuatoi.Core.Infrastructure.EntityFramework
{
    public class RepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId> where T : class, IEntityWithTypedId<TId>
    {
        public RepositoryWithTypedId(HvDbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        protected DbContext Context { get; }

        public DbSet<T> DbSet { get; }

        public T Get(TId id)
        {
            return null;
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void SaveChange()
        {
            Context.SaveChanges();
        }

        public IQueryable<T> Query()
        {
            return DbSet;
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}