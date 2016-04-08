using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Shopcuatoi.Core.ApplicationServices;
using Shopcuatoi.Core.Domain.Models;
using Shopcuatoi.Infrastructure;
using Shopcuatoi.Infrastructure.Domain.Models;

namespace Shopcuatoi.Core.Infrastructure.EntityFramework
{
    public class HvDbContext : IdentityDbContext<User, Role, long>
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // Database.SetInitializer(new MigrateDatabaseToLatestVersion<HvDbContext, AutomaticMigrationsConfiguration>());

            List<Type> typeToRegisters = new List<Type>();
            foreach(var module in GlobalConfiguration.Modules)
            {
                typeToRegisters.AddRange(TypeLoader.FromAssemblies(Assembly.Load(module.AssemblyName)));
            }

            RegisterEntities(modelBuilder, typeToRegisters);

            RegiserConvention(modelBuilder);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .ToTable("Core_User");

            modelBuilder.Entity<Role>()
                .ToTable("Core_Role");

            modelBuilder.Entity<IdentityUserClaim<long>>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("Core_UserClaim");
            });

            modelBuilder.Entity<IdentityRoleClaim<long>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("Core_RoleClaim");
            });

            modelBuilder.Entity<IdentityUserRole<long>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("Core_UserRole");
            });

            modelBuilder.Entity<IdentityUserLogin<long>>(b =>
            {
                b.ToTable("Core_UserLogin");
            });
        }

        private static void RegiserConvention(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.ClrType.Namespace != null)
                {
                    var nameParts = entity.ClrType.Namespace.Split('.');
                    var tableName = string.Concat(nameParts[1], "_", entity.ClrType.Name);
                    modelBuilder.Entity(entity.Name).ToTable(tableName);
                }
            }
        }

        private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            var entityTypes = typeToRegisters.Where(x => x.IsSubclassOf(typeof (Entity)) && !x.IsAbstract);
            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }
    }
}