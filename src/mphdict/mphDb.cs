using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mphdict.Models.morph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdict
{
    public class mphDb
    {
        public mphDb()
        {
        }
        private string schema;
        DbContextOptions<mphContext> options;
        DbContextOptionsBuilder<mphContext> optionsBuilder;
        private mphContext db;
        private static ILogger Logger { get; set; }
        public mphDb(mphContext db, ILoggerFactory DbLogging)
        {
            this.db = db;
            Logger = DbLogging.CreateLogger<mphDb>();
            //Logger.LogError("mphDb created");
        }
        public mphDb(DbContextOptions<mphContext> options, string schema, ILoggerFactory DbLogging)
        {
            this.schema = schema;
            this.options = options;
            Logger = DbLogging.CreateLogger<mphDb>();
        }
        public mphDb(DbContextOptionsBuilder<mphContext> optionsBuilder, string schema, ILoggerFactory DbLogging)
        {
            this.schema = schema;
            this.optionsBuilder = optionsBuilder;
            Logger = DbLogging.CreateLogger<mphDb>();
        }
        public mphContext getContext()
        {
            return db != null ? db : (options != null ? new mphContext(options, schema) : (optionsBuilder != null ? mphContext.Create(optionsBuilder, schema) : null));
        }

        public void removeData()
        {
            var context = getContext();
            try
            {
                context.alphadigits.RemoveRange(context.alphadigits.Where(c=>c.lang>= int.MinValue));
                //context.allangs.RemoveRange(context.allangs.Where(c => c.id_lang >= int.MinValue));
                context.grs.RemoveRange(context.grs.Where(c => c.id >=byte.MinValue));
                context.parts.RemoveRange(context.parts.Where(c => c.id >= byte.MinValue));
                context.indents.RemoveRange(context.indents.Where(c => c.type >= short.MinValue));
                context.accents_class.RemoveRange(context.accents_class.Where(c => c.id >= short.MinValue));
                context.accent.RemoveRange(context.accent.Where(c => c.id >= int.MinValue));
                context.flexes.RemoveRange(context.flexes.Where(c => c.id >= int.MinValue));
                context.words_list.RemoveRange(context.words_list.Where(c => c.nom_old > int.MinValue));
                context.minor_acc.RemoveRange(context.minor_acc.Where(c => c.nom_old >= int.MinValue));
                context.lang.RemoveRange(context.lang.Where(c => c.id_lang >= int.MinValue));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(0), ex, ex.Message);
            }
            finally
            {
                //if ((db == null) && (context != null)) context.Dispose();
            }
        }

        public void testAddAccents_class()
        {
            var context = getContext();
            try
            {
                context.accents_class.Add(new accents_class() { id=short.MinValue, part_desc="test adding"});
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(0), ex, ex.Message);
            }
            finally
            {
                if ((db == null) && (context != null)) context.Dispose();
            }
        }
        public void copyContextTo(mphDb resultmphDb)
        {
            var context = getContext();

            try
            {
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                resultmphDb.removeData();
                var ad = context.alphadigits.Where(c => c.lang == 1058).AsNoTracking().ToArray();
                //var allang = context.allangs.AsNoTracking().Where(c => c.id_lang == 1058).ToArray();
                var gr = context.grs.AsNoTracking().ToArray();
                var parts = context.parts.AsNoTracking().ToArray();
                var indents = context.indents.AsNoTracking().ToArray();
                var ac_cls = context.accents_class.AsNoTracking().OrderBy(c=>c.id).ToArray();
                var ac = context.accent.AsNoTracking().ToArray();
                var flxs = context.flexes.AsNoTracking().ToArray();
                var wlist = context.words_list.AsNoTracking().Where(c => c.isdel == false).ToArray();
                var m_acc = context.minor_acc.AsNoTracking().ToArray();
                var lang = context.lang.AsNoTracking().ToArray();
                //context.Dispose();

                var resultContext = resultmphDb.getContext();
                resultContext.lang.AddRange(lang);
                resultContext.SaveChanges();

                resultContext.alphadigits.AddRange(ad);
                resultContext.SaveChanges();

                //resultContext.allangs.AddRange(allang);
                resultContext.SaveChanges();

                //foreach (var item in ac_cls)
                //{
                //    if (item.id == 0) continue;
                //    var e = new accents_class() { id = item.id, part_desc = item.part_desc };
                //    resultContext.accents_class.Add(item);
                //    resultContext.SaveChanges();
                //}
                resultContext.accents_class.AddRange(ac_cls);
                resultContext.SaveChanges();

                resultContext.accent.AddRange(ac);
                resultContext.SaveChanges();

                resultContext.grs.AddRange(gr);
                resultContext.SaveChanges();

                resultContext.parts.AddRange(parts);
                resultContext.SaveChanges();

                resultContext.indents.AddRange(indents);
                resultContext.SaveChanges();

                resultContext.flexes.AddRange(flxs);
                resultContext.SaveChanges();

                resultContext.words_list.AddRange(wlist);
                resultContext.SaveChanges();

                resultContext.minor_acc.AddRange(m_acc);
                resultContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(0), ex, ex.Message);
            }
            finally
            {
                if ((db == null) && (context != null)) context.Dispose();
            }
        }
        public void copyFromObj(mphODInfo o)
        {
            var context = getContext();

            try
            {
                this.removeData();

                context.alphadigits.AddRange(o.alphadigits);
                context.SaveChanges();

                context.lang.AddRange(o.lang);
                context.SaveChanges();

                //for(int i=0;i< o.accents_class.Length;i++)
                //{
                //    context.accents_class.Add(o.accents_class[i]);
                //}
                //context.SaveChanges();
                //context.accents_class.AddRange(o.accents_class);
                //context.SaveChanges();

                //context.accent.AddRange(o.accents);
                //context.SaveChanges();

                //context.grs.AddRange(o.grs.Select(c=>new Models.morph.gr() { id=c.id, part_of_speech = c.part_of_speech }));
                //context.SaveChanges();

                //context.parts.AddRange(o.parts);
                //context.SaveChanges();

                //context.indents.AddRange(o.indents);
                //context.SaveChanges();

                //context.flexes.AddRange(o.flexes);
                //context.SaveChanges();

                //context.words_list.AddRange(o.words_list);
                //context.SaveChanges();

                //context.minor_acc.AddRange(o.minor_accs);
                //context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(0), ex, ex.Message);
            }
            finally
            {
                if ((db == null) && (context != null)) context.Dispose();
            }
        }
        public mphODInfo getObj()
        {
            var context = getContext();
            mphODInfo dataObj = new mphODInfo();
            try
            {
                //var data = (from c in context.noms.Include(acc=>acc.accents_class).Include(part=>part.parts).Include(indent => indent.indents).Include(minoracc => minoracc.minor_acc).Include(flex=>flex.indents.flexes) where c.isactive == true && c.isdel == false orderby c.digit, c.field2, c.reestr select c);
                var data = (from c in context.words_list.AsNoTracking() where c.isdel == false orderby c.digit, c.field2, c.reestr select c);
                dataObj.words_list = data./*Take(5).*/ToArray();
                dataObj.types = (from c in context.indents.AsNoTracking() orderby c.type select c).ToArray(); // context.Local(context.indents).OrderBy(x => x.type).ToArray();
                dataObj.flexes = (from c in context.flexes.AsNoTracking() orderby c.type, c.ord select c)/*.Take(5)*/.ToArray(); // context.Local(context.flexes).OrderBy(x => x.type).ThenBy(y=>y.ord).ToArray();
                dataObj.grs = (from c in context.grs.AsNoTracking() orderby c.id select c).ToArray(); // context.Local(context.grs).OrderBy(x => x.id).ToArray();
                dataObj.accents = (from c in context.accent.AsNoTracking() orderby c.accent_type, c.gram select c)./*Take(5).*/ToArray(); // context.Local(context.accent).OrderBy(x => x.accent_type).ThenBy(y=>y.gram).ToArray();
                dataObj.parts = (from c in context.parts.AsNoTracking() orderby c.id select c).ToArray(); // context.Local(context.parts).OrderBy(x => x.id).ToArray();
                dataObj.accents_class = (from c in context.accents_class.AsNoTracking() orderby c.id select c).ToArray(); // context.Local(context.accents_class).OrderBy(x => x.id).ToArray();
                dataObj.minor_accs = (from c in context.minor_acc.AsNoTracking() orderby c.nom_old select c)./*Take(5).*/ToArray(); // context.Local(context.minor_acc).OrderBy(x => x.nom_old).ToArray();
                //dataObj.allangs = (from c in context.allangs.AsNoTracking() where c.id_lang == 1058 orderby c.full_lang select c).ToArray(); // context.allangs.OrderBy(x => x.full_lang).ToArray();
                dataObj.lang = (from c in context.lang.AsNoTracking() select c).ToArray(); // context.allangs.OrderBy(x => x.full_lang).ToArray();
                dataObj.alphadigits = (from c in context.alphadigits.AsNoTracking() orderby c.lang, c.digit, c.ls select c).ToArray(); // context.Local(context.alphadigits).OrderBy(x => x.lang).ThenBy(y=>y.digit).ThenBy(z => z.ls).ToArray();
                return dataObj;
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(0), ex, ex.Message);
                return null;
            }
            finally
            {
                if ((db == null) && (context != null)) context.Dispose();
            }
        }
    }
    public class mphODInfo
    {
        public word_param_base[] words_list { get; set; }
        public indents_base[] types { get; set; }
        public flexes_base[] flexes { get; set; }
        public gr_base[] grs { get; set; }
        public accent_base[] accents { get; set; }
        public parts_base[] parts { get; set; }
        public accents_class_base[] accents_class { get; set; }
        public minor_acc_base[] minor_accs { get; set; }
        //public allang[] allangs { get; set; }
        public langid[] lang { get; set; }
        public alphadigit[] alphadigits { get; set; }
    }
}
