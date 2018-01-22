using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Monocle
{
    public static class SaveLoad
    {
        public enum SerializeModes { Binary, XML };

        #region Save

        /// <summary>
        /// Save an object to a file so you can load it later
        /// </summary>
        public static void SerializeToFile<T>(T obj, string filepath, SerializeModes mode)
        {
            using (var fileStream = new FileStream(filepath, FileMode.Create))
            {
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
            }
        }

        /// <summary>
        /// Save an object to a file so you can load it later.
        /// Will not crash if the save fails
        /// </summary>
        /// <returns>Whether the save succeeded</returns>
        public static bool SafeSerializeToFile<T>(T obj, string filepath, SerializeModes mode)
        {
            try
            {
                SerializeToFile<T>(obj, filepath, mode);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Load

        /// <summary>
        /// Load an object that was previously serialized to a file
        /// </summary>
        public static T DeserializeFromFile<T>(string filepath, SerializeModes mode)
        {
            T data;
            using (var fileStream = File.OpenRead(filepath))
            {

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
            }

            return data;
        }

        /// <summary>
        /// Load an object that was previously serialized to a file
        /// If the load fails or the file does not exist, default(T) will be returned
        /// </summary>
        public static T SafeDeserializeFromFile<T>(string filepath, SerializeModes mode, bool debugUnsafe = false)
        {
            if (File.Exists(filepath))
            {
                if (debugUnsafe)
                    return SaveLoad.DeserializeFromFile<T>(filepath, mode);
                else
                {
                    try
                    {
                        return SaveLoad.DeserializeFromFile<T>(filepath, mode);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
            }
            else
                return default(T);
        }

        /// <summary>
        /// Load an object that was previously serialized to a file
        /// If the load fails or the file does not exist, default(T) will be returned
        /// </summary>
        /// <param name="loadError">True if the load fails despite the requested file existing (for example due to corrupted data)</param>
        public static T SafeDeserializeFromFile<T>(string filepath, SerializeModes mode, out bool loadError, bool debugUnsafe = false)
        {
            if (File.Exists(filepath))
            {
                if (debugUnsafe)
                {
                    loadError = false;
                    return SaveLoad.DeserializeFromFile<T>(filepath, mode);
                }
                else
                {
                    try
                    {
                        loadError = false;
                        return SaveLoad.DeserializeFromFile<T>(filepath, mode);
                    }
                    catch
                    {
                        loadError = true;
                        return default(T);
                    }
                }
            }
            else
            {
                loadError = false;
                return default(T);
            }
        }

        #endregion
    }
}
