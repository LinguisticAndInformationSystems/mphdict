// Copyright © 2016 uSofTrod. Contacts: <uSofTrod@outlook.com>
// License: http://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uSofTrod.TextUtility.txToolsCore
{
    public static class stringext
    {
        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static public string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static Stream ToStream(this string str)
        {
            Stream StringStream = new MemoryStream(str.GetBytes());
            //StringStream.Read(str.GetBytes(), 0, str.Length);
            return StringStream;
        }
    }
    //static public class binformat
    //{
    //    // Convert an object to a byte array
    //    public static byte[] SerializeObj(this object obj)
    //    {
    //        if (obj == null) return null;
    //        var bf = new BinaryFormatter();
    //        using (var ms = new MemoryStream())
    //        {
    //            bf.Serialize(ms, obj);
    //            return ms.ToArray();
    //        }
    //    }

    //    // Convert a byte array to an Object
    //    public static T DeserializeObj<T>(this byte[] byteArray) where T : class
    //    {
    //        if (byteArray == null) return null;
    //        using (var memStream = new MemoryStream())
    //        {
    //            var binForm = new BinaryFormatter();
    //            memStream.Write(byteArray, 0, byteArray.Length);
    //            memStream.Seek(0, SeekOrigin.Begin);
    //            var obj = (T)binForm.Deserialize(memStream);
    //            return obj;
    //        }
    //    }
    //}
    public static class GZip
    {
        public static byte[] decompress(byte[] b)
        {
            MemoryStream sourceStream = null;
            GZipStream decompressedStream = null;
            byte[] buffer = null;
            try
            {
                byte[] quartetBuffer = null;

                sourceStream = new MemoryStream(b);
                decompressedStream = new GZipStream(sourceStream, CompressionMode.Decompress, true);

                quartetBuffer = new byte[4];
                int position = (int)sourceStream.Length - 4;
                sourceStream.Position = position;
                sourceStream.Read(quartetBuffer, 0, 4);
                sourceStream.Position = 0;
                int checkLength = BitConverter.ToInt32(quartetBuffer, 0);
                int size = checkLength;

                buffer = new byte[size];

                int offset = 0;
                int total = 0;

                // Read the compressed data into the buffer
                while (true)
                {
                    int bytesRead = 0;
                    bytesRead = decompressedStream.Read(buffer, offset, size);

                    if (bytesRead == 0)
                        break;

                    offset += bytesRead;
                    total += bytesRead;
                    if (total == checkLength) break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Make sure we allways close all streams
                if (sourceStream != null)
                {
                    sourceStream.Dispose();
                    sourceStream = null;
                }

                if (decompressedStream != null)
                {
                    decompressedStream.Dispose();
                    decompressedStream = null;
                }
            }
            return buffer;
        }
        public static byte[] compress(byte[] buffer)
        {
            MemoryStream destinationStream = null;
            GZipStream compressedStream = null;
            destinationStream = new MemoryStream();
            compressedStream = new GZipStream(destinationStream, CompressionMode.Compress, true);
            compressedStream.Write(buffer, 0, buffer.Length);
            compressedStream.Dispose();
            byte[] r = destinationStream.ToArray();
            return r;
        }
    }

}
