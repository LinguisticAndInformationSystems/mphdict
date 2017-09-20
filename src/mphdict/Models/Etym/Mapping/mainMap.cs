using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace mphdict.Models.Etym.Mapping
{
    public static partial class ExplDbMaps
    {
        public static void mainMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<main>();

            Entity
                .HasKey(t => t.id);

            Entity
      .HasMany(t => t.bibls)
      .WithOne(i => i.main)
      .HasForeignKey(b => b.id_main);
            Entity
  .HasMany(t => t.e_classes)
  .WithOne(i => i.main)
  .HasForeignKey(b => b.id_main);
            Entity
  .HasMany(t => t.links)
  .WithOne(i => i.main)
  .HasForeignKey(b => b.id_p_info);
        }
    }
}