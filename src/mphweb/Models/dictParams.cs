using mphdict;
using mphdict.Models.morph;
using mphdict.Models.SynonymousSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphweb.Models
{
    public class grdictParams
    {
        public filter f { get; set; }
        public incParams incp { get; set; }
        public word_param[] page { get; set; }
        //public word_param_base SearchedWord { get; set; }
        public int maxpage { get; set; }
        public int count { get; set; }
        public int id_lang { get; set; }
        public word_param entry { get; set; }
    }
    public class syndictParams
    {
        public synsetsfilter f { get; set; }
        public synincParams incp { get; set; }
        public wlist[] page { get; set; }
        //public word_param_base SearchedWord { get; set; }
        public int maxpage { get; set; }
        public int count { get; set; }
        public int id_lang { get; set; }
        public synsets entry { get; set; }
    }
}
