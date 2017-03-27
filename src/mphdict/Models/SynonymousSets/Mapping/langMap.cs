using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uSofTrod.generalTypes.Models;

namespace mphdict.Models.SynonymousSets.Mapping
{
    public static partial class SynSetsDbMaps
    {
        public static void langMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<langid>();

            Entity
                .HasKey(c => new { c.pref });

            Entity.Property(p => p.pref).HasMaxLength(255);
            Entity.Property(p => p.lang).HasMaxLength(255);
        }
    }
}
