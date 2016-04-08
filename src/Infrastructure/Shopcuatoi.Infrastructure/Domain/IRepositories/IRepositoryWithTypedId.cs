using System.Linq;
using Microsoft.Data.Entity;
using Shopcuatoi.Infrastructure.Domain.Models;

namespace Shopcuatoi.Infrastructure.Domain.IRepositories
{
    public interface IRepositoryWithTypedId<T, in TId> where T : class, IEntityWithTypedId<TId>
    {
        /// <summary>
        /// Returns null if a row is not found matching the provided Id.
        /// </summary>
        T Get(TId id);

        DbSet<T> DbSet { get; }

        IQueryable<T> Query();

        void Add(T entity);

        void SaveChange();

        void Remove(T entity);
    }
}
