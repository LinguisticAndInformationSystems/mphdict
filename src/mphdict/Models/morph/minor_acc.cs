using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mphdict.Models.morph
{
    [JsonObject(IsReference = true)]
    //[Serializable]
    public class minor_acc: minor_acc_base
    {
        [JsonIgnore]
        public virtual word_param word_param { get; set; }
    }
    public class minor_acc_base
    {
        [Key, ForeignKey("word_param")]
        [Required]
        public int nom_old { get; set; }
        [StringLength(255)]
        public string word_e1 { get; set; }
        public byte? occur1 { get; set; }
        public byte? occur2 { get; set; }
        public byte? occur3 { get; set; }
        public byte? double1 { get; set; }
        public byte? double2 { get; set; }
    }
}
