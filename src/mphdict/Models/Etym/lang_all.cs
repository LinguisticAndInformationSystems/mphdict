using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace mphdict.Models.Etym
{
    public class lang_all: lang_all_base
    {
        public virtual ObservableCollection<etymons> etymons { get; set; }
    }
    public partial class lang_all_base
    {
        public int lang_code { get; set; }
        public string lang_marker { get; set; }
        public string lang_marker_syn { get; set; }
        public string lang_name { get; set; }
        public string lang_name_syn { get; set; }
        public Nullable<int> volume { get; set; }
    }
}
