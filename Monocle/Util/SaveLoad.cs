using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Monocle
{
    static public class SaveLoad
    {
        public enum SerializeModes { Binary, XML };

        /// <summary>
        /// Use the overload without compression for OS X portability!
        /// </summary>
        static public void SerializeToFile<T>(T obj, string filepath, SerializeModes mode, bool compressed)
        {
            FileStream fileStream = File.OpenWrite(filepath);
            GZipStream zipStream = null;
            Stream writeStream;

            //Compress
            if (compressed)
                writeStream = zipStream = new GZipStream(fileStream, CompressionMode.Compress, true);
            else
                writeStream = fileStream;

            //Serialize
            if (mode == SerializeModes.Binary)
            {
                var bf = new BinaryFormatter();
                bf.Serialize(writeStream, obj);
            }
            else if (mode == SerializeModes.XML)
            {
                var xs = new XmlSerializer(typeof(T));
                xs.Serialize(writeStream, obj);
            }

            //Cleanup
            if (zipStream != null)
            {
                zipStream.Close();
                zipStream.Dispose();
            }
            fileStream.Close();
            fileStream.Dispose();
        }

        /// <summary>
        /// Use the overload without compression for OS X portability!
        /// </summary>
        static public T DeserializeFromFile<T>(string filepath, SerializeModes mode, bool compressed)
        {
            T data;
            FileStream fileStream = File.OpenRead(filepath);
            GZipStream zipStream = null;
            Stream readStream;

            //Decompress
            if (compressed)
                readStream = zipStream = new GZipStream(fileStream, CompressionMode.Decompress, true);
            else
                readStream = fileStream;

            //Deserialize
            if (mode == SerializeModes.Binary)
            {
                var bf = new BinaryFormatter();
                data = (T)bf.Deserialize(readStream);
            }
            else
            {
                var xs = new XmlSerializer(typeof(T));
                data = (T)xs.Deserialize(readStream);
            }

            //Cleanup
            fileStream.Close();
            fileStream.Dispose();
            if (zipStream != null)
            {
                zipStream.Close();
                zipStream.Dispose();
            }

            return data;
        }

        static public void SerializeToFile<T>(T obj, string filepath, SerializeModes mode)
        {
            FileStream fileStream = File.OpenWrite(filepath);

            //Serialize
            if (mode == SerializeModes.Binary)
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fileStream, obj);
            }
            else if (mode == SerializeModes.XML)
            {
                var xs = new XmlSerializer(typeof(T));
                xs.Serialize(fileStream, obj);
            }

            //Cleanup
            fileStream.Close();
            fileStream.Dispose();
        }

        static public T DeserializeFromFile<T>(string filepath, SerializeModes mode)
        {
            T data;
            FileStream fileStream = File.OpenRead(filepath);

            //Deserialize
            if (mode == SerializeModes.Binary)
            {
                var bf = new BinaryFormatter();
                data = (T)bf.Deserialize(fileStream);
            }
            else
            {
                var xs = new XmlSerializer(typeof(T));
                data = (T)xs.Deserialize(fileStream);
            }

            //Cleanup
            fileStream.Close();
            fileStream.Dispose();

            return data;
        }

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
