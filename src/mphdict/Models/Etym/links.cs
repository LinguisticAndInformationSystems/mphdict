using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace mphdict.Models.Etym
{
    public class links: links_base
    {
        public virtual root root { get; set; }
    }
    public partial class links_base
    {
        public int id { get; set; }
        public int id_root { get; set; }
        public byte link_type { get; set; }
        public int link_num { get; set; }
        public string word { get; set; }
        public Nullable<byte> homonym { get; set; }
    }
}
