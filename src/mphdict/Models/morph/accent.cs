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
    public class accent: accent_base
    {
        #region Navigation Properties
        [JsonIgnore]
        [InverseProperty("accents")]
        [ForeignKey("accent_type")]
        public virtual accents_class accents_class { get; set; }
        #endregion
    }

    [JsonObject(IsReference = true)]
    //[Serializable]
    public class accent_base
    {
        [Key]
        [Required]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public short? indent1 { get; set; }
        public short? indent2 { get; set; }
        public short? indent3 { get; set; }
        public short? indent4 { get; set; }
        public short? accent_type { get; set; }
        public short? gram { get; set; }
        [StringLength(50)]
        public string xmpl { get; set; }

    }
}
