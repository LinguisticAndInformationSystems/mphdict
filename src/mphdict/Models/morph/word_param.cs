using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mphdict.Models.morph
{
    [JsonObject(IsReference = true)]
    //[Serializable]
    public class word_param: word_param_base
    {
        #region Navigation Properties
        [JsonIgnore]
        [InverseProperty("words_list")]
        [ForeignKey("part")]
        public virtual parts parts { get; set; }
        [JsonIgnore]
        [InverseProperty("words_list")]
        [ForeignKey("type")]
        public virtual indents indents { get; set; }
        [JsonIgnore]
        [InverseProperty("words_list")]
        [ForeignKey("accent")]
        public virtual accents_class accents_class { get; set; }
        [JsonIgnore]
        public virtual minor_acc minor_acc { get; set; }
        #endregion

    }
    public class word_param_base
    {
        [Required]
        [StringLength(255)]
        public string reestr { get; set; }
        [Required]
        public byte field2 { get; set; }
        [Required]
        public byte part { get; set; }
        [Required]
        public short type { get; set; }
        [StringLength(255)]
        public string field5 { get; set; }
        [StringLength(255)]
        public string field6 { get; set; }
        [StringLength(255)]
        public string field7 { get; set; }
        [Required]
        [StringLength(255)]
        public string digit { get; set; }
        [Key]
        [Required]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int nom_old { get; set; }
        public byte? own { get; set; }
        public bool isdel { get; set; }
        [Required]
        [StringLength(255)]
        public string reverse { get; set; }
        public bool? isproblem { get; set; }
        public short? accent { get; set; }
        public bool? suppl_accent { get; set; }
        [NotMapped]
        public int wordsPageNumber { get; set; }
        [NotMapped]
        public int CountOfWords { get; set; }
    }
}
