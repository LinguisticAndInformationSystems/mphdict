using Microsoft.EntityFrameworkCore;
//using Microsoft.Data.Entity;
//using Microsoft.Data.Entity.Infrastructure;

using mphdict.Models.morph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//namespace Microsoft.Data.Entity
//{

//    public static class DbSetExtensions
//    {
//        public 
//        public static IEnumerable<T> Local<T>(this DbSet<T> set)
//          where T : class
//        {
//            //var infrastructure = (Microsoft.Data.Entity.Infrastructure.IInfrastructure<IServiceProvider>)set;
//            //var context = set..GetService<mphContext>();
//            var context = (mphContext)infrastructure.Instance.GetService(typeof(DbContext));
//            return context.ChangeTracker.Entries<T>()
//              .Where(e => e.State == EntityState.Added || e.State == EntityState.Unchanged || e.State == EntityState.Modified)
//              .Select(e => e.Entity);
//        }
//    }

//}
