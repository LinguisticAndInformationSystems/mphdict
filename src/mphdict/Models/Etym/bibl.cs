﻿using System;
using System.Collections.Generic;
using System.Text;

namespace mphdict.Models.Etym
{
    public class bibl: bibl_base
    {
        public virtual root root { get; set; }
    }
    public partial class bibl_base
    {
        public int id { get; set; }
        public int id_root { get; set; }
        public int biblio_num { get; set; }
        public string biblio_text { get; set; }

    }
}
