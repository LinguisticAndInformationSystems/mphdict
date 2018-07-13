// Copyright © 2016 uSofTrod. Contacts: <uSofTrod@outlook.com>
// License: http://opensource.org/licenses/MIT
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using mphdict.Models.Etym.Mapping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uSofTrod.generalTypes.Models;

namespace mphdict.Models.Etym
{
    public class etymContext: DbContext
    {
        public string _schema { get; } = string.Empty;

        public DbSet<bibl> bibls { get; set; }
        public DbSet<e_classes> e_classes { get; set; }
        public DbSet<etymons> etymons { get; set; }
        public DbSet<lang_all> lang_all { get; set; }
        public DbSet<links> links { get; set; }
        public DbSet<root> roots { get; set; }

        public etymContext(string schema)
        {
            _schema = schema;
        }
        public etymContext(DbContextOptions<etymContext> options) 
            : base(options)
        {
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;
            //Configuration.AutoDetectChangesEnabled = false;
        }
        public etymContext(DbContextOptions<etymContext> options, string schema)
            : base(options)
        {
            _schema = schema;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .ReplaceService<IModelCacheKeyFactory, MyModelCacheKeyFactory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RemovePluralizingTableNameConvention();
            modelBuilder.biblMap();
            modelBuilder.e_classesMap();
            modelBuilder.etymonsMap();
            modelBuilder.lang_allMap();
            modelBuilder.linksMap();
            modelBuilder.rootMap();

            if (!string.IsNullOrEmpty(_schema))
            {
                modelBuilder.HasDefaultSchema(_schema);
            }


            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.Snapshot);


            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();//не змінювати імена під час сворення БД
            ////повне відключення угоди по замовчанню (якщо зовнішній ключ не може містити null, для таких колонок Code First вказує флаг каскадного видалення записів):
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<StoreGeneratedIdentityKeyConvention>();//не генерувати нові ключові поля (з тих, які закінчуються на ID)
        }

        #region for working with multiple schemas
        class MyModelCacheKeyFactory : IModelCacheKeyFactory
        {
            public object Create(DbContext context)
                => new MyModelCacheKey(context);
        }
        class MyModelCacheKey : ModelCacheKey
        {
            string _schema;

            public MyModelCacheKey(DbContext context)
                : base(context)
            {
                _schema = (context as etymContext)?._schema;
            }

            protected override bool Equals(ModelCacheKey other)
                => base.Equals(other)
                    && (other as MyModelCacheKey)?._schema == _schema;

            public override int GetHashCode()
            {
                var hashCode = base.GetHashCode() * 397;
                if (_schema != null)
                {
                    hashCode ^= _schema.GetHashCode();
                }

                return hashCode;
            }
        }
        #endregion
        public IEnumerable<T> Local<T>(DbSet<T> set)
          where T : class
        {
            return this.ChangeTracker.Entries<T>()
              .Where(e => e.State == EntityState.Added || e.State == EntityState.Unchanged || e.State == EntityState.Modified)
              .Select(e => e.Entity);
        }

    }
}
