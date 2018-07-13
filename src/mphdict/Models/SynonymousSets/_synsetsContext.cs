// Copyright © 2016 uSofTrod. Contacts: <uSofTrod@outlook.com>
// License: http://opensource.org/licenses/MIT
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using mphdict.Models.SynonymousSets.Mapping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uSofTrod.generalTypes.Models;

namespace mphdict.Models.SynonymousSets
{
    public class synsetsContext: DbContext
    {
        public string _schema { get; } = string.Empty; 

        public DbSet<langid> lang { get; set; }
        public DbSet<alphadigit> alphadigits { get; set; }
        public DbSet<wlist> wlist { get; set; }
        public DbSet<synsets> synsets { get; set; }
        public DbSet<pofs> pofs { get; set; }

        public synsetsContext(string schema)
        {
            _schema = schema;
        }
        public synsetsContext(DbContextOptions<synsetsContext> options) 
            : base(options)
        {
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;
            //Configuration.AutoDetectChangesEnabled = false;
        }
        public synsetsContext(DbContextOptions<synsetsContext> options, string schema)
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
            modelBuilder.pofsMap();
            modelBuilder.synsetsMap();
            modelBuilder.wlistMap();
            modelBuilder.langMap();
            modelBuilder.alphadigitMap();

            modelBuilder.Entity<langid>().ToTable("lang"); 
            if (!string.IsNullOrEmpty(_schema))
            {
                modelBuilder.HasDefaultSchema(_schema);
                modelBuilder.Entity<alphadigit>().ToTable("alphadigit", "dbo");
            }
            modelBuilder.Entity<alphadigit>()
                .HasKey(c => new { c.lang, c.alpha, c.ls});

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
                _schema = (context as synsetsContext)?._schema;
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
