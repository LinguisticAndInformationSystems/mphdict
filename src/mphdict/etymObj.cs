using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mphdict.Models.Etym;
using mphdict.Models.SynonymousSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uSofTrod.generalTypes.Models;
using uSofTrod.TextUtility.txToolsCore;

namespace mphdict.Models.Etym
{
    public partial class etymContext
    {
        static private List<ps[]> _pofs = null;
        public List<ps[]> pofs
        {
            get
            {
                if (_pofs == null)
                {
                    lock (o)
                    {
                        _pofs = new List<ps[]>();
                        var g1 = (from c in this.lang_all /*where (c.id <= 16)*/ orderby c.lang_name select c).ToArray();
                        _pofs.Add((from c in g1 select new ps() { id = (short)c.lang_code, name = c.lang_name }).ToArray());
                    }
                }
                return _pofs;
            }
        }
        private static object o = new object();
        private IQueryable<etymons> setWordListQuerySorting(IQueryable<etymons> q)
        {
            q = q.OrderBy(c => c.word.ToUpperInvariant().Replace("<SUP>", "").Replace("</SUP>", "").Replace("\'", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("*", "")).ThenBy(c => c.homonym);
            return q;
        }
        private IQueryable<etymons> setWordListQueryFilter(etymfilter f, IQueryable<etymons> q)
        {
            if ((f.isStrFiltering) && (!string.IsNullOrEmpty(f.str)))
            {
                string s = f.str.Replace('*', '%');
                q = q.Where(c => EF.Functions.Like(c.word, s));
            }
            if (f.isHead)
            {
                q = q.Where(c => c.ishead == true);
            }
            if (f.isLang)
            {
                q = q.Where(c => c.lang_code == f.langId);
            }
            if (f.isType)
            {
                switch (f.typeId)
                {
                    case 0://Літературні
                        q = q.Where(c => c.dialect == false);
                        break;
                    case 1://Діалектні
                        q = q.Where(c => c.dialect == true);
                        break;
                    case 2://Омоніми
                        q = q.Where(c => c.homonym > 0);
                        break;
                    case 3://Антропоніми
                        q = q.Where(c => c.antroponym == true);
                        break;
                }
            }
            return q;
        }
        public async Task<int> CountWords(etymfilter f)
        {
            var q = (from c in this.etymons select c);
            q = setWordListQueryFilter(f, q);
            return await q.CountAsync();
        }
        public async Task<etymons[]> getPage(etymfilter f, int start, int pageSize)
        {
            var q = (from c in this.etymons.AsNoTracking() select c);
            q = setWordListQueryFilter(f, q);
            q = setWordListQuerySorting(q);
            q = q.Skip(start * pageSize).Take(pageSize);

            return await q.ToArrayAsync();
        }
        public async Task<etymons_base> searchWord(etymfilter f, string word)
        {
            string w = word ?? "";
            int start = 0;
            var q = (from c in this.etymons select c);
            q = setWordListQueryFilter(f, q);

            q = setWordListQuerySorting(q);
            start = await (from c in q where w.CompareTo(c.word.ToUpperInvariant().Replace("<SUP>", "").Replace("</SUP>", "").Replace("\'", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("*", "")) > 0 select c).CountAsync();

            int pagenumber = start / 100;
            int count = q.Count();

            if (count <= start)
            {
                q = q.Skip((start - 1)).Take(1);
            }
            else
            {
                q = q.Skip(start).Take(1);
            }
            etymons wp = await q.FirstOrDefaultAsync();
            etymons_base r = null;
            if (wp != null)
            {
                r = (new etymons_base() { CountOfWords = count, wordsPageNumber = pagenumber, id = wp.id, id_e_classes = wp.id_e_classes, homonym = wp.homonym, word = wp.word });

            }
            return r;
        }
        public async Task<root> getEntry(int idclass)
        {
            var ss = await (from c in this.roots.Include("e_classes.etymons").Include("bibls").Include("links")
                            where c.e_classes.Any(m => m.id == idclass)
                            select c).FirstOrDefaultAsync();
            //ss._wlist = ss._wlist.OrderBy(o => o.id_syn).ToArray();
            return ss;
        }
        public async Task<string> getWord(int wid)
        {
            return await (from c in this.etymons where c.id == wid select c.word).FirstOrDefaultAsync();
        }
    }
    public class etymfilter
    {
        public string str { get; set; }
        public bool isStrFiltering { get; set; }
        public bool isHead { get; set; } = true;
        public bool isLang { get; set; }
        public int langId { get; set; }
        public bool isType { get; set; }
        public int typeId { get; set; }
    }
}
