using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdict.Models.SynonymousSets.Mapping
{
    public static partial class SynSetsDbMaps
    {
        public static void synsetsMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<synsets>();

            Entity
                .HasKey(c => new { c.id });

            Entity.Property(p => p.id).ValueGeneratedOnAdd();
            Entity.Property(p => p.interpretation).HasMaxLength(4000);

            Entity
                .HasMany(p => p._wlist)
                .WithOne(i => i._synsets)
                .HasForeignKey(b => b.id_set);

            Entity.HasIndex(p => p.pofs);

        }
    }
}
