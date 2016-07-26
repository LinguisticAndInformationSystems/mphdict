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
    //[Serializable]
    [JsonObject(IsReference = true)]
    public class allang
    {
        [Key]
        [Required]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id_lang { get; set; }
        [StringLength(255)]
        public string full_lang { get; set; }
        [StringLength(255)]
        public string short_lang { get; set; }
    }
}
