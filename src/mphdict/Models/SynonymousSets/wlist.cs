using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdict.Models.SynonymousSets
{
    [JsonObject(IsReference = true)]
    public class wlist: wlist_base
    {
        #region Navigation Properties
        [JsonIgnore]
        public virtual synsets _synsets { get; set; }
        #endregion
    }
    public class wlist_base
    {
        public int id { get; set; }
        public int id_set { get; set; }
        public int id_syn { get; set; }
        public string word { get; set; }
        public string comm { get; set; }
        public string comm2 { get; set; }
        public string interpretation { get; set; }
        public int? sign { get; set; }
        public int? id_r { get; set; }
        public int? id_int { get; set; }
        public string digit { get; set; }
        public int? nom { get; set; }
        public string sword { get; set; }
        public int? intsum { get; set; }
        public string hyperonym { get; set; }
        public int? id_hyp { get; set; }
        public int? id_phon { get; set; }
        public int? homonym { get; set; }
        public int? login { get; set; }
        public bool inactive { get; set; }
        public DateTime? timemarker { get; set; }

    }
}
