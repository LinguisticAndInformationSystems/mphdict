using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace mphdict.Models.morph
{
    [JsonObject(IsReference = true)]
    //[Serializable]
    public class parts: parts_base
    {
        #region Navigation Properties
        [JsonIgnore]
        public virtual ICollection<word_param> words_list { get; set; }
        [JsonIgnore]
        [InverseProperty("parts")]
        [ForeignKey("gr_id")]
        public virtual gr gr { get; set; }
        #endregion
    }
    public class parts_base
    {
        [Key]
        [Required]
        public byte id { get; set; }
        [StringLength(255)]
        public string part { get; set; }
        [StringLength(255)]
        public string com { get; set; }
        [StringLength(50)]
        [JsonIgnore]
        public string ac { get; set; }
        [JsonIgnore]
        public int? rid { get; set; }
        [JsonIgnore]
        public int? mnozh { get; set; }
        [JsonIgnore]
        public int? istota { get; set; }
        [JsonIgnore]
        public int? vid { get; set; }
        [JsonIgnore]
        public int? adjekt { get; set; }
        public byte? gr_id { get; set; }
    }
}
