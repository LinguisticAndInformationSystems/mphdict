using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace mphdict.Models.Etym.Mapping
{
    public static partial class ExplDbMaps
    {
        public static void e_classesMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<e_classes>();

            Entity
                .HasKey(t => t.id);

            Entity
      .HasMany(t => t.etymons)
      .WithOne(i => i.e_classes)
      .HasForeignKey(b => b.id_e_classes);
        }
    }
}