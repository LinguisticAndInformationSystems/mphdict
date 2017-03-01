using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mphdict.Models.SynonymousSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uSofTrod.generalTypes.Models;

namespace mphdict
{
    public class synsetsObj : IDisposable
    {
        private string schema;
        private synsetsContext db;
        private ILogger Logger { get; set; }
        public synsetsObj(synsetsContext db, ILogger<synsetsContext> log)
        {
            this.db = db;
            Logger = log;
        }
        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
                Logger = null;
            }
        }
        private static object o = new object();
        static private alphadigit[] _talpha = null;

        public alphadigit[] talpha
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
        static private List<ps[]> _pofs = null;
        public List<ps[]> pofs
        {
            get
            {
                try
                {
                    if (_pofs == null)
                    {
                        lock (o)
                        {
                            _pofs = new List<ps[]>();
                            var g1 = (from c in db.pofs where (c.id <= 16) orderby c.name select c).ToArray();
                            var g2 = (from c in db.pofs where (c.id > 16) orderby c.name select c).ToArray();
                            _pofs.Add((from c in g1 select new ps() { id = (short)c.id, name = c.name, category = "Змінні" }).ToArray());
                            _pofs.Add(g2.Select(c => new ps() { id = (short)c.id, name = c.name, category = "Незмінні" }).ToArray());
                        }
                    }
                    return _pofs;
                }
                catch (Exception ex)
                {
                    _pofs = null;
                    if (Logger != null)
                        Logger.LogError(new EventId(0), ex, ex.Message);
                    else
                        throw ex;
                    return null;
                }
            }
        }
        private static langid _lid;
        public langid lid
        {
            get
            {
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

        private IQueryable<wlist> setWordListQueryFilter(synsetsfilter f, IQueryable<wlist> q)
        {
            try
            {
                if ((f.isStrFiltering) && (!string.IsNullOrEmpty(f.str)))
                {
                    string s = sharedTypes.atod(f.str, talpha);
                    switch (f.fetchType)
                    {
                        case FetchType.StartsWith:
                            q = q.Where(c => c.digit.StartsWith(s));
                            //q = q.Where(c => c.reestr.Replace("\"","").StartsWith(f.str));
                            break;
                        case FetchType.EndsWith:
                            q = q.Where(c => c.digit.EndsWith(s));
                            //q = q.Where(c => c.reestr.Replace("\"", "").EndsWith(f.str));
                            break;
                        case FetchType.Contains:
                            q = q.Where(c => c.digit.Contains(s));
                            //q = q.Where(c => c.reestr.Replace("\"", "").Contains(f.str));
                            break;
                    }
                }
                if (f.ispofs)
                {
                    q = (from c in q join ss in db.synsets on c.id_set equals ss.pofs where ss.pofs == f.pofs select c);
                }
                return q;
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
        public async Task<int> CountWords(synsetsfilter f)
        {
            try
            {
                var q = (from c in db.wlist select c);
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
        public async Task<wlist[]> getPage(synsetsfilter f, int start, int pageSize)
        {
            try
            {
                var q = (from c in db.wlist.AsNoTracking() select c);
                q = setWordListQueryFilter(f, q);
                q = q.OrderBy(c => c.digit).ThenBy(c => c.homonym).Skip(start * pageSize).Take(pageSize);

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
        public async Task<wlist_base> searchWord(synsetsfilter f, string word)
        {
            //System.Runtime.CompilerServices.StrongBox <T>
            try
            {
                string w =  sharedTypes.atod(word, talpha);
                int start = 0;
                var q = (from c in db.wlist select c);

                q = setWordListQueryFilter(f, q);

                q = q.OrderBy(c => c.digit).ThenBy(c => c.homonym);
                start = await (from c in q where w.CompareTo(c.digit) > 0 select c).CountAsync();

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
                wlist wp = await q.FirstOrDefaultAsync();
                wlist_base r = null;
                if (wp != null)
                {
                    r = (new wlist_base() { CountOfWords = count, wordsPageNumber = pagenumber, id = wp.id, comm= wp.comm, comm2= wp.comm2, digit= wp.digit, homonym= wp.homonym, hyperonym= wp.hyperonym, id_hyp= wp.id_hyp, id_int= wp.id_int, id_phon= wp.id_phon, id_r=wp.id_r, id_set= wp.id_set, id_syn= wp.id_syn, inactive= wp.inactive, interpretation= wp.interpretation, intsum= wp.intsum, login= wp.login, nom= wp.nom, sign=wp.sign, sword=wp.sword, timemarker= wp.timemarker, word= wp.word });
                }
                return r;

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
        public async Task<synsets> getEntry(int idset)
        {
            try
            {
                var ss = await (from c in db.synsets.Include(o=>o._pofs).Include(o=>o._wlist).AsNoTracking() where c.id== idset select c).FirstOrDefaultAsync();
                ss._wlist = ss._wlist.OrderBy(o => o.id_syn).ToArray();
                return ss;
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

        #region сервісні функції
        public void forming_nrw()
        {
            try
            {
                int q = 0, cp = 0;
                var a = db.wlist.ToArray();
                for (int i = 0; i < a.Length; i++)
                {
                    a[i].digit = sharedTypes.atod(a[i].word, talpha);
                    q++;
                    if (q == 4000)
                    {
                        q = 0;
                        db.SaveChanges();
                        Console.WriteLine($"prepared next 4000 rows - {i}...");
                    }
                    cp++;
                }
                db.SaveChanges();
                Console.WriteLine($"finished ({cp})");
                Console.Read();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(0), ex, ex.Message);
            }
            finally
            {
            }
        }

        #endregion
    }
    public class synsetsfilter
    {
        public FetchType fetchType { get; set; }
        public string str { get; set; }
        public bool isStrFiltering { get; set; }
        public bool ispofs { get; set; }
        public byte pofs { get; set; }
    }

}
