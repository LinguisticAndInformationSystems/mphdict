using mphdict.Models.morph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphweb.Models
{
    public class mphEntry
    {
        // формування незмінної частини мови
        private string formConst(word_param item)
        {
            string formstr = "<div align=\"center\"><span class=\"word_style\">" + item.reestr.Replace("\"", "\x0301");
            if (item.field2 != 0) formstr += (" " + item.field2.ToString());
            formstr += "</span><span class=\"gram_style\"> - ";
            formstr += item.parts.com;    // частина мови
            formstr += "</span></div><br><div align=\"center\" class=\"comment_style\">";
            formstr += item.field5;   // коментар
            formstr += "</div><hr><div align=\"center\" class=\"gram_style\">";
            //if (langid == 1049)
            //    formstr += "неизменяемая словарная единица";
            //else
            formstr += "незмінювана словникова одиниця";
            formstr += "</div><p class=\"comm_end_style\"></p>";
            return formstr;
        }
        public string formEntry(word_param item)
        {
            if (item == null) return null;
            string word = item.reestr.Replace("\"", "");
            string id = item.nom_old.ToString();
            
            string pcomm = "";
            string unchangeable;
            if (item.type != 0) 
            {
                try
                {   // побудова початкової форми
                    unchangeable = word.Substring(0, word.Length - item.indents.indent);
                    //if ((item.indents.field3 != 0) && (item.indents.field4 != 0))
                    //    invar = word.Substring(unchangeable.Length + (int)item.indents.field3 - 1, (int)item.indents.field4);
                    //else invar = "";
                    unchangeable += item.indents.flexes.First().flex;
                    //unchangeable = unchangeable.Replace("#", invar);
                }
                catch
                {
                    unchangeable = word;
                    return null;
                }

                if (item.indents.comment != null) pcomm = item.indents.comment.Replace("<", "&#3C;").Replace(">", "&#3E;");
            }
            else unchangeable = word;	

                // додаємо індекс омонімії
            byte homon = item.field2;
            if (homon != 0) unchangeable += (" " + homon.ToString());

            string templ = "", str = "";
            if ((item.part >= 70) || (item.part == 0))
            {
                templ = formConst(item);
            }
            else // иначе - для изм. ч.р.:
            {
                // заповнюємо поля коментарів та викликаємо побудову
                if (item.field7 != null)
                    str = item.field7.Replace("<", "&#60;").Replace(">", "&#62;");
                str += "<br>";
                if (item.field6 != null)
                    str += item.field6.Replace("<", "&#60;").Replace(">", "&#62;");
                str += "<br>"; 
                str += pcomm;
                // ------------------templ += phverbHtml(item);

                templ = templ.Replace("[WORD]", unchangeable);
                templ = templ.Replace("[gram]", item.parts.com);
                // ------------str = str.Replace("$", rodv);
                templ = templ.Replace("*[text]", str);
                if (item.field5 != null)
                    templ = templ.Replace("[(sem comment)]", item.field5.Replace("<", "&#60;").Replace(">", "&#62;"));
                else templ = templ.Replace("[(sem comment)]", "");
            }
                return templ;
        }
        private string generateTempl(word_param item)
        {
            int langid = 1058;
            int n = 0;
            string templ="";
            switch (item.parts.gr_id)
            {
                case 1:
                case 16://іменник
                    if (langid == 1058) templ += Resources.infltab.ua_i;
                    if (langid == 1049) templ += Resources.infltab.ru_i;
                    n = 24; break;
                case 14:    //прізвище
                    if (langid == 1058) templ += Resources.infltab.ua_f1;
                    if (langid == 1049) templ += Resources.infltab.ru_f1;
                    n = 21; break;
                case 15:    //прізвище
                    if (langid == 1058) templ += Resources.infltab.ua_f2;
                    if (langid == 1049) templ += Resources.infltab.ru_f2;
                    n = 21; break;
                case 2: //прізвище
                    if (langid == 1058) templ += Resources.infltab.ua_f;
                    if (langid == 1049) templ += Resources.infltab.ru_f;
                    n = 21; break;
                case 3://прикм.
                    if (langid == 1058) templ += Resources.infltab.ua_p;
                    if (langid == 1049) templ += Resources.infltab.ru_p;
                    n = 34; break;
                case 4://займ.
                    if (langid == 1058) templ += Resources.infltab.ua_z;
                    if (langid == 1049) templ += Resources.infltab.ru_z;
                    n = 16; break;
                case 5: //прикм. займ.
                    if (langid == 1058) templ += Resources.infltab.ua_zp;
                    if (langid == 1049) templ += Resources.infltab.ru_zp;
                    n = 34; break;
                case 6: //diesl nedok.
                    if (langid == 1058) templ += Resources.infltab.ua_vn;
                    if (langid == 1049) templ += Resources.infltab.ru_vn;
                    n = 49; break;
                case 7://дієприкм
                    if (langid == 1058) templ += Resources.infltab.ua_d;
                    if (langid == 1049) templ += Resources.infltab.ru_d;
                    n = 34; break;
                case 8: //дієсл. док.
                    if (langid == 1058) templ += Resources.infltab.ua_vd;
                    if (langid == 1049) templ += Resources.infltab.ru_vd;
                    n = 49; break;
                case 9: //кільк. числ.
                    if (langid == 1058) templ += Resources.infltab.ua_ck;
                    if (langid == 1049) templ += Resources.infltab.ru_ck;
                    n = 16; break;
                case 10:    //пор. числ.
                    if (langid == 1058) templ += Resources.infltab.ua_cp;
                    if (langid == 1049) templ += Resources.infltab.ru_cp;
                    n = 24; break;
                case 12:    //числ.2
                    if (langid == 1058) templ += Resources.infltab.ua_c2;
                    if (langid == 1049) templ += Resources.infltab.ru_c2;
                    n = 24; break;
                case 11:    //числ.1
                    if (langid == 1058) templ += Resources.infltab.ua_c1;
                    if (langid == 1049) templ += Resources.infltab.ru_c1;
                    n = 24; break;
                case 13:    //дієсл. недок. док.
                    if (langid == 1058) templ += Resources.infltab.ua_vnd;
                    if (langid == 1049) templ += Resources.infltab.ru_vnd;
                    n = 49; break;
                default: break;
            }
            //------------bool err = formInflexion(ref blank, part, trscr, gramData);
            templ = templ.Replace("#", "");

            // убираем все словоформы, которых у данного слова нет:
            for (int i = n; i >= 1; i--)
                templ = templ.Replace("Field__" + i.ToString() + "_", "&nbsp;");
            return templ;
        }

    }
}
