// Copyright © 2016 uSofTrod. Contacts: <uSofTrod@outlook.com>
// License: http://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uSofTrod.TextUtility.txToolsCore
{
    public static class stringext
    {
        private static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static Stream ToStream(this string str)
        {
            Stream StringStream = new MemoryStream(str.GetBytes());
            //StringStream.Read(str.GetBytes(), 0, str.Length);
            return StringStream;
        }
    }
}
