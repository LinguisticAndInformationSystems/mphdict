using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace mphdict.Models.Etym
{
    public class etymons: etymons_base
    {
        public virtual e_classes e_classes { get; set; }
        public virtual lang_all lang_all { get; set; }
    }
    public partial class etymons_base
    {
        public int id { get; set; }
        public int id_e_classes { get; set; }
        public int word_num { get; set; }
        public bool ishead { get; set; }
        public string word { get; set; }
        public int homonym { get; set; }
        public bool dialect { get; set; }
        public bool antroponym { get; set; }
        //public string digit { get; set; }
        public Nullable<int> lang_code { get; set; }
        public string lang_marker { get; set; }
        public string lang_note { get; set; }
        public string note { get; set; }
        public string sense { get; set; }
        public string bibliography { get; set; }
        [JsonIgnore]
        public int CountOfWords { get; set; }
        [JsonIgnore]
        public int wordsPageNumber { get; set; }
    }
}
