using mphdict.Models.SynonymousSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mphweb.Models
{
    public static class synsetEntry
    {
        static public string formEntry(synsets item)
        {
            //
            StringBuilder s = new StringBuilder();

            if (item.interpretation != null)
            {
                s.Append("<div class=\"syn-block-name\">Тлумачення:</div>");
                s.Append("<div class=\"syn-int\">");
                s.Append(item.interpretation.prepareString());
                s.Append("</div>");
            }
            if (item.illustrations != null)
            {
                s.Append("<div class=\"syn-block-name\">Ілюстративний матеріал:</div>");
                s.Append("<div class=\"syn-ill\">");
                s.Append(item.illustrations.prepareString());
                s.Append("</div>");
            }
            if (item._pofs != null)
            {
                s.Append("<div class=\"syn-pofs\">");
                s.Append($"<span class=\"syn-gr\">{item._pofs.name}</span>");
                s.Append("</div>");
            }
            if (item._wlist != null)
            {
                s.Append("<ul class=\"syn-set\">");
                s.Append("<div class=\"syn-block-name\">Синонімічний ряд:</div>");
                foreach (var i in item._wlist)
                {
                    s.Append("<li class=\"syn-set-item\">");
                    s.Append($"<span class=\"word_style\">&nbsp;{i.word.prepareWord(i.homonym!=null?(int)i.homonym:0)}</span>");
                    s.Append($"<span class=\"syn-comm\">&nbsp;{i.comm.prepareString()}</span>");
                    s.Append($"<span class=\"syn-int\">&nbsp;{i.interpretation.prepareString()}</span>");
                    s.Append("</li>");
                }
                s.Append("</ul>");
            }



            return s.ToString();
        }
        static public string prepareWord(this string s, int h=0)
        {
            if (s != null)
                return $"{s.Replace("\"", "\x301")}<sup>{((h > 0) ? (h.ToString()) : string.Empty)}</sup>";
            else return "";
        }
        static public string prepareString(this string s)
        {
            if (s != null)
                return s.Replace("[S]", "<span class=\"S\">").Replace("[/S]", "</span>").Replace("[/B]", "</span>").Replace("[B]", "<span class=\"B\">").Replace("[BUX]", "<span class=\"BUX\">").Replace("[/BUX]", "</span>").Replace("[I]", "<span class=\"I\">").Replace("[/I]", "</span>");
            else return "";
        }
    }
}
