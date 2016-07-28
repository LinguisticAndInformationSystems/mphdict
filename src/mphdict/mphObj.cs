using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mphdict.Models.morph;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mphdict
{
    public class mphObj: IDisposable
    {
        private mphContext db;
        public ILogger Logger { get; set; }
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
        public async Task<word_param[]> getList()
        {
            try
            {
                return await (from c in db.words_list.AsNoTracking() orderby c.digit, c.field2 select c).Take(100).ToArrayAsync();
            }
            catch (Exception ex)
            {
                if(Logger!=null)
                    //Logger.Log(LogLevel.Error, 0, ex.Message, ex, null);
                    Logger.LogError(new EventId(0), ex, ex.Message);
                return null;
            }
            finally
            {
            }
        }
        private IQueryable<word_param> setWordListQueryFilter(filter f, IQueryable<word_param> q)
        {
            if ((f.isStrFiltering) && (!string.IsNullOrEmpty(f.str)))
            {
                switch (f.fetchType)
                {
                    case FetchType.StartsWith:
                        q = q.Where(c => c.reestr.StartsWith(f.str));
                        break;
                    case FetchType.EndsWith:
                        q = q.Where(c => c.reestr.EndsWith(f.str));
                        break;
                    case FetchType.Contains:
                        q = q.Where(c => c.reestr.Contains(f.str));
                        break;
                }
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
        public string str { get; set; }
        public bool isStrFiltering { get; set; }
        public bool isInverse { get; set; } 
    }
}
