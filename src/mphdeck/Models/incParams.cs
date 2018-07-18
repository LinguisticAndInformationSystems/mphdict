using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdeck.Models
{
    public class incParams
    {
        public int wid { get; set; }
        public int currentPage { get; set; }
        public string wordSearch { get; set; }
    }
    public class synincParams
    {
        public int idset { get; set; }
        public int wid { get; set; }
        public int currentPage { get; set; }
        public string wordSearch { get; set; }
    }
    public class etymincParams
    {
        public int idclass { get; set; }
        public int wid { get; set; }
        public int currentPage { get; set; }
        public string wordSearch { get; set; }
    }
}
