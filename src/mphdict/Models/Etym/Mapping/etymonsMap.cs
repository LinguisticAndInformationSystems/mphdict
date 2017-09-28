using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace mphdict.Models.Etym.Mapping
{
    public static partial class ExplDbMaps
    {
        public static void etymonsMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<etymons>();

            Entity
                .HasKey(t => t.id);

            Entity.Property(t => t.word).HasMaxLength(255);
            //Entity.Property(t => t.digit).HasMaxLength(255);
            Entity.Property(t => t.lang_marker).HasMaxLength(255);
            Entity.Property(t => t.lang_note).HasMaxLength(50);
        }
    }
}