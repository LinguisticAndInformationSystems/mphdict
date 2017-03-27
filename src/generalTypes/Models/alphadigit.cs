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
        public int lang { get; set; }
        public string alpha { get; set; }
        public string digit { get; set; }
        public byte ls { get; set; }
    }
}
