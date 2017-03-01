using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdict.Models.SynonymousSets.Mapping
{
    public static partial class SynSetsDbMaps
    {
        public static void wlistMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<wlist>();

            Entity
                .HasKey(c => new { c.id });

            Entity.Property(p => p.id).ValueGeneratedOnAdd();

            Entity.Property(p => p.word).HasMaxLength(100);
            Entity.Property(p => p.comm).HasMaxLength(4000);
            Entity.Property(p => p.comm2).HasMaxLength(4000);
            Entity.Property(p => p.interpretation).HasMaxLength(4000);

            Entity.Ignore(p => p.CountOfWords);
            Entity.Ignore(p => p.wordsPageNumber);

            Entity.HasIndex(p => p.word);
            Entity.HasIndex(p => p.digit);
            Entity.HasIndex(p => p.id_set);
            Entity.HasIndex(p => p.id_syn);
            Entity.HasIndex(p => p.nom);
            Entity.HasIndex(p => p.id_phon);
            Entity.HasIndex(p => p.intsum);
        }
    }
}
