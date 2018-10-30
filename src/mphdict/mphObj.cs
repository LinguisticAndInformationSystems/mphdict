using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mphdict.Models.morph;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uSofTrod.generalTypes.Models;

namespace mphdict.Models.morph
{
    public partial class mphContext
    {
        private static object o = new object();
        static private alphadigit[] _talpha=null;

        public alphadigit[] talpha
        {
            get
            {
                if (_talpha == null)
                {
                    lock (o)
                    {
                        _talpha = (from c in this.alphadigits orderby c.digit, c.ls select c).ToArray();
                    }
                }
                return _talpha;
            }
        }

        static private short[] _pclass = null;
        public short[] pclass
        {
            get
            {
                if (_pclass == null)
                {
                    lock (o)
                    {
                        _pclass = (from c in this.indents orderby c.type select c.type).ToArray();
                    }
                }
                return _pclass;
            }
        }

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
                        var gc_gr = (from c in this.grs where !string.IsNullOrEmpty(c.part_of_speech) orderby c.part_of_speech select c).ToArray();
                        var gc_parts = (from c in this.parts where ((!string.IsNullOrEmpty(c.com)) && (c.id > 70)) orderby c.com select c).ToArray();
                        _pofs.Add((from c in gc_gr select new ps() { id = c.id, name = c.part_of_speech, category = "Змінні" }).ToArray());
                        _pofs.Add(gc_parts.Select(c => new ps() { id = c.id, name = c.com, category = "Незмінні" }).ToArray());
                    }
                }
                return _pofs;
            }
        }

        static private indents_base[] _indents = null;
        public indents_base[] indent
        {
            get
            {
                if (_indents == null)
                {
                    lock (o)
                    {
                        //_indents = (from c in this.words_list.AsNoTracking()
                        //            group c by c.type into pg
                        //            join ind in this.indents.Include(t => t.gr).AsNoTracking()
                        //            on pg.FirstOrDefault().type equals ind.type
                        //            orderby ind.type

                        //            select new indents_base()
                        //            {
                        //                CountOfWords = pg.Count(),
                        //                comment = ind.comment,
                        //                type = ind.type,
                        //                field3 = ind.field3,
                        //                field4 = ind.field4,
                        //                gr_id = ind.gr_id,
                        //                grName = ind.gr.part_of_speech,
                        //                indent = ind.indent
                        //            }).ToArray();
                        //This query do not work in vs2017 (EF CORE 1.1)
                        _indents = (from c in this.indents.Include(t => t.gr).Include(t => t.words_list).AsNoTracking()
                                    orderby c.type
                                    select new indents_base()
                                    {
                                        CountOfWords = c.words_list.Count(),
                                        comment = c.comment,
                                        type = c.type,
                                        field3 = c.field3,
                                        field4 = c.field4,
                                        gr_id = c.gr_id,
                                        grName = c.gr.part_of_speech,
                                        indent = c.indent
                                    }).ToArray();
                    }
                }
                return _indents;
            }
        }
        private static langid _lid;
        public langid lid
        {
            get
            {
                if (_lid == null)
                {
                    lock (o)
                    {
                        _lid = (from c in this.lang select c).FirstOrDefault();
                    }
                }
                return _lid;
            }
        }
        // перетворення рядка в код (якщо не потрібно враховувати \', то askip=true)
        private IQueryable<word_param> setWordListQueryFilter(filter f, IQueryable<word_param> q)
        {
            if ((f.isStrFiltering) && (!string.IsNullOrEmpty(f.str)))
            {
                string s = sharedTypes.atod(f.str, talpha);
                //switch (f.fetchType)
                //{
                //    case FetchType.StartsWith:
                //        q = q.Where(c => c.digit.StartsWith(s));
                //        //q = q.Where(c => c.reestr.Replace("\"","").StartsWith(f.str));
                //        break;
                //    case FetchType.EndsWith:
                //        q = q.Where(c => c.digit.EndsWith(s));
                //        //q = q.Where(c => c.reestr.Replace("\"", "").EndsWith(f.str));
                //        break;
                //    case FetchType.Contains:
                //        q = q.Where(c => c.digit.Contains(s));
                //        //q = q.Where(c => c.reestr.Replace("\"", "").Contains(f.str));
                //        break;
                //}
                q = q.Where(c => EF.Functions.Like(c.digit, s));
            }
            if (f.ispclass) {
                q = q.Where(c => c.type == f.pclass);
            }
            if (f.ispofs) {
                if (f.pofs > 70)
                {
                    q = q.Where(c => c.part == f.pofs);
                }
                else {
                    q = q.Where(c => ((c.parts.gr_id == f.pofs)&&(c.part<70)));
                }
            }
            return q;
        }
        public async Task<int> CountWords(filter f)
        {
            var q = (from c in this.words_list select c);
            q = setWordListQueryFilter(f, q);
            return await q.CountAsync();
        }

        public async Task<word_param[]> getPage(filter f, int start, int pageSize)
        {
                var q = (from c in this.words_list.AsNoTracking() select c);
                q = setWordListQueryFilter(f, q);
                if (f.isInverse)
                    q = q.OrderBy(c => c.reverse).ThenBy(c => c.field2).Skip(start * pageSize).Take(pageSize);
                else
                    q = q.OrderBy(c=>c.digit).ThenBy(c=>c.field2).Skip(start * pageSize).Take(pageSize);
                //var sql = q.ToSql();
                return await q.ToArrayAsync();
        }
        public async Task<word_param_base> searchWord(filter f, string word)
        {
            string w = f.isInverse == true ? sharedTypes.atod(new string(word.Reverse().ToArray()), talpha) : sharedTypes.atod(word, talpha);
            int start = 0;
            var q = (from c in this.words_list select c);

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
            else
            {
                q = q.Skip(start).Take(1);
            }
            word_param wp = await q.FirstOrDefaultAsync();
            word_param_base r = null;
            if (wp != null)
            {
                r = (new word_param_base() { CountOfWords = count, wordsPageNumber = pagenumber, accent = wp.accent, digit = wp.digit, field2 = wp.field2, field5 = wp.field5, field6 = wp.field6, field7 = wp.field7, isdel = wp.isdel, isproblem = wp.isproblem, nom_old = wp.nom_old, own = wp.own, part = wp.part, reestr = wp.reestr, reverse = wp.reverse, suppl_accent = wp.suppl_accent, type = wp.type });
            }
            return r;

        }
        public async Task<word_param> getEntry(int id)
        {
            var word_param = await (from c in this.words_list.AsNoTracking() where c.nom_old == id select c).FirstOrDefaultAsync();
            indents indent = await (from c in this.indents.AsNoTracking() where c.type == word_param.type select c).FirstOrDefaultAsync();
            List<flexes> flex = await (from c in this.flexes.AsNoTracking() where (c.type == word_param.type && (c.field2 > 0)) orderby c.field2, c.id select c).ToListAsync();
            accents_class aclass = await (from c in this.accents_class.AsNoTracking() select c).FirstOrDefaultAsync();
            accent[] acnt = await (from c in this.accent.AsNoTracking() where c.accent_type == word_param.accent select c).ToArrayAsync();
            //minor_acc macc = await (from c in this.minor_acc.AsNoTracking() where c.nom_old == id select c).FirstOrDefaultAsync();
            parts part = await (from c in this.parts.AsNoTracking() where c.id == word_param.part select c).FirstOrDefaultAsync();
            word_param.parts = part;
            word_param.indents = indent;
            word_param.indents.flexes = flex;
            word_param.accents_class = aclass;
            word_param.accents_class.accents = acnt;
            //word_param.minor_acc = macc;
            return word_param;
        }
        public async Task<pclass_info> getPClass(short id)
        {
            return new pclass_info()
            {
                cls = (from c in indents where (c.type == id) select c).FirstOrDefault(),
                flexes = await (from c in this.flexes.AsNoTracking() where (c.type == id) orderby c.field2, c.id select c).ToArrayAsync()
            };
        }
    }

    public class pclass_info {
        public indents_base cls { get; set; }
        public flexes_base[] flexes { get; set; }
    }
    public class filter
    {
        //public FetchType fetchType { get; set; }
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
    public class pclsfilter
    {
        public bool ispofsPcls { get; set; }
        public byte pofsPcls { get; set; }
        public short pclassPcls { get; set; }
    }
}
