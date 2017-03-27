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
        public static void alphadigitMap(this ModelBuilder modelBuilder)
        {
            var Entity = modelBuilder.Entity<alphadigit>();

            Entity
                .HasKey(c => new { c.alpha, c.lang, c.ls });

            Entity.Property(p => p.digit).HasMaxLength(10);
            Entity.Property(p => p.alpha).HasMaxLength(10);
        }
    }
}
