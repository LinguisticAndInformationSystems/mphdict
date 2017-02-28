// Copyright © 2016 uSofTrod. Contacts: <uSofTrod@outlook.com>
// License: http://opensource.org/licenses/MIT
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uSofTrod.generalTypes.Models
{
    [JsonObject(IsReference = true)]
    public class alphadigit
    {
        [Key, Column(Order = 1)]
        [Required]
        public int lang { get; set; }
        [Key, Column(Order = 2)]
        [Required]
        [StringLength(10)]
        public string alpha { get; set; }
        [StringLength(10)]
        public string digit { get; set; }
        [Key, Column(Order = 3)]
        [Required]
        public byte ls { get; set; }
    }
}
