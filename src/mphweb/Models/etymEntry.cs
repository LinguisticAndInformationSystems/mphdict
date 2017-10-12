using mphdict;
using mphdict.Models.Etym;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uSofTrod.TextUtility.txToolsCore;

namespace mphweb.Models
{
    public static class etymEntry
    {
        static public string formEntry(root item)
        {
            StringBuilder html = new StringBuilder(), hindex = new StringBuilder("");
            if (item != null)
            {

                string[] roman = new string[10];
                roman[0] = "I"; roman[1] = "II"; roman[2] = "III"; roman[3] = "IV"; roman[4] = "V";
                roman[5] = "VI"; roman[6] = "VII"; roman[7] = "VIII"; roman[8] = "IX"; roman[9] = "X";
                html.Append("<div class=\"etym-ref\">");
                html.Append("<div class=\"etym-ref-part\">Етимологічний словник української мови</div>");
                html.Append("<div class=\"etym-ref-part\">Інституту мовознавства ім. О.О. Потебні НАН України</div>");
                html.Append("<div class=\"etym-ref-part\">Том "); html.Append(item.volume_num.ToString());
                html.Append(", стор. "); html.Append(item.page_initial.ToString());
                if (item.page_initial != item.page_last)
                {
                    html.Append("-");
                    html.Append(item.page_last.ToString());
                }
                html.Append("</div></div>");

                int cur = 0;
                int cls = -1;
                bool first_c_type = true;
                bool first_in_group = true;
                if (item.e_classes != null && item.e_classes.Count() > 0)
                {
                    //html.Append("<div class=\"etym-block-name\">Етимологія</div>");
                    html.Append("<div class=\"etym-classes\">");
                    
                    foreach (e_classes row in item.e_classes.OrderBy(o => o.class_type).ThenBy(o => o.class_num).ThenBy(o => o.subclass_num)) //"class_type, class_num, subclass_num"
                    {

                        int clid = row.id, ocur = cur;
                        if (row.subclass_num == 1)
                        {
                            if (row.class_type > 1)
                                html.Append(";</div>");
                            else if (row.class_type != 0) html.Append(",</div>");
                            //if (row.class_num > 1) hindex.Append("/ul");
                        }
                        else html.Append(";</div>");
                        html.Append("<div class=\"etym-class\">");
                        if (row.class_type > 1&& row.subclass_num == 1)
                            html.Append("– ");
                        if (!row.formal_type)
                            html.Append((row.subclass_text ?? "").ToString());

                        bool first = true;

                        foreach (etymons drow in row.etymons.OrderBy(o => o.word_num))
                        {
                           
                            if (row.formal_type)
                            {
                                html.Append("<span class=\"etym-etymon\">");
                                if (!drow.ishead)
                                {
                                    if (!first) html.Append(", ");
                                    if (drow.lang_marker != "укр.")
                                        html.Append(drow.lang_marker + " ");
                                }
                                else html.Append("<span class=\"etym-word is-head\">");
                                if (drow.dialect) html.Append("[");
                                if (!drow.ishead) html.Append("<i>");
                                html.Append((drow.word ?? "").ToString());
                                if (!drow.ishead) html.Append("</i>");
                                if (drow.homonym > 0)
                                    html.Append("<SUP>" + drow.homonym.ToString() + "</SUP>");
                                if (drow.dialect) html.Append("]");
                                if (drow.ishead) html.Append("</span>");

                                if ((drow.sense != null) && (drow.sense != ""))
                                    html.Append(" " + drow.sense);
                                if ((drow.note != null) && (drow.note != ""))
                                    html.Append(" " + drow.note);
                                if ((drow.bibliography != null) && (drow.bibliography != ""))
                                    html.Append(" " + drow.bibliography);
                                html.Append("</span>");
                            }
                            if (cls != row.class_type)
                            {
                                cls = row.class_type;
                                if (first_c_type == false) hindex.Append(".</li></ul>");
                                switch (cls)
                                {
                                    case 1: hindex.Append("<div class=\"etym-block-name\">Фонетичні та словотвірні варіанти</div>"); break;
                                    case 2: hindex.Append("<div class=\"etym-block-name\">Етимологічні відповідники у слов'янських мовах</div>"); break;
                                    case 3: hindex.Append("<div class=\"etym-block-name\">Етимологічні відповідники</div>"); break;
                                }
                                if (cls != 0)
                                {
                                    hindex.Append("<ul class=\"etym-equivalents\">");
                                    first_c_type = false;
                                    first_in_group = true;
                                }
                            }
                            if (cls > 0)
                            {
                                if(!first_in_group) hindex.Append(";</li>");
                                hindex.Append("<li class=\"etym-equivalent\">");
                                if (first_in_group) first_in_group = false;
                                hindex.Append("<span class=\"etym-word\">");
                                if (drow.dialect) hindex.Append("[");
                                hindex.Append((drow.word ?? "").ToString());
                                if (drow.homonym > 0)
                                    hindex.Append("<SUP>" + drow.homonym.ToString() + "</SUP>");
                                if (drow.dialect) hindex.Append("]");
                                hindex.Append("</span>");
                                if (cls > 1)
                                {
                                    if (drow.lang_code != 0)
                                    {
                                        ps[] rows = null;
                                        if (variables.etymLang_all != null) rows = variables.etymLang_all.Where(l => l.id == drow.lang_code).ToArray();
                                        if ((rows != null) && (rows.Count() > 0))
                                            hindex.Append(" <span class=\"etym-lang-name\">‹" + rows.First().name + "›</span>");
                                        else hindex.Append(" <span class=\"etym-lang-name\">‹" + drow.lang_marker + "›</span>");
                                    }
                                }
                                //hindex.Append("<span>;</span>");
                                //hindex.Append("</li>");
                            }
                            first = false;
                            cur++;
                            
                        }
                    }
                    html.Append(".</div></div>");
                }
                    if (item.bibls != null && item.bibls.Count() > 0)
                {
                    html.Append("<div class=\"etym-block-name\">Бібліографія</div>");
                    html.Append("<ul class=\"etym-bibls\">");
                    foreach (bibl row in item.bibls.OrderBy(c=>c.biblio_num))
                    {                      
                        if (row.biblio_num != 1) html.Append(";</li>");
                        html.Append("<li class=\"etym-bibl\">");
                        html.Append((row.biblio_text ?? "").ToString());
                    }
                    html.Append(".</li></ul>");
                }
                byte ltype = 255;
                if (item.links != null && item.links.Count() > 0)
                {
                    html.Append("<div class=\"etym-block-name\">Посилання</div>");
                    html.Append("<div class=\"etym-links\">");
                    int count = 1;
                    foreach (links row in item.links)
                {
                    if (ltype != row.link_type)
                    {
                        ltype = row.link_type;
                            if(count!=1) html.Append(".</div>");
                            html.Append("<div class=\"etym-link\">");
                            if (ltype == 0) html.Append("Див. ще ");
                        if (ltype == 1) html.Append("Пор. ");
                    }
                    else html.Append(", ");
                    //if (comm) html.Append("<FONT COLOR=\"Blue\">&lt;LINK " + (ltype == 0 ? "DYV " : "POR ") + row.link_num.ToString() + "&gt;</FONT>");
                    html.Append("<span class=\"etym-word\">");
                    html.Append((row.word ?? "").ToString());
                        if (row.homonym > 0)
                        {
                            html.Append("<SUP>");
                            html.Append(row.homonym);
                            html.Append("</SUP>");
                        }
                        html.Append("</span>");
                        count++;
                    //if (comm) html.Append("<FONT COLOR=\"Blue\">&lt;/--LINK--&gt;</FONT>");
                }
                    html.Append(".</div></div>");
                }
                if (first_c_type == false) hindex.Append(".</li></ul>");
                html.Append(hindex);
            }
            return html.ToString();
        }
  
    }
}
