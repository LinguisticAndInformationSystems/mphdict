using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace mphdict.Models.morph
{
    [JsonObject(IsReference = true)]
    public class flexes: flexes_base
    {
        #region Navigation Properties
        [JsonIgnore]
        [InverseProperty("flexes")]
        [ForeignKey("type")]
        public virtual indents indents { get; set; }
        #endregion
    }
    public class flexes_base
    {

        [Key]
        [Required]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [StringLength(255)]
        public string flex { get; set; }
        public int? field2 { get; set; }
        [StringLength(255)]
        public string xmpl { get; set; }
        public int? field4 { get; set; }
        public short type { get; set; }
        [StringLength(255)]
        public string digit { get; set; }
        public int? ord { get; set; }
    }
}
