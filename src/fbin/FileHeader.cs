using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace fbin
{
    public class FileHeader : IDisposable
    {

        protected string fname;
        protected Stream fstream = null;
        protected BinaryReader fp = null;
        public fileHeader fhead;
        public byte[] atype;    // масив типів
        public FileHeader()
        {
            fhead = new fileHeader();
            atype = null;
            fname = null;
            fp = null;
        }
        public FileHeader(string fn)
        {
            atype = null;
            fname = null;
            if (init(fn) == 0)
            {
                //throw fname;
            }
        }
        public virtual void Dispose()
        {
            deprov();
        }
        //virtual ~CFHead(void);

        public virtual int init(string fn)
        {
            if (fname != null)
            {
                deprov();
            }
            fname = fn;
            try
            {
                fstream = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read);
                Encoding unicode = Encoding.UTF8;
                fp = new BinaryReader(fstream, unicode);
            }
            catch (Exception ex)
            {
                fname = null;
                fp = null;
                throw ex;
            }
            readFHeading(ref fhead);
            return 1;
        }
        public virtual void deprov()
        {
            atype = null;
            fname = null;
            if (fp != null)
            {
                fp.Close();
                fp = null;
            }
            if (fstream != null)
            {
                fstream.Close();
                fstream = null;
            }
        }
        public virtual void readFHeading(ref fileHeader fh)
        {
            fh.signature = fp.ReadInt32();
            fh.v1 = fp.ReadInt32();
            fh.v2 = fp.ReadInt32();
            fh.fsize = fp.ReadInt32();
            fh.root = fp.ReadInt32();
            fh.NumberOfItems = fp.ReadInt32();
            fh.hsize = fp.ReadInt32();

            if (fh.signature != 9305)    // якщо це не файл морфології - читаємо тип ключа та параметрів
            { 
                fh.q = fp.ReadUInt16();     // число параметрів з ключем
                fh.index = fp.ReadUInt16(); // номер індекса
                atype = fp.ReadBytes(fh.q);
                /* ===================================== */
            }

            fh.LangId = fp.ReadInt32();
            fh.p0 = fp.ReadInt32();
            fh.p1 = fp.ReadInt32();
            fh.p2 = fp.ReadInt32();
            fh.p3 = fp.ReadInt32();
            fh.p4 = fp.ReadInt32();
            fh.p5 = fp.ReadInt32();
            fh.p6 = fp.ReadInt32();
            fh.p7 = fp.ReadInt32();
            fh.p8 = fp.ReadInt32();
            fh.p9 = fp.ReadInt32();
            fh.dt = new string(fp.ReadChars(12));
        }

        public virtual byte[] GetArrayType()
        {
            return atype;
        }
        public virtual long GetLen()
        {
            return fhead.NumberOfItems;
        }
        public virtual ushort GetNParameter()
        {
            return fhead.q;
        }
        public virtual ushort GetNIndex()
        {
            return fhead.index;
        }
        public virtual long GetHSize()
        {
            return fhead.hsize;
        }
        public virtual long GetParameter(byte n)
        {
            switch (n)
            {
                case 0:
                    return fhead.p0;
                case 1:
                    return fhead.p1;
                case 2:
                    return fhead.p2;
                case 3:
                    return fhead.p3;
                case 4:
                    return fhead.p4;
                case 5:
                    return fhead.p5;
                case 6:
                    return fhead.p6;
                case 7:
                    return fhead.p7;
                case 8:
                    return fhead.p8;
                case 9:
                    return fhead.p9;
            }
            return 0;
        }
        public virtual long GetRoot()
        {
            return fhead.root;
        }

        public virtual void getinfo(out string s)
        {
            s = "<mphdict>\r\n";

            s += "<sign>\r\n";
            s += fhead.signature.ToString();
            s += "\r\n</sign>\r\n";

            s += "<ver>\r\n";
            s += fhead.v1.ToString();
            s += ".";
            s += fhead.v2.ToString();
            s += "\r\n</ver>\r\n";

            s += "<time>\r\n";
            s += fhead.dt;
            s += "\r\n</time>\r\n";

            s += "<fsize>\r\n";
            s += fhead.fsize.ToString();
            s += "\r\n</fsize>\r\n";

            s += "<tsize>\r\n";
            s += fhead.NumberOfItems.ToString();
            s += "\r\n</tsize>\r\n";

            s += "<lang_id>\r\n";
            s += fhead.LangId.ToString();
            s += "\r\n</lang_id>\r\n";

            s += "</mphdict>\r\n";
        }
        public virtual string getfname()
        {
            return fname;
        }
        public virtual byte[] getTypes()
        {
            return atype;
        }
        public int getLangID()
        {
            return fhead.LangId;
        }
    };
}
