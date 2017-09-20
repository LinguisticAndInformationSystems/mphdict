using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace mphdict.Models.Etym.Mapping
{
    public static partial class ExplDbMaps
    {
        public static void linksMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<links>();

            Entity
                .HasKey(t => t.id);

            Entity.Property(t => t.word).HasMaxLength(255);
        }
    }
}