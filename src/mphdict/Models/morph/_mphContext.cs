// Copyright © 2016 uSofTrod. Contacts: <uSofTrod@outlook.com>
// License: http://opensource.org/licenses/MIT
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using mphdict.Models.SynonymousSets.Mapping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uSofTrod.generalTypes.Models;

namespace mphdict.Models.morph
{
    public class mphContext : DbContext
    {
        private string _schema = string.Empty; // "ukgram";

        public DbSet<alphadigit> alphadigits { get; set; }
        public DbSet<word_param> words_list { get; set; }
        public DbSet<gr> grs { get; set; }
        public DbSet<indents> indents { get; set; }
        public DbSet<accent> accent { get; set; }
        public DbSet<parts> parts { get; set; }
        public DbSet<flexes> flexes { get; set; }
        public DbSet<accents_class> accents_class { get; set; }
        //public DbSet<parts_gr> parts_gr { get; set; }
        public DbSet<minor_acc> minor_acc { get; set; }
        //public DbSet<allang> allangs { get; set; }
        public DbSet<langid> lang { get; set; }

        public mphContext(DbContextOptions<mphContext> options)
            : base(options)
        {
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;
            //Configuration.AutoDetectChangesEnabled = false;
        }
        public mphContext(DbContextOptions<mphContext> options, string schema)
            : base(options)
        {
            _schema = schema;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
        //}
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.langMap();
            modelBuilder.alphadigitMap();

            modelBuilder.Entity<word_param>().ToTable("nom");
            modelBuilder.Entity<word_param>().HasIndex(b => b.accent);
            modelBuilder.Entity<word_param>().HasIndex(b => b.digit);
            modelBuilder.Entity<word_param>().HasIndex(b => b.reverse);
            modelBuilder.Entity<word_param>().HasIndex(b => b.isdel);
            modelBuilder.Entity<word_param>().HasIndex(b => b.part);
            modelBuilder.Entity<word_param>().HasIndex(b => b.reestr);
            modelBuilder.Entity<word_param>().HasIndex(b => b.type);

            modelBuilder.Entity<langid>().ToTable("lang");
            modelBuilder.Entity<flexes>().HasIndex(b => b.type);
            modelBuilder.Entity<flexes>().HasIndex(b => b.flex);
            modelBuilder.Entity<indents>().HasIndex(b => b.gr_id);
            modelBuilder.Entity<accent>().HasIndex(b => b.accent_type);

            if (!string.IsNullOrEmpty(_schema))
            {
                modelBuilder.HasDefaultSchema(_schema);
                modelBuilder.Entity<alphadigit>().ToTable("alphadigit", "dbo");
            }
            modelBuilder.Entity<alphadigit>()
                .HasKey(c => new { c.lang, c.alpha, c.ls });

            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.Snapshot);


            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();//не змінювати імена під час сворення БД
            ////повне відключення угоди по замовчанню (якщо зовнішній ключ не може містити null, для таких колонок Code First вказує флаг каскадного видалення записів):
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<StoreGeneratedIdentityKeyConvention>();//не генерувати нові ключові поля (з тих, які закінчуються на ID)
        }

        #region for working with multiple schemas
        private static ConcurrentDictionary<string, IModel> modelCache = new ConcurrentDictionary<string, IModel>();
        public static mphContext Create(DbContextOptionsBuilder<mphContext> optionsBuilder, string schema)
        {

            IModel compiledModel;
            if (schema == null || schema.Trim().Length == 0) schema = "/";
            compiledModel = modelCache.GetOrAdd(schema, (t) => GetCompiled(optionsBuilder, t));

            optionsBuilder.UseModel(compiledModel);

            return new mphContext(optionsBuilder.Options, schema);
        }
        private static IModel GetCompiled(DbContextOptionsBuilder<mphContext> optionsBuilder, string schema)
        {
            //https://github.com/aspnet/EntityFramework/issues/3909
            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddEntityFramework().AddSqlServer();
            //var serviceProvider = serviceCollection.BuildServiceProvider();
            var coreConventionSetBuilder = new CoreConventionSetBuilder(null);
            //var sqlConventionSetBuilder = new SqlServerConventionSetBuilder(new SqlServerTypeMapper());
            //var conventionSet = sqlConventionSetBuilder.AddConventions(coreConventionSetBuilder.CreateConventionSet());

            var conventionSet = coreConventionSetBuilder.CreateConventionSet();
            var builder = new ModelBuilder(conventionSet);
            builder.Entity<alphadigit>();
            builder.Entity<langid>();
            builder.Entity<word_param>();
            builder.Entity<gr>();
            builder.Entity<indents>();
            builder.Entity<accent>();
            builder.Entity<parts>();
            builder.Entity<flexes>();
            builder.Entity<accents_class>();
            builder.Entity<minor_acc>();
            //Cal Manually OnModelCreating from base class to create model objects relatet di Asp.Net identity (not nice)
            (new mphContext(optionsBuilder.Options, schema)).OnModelCreating(builder);
            //builder.Entity<Invoice>().ToTable("REGION", schema);

            return builder.Model;
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
public static class ModelBuilderExtensions
{
    public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.Relational().TableName = entity.Name.Substring(entity.Name.LastIndexOf('.') + 1);//.DisplayName();
        }
    }
}
