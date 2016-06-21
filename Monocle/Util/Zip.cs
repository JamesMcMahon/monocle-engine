using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    static public class Zip
    {
        // Adapted from code found at: http://www.codeproject.com/Articles/27203/GZipStream-Compress-Decompress-a-string

        static public string CompressString(string str)
        {
            //Convert to byte array
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
                bytes[i] = (byte)str[i];

            //Create streams
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress);

            //Compress
            zip.Write(bytes, 0, bytes.Length);
            zip.Close();

            //Convert back to string
            bytes = ms.ToArray();
            StringBuilder sb = new StringBuilder(bytes.Length);
            foreach (var b in bytes)
                sb.Append((char)b);

            //Cleanup
            ms.Close();
            zip.Dispose();
            ms.Dispose();

            return sb.ToString();
        }

        static public string DecompressString(string str)
        {
            //Convert to byte array
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)str[i];

            //Create Streams
            MemoryStream ms = new MemoryStream(bytes);
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);

            //Decompress
            bytes = new byte[bytes.Length];
            int amount = zip.Read(bytes, 0, bytes.Length);

            //Convert back to string
            StringBuilder sb = new StringBuilder(amount);
            for (int i = 0; i < amount; i++)
                sb.Append((char)bytes[i]);

            //Cleanup
            zip.Close();
            ms.Close();
            zip.Dispose();
            ms.Dispose();

            return sb.ToString();
        }
    }
}
