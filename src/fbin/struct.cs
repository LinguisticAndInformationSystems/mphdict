using System;

namespace fbin
{
    /// <summary>
    /// 
    /// </summary>
    public struct fileHeader
    {
        public int signature;       // 0    сигнатура файлу
        public int v1;              // 4    
        public int v2;              // 8    
        public int fsize;           // 12   розмір файлу
        public int root;            // 16   адреса х-таблиці
        public int NumberOfItems;   // 20   кільк. елементів
        public int hsize;           // 24   розмір х-таблиці
        public ushort q;            // 28   число параметрів с ключем
        public ushort index;        // 30   номер індекса
        public int LangId;          // 32   
        public int p0;              // 36   
        public int p1;              // 40   
        public int p2;              // 44   
        public int p3;              // 48   
        public int p4;              // 52   
        public int p5;              // 56   
        public int p6;              // 60   
        public int p7;              // 64   
        public int p8;              // 68   
        public int p9;              // 72   
        public string dt;           // 76 DateTime.Now.ToString("ddMMyyyyHHmm"); (12 byte UTF8)
    }

    public struct HashPoint
    {
        public int hptr;
        public ushort qty;
    };
    public struct ListPoint
    {
        public int hptr;
        public int ln;
    };
    public struct MphHashPoint
    {
        public int hptr;
        public ushort qty;
        public ushort ln;
    };

    /// <summary>
    /// основа та її параметри (для класа CWBhashMrph)
    /// </summary>
    public class WordBasis
    {
        public int uni;
        public int nom;
        public short typeinfl;
        public short typeacc;
        public byte part;
        public byte own;
        public byte homonym;
        public char[] wbase;
        public WordBasis() { wbase = new char[255]; }
    };

    /// <summary>
    /// основа та її параметри (для класа CMWBhashMrph)
    /// </summary>
    public class mWordBasis
    {
        public int uni;
        public int nom;
        public short typeinfl;
        public short typeacc;
        public byte part;
        public byte own;
        public byte homonym;
        public char[] wbase;
    };

    /// <summary>
    /// масив основ в пам'яти (для класа CMPTypeMrph)
    /// </summary>
    public class WordBasisPtr
    {
        public ushort qty;
        public mWordBasis[] awbs;   // масив основ з параметрами
    }

    /// <summary>
    /// пар. клас
    /// </summary>
    public struct pClass
    {
        public short type;
        public byte part;
        public ushort qty;
    };

    /// <summary>
    /// // масив пар класів в пам'яти (для класа CMPTypeMrph)
    /// </summary>
    public class atype 
    {
        public byte part;
        public ushort qty;
        public Inflection[] aflx;    // масив флексій з параметрами
    }

    /// <summary>
    /// флексія та її параметри (для класа CMPTypeMrph)
    /// </summary>
    public class Inflection
    {
        public ushort flexnum;
        public ushort gr;
        public int f4;
        public byte sgn;
        public char[] flx;
    };

    /// <summary>
    /// флексія та її параметри (для класа CPTypeMrph)
    /// </summary>
    public class pInflection
    {
        public ushort flexnum;
        public ushort gr;
        public byte sgn;
        public int f4;
        public byte flen;
        public char[] flex;
        public pInflection() { flex = new char[255]; }
    };

    /// <summary>
    /// словоформа та її параметри
    /// </summary>
    public class WordForm
    {
        public int uni;
        public int ord;
        public byte homonym;
        public short type;
        public byte part;
        public short typeacc;
        public int acc;
        public ushort qty;
        public ushort gr;
        public ushort flexnum;
        public byte own;
        public byte sgn;
        public int f4;
        public char[] wf;
        public WordForm() { wf = new char[255]; }
    }

}
