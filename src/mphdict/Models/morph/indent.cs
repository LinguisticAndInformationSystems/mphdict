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
    public class indents: indents_base
    {
        #region Navigation Properties
        [JsonIgnore]
        public virtual ICollection<word_param> words_list { get; set; }
        [JsonIgnore]
        public virtual List<flexes> flexes { get; set; }
        [InverseProperty("indents")]
        [ForeignKey("gr_id")]
        [JsonIgnore]
        public virtual gr gr { get; set; }
        #endregion
    }
    public class indents_base
    {
        [Key]
        [Required]
        public short type { get; set; }
        public int indent { get; set; }
        public int? field3 { get; set; }
        public int? field4 { get; set; }
        [StringLength(255)]
        public string comment { get; set; }
        public byte gr_id { get; set; }
    }
}
