using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace mphdict.Models.Etym.Mapping
{
    public static partial class ExplDbMaps
    {
        public static void rootMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<root>();

            Entity
                .HasKey(t => t.id);

            Entity
      .HasMany(t => t.bibls)
      .WithOne(i => i.root)
      .HasForeignKey(b => b.root);
            Entity
  .HasMany(t => t.e_classes)
  .WithOne(i => i.root)
  .HasForeignKey(b => b.id_root);
            Entity
  .HasMany(t => t.links)
  .WithOne(i => i.root)
  .HasForeignKey(b => b.id_root);
        }
    }
}