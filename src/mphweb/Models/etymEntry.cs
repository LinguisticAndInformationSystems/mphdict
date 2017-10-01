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

                hindex.Append("<P>");
                html.Append("<P ALIGN=\"Right\"><FONT COLOR=\"Gray\">");
                html.Append("Том "); html.Append(item.volume_num.ToString());
                html.Append(", стор. "); html.Append(item.page_initial.ToString());
                if (item.page_initial != item.page_last)
                {
                    html.Append("-");
                    html.Append(item.page_last.ToString());
                }
                html.Append("</FONT></P><P>");

                int cur = 0;
                int cls = -1;
                if (item.e_classes != null && item.e_classes.Count() > 0)
                {
                    html.Append("<div class=\"etym-block-name\">Етимологія</div>");
                    html.Append("<div class=\"etym-classes\">");
                    foreach (e_classes row in item.e_classes.OrderBy(o => o.class_type).ThenBy(o => o.class_num).ThenBy(o => o.subclass_num)) //"class_type, class_num, subclass_num"
                    {

                        int clid = row.id, ocur = cur;
                        if (row.subclass_num == 1)
                        {
                            if (row.class_type > 1)
                                html.Append(";</div>");
                            else if (row.class_type != 0) html.Append(",</div>");
                            //if (row.class_num > 1) hindex.Append("<BR>");
                        }
                        else html.Append(";</div>");
                        html.Append("<div class=\"etym-class\">");
                        if (row.class_type > 1&& row.subclass_num == 1)
                            html.Append("--");
                        if (!row.formal_type)
                            html.Append((row.subclass_text ?? "").ToString());

                        bool first = true;
                        foreach (etymons drow in row.etymons.OrderBy(o => o.word_num))
                        {
                            html.Append("<span class=\"etym-etymon\">");
                            if (row.formal_type)
                            {
                                if (!drow.ishead)
                                {
                                    if (!first) html.Append(", ");
                                    if (drow.lang_marker != "укр.")
                                        html.Append(drow.lang_marker + " ");
                                }
                                else html.Append("<span class=\"B\">");
                                if (drow.dialect) html.Append("[");
                                if (!drow.ishead) html.Append("<span class=\"I\">");
                                html.Append((drow.word ?? "").ToString());
                                if (!drow.ishead) html.Append("</span>");
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
                            }
                            if (cls != row.class_type)
                            {
                                cls = row.class_type;
                                switch (cls)
                                {
                                    case 1: hindex.Append("<FONT COLOR=\"DarkRed\"><H5>Фонетичні та словотворчі варіанти</H5></FONT>"); break;
                                    case 2: hindex.Append("<FONT COLOR=\"DarkRed\"><H5>Етимологічні відповідники у слов'янських мовах</H5></FONT>"); break;
                                    case 3: hindex.Append("<FONT COLOR=\"DarkRed\"><H5>Праслов'янська мова+інші індоєвропейські мови+...</H5></FONT>"); break;
                                }
                            }
                            if (cls > 0)
                            {
                                hindex.Append("<B>");
                                if (drow.dialect) hindex.Append("[");
                                hindex.Append((drow.word ?? "").ToString());
                                if (drow.homonym > 0)
                                    hindex.Append("<SUP>" + drow.homonym.ToString() + "</SUP>");
                                if (drow.dialect) hindex.Append("]");
                                hindex.Append("</B> ");
                                if (cls > 1)
                                {
                                    ps[] rows = null;
                                    if (variables.etymLang_all != null) rows = variables.etymLang_all.Where(l => l.id == drow.lang_code).ToArray();
                                    if ((rows != null) && (rows.Count() > 0))
                                        hindex.Append("<FONT COLOR=\"DarkRed\">&lt;" + rows.First().name + "&gt;</FONT>");
                                    else hindex.Append("<FONT COLOR=\"DarkRed\">&lt;" + drow.lang_marker + "&gt;</FONT>");
                                }
                                hindex.Append(" <BR>");
                            }
                            first = false;
                            cur++;
                            html.Append("</span>");
                        }
                    }
                    html.Append(".</div></div>");
                }
                    //if (langs != null) foreach (int lng in langs)
                    //    {
                    //        List<lang_all> row = tlangall.Where(l => l.lang_code == lng).ToList();
                    //        if (row != null) for (int i = 0; i < row.Count; i++)
                    //            {
                    //                string slang = row[i].lang_marker.ToString();
                    //                int pos = html.ToString().IndexOf("<!--HERE!AND!NOW-->");
                    //                if (pos > 0) pos = html.ToString().IndexOf(slang, pos);
                    //                while (pos > 0)
                    //                {
                    //                    if ("\r\n\t >".IndexOf(html[pos - 1]) >= 0)
                    //                    {
                    //                        html.Insert(pos, "<FONT COLOR=\"DarkRed\"><B>");
                    //                        html.Insert(pos + slang.Length + 25, "</B></FONT>");
                    //                        pos = html.ToString().IndexOf(slang, pos + slang.Length + 25);
                    //                    }
                    //                    else pos = html.ToString().IndexOf(slang, pos + slang.Length);
                    //                }
                    //            }
                    //    }
                    //if (comm) html.Append("<FONT COLOR=\"Blue\">&lt;/--SUBCLASS--&gt;&lt;/--CLASS--&gt;</FONT>");
                    if (item.bibls != null && item.bibls.Count() > 0)
                {
                    html.Append("<div class=\"etym-block-name\">Бібліографія</div>");
                    html.Append("<ul class=\"etym-bibls\">");
                    foreach (bibl row in item.bibls)
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
                    html.Append("<B>");
                    html.Append((row.word ?? "").ToString());
                    if (row.homonym > 0)
                        html.Append(row.homonym);
                    html.Append("</B>");
                        count++;
                    //if (comm) html.Append("<FONT COLOR=\"Blue\">&lt;/--LINK--&gt;</FONT>");
                }
                    html.Append(".</div></div>");
                }
                if (html[html.Length - 1] != '.') html.Append('.');
                html.Append("</P></FONT>");
                hindex.Append("</P></FONT>");
                //if (head)
                //{
                //    html.Append("</BODY></HTML>");
                //    hindex.Append("</BODY></HTML>");
                //}
                //showArt.article = html.ToString(); showArt.index = hindex.ToString();
                //showArt.tm = item.tmarker;
            }
            return html.ToString();
        }
  
    }
}
