using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace mphdict.Models.Etym
{
    public class e_classes: e_classes_base
    {
        public virtual ObservableCollection<etymons> etymons { get; set; }
        public virtual main main { get; set; }
    }
    public partial class e_classes_base
    {
        public int id { get; set; }
        public int id_main { get; set; }
        public int class_type { get; set; }
        public int class_num { get; set; }
        public int subclass_num { get; set; }
        public bool formal_type { get; set; }
        public string subclass_text { get; set; }
        

    }
}
