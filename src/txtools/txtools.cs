// Copyright © 2016 uSofTrod. Contacts: <uSofTrod@outlook.com>
// License: http://opensource.org/licenses/MIT
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace uSofTrod.TextUtility.txToolsCore
{
    #region Lang
    public class Lang: IComparable<Lang>
    {
        protected HashSet<char> alphabet_small;
        protected HashSet<char> alphabet_big;
        protected bool IsBigEnabled;
        public int LangID;
        protected int small_code=1002;
        protected int big_code=1001;
        public Lang(){}
        public Lang(char[] alphabet_small, char[] alphabet_big, int LangID, bool IsBigEnabled)
        {
            this.alphabet_big=new HashSet<char>(alphabet_big);
            this.alphabet_small=new HashSet<char>(alphabet_small);
            this.LangID=LangID;
            this.IsBigEnabled=IsBigEnabled;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns>type of symbol</returns>
        public int IsAlpha(char c)
        {
            bool r;
            //(from ch in alphabet_small where c == ch select ch);
            r = alphabet_small.Contains(c);
            if(r==true) return small_code;
            if(IsBigEnabled) {
                r = alphabet_big.Contains(c);
                if(r==true) return big_code;
            }
            return-1;
        }
        public int isStrPartOfSet(string s)
        {
            foreach(char c in s)
            {
                if (IsAlpha(c) < 0) return -1;
            }
            return 1;
        }

        #region IComparable<Lang> Members

        public int CompareTo(Lang other)
        {
 	        if(LangID<other.LangID) return -1;
            if(LangID>other.LangID) return 1;
            return 0;
        }

        #endregion
}
    public class SysLang: Lang
    {
        public SysLang(HashSet<char> alphabet, int LangID, int code)
        {
            small_code=code;
            this.alphabet_small=alphabet;
            this.LangID=LangID;
            this.IsBigEnabled=false;
        }
    }
    public class Langs: IList<Lang>
    {
        public List<Lang> langs;
        public Langs(){
            langs=new List<Lang>();
        }
        public Langs(params Lang[] lgs) {
            langs=new List<Lang>();
            foreach(Lang l in lgs) {
                langs.Add(l);
            }
        }
        //type of symbol
        public List<typeSymbol> IsAlpha(char c)
        {
            List<typeSymbol> ts=new List<typeSymbol>();
            int r;
            foreach(Lang l in langs) {
                r=l.IsAlpha(c);
                if(r>=0){
                    typeSymbol t=new typeSymbol(l.LangID, r);
                    ts.Add(t);
                }
            }
            return ts;
        }
    
        #region IList<Lang> Members

        public int  IndexOf(Lang item)
        {
 	        return langs.IndexOf(item);
        }

        public void Insert(int index, Lang item)
        {
 	        langs.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
 	        langs.RemoveAt(index);
        }

        public Lang  this[int index]
        {
	          get 
	        { 
		        return langs[index];
	        }
	          set 
	        { 
                  langs[index]=value;
	        }
        }

        #endregion

        #region ICollection<Lang> Members

        public void  Add(Lang item)
        {
 	        langs.Add(item);
        }

        public void  Clear()
        {
 	        langs.Clear();
        }

        public bool  Contains(Lang item)
        {
 	        return langs.Contains(item);
        }

        public void  CopyTo(Lang[] array, int arrayIndex)
        {
 	        langs.CopyTo(array,arrayIndex);
        }

        public int  Count
        {
	        get {return langs.Count();}
        }

        public bool  IsReadOnly
        {
	        get{ return false;}
        }

        public bool  Remove(Lang item)
        {
 	        return langs.Remove(item);
        }

        #endregion

        #region IEnumerable<Lang> Members

        public IEnumerator<Lang>  GetEnumerator()
        {
            for (int i = 0; i < Count; i++) //
            {
                yield return this[i];
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator  System.Collections.IEnumerable.GetEnumerator()
        {
 	        return GetEnumerator();
        }

        #endregion
    }
    public struct typeSymbol: IComparable<typeSymbol>
    {
        public typeSymbol(int langid, int type)
        {
            this.langid=langid;
            this.type=type;
        }
        public int langid;
        public int type;
    
        #region IComparable<typeSymbol> Members

        public int  CompareTo(typeSymbol other)
        {
 	        if(langid<other.langid) return -1;
            if(langid>other.langid) return 1;
            return 0;
        }

        #endregion
    }
    public struct typeWord
    {
        public typeWord(int langid, int type, int flag)
        {
            this.langid = langid;
            this.type = type;
            this.flag = flag;
        }
        public int langid;
        public int type;
        public int flag;

    }

    #endregion

    struct Wrd: IDisposable
    {
        private bool isdisp;
        public Wrd(ICharProvider cp, typeOfSymbol stype_func)
        {
            isdisp = false;
            word = new StringBuilder();
            ts = null;
            this.chProvider = cp;
            this.stype_func = stype_func;
            IsLangSymbol=default(bool);
            IsComplexWord = false;
            IsUnknownLang = false;
            IsCombiningDiacrMarks = false;
        }
        private typeOfSymbol stype_func;
        private bool IsLangSymbol, IsComplexWord, IsUnknownLang, IsCombiningDiacrMarks;
        private List<typeSymbol> ts;
        private StringBuilder word;
        public override string  ToString() { return word.ToString(); }
        public List<typeWord> typeWord
        {
            get
            {
                List<typeWord> t = new List<typeWord>();
                foreach (typeSymbol tp in ts)
                {
                    if (tp.langid >= 0) //слово
                    {
                        int flag = 0;
                        if (IsComplexWord == true)
                            flag = 1;
                        if(IsCombiningDiacrMarks == true)
                            flag=flag|2;
                        t.Add(new typeWord(tp.langid, tp.type, flag));
                    }
                    else
                    {
                        if ((tp.type > 10) && (tp.type < 100)) t.Add(new typeWord(-1, tp.type, 0));
                        switch (tp.type)
                        {
                            case 1003: // цифра
                                t.Add(new typeWord(-1, 6, 0));
                                break;
                            case 1004: // punctuation mark
                                t.Add(new typeWord(-1, 3, 0));
                                break;
                            case 1005: // delimiter
                                t.Add(new typeWord(-1, 105, 0));
                                break;
                            case 1006: // \t
                                t.Add(new typeWord(-1, 101, 0));
                                break;
                            case 1007: // \n
                                t.Add(new typeWord(-1, 100, 0));
                                break;
                            case 1008: // керуючий символ
                                t.Add(new typeWord(-1, 111, 0));
                                break;
                            case 1009: // кінець речення
                                t.Add(new typeWord(-1, 5, 0));
                                break;
                            case 1010: // пробіл
                                t.Add(new typeWord(-1, 102, 0));
                                break;
                            default:
                                t.Add(new typeWord(-1, -1, 0));
                                break;
                        }
                    }
                }
                return t;
            }
        }
        private ICharProvider chProvider;
        // перегляд типів мов на співпадіння з новим символом
        private bool matchLangs(List<typeSymbol> type)
        {
            try
            {
                List<typeSymbol> t = new List<typeSymbol>();
                typeSymbol p;
                for (int i = 0; i < ts.Count; i++)
                { // перегляд типів мов на співпадіння з новим символом
                    int id = ts[i].langid;
                    try { p = (from s in type where s.langid == id select s).First(); }
                    catch { continue; }
                    t.Add(p);
                }
                if (t.Count > 0)
                { // якщо мови до яких належить символ є - додаємо символ до слова
                    ts = t;
                    return true;
                }
            }
            catch { }
            return false;
        }
        private bool skipSymbolType(HashSet<char> range, ref int startIndex)
        {
            bool IsEndOfStream = false;
            char symbol;
            for(;;) {
                symbol = chProvider[startIndex];
                if (symbol == '\0') { IsEndOfStream = true; break; }
                if(range.Contains(symbol)) {
                    startIndex++;
                }
                else break;
            }
            return IsEndOfStream;
        }
        public bool addSymbol(char c, List<typeSymbol> type, bool IsLang)
        {
            try
            {
                char symbol;
                bool islang;
                List<typeSymbol> t;
                if (c == '\0') return false;
                if (word.Length == 0) // якщо перший символ в рядку
                { 
                    if (type[0].langid == 0) IsUnknownLang = true;
                    ts = type;
                    word.Append(c);
                    IsLangSymbol = IsLang;
                    return true;
                }
                if (IsLangSymbol) //виділене слово є словом якоїсь мови
                {
                    switch (c)
                    {
                        // якщо '\' та слово, перевіряємо наступний символ (він повинен належати теж до якоїсь з мов ts)
                        case '\'':
                        case '-':
                            symbol = chProvider[1];
                            if (symbol == '\0') { return false; }
                            t = stype_func(symbol, out islang);
                            if (matchLangs(t))
                            {
                                word.Append(c);
                                if (c == '-') IsComplexWord = true;
                                return true;
                            }
                            else return false;
                        default:
                            //Combining Diacritical Marks   U+0300 – U+036F
                            if ((c >= xparse.ranges[0].start) && (c <= xparse.ranges[0].end)) {
                                symbol = chProvider[1];
                                if (symbol == '\0') { return false; }
                                t = stype_func(symbol, out islang);
                                if (matchLangs(t))
                                {
                                    word.Append(c);
                                    IsCombiningDiacrMarks = true;
                                    return true;
                                }
                                else return false;
                            }
                            break;
                    }
                }
                if (IsLangSymbol != IsLang) return false; // типи не співпадають
                if ((IsLangSymbol) && (IsLang)) // поточний символ та виділене слово належать мові
                { 
                    // слово містить символи невизначеної мови або поточний символ належить до невизн.
                    // мови або поточний символ входить до однієї з мов, до яких належить слово
                    if ((type[0].langid == 0) || (IsUnknownLang) || (matchLangs(type))) { 
                        word.Append(c);
                        if ((type[0].langid == 0) && (!IsUnknownLang)) {
                            IsUnknownLang = true; 
                            ts = new List<typeSymbol>(new typeSymbol[] { new typeSymbol(0, 2) }); 
                        }
                        return true; 
                    }
                    else return false;
                }
                else // поточний символ та виділене слово не належать мові
                {
                    if (type[0].type == ts[0].type)
                    {
                        word.Append(c);
                        return true;
                    }
                    else {
                        if (ts[0].type == 1009) // слово - символи кінця речення, а поточний символ не належить до символів кінця речення
                        {
                            if (!IsEndSent()) {
                                ts = new List<typeSymbol>(new typeSymbol[] { new typeSymbol(-1, 1005) }); 
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }
        bool IsEndSent() 
        {
            int ptr = 0;
            bool fl;
            char symbol;
            fl = skipSymbolType(xparse.delimiters, ref ptr);
            if (fl==false) //((c >= 2000) && (c <= 206f))
            {// пропускаємо всі символи delimiters
                symbol = chProvider[ptr];
                if ((symbol == '\n') || (symbol == '\r') || (symbol == '\t'))
                {
                    return true;
                }
                else
                {
                    List<typeSymbol> t = stype_func(symbol, out fl);
                    typeSymbol r;
                    try
                    {
                        r = t.First(s => s.type == 1001); // велика літера
                        return true;
                    }
                    catch { return false; }
                }
            }
            else 
                return true;
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (!isdisp)
            {
                isdisp=true;
                chProvider = null;
                stype_func = null;
            }
        }

        #endregion
    }
    delegate List<typeSymbol> typeOfSymbol(char c, out bool IsLangSymbol);

    public class xparse : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            langs=null;
            if (chProvider != null) {
                chProvider.Dispose();
                chProvider = null;
            }
        }

        #endregion

        private ICharProvider chProvider;
        
        #region символи та регіони
        static readonly public HashSet<char> delimiters = new HashSet<char>(new char[] { '•', ',', '.', '…', ';', ' ', '\x00A0', '\'', ')', '(', '!', '?', '\x2019', '\x2018', '\x2032', '\x201B', '\x2035', '\x0060', '\x00B4', '\x02B9', '\x02bb', '\x02bc', '\x02bd', '\x02ca', '\x02cb', '\x0343', '\x0374', '\x0384', '\x1fbd', '\x1fbf', '\x1fef', '\x1ffe', '\x0559', '\x055a', '\x055b', '\x055d', '\x05f3', '\x0954', '-', ':', ';', '[', '\\', ']', '{', '|', '}', '/', '\x2013', '\x2014', '\x2015', '\x2043', '\x0000', '"', '#', '$', '%', '&', '(', ')', '*', '+', '-', '.', '/', '<', '=', '>', '@', '«', '»', '„', '”', '“', '_', '~' });
        static readonly public HashSet<char> numerals = new HashSet<char>(new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' });

        static readonly public HashSet<char> quotation_mark = new HashSet<char>(new char[]{ '\'', '\x2019', '\x2018', '\x2032', '\x201B', '\x2035', '\x0060', '\x00B4', '\x02B9', '\x02bb', '\x02bc', '\x02bd', '\x02ca', '\x02cb', '\x0343', '\x0374', '\x0384', '\x1fbd', '\x1fbf', '\x1fef', '\x1ffe', '\x0559', '\x055a', '\x055b', '\x055d', '\x05f3', '\x0954' });
        static readonly public HashSet<char> punctuation_mark = new HashSet<char>(new char[]{ ',', '.', ';', '-', ':', '!', '?', '\x2013', '\x2014', '\x2015', '–', '…' });
        static readonly public HashSet<char> dot_sentence = new HashSet<char>(new char[]{ '.', '!', '?', '\x2026', '\x22EF', ':', ';', '…' });
        //static readonly public char[] ruling_symbol = { '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\x0007', '\x0008', '\x0009', '\x000A', '\x000B', '\x000C', '\x000D', '\x000E', '\x000F', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x0020', '\x001A' };
        
        static readonly public Lang Ukrainian = new Lang(new char[] {'а','б','в','г','ґ','д','е','є','ж','з','и','і','ї','й','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ю','я','ь'}, new char[]{'А','Б','В','Г','Ґ','Д','Е','Є','Ж','З','И','І','Ї','Й','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ю','Я','Ь'},1058,true);
        static readonly public Lang Russian = new Lang(new char[] {'а','б','в','г','д','е','ё','ж','з','и','ы','й','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','э','ю','я','ь','ъ'}, new char[]{'А','Б','В','Г','Д','Е','Ё','Ж','З','И','Ы','Й','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ю','Э','Я','Ь','Ъ'},1049,true);
        static readonly public Lang Cyrillic = new Lang(new char[] { 'а', 'б', 'в', 'г', 'ґ', 'д', 'е', 'є', 'ё', 'ж', 'з', 'и', 'і', 'ї', 'ы', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'э', 'ю', 'я', 'ь', 'ъ', 'ѣ', 'ѳ', 'ѱ', 'ѯ', 'ѭ', 'ѫ', 'ѩ', 'ѧ', 'ѥ', 'ѡ', 'ѵ', 'ѷ', 'ѹ', 'ѻ', 'ѽ', 'ѿ', 'ҁ', 'ғ', 'ҕ', 'җ', 'ҙ', 'қ', 'ҝ', 'ҟ', 'ҡ', 'ң', 'ҥ', 'ҧ', 'ҩ', 'ҫ', 'ҭ', 'ү', 'ұ', 'ҳ', 'ҵ', 'ҷ', 'ҹ', 'һ', 'ҽ', 'ҿ', 'Ӏ', 'ӂ', 'ӄ', 'ӈ', 'ӌ', 'ӑ', 'ӓ', 'ӕ', 'ӗ', 'ә', 'ӛ', 'ӝ', 'ӟ', 'ӡ', 'ӣ', 'ӥ', 'ӧ', 'ө', 'ӫ', 'ӯ', 'ӱ', 'ӳ', 'ӵ', 'ӹ', 'ђ', 'ѓ', 'ѕ', 'ј', 'љ', 'њ', 'ћ', 'ќ', 'ў', 'џ' }, new char[] { 'А', 'Б', 'В', 'Г', 'Ґ', 'Д', 'Е', 'Є', 'Ё', 'Ж', 'З', 'И', 'І', 'Ї', 'Ы', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ю', 'Э', 'Я', 'Ь', 'Ъ', 'Ђ', 'Ѓ', 'Ѕ', 'Ј', 'Љ', 'Њ', 'Ћ', 'Ќ', 'Ў', 'Џ', 'Ѡ', 'Ѣ', 'Ѥ', 'Ѧ', 'Ѩ', 'Ѫ', 'Ѭ', 'Ѯ', 'Ѱ', 'Ѳ', 'Ѵ', 'Ѷ', 'Ѹ', 'Ѻ', 'Ѽ', 'Ѿ', 'Ҁ', 'Ғ', 'Ҕ', 'Җ', 'Ҙ', 'Қ', 'Ҝ', 'Ҟ', 'Ҡ', 'Ң', 'Ҥ', 'Ҧ', 'Ҩ', 'Ҫ', 'Ҭ', 'Ү', 'Ұ', 'Ҳ', 'Ҵ', 'Ҷ', 'Ҹ', 'Һ', 'Ҽ', 'Ҿ', 'Ӂ', 'Ӄ', 'Ӈ', 'Ӌ', 'Ӑ', 'Ӓ', 'Ӕ', 'Ӗ', 'Ә', 'Ӛ', 'Ӝ', 'Ӟ', 'Ӡ', 'Ӣ', 'Ӥ', 'Ӧ', 'Ө', 'Ӫ', 'Ӯ', 'Ӱ', 'Ӳ', 'Ӵ', 'Ӹ' }, 1251, true);
        static readonly public Lang Latin = new Lang(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'à', 'á', 'â', 'ã', 'ä', 'å', 'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í', 'î', 'ï', 'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö', 'ø', 'ù', 'ú', 'û', 'ü', 'ý', 'þ', 'ÿ', 'ā', 'ă', 'ą', 'ć', 'ĉ', 'ċ', 'č', 'ď', 'đ', 'ē', 'ĕ', 'ė', 'ę', 'ě', 'ĝ', 'ğ', 'ġ', 'ģ', 'ĥ', 'ħ', 'ĩ', 'ī', 'ĭ', 'į', 'ı', 'ĳ', 'ĵ', 'ķ', 'ĸ', 'ĺ', 'ļ', 'ľ', 'ŀ', 'ł', 'ń', 'ņ', 'ň', 'ŉ', 'ŋ', 'ō', 'ŏ', 'ő', 'œ', 'ŕ', 'ŗ', 'ř', 'ś', 'ŝ', 'ş', 'š', 'ţ', 'ť', 'ŧ', 'ũ', 'ū', 'ŭ', 'ů', 'ű', 'ų', 'ŵ', 'ŷ', 'ź', 'ż', 'ž' }, new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï', 'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ø', 'Ù', 'Ú', 'Û', 'Ü', 'Ý', 'Þ', 'ß', 'Ā', 'Ă', 'Ą', 'Ć', 'Ĉ', 'Ċ', 'Č', 'Ď', 'Đ', 'Ē', 'Ĕ', 'Ė', 'Ę', 'Ě', 'Ĝ', 'Ğ', 'Ġ', 'Ģ', 'Ĥ', 'Ħ', 'Ĩ', 'Ī', 'Ĭ', 'Į', 'İ', 'Ĳ', 'Ĵ', 'Ķ', 'Ĺ', 'Ļ', 'Ľ', 'Ŀ', 'Ł', 'Ń', 'Ņ', 'Ň', 'Ŋ', 'Ō', 'Ŏ', 'Ő', 'Œ', 'Ŕ', 'Ŗ', 'Ř', 'Ś', 'Ŝ', 'Ş', 'Š', 'Ţ', 'Ť', 'Ŧ', 'Ũ', 'Ū', 'Ŭ', 'Ů', 'Ű', 'Ų', 'Ŵ', 'Ŷ', 'Ÿ', 'Ź', 'Ż', 'Ž' }, 1252, true);
       public struct range
        {
            public range(char s, char e) { start = s; end = e; }
            public char start;
            public char end;
        }
        static readonly public range[] ranges = { new range ( '\x0300', '\x036f' ),   //Combining Diacritical Marks   U+0300 – U+036F
            new range ('\x2070', '\x209f'),	//Superscripts and Subscripts   U+2070 – U+209F   (8304–8351)
            new range ('\x20a0', '\x20cf'),	//Currency Symbols   U+20A0 – U+20CF   (8352–8399)
            new range ('\x20d0','\x20ff'),	//Combining Diacritical Marks for Symbols   U+20D0 – U+20FF   (8400–8447)
            new range ('\x2100','\x214f'),	//Letterlike Symbols   U+2100 – U+214F   (8448–8527)
            new range ('\x2150','\x218f'),	//Number Forms   U+2150 – U+218F   (8528–8591)
            new range ('\x2190','\x21ff'),	//Arrows   U+2190 – U+21FF   (8592–8703)
            new range ('\x2200','\x22ff'),	//Mathematical Operators   U+2200 – U+22FF   (8704–8959)
            new range ('\x2300','\x23ff'),	//Miscellaneous Technical   U+2300 – U+23FF   (8960–9215)
            new range ('\x2400','\x243f'),	//Control Pictures   U+2400 – U+243F   (9216–9279)
            new range ('\x2440','\x245f'),	//Optical Character Recognition   U+2440 – U+245F   (9280–9311)
            new range ('\x2460','\x24ff'),	//Enclosed Alphanumerics   U+2460 – U+24FF   (9312–9471)
            new range ('\x2500','\x257f'),	//Box Drawing   U+2500 – U+257F   (9472–9599)
            new range ('\x2580','\x259f'),	//Block Elements   U+2580 – U+259F   (9600–9631)
            new range ('\x25a0','\x25ff'),	//Geometric Shapes   U+25A0 – U+25FF   (9632–9727)
            new range ('\x2600','\x26ff'),	//Miscellaneous Symbols   U+2600 – U+26FF   (9728–9983)
            new range ('\x2700','\x27bf'),	//Dingbats   U+2700 – U+27BF   (9984–10175)
            new range ('\x2800','\x28ff'),	//Braille Patterns   U+2800 – U+28FF   (10240–10495)
            new range ('\x2e80','\x2eff'),	//CJK Radicals Supplement   U+2E80 – U+2EFF   (11904–12031)
            new range ('\x2f00','\x2fdf'),	//KangXi Radicals   U+2F00 – U+2FDF   (12032–12255)
            new range ('\x2ff0','\x2fff'),	//Ideographic Description Characters   U+2FF0 – U+2FFF   (12272–12287)
            new range ('\x3000','\x303f'),	//CJK Symbols and Punctuation   U+3000 – U+303F   (12288–12351)
            new range ('\x3200','\x9fff'),	//Enclosed CJK Letters and Months   U+3200 – U+32FF   (12800–13055)
            //CJK Compatibility   U+3300 – U+33FF   (13056–13311)
            //CJK Unified Ideographs Extension A   U+3400 – U+4DB5   (13312–19893)
            //CJK Unified Ideographs   U+4E00 – U+9FFF   (19968–40959)
            new range ('\xfb00','\xfb4f'),	//Alphabetic Presentation Forms   U+FB00 – U+FB4F   (64256–64335)
            new range ('\xfb50','\xfdff'),	//Arabic Presentation Forms-A   U+FB50 – U+FDFF   (64336–65023)
            new range ('\xfe20','\xfe2f'),	//Combining Half Marks   U+FE20 – U+FE2F   (65056–65071)
            new range ('\xfe30','\xfe4f'),	//CJK Compatibility Forms   U+FE30 – U+FE4F   (65072–65103)
            new range ('\xfe50','\xfe6f'),	//Small Form Variants   U+FE50 – U+FE6F   (65104–65135)
            new range ('\xfb50','\xfdff'),	//Arabic Presentation Forms-B   U+FE70 – U+FEFF   (65136–65279)
            new range ('\xff00','\xffef'),	//Halfwidth and Fullwidth Forms   U+FF00 – U+FFEF  
            new range ('\xfff0','\xffff'),	//Specials   U+FEFF, U+FFF0 – U+FFFF   (65279, 65520–65535)
        };
        #endregion

        #region алфавіт
        Langs langs;
        public Langs syslangs;
        #endregion

        #region constructors
        public xparse(ICharProvider charProvider, params int[] lgs)
        {
            init();
            Lang l;
            foreach(int lang in lgs) {
                switch (lang) { 
                    case 1058:
                        l = Ukrainian;
                        langs.Add(l);
                        break;
                    case 1049:
                        l=Russian;
                        langs.Add(l);
                        break;
                    case 1251:
                        l = Cyrillic;
                        langs.Add(l);
                        break;
                    case 1252:
                        l = Latin;
                        langs.Add(l);
                        break;
                    default:
                        break;
                }
            }
            chProvider = charProvider;
        }
        public xparse(ICharProvider charProvider, params Lang[] lgs)
        {
            init();
            foreach(Lang l in lgs) {
                langs.Add(l);
            }
            chProvider = charProvider;
        }
        private void init() 
        {
            syslangs=new Langs(
                new SysLang(delimiters, -1, 1005), 
                new SysLang(numerals, -2, 1003));
            langs =new Langs();
        }
        #endregion

        public void initProvider(ICharProvider charProvider)
        {
            chProvider = charProvider;
        }

            // Значення, що повертаються:
            // 0 - пусте слово (кінець тексту)
            // 1 - слово даної мови
            // 2 - слово іншої мови
            // 3 - розділовий знак
            // 4 - скорочення
            // 5 - кінець речення
            // 6 - число
            // 7 - рубрикатор
            // 8 - ініциал
            // 9 - слово містить точку/и-	не реализовано
            // 10 - слово містить Combining Diacritical Marks
            // 100 - переведення рядка
            // 101 - табуляція
            // 102 - пробіл
            // 105 - роздільник
            // 111 - керуючий символ


            // ----	Унікодівські діапазони:		---- 
            // 11 - Combining Diacr. Marks
            // 12 - Superscript & Subscript
            // 13 - Currency
            // 14 - Combining Diacr. Marks for Symbols
            // 15 - Letterlike Symbols
            // 16 - Number Forms
            // 17 - Arrows
            // 18 - Mathematical Ops
            // 19 - Miscellaneous Technical
            // 20 - Control Pictures
            // 21 - OCR
            // 22 - Enclosed Alphanumeric
            // 23 - Box Drawing
            // 24 - Block Elements
            // 25 - Geometric Shapes
            // 26 - Miscellaneous Symbols
            // 27 - Dingbats
            // 28 - Braille Patterns
            // 29 - CJK Radical Supplements
            // 30 - KangXi Radicals
            // 31 - Ideographic Description Characters
            // 32 - CJK Symbols And Punctuation
            // 33 - Other CJK
            // 34 - Alphabetic Presentation Forms
            // 35 - Arabic Presentation Forms-A
            // 36 - Combining Half Marks
            // 37 - CJK Compatibility Forms
            // 38 - Small Form Variants
            // 39 - Arabic Presentation Forms-B
            // 40 - Halfwidth & Fullwidth Forms
            // 41 - Specials

        public List<typeWord> nextWord(out string word, out bool IsNotEnd, out long position)
        {
            List<typeSymbol> ts;
            StringBuilder wrd=new StringBuilder(); 
            bool IsNotEndOfStream, IsLangSymbol;
            char symbol;
            IsNotEnd=true;
            position= chProvider.StreamPosition;
            IsNotEndOfStream = chProvider.GetNextChar(out symbol);
            if (!IsNotEndOfStream) { IsNotEnd = false; word = ""; return null; }    // кінець потоку

            ts = typeof_symbol(symbol, out IsLangSymbol);
            using (Wrd w = new Wrd(chProvider, this.typeof_symbol))
            {
                if (!w.addSymbol(symbol, ts, IsLangSymbol)) { IsNotEnd = false; word = ""; return null; }

                for (; ; )
                {
                    symbol = chProvider[0];
                    if (symbol == '\0') { IsNotEnd = false; break; }
                    ts = typeof_symbol(symbol, out IsLangSymbol);
                    if (!w.addSymbol(symbol, ts, IsLangSymbol)) { IsNotEnd = true; break; }
                    if (!chProvider.GetNextChar(out symbol))
                    {
                        IsNotEnd = false;
                        break;
                    }
                }
                word = w.ToString();
                return w.typeWord;
            }
        }

        //1001-велика літера даної мови; 
        //1002-маленька літера даної мови; 
        //1003-цифра
        //1004-punctuation mark
        //1005-роздільник
        //1006-'\t'
        //1007-'\n'
        //1008-керуючий символ;
        //1009-символи кінця речення;
        //1020-пробіл
        //-1 - не увійшов ні в жоден з діапазонів;
        //-100 '\0'
        private List<typeSymbol> typeof_symbol(char c, out bool IsLangSymbol)
        {
            List<typeSymbol> lts;
            try
            {
                IsLangSymbol=true;

                switch (c) { 
                    case '\0':
                        return new List<typeSymbol>(new typeSymbol[] {new typeSymbol(-1,-1000)});
                    case '\'':
                        return new List<typeSymbol>(new typeSymbol[] { new typeSymbol(-1, 1005) });
                    case '-':
                        return new List<typeSymbol>(new typeSymbol[] { new typeSymbol(-1, 1005) });
                }
                


                lts=langs.IsAlpha(c);
                if(lts.Count>0) return lts;
                
                IsLangSymbol=false;

                if (dot_sentence.Contains(c))
                    return new List<typeSymbol>(new typeSymbol[] { new typeSymbol(-1, 1009) });
                if (punctuation_mark.Contains(c))
                    return new List<typeSymbol>(new typeSymbol[] { new typeSymbol(-1, 1004) });
                
                lts=syslangs.IsAlpha(c);
                if(lts.Count>0) return lts;
                if(c == ' ')
                    return new List<typeSymbol>(new typeSymbol[] { new typeSymbol(-1, 1010) });
                if ((c >= 2000) && (c <= 206f))
                    return new List<typeSymbol>(new typeSymbol[] {new typeSymbol(-1,1005)});
                if (c == '\t')
                    return new List<typeSymbol>(new typeSymbol[] {new typeSymbol(-1,1006)});
                if ((c == '\n') || (c == '\r'))
                    return new List<typeSymbol>(new typeSymbol[] {new typeSymbol(-1,1007)});
                if ((c >= '\x0001') && (c <= '\x0020'))
                    return new List<typeSymbol>(new typeSymbol[] {new typeSymbol(-1,1008)});
                lts=determineRange(c);
                if (lts[0].langid == 0) IsLangSymbol = true;
                return lts;
            }
            catch {
                IsLangSymbol=false;
                return new List<typeSymbol>(new typeSymbol[] {new typeSymbol(-1,-1)});
            }
        }
        private List<typeSymbol> determineRange(char c)
        {
            for (int i = 0; i < ranges.Length; i++) {
                if (c >= ranges[i].start && c <= ranges[i].end)
                    return new List<typeSymbol>(new typeSymbol[] {new typeSymbol(-1,i + 11)});
            }
            return new List<typeSymbol>(new typeSymbol[] {new typeSymbol(0,2)});
        }
    }

    #region interface ICharProvider
    public interface ICharProvider: IDisposable
    {
        long StreamPosition { get; }
        // не обов'язково адекватно сигналізує розмір потоку
        long StreamLength { get; }
        char this[int i] { get; }
        bool GetNextChar(out char c);
    }
    #endregion

    #region Stream charProvider
    public class charStreamProvider : ICharProvider
    {
        private Stream stream { get; set; }
        private BinaryReader binaryReader;
        // буфер (bufer = new char[bufer_size]) масиву символів зачитаних з потока stream
        private char[] bufer;
        // масив зміщень на символи в масиві bufer
        private long[] pointer;
        // bufer_size - розмі буфера для читання символів, дорівнює before_after * 3
        // actual_bufer_size - реальний розмір буфера може бути менше bufer_size = before_after * 3, залежить від stream
        private int bufer_size, actual_bufer_size;
        // point - покажчик на поточний символ в буфері (номер поточного символа в bufer)
        // boundary - межа, при досягненні якої (при читанні символів функцією GetNextChar) перезаповнюється масив символів bufer
        // boundary = before_after * 2;
        private int point, before_after = 1024, boundary;
        // показує, чи був досягнутий кінець stream при попередньому читанні
        private bool IsEndOfStream;
        public long StreamLength {get{return binaryReader.BaseStream.Length;}}
        public charStreamProvider(Stream s)
        {
            Encoding e = determineEncoding(s);
            IsEndOfStream = false;
            bufer_size = before_after * 3;
            boundary = before_after * 2;
            actual_bufer_size = bufer_size;
            point = 0;
            bufer = new char[bufer_size];
            pointer = new long[bufer_size];
            stream = s;
            binaryReader = new BinaryReader(stream, e);
            fillBufer();
        }
        // заповнює масив bufer з потоку stream
        private void fillBufer()
        {
            int i=0,j;
            if (point != 0) // якщо це не перше читання
            {
                if (IsEndOfStream) return;
                // останні дві третини bufer переписуємо на початок 
                for (i = 0; i < actual_bufer_size - before_after; i++)
                {
                    pointer[i] = pointer[before_after + i];
                    bufer[i] = bufer[before_after + i];
                }
                point = before_after;
                if (!IsEndOfStream)
                {
                    // заповнюємо останню третину bufer
                    for (j = i; j < bufer_size; j++)
                    {
                        try
                        {
                            pointer[j] = stream.Position;
                            bufer[j] = binaryReader.ReadChar();
                        }
                        catch (EndOfStreamException ex)
                        {
                            actual_bufer_size = j;
                            IsEndOfStream = true;
                            return;
                        }
                    }
                }
            }
            else
            {
                for (i = 0; i < bufer_size; i++)
                {
                    try
                    {
                        pointer[i] = stream.Position;
                        bufer[i] = binaryReader.ReadChar();
                    }
                    catch (EndOfStreamException ex)
                    {
                        actual_bufer_size = i;
                        IsEndOfStream = true;
                        return;
                    }
                }
            }
        }

        #region ICharProvider Members
        public long StreamPosition { get { return pointer[point];/*binaryReader.BaseStream.Position;*/} }
        public char this[int i] {
            get{
                int index = point + i;
                if ((index < actual_bufer_size)&&(index>=0))
                {
                    if(bufer[index]=='\0') return ' ';
                    return bufer[index];
                }
                return '\0';
            }
        }
        // false - кінець файла
        public bool GetNextChar(out char c) 
        { 
            int p=point;
            if (p < actual_bufer_size)
            {
                if (p < boundary)
                {
                    c = this[0];
                    point++;
                    return true;
                }
                else
                {
                    fillBufer();
                    c=this[0];
                    point++;
                    if (point >= actual_bufer_size) return false;
                    return true;

                }
            }
            else { 
                c = '\0';
                return false;
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (binaryReader != null)
            {
                binaryReader.Dispose();
                binaryReader = null;
                stream = null;
            }
        }
        #endregion

        private Encoding determineEncoding(Stream s)
        {
            int fByte, sByte, tByte;
            fByte = s.ReadByte();
            if (fByte == -1) throw new Exception("Stream is empty!");
            sByte = s.ReadByte();
            if (sByte == -1) throw new Exception("Stream is empty!");
            tByte = s.ReadByte();
            if (tByte == -1) throw new Exception("Stream is empty!");

            if ((fByte == 255) && (sByte == 254))//FF FE-UNICODE
            {
                s.Seek(2, SeekOrigin.Begin);
                return Encoding.Unicode;
            }
            else
            {
                if ((fByte == 254) && (sByte == 255))//FE FF-BigEndianUnicode
                {
                    s.Seek(2, SeekOrigin.Begin);
                    return Encoding.BigEndianUnicode;
                }
                else
                {
                    if ((fByte == 239) && (sByte == 187) && (tByte == 191))
                    {
                        return Encoding.UTF8;//EF BB BF- UTF-8
                    }
                    else
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        return null;
                    }
                }
            }

        }
    }
    #endregion
    
    #region String charProvider
    public class charStringProvider : ICharProvider
    {
        protected string s;
        protected char ch;

        protected int point;
        //private long StreamLength { get { return s.Length; } }
        public charStringProvider(string s)
        {
            point = 0;
            this.s = s;
        }
        //public charStringProvider() { }

        #region ICharProvider Members
        public long StreamPosition { get { return point; } }
        public long StreamLength { get { return s.Length; } }
        public char this[int i]
        {
            get
            {
                int index = point + i;
                if ((index < s.Length) && (index >= 0))
                {
                    if (s[index] == '\0') return ' ';
                    return s[index];
                }
                return '\0';
            }
        }
        // false - кінець файла
        public bool GetNextChar(out char c)
        {
            if (point < s.Length)
            {
                ch = c = this[0];
                point++;
                return true;
            }
            else
            {
                ch = c = '\0';
                return false;
            }
        }
        #endregion
        
        #region IDisposable Members
        public void Dispose()
        {
            if (s != null)
            {
                s = null;
            }
        }
        #endregion
    }
    #endregion

    #region XmlString charProvider
    // A numeric character reference refers to a character by its Universal Character Set/Unicode code point, and uses the format
    // &#nnnn; or &#xhhhh;
    // where nnnn is the code point in decimal form, and hhhh is the code point in hexadecimal form. The x must be lowercase in XML documents. The nnnn or hhhh may be any number of digits and may include leading zeros. The hhhh may mix uppercase and lowercase, though uppercase is the usual style.

    // In contrast, a character entity reference refers to a character by the name of an entity which has the desired character as its replacement text. The entity must either be predefined (built into the markup language) or explicitly declared in a Document Type Definition (DTD). The format is the same as for any entity reference:
    // &name;
    // where name is the case-sensitive name of the entity. The semicolon is required.

    public class charXmlStringProvider : charStringProvider
    {
        public charXmlStringProvider(string s): base(s)
        {
            this.s = UnescapeUnicode(RemoveTags(this.s));
        }

        protected static string UnescapeUnicode(string line)
        {
            return line; // HttpUtility.HtmlDecode("<a>" + line + "");
        }
        protected static string RemoveTags(string line)
        {
            //Regex regex = new Regex("\\<[^\\>]*\\>");
            Regex regex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", RegexOptions.Singleline); // for <div id="x<4>">
            return regex.Replace(line, String.Empty);
        }
        //public static string UnescapeUnicode(string line)
        //{
        //    using (StringReader reader = new StringReader("<a>" + line + ""))//</a>
        //    {
        //        XmlReaderSettings settings = new XmlReaderSettings();
        //        //settings.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
        //        //settings.ValidationType = ValidationType.None;

        //        using (XmlReader xmlReader = XmlReader.Create(reader, settings))
        //        {
        //            xmlReader.MoveToContent();
        //            return xmlReader.ReadElementContentAsString();
        //        }
        //    }
        //}
    }
    #endregion

    #region XmlStream charProvider
    // A numeric character reference refers to a character by its Universal Character Set/Unicode code point, and uses the format
    // &#nnnn; or &#xhhhh;
    // where nnnn is the code point in decimal form, and hhhh is the code point in hexadecimal form. The x must be lowercase in XML documents. The nnnn or hhhh may be any number of digits and may include leading zeros. The hhhh may mix uppercase and lowercase, though uppercase is the usual style.

    // In contrast, a character entity reference refers to a character by the name of an entity which has the desired character as its replacement text. The entity must either be predefined (built into the markup language) or explicitly declared in a Document Type Definition (DTD). The format is the same as for any entity reference:
    // &name;
    // where name is the case-sensitive name of the entity. The semicolon is required.
    public class charXmlStreamProvider : ICharProvider
    {
        private Stream stream { get; set; }
        private BinaryReader binaryReader;
        // буфер (bufer = new char[bufer_size]) масиву символів зачитаних з потока stream
        private char[] bufer;
        // масив зміщень на символи в масиві bufer
        private long[] pointer;
        // bufer_size - розмі буфера для читання символів, дорівнює before_after * 3
        // actual_bufer_size - реальний розмір буфера може бути менше bufer_size = before_after * 3, залежить від stream
        private int bufer_size, actual_bufer_size;
        // point - покажчик на поточний символ в буфері (номер поточного символа в bufer)
        // boundary - межа, при досягненні якої (при читанні символів функцією GetNextChar) перезаповнюється масив символів bufer
        // boundary = before_after * 2;
        private int point, before_after = 1024, boundary;
        // показує, чи був досягнутий кінець stream при попередньому читанні
        private bool IsEndOfStream;
        public long StreamLength { get { return binaryReader.BaseStream.Length; } }
        public charXmlStreamProvider(Stream s)
        {
            Encoding e = determineEncoding(s);
            IsEndOfStream = false;
            bufer_size = before_after * 3;
            boundary = before_after * 2;
            actual_bufer_size = bufer_size;
            point = 0;
            bufer = new char[bufer_size];
            pointer = new long[bufer_size];
            stream = s;
            binaryReader = new BinaryReader(stream, e);
            fillBufer();
        }
        // заповнює масив bufer з потоку stream
        private void fillBufer()
        {
            int i = 0, j;
            if (point != 0) // якщо це не перше читання
            {
                if (IsEndOfStream) return;
                // останні дві третини bufer переписуємо на початок 
                for (i = 0; i < actual_bufer_size - before_after; i++)
                {
                    pointer[i] = pointer[before_after + i];
                    bufer[i] = bufer[before_after + i];
                }
                point = before_after;
                if (!IsEndOfStream)
                {
                    // заповнюємо останню третину bufer
                    for (j = i; j < bufer_size; j++)
                    {
                        try
                        {
                            pointer[j] = stream.Position;
                            bufer[j] = binaryReader.ReadChar();
                        }
                        catch (EndOfStreamException ex)
                        {
                            actual_bufer_size = j;
                            IsEndOfStream = true;
                            return;
                        }
                    }
                }
            }
            else
            {
                for (i = 0; i < bufer_size; i++)
                {
                    try
                    {
                        pointer[i] = stream.Position;
                        bufer[i] = binaryReader.ReadChar();
                    }
                    catch (EndOfStreamException ex)
                    {
                        actual_bufer_size = i;
                        IsEndOfStream = true;
                        return;
                    }
                }
            }
        }

        #region ICharProvider Members
        public long StreamPosition { get { return pointer[point];/*binaryReader.BaseStream.Position;*/} }
        public char this[int i]
        {
            get
            {
                int index = point + i;
                if ((index < actual_bufer_size) && (index >= 0))
                {
                    if (bufer[index] == '\0') return ' ';
                    return bufer[index];
                }
                return '\0';
            }
        }
        // false - кінець файла
        public bool GetNextChar(out char c)
        {
            int p = point;
            if (p < actual_bufer_size)
            {
                if (p < boundary)
                {
                    c = this[0];
                    point++;
                    return true;
                }
                else
                {
                    fillBufer();
                    c = this[0];
                    point++;
                    if (point >= actual_bufer_size) return false;
                    return true;

                }
            }
            else
            {
                c = '\0';
                return false;
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (binaryReader != null)
            {
                binaryReader.Dispose();
                binaryReader = null;
                stream = null;
            }
        }
        #endregion

        private Encoding determineEncoding(Stream s)
        {
            int fByte, sByte, tByte;
            fByte = s.ReadByte();
            if (fByte == -1) throw new Exception("Stream is empty!");
            sByte = s.ReadByte();
            if (sByte == -1) throw new Exception("Stream is empty!");
            tByte = s.ReadByte();
            if (tByte == -1) throw new Exception("Stream is empty!");

            if ((fByte == 255) && (sByte == 254))//FF FE-UNICODE
            {
                s.Seek(2, SeekOrigin.Begin);
                return Encoding.Unicode;
            }
            else
            {
                if ((fByte == 254) && (sByte == 255))//FE FF-BigEndianUnicode
                {
                    s.Seek(2, SeekOrigin.Begin);
                    return Encoding.BigEndianUnicode;
                }
                else
                {
                    if ((fByte == 239) && (sByte == 187) && (tByte == 191))
                    {
                        return Encoding.UTF8;//EF BB BF- UTF-8
                    }
                    else
                    {
                        s.Seek(0, SeekOrigin.Begin);
                        return null;
                    }
                }
            }

        }
    }
    #endregion
}
