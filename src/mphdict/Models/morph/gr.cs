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
    public class gr: gr_base
    {
        [JsonIgnore]
        public virtual ICollection<parts> parts { get; set; }
        [JsonIgnore]
        public virtual ICollection<indents> indents { get; set; }

    }
    public class gr_base
    {
        [Key]
        [Required]
        public byte id { get; set; }
        [StringLength(50)]
        public string part_of_speech { get; set; }
        /*[StringLength(50, MinimumLength = 0)]
        public string field4 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field5 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field6 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field7 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field8 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field9 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field10 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field11 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field12 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field13 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field14 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field15 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field16 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field17 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field18 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field19 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field20 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field21 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field22 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field23 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field24 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field25 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field26 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field27 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field28 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field29 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field30 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field31 { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string field32 { get; set; }*/
    }
}
