using mphdict.Models.morph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mphweb.Models
{
    static public class mphEntry
    {
        static private string loadTempl(string name)
        {
            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = assembly.GetManifestResourceStream($"mphweb.Resources.{name}.txt");

            using (var reader = new StreamReader(resourceStream, Encoding.UTF32))
            {
                return reader.ReadToEnd();
            }
        }
        static public HashSet<char> vowel { get; } = new HashSet<char>("АЕЄИІЇОУЮЯЁЫЭаеєиіїоуюяёыэ");
        static public bool isSetAccent(this string s)
        {
            int c = 0;
            for (int i = 0; i < s.Length; i++) {
                if (vowel.Contains(s[i])) c++;
                if (c == 2) return true;
            }
            return false;
        }
        // формування незмінної частини мови
        static private string formConst(word_param item)
        {
            string output_word = $"{item.reestr.Replace("\"", "\x301")}<sup>{((item.field2 > 0) ? (item.field2.ToString()) : string.Empty)}</sup>";
            string formstr = "<div align=\"center\"><span class=\"word_style\">" + output_word;
            formstr += "</span><span class=\"gram_style\"> - ";
            formstr += item.parts.com;    // частина мови
            formstr += "</span></div><br><div align=\"center\" class=\"comment_style\">";
            formstr += item.field5;   // коментар
            formstr += "</div><hr><div align=\"center\" class=\"gram_style\">";
            //if (langid == 1049)
            //    formstr += "незмінна словникова одиниця";
            //else
            formstr += "незмінювана словникова одиниця";
            formstr += "</div><p class=\"comm_end_style\"></p>";
            return formstr;
        }
        static public string formEntry(word_param item, int langid)
        {
            if (item == null) return null;
            string word = item.reestr.Replace("\"", "");
            string output_word= $"{item.reestr.Replace("\"", "\x301")}<sup>{((item.field2 > 0) ? (item.field2.ToString()) : string.Empty)}</sup>";
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
            //byte homon = item.field2;
            //if (homon != 0) unchangeable += (" " + homon.ToString());

            string templ = "", str = "";
            if ((item.part >= 70) || (item.part == 0))
            {
                templ = formConst(item);
            }
            else // інакше - для змінної. ч.м.:
            {
                // заповнюємо поля коментарів та викликаємо побудову
                if (langid == 1058)
                {
                    if (item.field7 != null)
                        str = item.field7.Replace("<", "&#60;").Replace(">", "&#62;");
                    str += "<br>";
                }
                if (item.field6 != null)
                    str += item.field6.Replace("<", "&#60;").Replace(">", "&#62;");
                str += "<br>"; 
                str += pcomm;
                string rdv=string.Empty;
                templ += generateTempl(item, out rdv, langid);

                templ = templ.Replace("[WORD]", output_word);
                templ = templ.Replace("[gram]", item.parts.com);
                str = str.Replace("$", rdv);
                templ = templ.Replace("*[text]", str);
                if (item.field5 != null)
                    templ = templ.Replace("[(sem comment)]", item.field5.Replace("<", "&#60;").Replace(">", "&#62;"));
                else templ = templ.Replace("[(sem comment)]", "");
            }
                return templ;
        }
        static private string generateTempl(word_param item, out string rdv, int langid=1058)
        {
            int n = 0;
            string templ="";
            switch (item.parts.gr_id)
            {
                case 1:
                case 16://іменник
                    templ += (langid == 1058) ? templ += loadTempl("ua_i") : (langid == 1049) ? templ += loadTempl("ru_i") : "";
                    n = 24; break;
                case 14:    //прізвище
                    templ += (langid == 1058) ? templ += loadTempl("ua_f1") : (langid == 1049) ? templ += loadTempl("ru_f1") : "";
                    n = 21; break;
                case 15:    //прізвище
                    templ += (langid == 1058) ? templ += loadTempl("ua_f2") : (langid == 1049) ? templ += loadTempl("ru_f2") : "";
                    n = 21; break;
                case 2: //прізвище
                    templ += (langid == 1058) ? templ += loadTempl("ua_f") : (langid == 1049) ? templ += loadTempl("ru_f") : "";
                    n = 21; break;
                case 3://прикм.
                    templ += (langid == 1058) ? templ += loadTempl("ua_p") : (langid == 1049) ? templ += loadTempl("ru_p") : "";
                    n = 34; break;
                case 4://займ.
                    templ += (langid == 1058) ? templ += loadTempl("ua_z") : (langid == 1049) ? templ += loadTempl("ru_z") : "";
                    n = 16; break;
                case 5: //прикм. займ.
                    templ += (langid == 1058) ? templ += loadTempl("ua_zp") : (langid == 1049) ? templ += loadTempl("ru_zp") : "";
                    n = 34; break;
                case 6: //diesl nedok.
                    templ += (langid == 1058) ? templ += loadTempl("ua_vn") : (langid == 1049) ? templ += loadTempl("ru_vn") : "";
                    n = 49; break;
                case 7://дієприкм
                    templ += (langid == 1058) ? templ += loadTempl("ua_d") : (langid == 1049) ? templ += loadTempl("ru_d") : "";
                    n = 34; break;
                case 8: //дієсл. док.
                    templ += (langid == 1058) ? templ += loadTempl("ua_vd") : (langid == 1049) ? templ += loadTempl("ru_vd") : "";
                    n = 49; break;
                case 9: //кільк. числ.
                    templ += (langid == 1058) ? templ += loadTempl("ua_ck") : (langid == 1049) ? templ += loadTempl("ru_ck") : "";
                    n = 16; break;
                case 10:    //пор. числ.
                    templ += (langid == 1058) ? templ += loadTempl("ua_cp") : (langid == 1049) ? templ += loadTempl("ru_cp") : "";
                    n = 24; break;
                case 11:    //числ.1
                    templ += (langid == 1058) ? templ += loadTempl("ua_c1") : (langid == 1049) ? templ += loadTempl("ru_c1") : "";
                    n = 24; break;
                case 12:    //числ.2
                    templ += (langid == 1058) ? templ += loadTempl("ua_c2") : (langid == 1049) ? templ += loadTempl("ru_c2") : "";
                    n = 24; break;
                case 13:    //дієсл. недок. док.
                    templ += (langid == 1058) ? templ += loadTempl("ua_vnd") : (langid == 1049) ? templ += loadTempl("ru_vnd") : "";
                    n = 49; break;
                default: break;
            }
           rdv = formInflexion(ref templ, item, langid);
            templ = templ.Replace("#", "");

            // прибираємо всі словоформи, якіх у цього слова відсутні:
            for (int i = n; i >= 1; i--)
                templ = templ.Replace("Field__" + i.ToString() + "_", "&nbsp;");
            return templ;
        }
        static private string formInflexion(ref string templ, word_param item, int langid)
        {
            string unchangeable;
            string rdv = unchangeable = string.Empty;
            sbyte[] maccs = new sbyte[7];
            int[] ac = new int[4], ac_fl = new int[4];
            ac[0] = ac[1] = ac[2] = ac[3] = 0;

            // заповнюємо позиції наголосів для вихідної форми:
            for (int j = 0; j < 4; j++)
            {
                if (j == 0) ac[0] = item.reestr.IndexOf("\"");
                else ac[j] = item.reestr.IndexOf("\"", ac[j - 1] + 1);
                if (ac[j] == -1) { ac[j] = 0; break; }
            }

            string original_form = item.reestr.Replace("\"", "");
            for (int j = 0; j < 4; j++) ac_fl[j] = ac[j];
            // якщо наголосу в слові немає - шукаємо голосну
            if (ac[0] == 0)
            {
                for (int j = 0; j < original_form.Length; j++)
                {
                    if (vowel.Contains(original_form[j]))
                    {
                        ac[0] = j + 1; break;
                    }
                }
            }
            // отримуємо основу original_form та незм. частину флексії unchangeable:
            //if ((item.indents.field3 != 0) && (item.indents.field4 != 0))
            //    unchangeable = original_form.Substring(original_form.Length - (int)item.indents.indent + (int)item.indents.field3 - 1, (int)item.indents.field4);
            //else unchangeable = "";
            unchangeable = "";
            original_form = original_form.Substring(0, original_form.Length - (int)item.indents.indent);

            int i = 0;
            string tmp, current_wform;
            while (i < item.indents.flexes.Count)  // цикл по флексіях:
            {
                var frow = item.indents.flexes[i];   // поточний рядок табл. флексій
                int nflex = (int)frow.field2, flx = nflex;
                tmp = "";
                if (item.parts.gr_id == 8)  // для доконаних дієслів
                {
                    if (langid == 1058)
                        switch (flx)
                        {
                            case 5: flx = 11; break;
                            case 6: flx = 12; break;
                            case 7: flx = 13; break;
                            case 8: flx = 14; break;
                            case 9: flx = 15; break;
                            case 10: flx = 16; break;
                            case 11: flx = 19; break;
                            case 12: flx = 20; break;
                            case 13: flx = 21; break;
                            case 14: flx = 22; break;
                            case 15: flx = 23; break;
                            case 16: flx = 24; break;
                            case 17: flx = 25; break;
                            case 18: flx = 26; break;
                        }
                    if (langid == 1049)
                        switch (flx)
                        {
                            case 14: flx = 15; break;
                            case 15: flx = 18; break;
                            case 16: flx = 19; break;
                            case 18: flx = 28; break;
                            case 19: flx = 29; break;
                            case 20: flx = 30; break;
                            case 21: flx = 31; break;
                            case 22: flx = 32; break;
                            case 23: flx = 33; break;
                            case 24: flx = 34; break;
                            case 25: flx = 35; break;
                            case 26: flx = 36; break;
                            case 27: flx = 37; break;
                            case 28: flx = 38; break;
                            case 29: flx = 39; break;
                            case 30: flx = 41; break;
                            case 31: flx = 43; break;
                        }
                }
                int ia = 0;
                while (frow.field2 == nflex) // цикл по грам. категорії:
                {
                    current_wform = original_form;
                    if (!string.IsNullOrEmpty(frow.flex)) current_wform += frow.flex;
                    //current_wform = current_wform.Replace("#", unchangeable);    // поточна словоформа
                    if (current_wform.isSetAccent())
                    {
                        // обробляємо наголоси
                        for (int j = 0; j < 4; j++) ac_fl[j] = ac[j];
                        if ((item.accent != null) && (item.accent != 0))
                        {
                            // шукаємо клас по таблиці та додаємо значення зсувів до початковим позиціям
                            accent[] arow = item.accents_class.accents.Where(c => c.gram == flx).OrderBy(c => c.gram).ThenBy(c => c.id).ToArray();
                            if (arow.Length > 0)
                            {
                                if (tmp != "") ia++;
                                if (ia == arow.Length) ia--;
                                if (arow[ia].indent1 != null)
                                {
                                    ac_fl[0] += (short)arow[ia].indent1;
                                    if ((short)arow[ia].indent1 == 255) ac_fl[0] = 0;
                                }
                                if (arow[ia].indent2 != null)
                                {
                                    if (ac_fl[1] != 0) ac_fl[1] += (short)arow[ia].indent2;
                                    else ac_fl[1] = ac[0] + (short)arow[ia].indent2 + 1;
                                    if ((short)arow[ia].indent2 == 255) ac_fl[1] = 0;
                                }
                                if (arow[ia].indent3 != null)
                                {
                                    if (ac_fl[2] != 0) ac_fl[2] += (short)arow[ia].indent3;
                                    else if (ac[1] != 0) ac_fl[2] = ac[1] + (short)arow[ia].indent3 + 1;
                                    else ac_fl[2] = ac[0] + (short)arow[ia].indent3 + 2;
                                    if ((short)arow[ia].indent3 == 255) ac_fl[2] = 0;
                                }
                                if (arow[ia].indent4 != null)
                                {
                                    if (ac_fl[3] != 0) ac_fl[3] += (short)arow[ia].indent4;
                                    else if (ac[2] != 0) ac_fl[3] = ac[2] + (short)arow[ia].indent4 + 1;
                                    else ac_fl[3] = ac[1] + (short)arow[ia].indent4 + 2;
                                    if ((short)arow[ia].indent4 == 255) ac_fl[3] = 0;
                                }
                            }
                        }
                        // вставляємо наголос з урахуванням отриманих позицій
                        if (item.accent != null)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if ((ac_fl[j] > 0) && (ac_fl[j] <= current_wform.Length))
                                    current_wform = current_wform.Insert(ac_fl[j], "\x301");
                                //else 
                                //if ((ac_fl[j] != 0) && (ac_fl[j] != ac[j]))
                                //    err = true;
                            }
                        }
                    }
                    // в потрібних словоформах додаємо прийм.
                    if (langid == 1058)
                    {
                        if (tmp == "")
                        {
                            switch (item.parts.gr_id)
                            {
                                case 1:
                                case 16://імен.
                                    if ((nflex == 6) || (nflex == 13))
                                        if (!vowel.Contains(current_wform[0])) current_wform = "на/у " + current_wform;
                                        else current_wform = "на/в " + current_wform;
                                    break;
                                case 3: 
                                case 4:
                                case 5:
                                case 7:
                                case 9:
                                case 10:
                                case 11:
                                case 12:
                                    if ((nflex == 6) || (nflex == 12) || (nflex == 18) || (nflex == 24))
                                        if (!vowel.Contains(current_wform[0])) current_wform = "на/у " + current_wform;
                                        else current_wform = "на/в " + current_wform;
                                    break;
                                case 2:
                                case 14:
                                case 15: // прізв.
                                    if ((nflex == 6) || (nflex == 13) || (nflex == 20) || (nflex == 27))
                                        current_wform = "при " + current_wform;
                                    break;
                            }
                        }
                        // обробляємо спец. позначки словоформ
                        if (current_wform.IndexOf("%") != -1)
                        {
                            current_wform = "по " + current_wform; current_wform = current_wform.Replace("%", "");
                        }
                        if (current_wform.IndexOf("$") != -1)
                        {
                            current_wform = "на " + current_wform; current_wform = current_wform.Replace("$", "");
                        }
                        if (current_wform.IndexOf("@") != -1)
                        {
                            current_wform = "до " + current_wform; current_wform = current_wform.Replace("@", "");
                        }
                        if (current_wform.IndexOf("&") != -1)
                        {
                            if (!vowel.Contains(current_wform[0])) current_wform = "у " + current_wform;
                            else current_wform = "в " + current_wform;
                            current_wform = current_wform.Replace("&", "");
                        }
                    }

                    if (current_wform.IndexOf("^") == -1)
                    {
                        //if (trscr)  // якщо транскрипція
                        //{
                        //    if (maccrow != null)
                        //    {
                        //        if (maccrow.occur1 != null) maccs[0] = (sbyte)maccrow.occur1;
                        //        if (maccrow.occur2 != null) maccs[1] = (sbyte)maccrow.occur2;
                        //        if (maccrow.occur3 != null) maccs[2] = (sbyte)maccrow.occur3;
                        //        if (maccrow.double1 != null) maccs[3] = (sbyte)maccrow.double1;
                        //        if (maccrow.double2 != null) maccs[4] = (sbyte)maccrow.double2;
                        //    }
                        //    else maccs = null;
                        //    utranscr.Transcribe(current_wform, out transcr, maccs);
                        //    tmp += (", " + transcr);
                        //    if (nflex == 2) rdv += (", " + current_wform);
                        //}
                        //else
                        tmp += (", " + current_wform);
                    }
                    if (tmp.StartsWith(", ")) tmp = tmp.Remove(0, 2);
                    // змінна для ком. до род. відмінку
                    if (rdv.StartsWith(", ")) rdv = rdv.Remove(0, 2);
                    i++;
                    if (i >= item.indents.flexes.Count) break;
                    else frow = item.indents.flexes[i];
                }
                // підставляємо в шаблон отриману словоформу
                templ = templ.Replace("Field__" + nflex.ToString() + "_", tmp);
                if ((nflex == 2) /*&& (!trscr)*/) rdv = tmp;
            }
            return rdv;
        }

    }
}
