using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace mphdict.Models.Etym
{
    public class root: root_base
    {
        public virtual ObservableCollection<bibl> bibls { get; set; }
        public virtual ObservableCollection<e_classes> e_classes { get; set; }
        public virtual ObservableCollection<links> links { get; set; }
    }
    public partial class root_base
    {
        public int id { get; set; }
        public Nullable<int> volume_num { get; set; }
        public Nullable<int> page_initial { get; set; }
        public Nullable<int> page_last { get; set; }
    }
}
