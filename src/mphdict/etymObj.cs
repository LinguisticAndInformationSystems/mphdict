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

namespace mphdict
{
    public class etymObj : IDisposable
    {
        private string schema;
        private etymContext db;
        private ILogger Logger { get; set; }
        public etymObj(etymContext db, ILogger<synsetsContext> log)
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

        private IQueryable<wlist> setWordListQueryFilter(etymfilter f, IQueryable<wlist> q)
        {
            try
            {
                if ((f.isStrFiltering) && (!string.IsNullOrEmpty(f.str)))
                {
                    string s = f.str;
                    q = q.Where(c => EF.Functions.Like(c.digit, s));
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
        public async Task<int> CountWords(etymfilter f)
        {
            try
            {
                //var q = (from c in db.wlist select c);
                //q = setWordListQueryFilter(f, q);
                //return await q.CountAsync();
                return 0;
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
        //public async Task<wlist[]> getPage(etymfilter f, int start, int pageSize)
        //{
        //    try
        //    {
        //        //var q = (from c in db.wlist.AsNoTracking() select c);
        //        //q = setWordListQueryFilter(f, q);
        //        //q = q.OrderBy(c => c.digit).ThenBy(c => c.homonym).Skip(start * pageSize).Take(pageSize);

        //        return await q.ToArrayAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (Logger != null)
        //        {
        //            Logger.LogError(new EventId(0), ex, ex.Message);
        //            return null;
        //        }
        //        else throw ex;
        //    }
        //    finally
        //    {
        //    }
        //}
        public async Task<wlist_base> searchWord(etymfilter f, string word)
        {
            //System.Runtime.CompilerServices.StrongBox <T>
            try
            {
                //string w = word;
                //int start = 0;
                //var q = (from c in db.etymons select c);

                //q = setWordListQueryFilter(f, q);

                //q = q.OrderBy(c => c.digit).ThenBy(c => c.homonym);
                //start = await (from c in q where w.CompareTo(c.digit) > 0 select c).CountAsync();

                //int pagenumber = start / 100;
                //int count = q.Count();

                //if (count <= start)
                //{
                //    q = q.Skip((start - 1)).Take(1);
                //}
                //else
                //{
                //    q = q.Skip(start).Take(1);
                //}
                //wlist wp = await q.FirstOrDefaultAsync();
                //wlist_base r = null;
                //if (wp != null)
                //{
                //    r = (new wlist_base() { CountOfWords = count, wordsPageNumber = pagenumber, id = wp.id, comm= wp.comm, digit= wp.digit, homonym= wp.homonym, hyperonym= wp.hyperonym, id_hyp= wp.id_hyp, id_int= wp.id_int, id_phon= wp.id_phon, id_r=wp.id_r, id_set= wp.id_set, id_syn= wp.id_syn, inactive= wp.inactive, interpretation= wp.interpretation, intsum= wp.intsum, userid= wp.userid, nom= wp.nom, sign=wp.sign, sword=wp.sword, timemarker= wp.timemarker, word= wp.word });
                //return r;
                //}

                return new wlist_base();

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
        public async Task<root> getEntry(int idset)
        {
            try
            {
                var ss = await (from c in db.roots select c).FirstOrDefaultAsync();
                //ss._wlist = ss._wlist.OrderBy(o => o.id_syn).ToArray();
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
        public async Task<string> getWord(int wid)
        {
            try
            {
                return await (from c in db.etymons where c.id== wid select c.word).FirstOrDefaultAsync();
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
    public class etymfilter
    {
        public string str { get; set; }
        public bool isStrFiltering { get; set; }
        public bool isLang { get; set; }
        public int LangID { get; set; }
    }
}
