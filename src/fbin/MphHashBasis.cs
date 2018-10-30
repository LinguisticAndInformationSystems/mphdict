using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace fbin
{
    public class MphHashBasis : FileHeader
    {
        protected int[] hashsizes ={251,509,1021,2039,4093,8191,16381,32749,65521,131071,262139,524287,1048573,2097143,4194301,
            8388593,16777213,33554393,67108859,134217689,268435399,536870909,1073741789,2147483647};
        protected int hsize;
        protected MphHashPoint[] hashptrs;
        public MphHashBasis(string fn) : base(fn)
        {
            hashptrs = new MphHashPoint[fhead.hsize];  //fhead->var01 - размер хеш-таблицы
            lock (this)
            {
                fp.BaseStream.Seek(fhead.root, SeekOrigin.Begin);
                for (int i = 0; i < fhead.hsize; i++)
                {
                    hashptrs[i].hptr = fp.ReadInt32();
                    hashptrs[i].qty = fp.ReadUInt16();
                    hashptrs[i].ln = fp.ReadUInt16();
                }
            }
            if (fhead.signature != 9305)
            {
                string e = "mismatch file type (9305) \n";
                e += fn;
                Dispose();
                //throw e;
            }
        }
        override public void Dispose()
        {
            hashptrs = null;
            base.Dispose();
        }
        public virtual void printAlloc()
        {
            FileStream fs = null;   //FILE *fp,*fptemp;
            BinaryWriter fpt = null;
            fs = new FileStream("hashalloc.log", FileMode.Create);
            Encoding unicode = Encoding.Unicode;
            fpt = new BinaryWriter(fs, unicode);
            string tmp;
            for (int i = 0; i < fhead.hsize; i++)
            {
                tmp = i.ToString();
                tmp += "\t";
                tmp += hashptrs[i].qty.ToString();
                tmp += "\r\n";
                for (int j = 0; j < tmp.Length; j++)
                    fpt.Write(tmp[j]);
            }
            fpt.Close();
        }
        public long lhash(long v, long M)
        {
            return v % M;
        }

        public int whash(string v, int M)
        {
            int h, i, a = 31415, b = 27183;
            for (h = 0, i = 0; i < v.Length; i++, a = a * b % (M - 1))
                h = (a * h + v[i]) % M;
            return (h < 0) ? (h + M) : h;
        }
        public int chash(char[] v, int M)
        {
            int h, i, a = 31415, b = 27183;
            for (h = 0, i = 0; v[i] != '\0'; i++, a = a * b % (M - 1))
                h = (a * h + v[i]) % M;
            return (h < 0) ? (h + M) : h;
        }
    }
}
