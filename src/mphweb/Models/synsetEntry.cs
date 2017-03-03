using mphdict.Models.SynonymousSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uSofTrod.TextUtility.txToolsCore;

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
                s.Append(item.illustrations.prepareString().prepareGId());
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
        static public string prepareGId(this string s)
        {
            if (s != null)
            {
                s = s.Replace("[/GID]", "</span>");
                string start_tag = "[GID";
                string end_tag = "[/GID]";
                int start = s.IndexOf(start_tag);
                int end = s.IndexOf(end_tag);
                for (;;)
                {
                    if (start < 0) break;
                    int indx = start + start_tag.Length;
                    while ((indx < s.Length) && (xparse.numerals.Contains(s[indx])))
                    {
                        indx++;
                    }
                    string id = s.Substring(start + start_tag.Length + 1, indx - (start + start_tag.Length) - 1);
                    s = s.Remove(start, start - (indx + 1));
                    s = s.Insert(start, $"<span data-gd-id=\"{id}\">");
                    start = s.IndexOf(start_tag);
                    end = s.IndexOf(end_tag);
                }
                return s;
            }
            else return "";
        }
    }
}
