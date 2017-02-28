using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdict.Models.SynonymousSets
{
    [JsonObject(IsReference = true)]
    public class pofs: pofs_base
    {
        #region Navigation Properties
        public virtual ICollection<synsets> _synsets { get; set; }
        #endregion
    }
    public class pofs_base
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
