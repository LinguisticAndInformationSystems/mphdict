using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mphdict.Models.morph;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mphdict
{
    public class mphObj: IDisposable
    {
        private mphContext db;
        public ILogger Logger { get; set; }
        private static object o = new object();
        static private alphadigit[] _talpha=null;

        private alphadigit[] talpha
        {
            get
            {
                try
                {
                    if (_talpha == null)
                    {
                        lock (o)
                        {
                            _talpha = (from c in db.alphadigits orderby c.digit, c.ls select c).ToArray();
                        }
                    }
                    return _talpha;
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogError(new EventId(0), ex, ex.Message);
                    else
                        throw ex;
                    return null;
                }
            }
        }

        static private short[] _pclass = null;
        public short[] pclass
        {
            get
            {
                try
                {
                    if (_pclass == null)
                    {
                        lock (o)
                        {
                            _pclass = (from c in db.indents orderby c.type select c.type).ToArray();
                        }
                    }
                    return _pclass;
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogError(new EventId(0), ex, ex.Message);
                    else
                        throw ex;
                    return null;
                }
            }
        }

        static private List<ps> _pofs = null;
        public List<ps> pofs
        {
            get
            {
                try
                {
                    if (_pofs == null)
                    {
                        lock (o)
                        {
                            var db_a = (from c in db.parts orderby c.com select c).ToArray();
                            var a = (from c in db_a select new ps() { id = c.id, name = c.com }).ToList();
                            _pofs = a.Where(c => c.id <= 70).OrderBy(c => c.name).ToList();
                            _pofs.AddRange(a.Where(c => c.id > 70).OrderBy(c => c.name));
                        }
                    }
                    return _pofs;
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                        Logger.LogError(new EventId(0), ex, ex.Message);
                    else
                        throw ex;
                    return null;
                }
            }
        }

        private static langid _lid;
        public langid lid {
            get {
                try
                {
                    if (_lid == null)
                    {
                        lock (o)
                        {
                            _lid = (from c in db.lang select c).FirstOrDefault();
                        }
                    }
                    return _lid;
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                    {
                        Logger.LogError(new EventId(0), ex, ex.Message);
                        return null;
                    }
                    else throw ex;
                }
            }
        }
        public mphObj(mphContext db)
        {
            this.db = db;
        }
        public void Dispose()
        {
            if (db != null) {
                db.Dispose();
                db = null;
            }
        }
        // перетворення рядка в код (якщо не потрібно враховувати \', то askip=true)
        public string atod(string a, bool askip = true)
        {
            if (a == null) return "";
            StringBuilder d = new StringBuilder("");
            for (int i = 0; i < a.Length; i++)
            {
                if ((askip) && ((a[i] == '\'') || (a[i] == ' '))) continue;
                alphadigit rs = talpha.FirstOrDefault(t => t.alpha == a[i].ToString());
                if (rs != null) d.Append(rs.digit);
            }
            return d.ToString();
        }

        private IQueryable<word_param> setWordListQueryFilter(filter f, IQueryable<word_param> q)
        {
            if ((f.isStrFiltering) && (!string.IsNullOrEmpty(f.str)))
            {
                switch (f.fetchType)
                {
                    case FetchType.StartsWith:
                        q = q.Where(c => c.reestr.Replace("\"","").StartsWith(f.str));
                        break;
                    case FetchType.EndsWith:
                        q = q.Where(c => c.reestr.Replace("\"", "").EndsWith(f.str));
                        break;
                    case FetchType.Contains:
                        q = q.Where(c => c.reestr.Replace("\"", "").Contains(f.str));
                        break;
                }
            }
            if (f.ispclass) {
                q = q.Where(c => c.type == f.pclass);
            }
            return q;
        }
        public async Task<int> CountWords(filter f)
        {
            try
            {
                var q = (from c in db.words_list select c);
                q = setWordListQueryFilter(f, q);
                return await q.CountAsync();
            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.LogError(new EventId(0), ex, ex.Message);
                    return -1;
                }
                else throw ex;
            }
        }

        public async Task<word_param[]> getPage(filter f, int start, int pageSize)
        {
            try
            {
                var q = (from c in db.words_list.AsNoTracking() select c);
                q = setWordListQueryFilter(f, q);
                if (f.isInverse)
                    q = q.OrderBy(c => c.reverse).ThenBy(c => c.field2).Skip(start * pageSize).Take(pageSize);
                else
                    q = q.OrderBy(c=>c.digit).ThenBy(c=>c.field2).Skip(start * pageSize).Take(pageSize);

                return await q.ToArrayAsync();
            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.LogError(new EventId(0), ex, ex.Message);
                    return null;
                }
                else throw ex;
            }
            finally
            {
            }
        }
        public async Task<word_param_base> searchWord(filter f, string word)
        {
            //System.Runtime.CompilerServices.StrongBox <T>
            try
            {
                string w = f.isInverse==true? atod(new string(word.Reverse().ToArray())) : atod(word);
                int start = 0;
                var q = (from c in db.words_list select c);

                q = setWordListQueryFilter(f, q);

                if (f.isInverse)
                {
                    q = q.OrderBy(c => c.reverse).ThenBy(c => c.field2);
                    start = await (from c in q where w.CompareTo(c.reverse) > 0 select c).CountAsync();
                }
                else
                {
                    q = q.OrderBy(c => c.digit).ThenBy(c => c.field2);
                    start = await (from c in q where w.CompareTo(c.digit) > 0 select c).CountAsync();
                }

                int pagenumber = start / 100;
                int count = q.Count();

                if (count <= start)
                {
                    q = q.Skip((start - 1)).Take(1);
                }
                else {
                    q = q.Skip(start).Take(1);
                }
                return await (from c in q select new word_param_base() { CountOfWords = count, wordsPageNumber = pagenumber, accent = c.accent, digit = c.digit, field2 = c.field2, field5 = c.field5, field6 = c.field6, field7 = c.field7, isdel = c.isdel, isproblem = c.isproblem, nom_old = c.nom_old, own = c.own, part = c.part, reestr = c.reestr, reverse = c.reverse, suppl_accent = c.suppl_accent, type = c.type }).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.LogError(new EventId(0), ex, ex.Message);
                    return null;
                }
                else throw ex;
            }
        }
        public async Task<word_param> getEntry(int id)
        {
            try
            {
                var word_param = await (from c in db.words_list.AsNoTracking() where c.nom_old==id select c).FirstOrDefaultAsync();
                indents indent= await (from c in db.indents.AsNoTracking() where c.type==word_param.type select c).FirstOrDefaultAsync();
                List<flexes> flex = await (from c in db.flexes.AsNoTracking() where (c.type==word_param.type && (c.field2>0)) orderby c.field2, c.id select c).ToListAsync();
                accents_class aclass = await (from c in db.accents_class.AsNoTracking() select c).FirstOrDefaultAsync();
                accent[] acnt = await (from c in db.accent.AsNoTracking() where c.accent_type== word_param.accent select c).ToArrayAsync();
                //minor_acc macc = await (from c in db.minor_acc.AsNoTracking() where c.nom_old == id select c).FirstOrDefaultAsync();
                parts part= await (from c in db.parts.AsNoTracking() where c.id == word_param.part select c).FirstOrDefaultAsync();
                word_param.parts = part;
                word_param.indents = indent;
                word_param.indents.flexes = flex;
                word_param.accents_class = aclass;
                word_param.accents_class.accents = acnt;
                //word_param.minor_acc = macc;
                return word_param;
            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.LogError(new EventId(0), ex, ex.Message);
                    return null;
                }
                else throw ex;
            }
        }
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

    public class filter
    {
        public FetchType fetchType { get; set; }
        //pattern="[A-za-zА-Яа-я\sЇїЁёЄєІі]+"
        //[RegularExpression(@"^([A-za-zА-Яа-я\sЇїЁёЄєІі]+)$", ErrorMessage = "Invalid characters")]
        public string str { get; set; }
        public bool isStrFiltering { get; set; }
        public bool isInverse { get; set; }
        public bool ispclass { get; set; }
        public short pclass { get; set; }
        public bool ispofs { get; set; }
        public byte pofs { get; set; }
    }
    public struct ps
    {
        public short id { get; set; }
        public string name { get; set; }
    }

}
