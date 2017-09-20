using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace mphdict.Models.Etym.Mapping
{
    public static partial class ExplDbMaps
    {
        public static void lang_allMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<lang_all>();

            Entity
                .HasKey(t => t.lang_code);

            Entity.Property(t => t.lang_marker).IsRequired().HasMaxLength(50);
            Entity.Property(t => t.lang_marker_syn).HasMaxLength(50);
            Entity.Property(t => t.lang_name).HasMaxLength(50);
            Entity.Property(t => t.lang_name_syn).HasMaxLength(50);


            Entity
      .HasMany(t => t.etymons)
      .WithOne(i => i.lang_all)
      .HasForeignKey(b => b.lang_code);
        }
    }
}