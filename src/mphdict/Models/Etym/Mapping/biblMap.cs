using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace mphdict.Models.Etym.Mapping
{
    public static partial class ExplDbMaps
    {
        public static void biblMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<bibl>();

            Entity
                .HasKey(t => t.id);

            Entity.Property(t => t.biblio_text).HasMaxLength(225);
        }
    }
}