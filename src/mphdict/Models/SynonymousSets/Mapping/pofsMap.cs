using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdict.Models.SynonymousSets.Mapping
{
    public static partial class SynSetsDbMaps
    {
        public static void pofsMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<pofs>();

            Entity
                .HasKey(c => new { c.id });

            Entity.Property(p => p.name).HasMaxLength(255);


            Entity
                .HasMany(p => p._synsets)
                .WithOne(i => i._pofs)
                .HasForeignKey(b => b.pofs);

        }
    }
}
