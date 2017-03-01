using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uSofTrod.generalTypes.Models;

namespace mphdict
{
    public static class sharedTypes
    {
        public static string atod(string a, alphadigit[] talpha, bool askip = true)
        {
            if (a == null) return "";
            StringBuilder d = new StringBuilder("");
            for (int i = 0; i < a.Length; i++)
            {
                if ((askip) && ((a[i] == '\'') || (a[i] == ' ') || (a[i] == '-'))) continue;
                alphadigit rs = talpha.FirstOrDefault(t => t.alpha == a[i].ToString());
                if (rs != null) d.Append(rs.digit);
            }
            return d.ToString();
        }

    }
    public struct ps
    {
        public short id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
    }
    public enum FetchType
    {
        [Display(Name = "StartsWith", ResourceType = typeof(Resources.main))]
        StartsWith = 0,
        [Display(Name = "Contains", ResourceType = typeof(Resources.main))]
        Contains = 1,
        [Display(Name = "EndsWith", ResourceType = typeof(Resources.main))]
        EndsWith = 2
    };

}
