using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace mphdict.Models.morph
{
    [JsonObject(IsReference = true)]
    //[Serializable]
    public class accents_class: accents_class_base
    {
        [JsonIgnore]
        public virtual ICollection<word_param> words_list { get; set; }
        [JsonIgnore]
        public virtual ICollection<accent> accents { get; set; }

    }
    public class accents_class_base
    {
        [Key]
        [Required]
        public short id { get; set; }
        [StringLength(50)]
        public string part_desc { get; set; }
    }
}
