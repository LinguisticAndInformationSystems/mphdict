using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdict.Models.SynonymousSets
{
    [JsonObject(IsReference = true)]
    public class synsets: synsets_base
    {
        #region Navigation Properties
        [JsonIgnore]
        public virtual ICollection<wlist> _wlist { get; set; }
        [JsonIgnore]
        public virtual pofs _pofs { get; set; }
        #endregion
    }
    public class synsets_base
    {
        public int id { get; set; }
        //public string illustrations { get; set; }
        public string interpretation { get; set; }
        public int pofs { get; set; }
        public bool? finished { get; set; }
        public int? userid { get; set; }
        public DateTime? timemarker { get; set; }
        public byte[] binill { get; set; }
    }
}
