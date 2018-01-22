using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Monocle
{
    public class Atlas
    {
        public List<Texture2D> Sources;
        private Dictionary<string, MTexture> textures = new Dictionary<string, MTexture>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<MTexture>> orderedTexturesCache = new Dictionary<string, List<MTexture>>();

        public enum AtlasDataFormat
        {
            TexturePacker_Sparrow,
            CrunchXml,
            CrunchBinary,
            CrunchXmlOrBinary,
            CrunchBinaryNoAtlas,
            Packer,
            PackerNoAtlas
        };

        public static Atlas FromAtlas(string path, AtlasDataFormat format)
        {
            var atlas = new Atlas();
            atlas.Sources = new List<Texture2D>();
            ReadAtlasData(atlas, path, format);
            return atlas;
        }

        private static void ReadAtlasData(Atlas atlas, string path, AtlasDataFormat format)
        {
            switch (format)
            {
                case AtlasDataFormat.TexturePacker_Sparrow:
                    {
                        XmlDocument xml = Calc.LoadContentXML(path);
                        XmlElement at = xml["TextureAtlas"];

                        var texturePath = at.Attr("imagePath", "");
                        var fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(path), texturePath), FileMode.Open, FileAccess.Read);
                        var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                        fileStream.Close();

                        var mTexture = new MTexture(texture);
                        atlas.Sources.Add(texture);

                        var subtextures = at.GetElementsByTagName("SubTexture");

                        foreach (XmlElement sub in subtextures)
                        {
                            var name = sub.Attr("name");
                            var clipRect = sub.Rect();
                            if (sub.HasAttr("frameX"))
                                atlas.textures[name] = new MTexture(mTexture, name, clipRect, new Vector2(-sub.AttrInt("frameX"), -sub.AttrInt("frameY")), sub.AttrInt("frameWidth"), sub.AttrInt("frameHeight"));
                            else
                                atlas.textures[name] = new MTexture(mTexture, name, clipRect);
                        }
                    }
                    break;
                case AtlasDataFormat.CrunchXml:
                    {
                        XmlDocument xml = Calc.LoadContentXML(path);
                        XmlElement at = xml["atlas"];

                        foreach (XmlElement tex in at)
                        {
                            var texturePath = tex.Attr("n", "");
                            var fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(path), texturePath + ".png"), FileMode.Open, FileAccess.Read);
                            var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                            fileStream.Close();

                            var mTexture = new MTexture(texture);
                            atlas.Sources.Add(texture);

                            foreach (XmlElement sub in tex)
                            {
                                var name = sub.Attr("n");
                                var clipRect = new Rectangle(sub.AttrInt("x"), sub.AttrInt("y"), sub.AttrInt("w"), sub.AttrInt("h"));
                                if (sub.HasAttr("fx"))
                                    atlas.textures[name] = new MTexture(mTexture, name, clipRect, new Vector2(-sub.AttrInt("fx"), -sub.AttrInt("fy")), sub.AttrInt("fw"), sub.AttrInt("fh"));
                                else
                                    atlas.textures[name] = new MTexture(mTexture, name, clipRect);
                            }
                        }
                    }
                    break;

                case AtlasDataFormat.CrunchBinary:
                    using (var stream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path)))
                    {
                        var reader = new BinaryReader(stream);
                        var textures = reader.ReadInt16();

                        for (int i = 0; i < textures; i++)
                        {
                            var textureName = reader.ReadNullTerminatedString();
                            var texturePath = Path.Combine(Path.GetDirectoryName(path), textureName + ".png");
                            var fileStream = new FileStream(texturePath, FileMode.Open, FileAccess.Read);
                            var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                            fileStream.Close();

                            atlas.Sources.Add(texture);

                            var mTexture = new MTexture(texture);
                            var subtextures = reader.ReadInt16();
                            for (int j = 0; j < subtextures; j++)
                            {
                                var name = reader.ReadNullTerminatedString();
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var w = reader.ReadInt16();
                                var h = reader.ReadInt16();
                                var fx = reader.ReadInt16();
                                var fy = reader.ReadInt16();
                                var fw = reader.ReadInt16();
                                var fh = reader.ReadInt16();
                                
                                atlas.textures[name] = new MTexture(mTexture, name, new Rectangle(x, y, w, h), new Vector2(-fx, -fy), fw, fh);
                            }
                        }
                    }
                    break;

                case AtlasDataFormat.CrunchBinaryNoAtlas:
                    using (var stream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".bin")))
                    {
                        var reader = new BinaryReader(stream);
                        var folders = reader.ReadInt16();

                        for (int i = 0; i < folders; i++)
                        {
                            var folderName = reader.ReadNullTerminatedString();
                            var folderPath = Path.Combine(Path.GetDirectoryName(path), folderName);

                            var subtextures = reader.ReadInt16();
                            for (int j = 0; j < subtextures; j++)
                            {
                                var name = reader.ReadNullTerminatedString();
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var w = reader.ReadInt16();
                                var h = reader.ReadInt16();
                                var fx = reader.ReadInt16();
                                var fy = reader.ReadInt16();
                                var fw = reader.ReadInt16();
                                var fh = reader.ReadInt16();

                                var fileStream = new FileStream(Path.Combine(folderPath, name + ".png"), FileMode.Open, FileAccess.Read);
                                var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                                fileStream.Close();

                                atlas.Sources.Add(texture);
                                atlas.textures[name] = new MTexture(texture, new Vector2(-fx, -fy), fw, fh);
                            }
                        }
                    }
                    break;

                case AtlasDataFormat.Packer:

                    using (var stream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
                    {
                        var reader = new BinaryReader(stream);
                        reader.ReadInt32(); // version
                        reader.ReadString(); // args
                        reader.ReadInt32(); // hash

                        var textures = reader.ReadInt16();
                        for (int i = 0; i < textures; i++)
                        {
                            var textureName = reader.ReadString();
                            var texturePath = Path.Combine(Path.GetDirectoryName(path), textureName + ".data");
                            var fileStream = new FileStream(texturePath, FileMode.Open, FileAccess.Read);
                            var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                            fileStream.Close();

                            atlas.Sources.Add(texture);

                            var mTexture = new MTexture(texture);
                            var subtextures = reader.ReadInt16();
                            for (int j = 0; j < subtextures; j++)
                            {
                                var name = reader.ReadString().Replace('\\', '/');
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var w = reader.ReadInt16();
                                var h = reader.ReadInt16();
                                var fx = reader.ReadInt16();
                                var fy = reader.ReadInt16();
                                var fw = reader.ReadInt16();
                                var fh = reader.ReadInt16();

                                atlas.textures[name] = new MTexture(mTexture, name, new Rectangle(x, y, w, h), new Vector2(-fx, -fy), fw, fh);
                            }
                        }
                    }

                    break;

                case AtlasDataFormat.PackerNoAtlas:
                    using (var stream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
                    {
                        var reader = new BinaryReader(stream);
                        reader.ReadInt32(); // version
                        reader.ReadString(); // args
                        reader.ReadInt32(); // hash

                        var folders = reader.ReadInt16();
                        for (int i = 0; i < folders; i++)
                        {
                            var folderName = reader.ReadString();
                            var folderPath = Path.Combine(Path.GetDirectoryName(path), folderName);

                            var subtextures = reader.ReadInt16();
                            for (int j = 0; j < subtextures; j++)
                            {
                                var name = reader.ReadString().Replace('\\', '/');
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var w = reader.ReadInt16();
                                var h = reader.ReadInt16();
                                var fx = reader.ReadInt16();
                                var fy = reader.ReadInt16();
                                var fw = reader.ReadInt16();
                                var fh = reader.ReadInt16();

                                var fileStream = new FileStream(Path.Combine(folderPath, name + ".data"), FileMode.Open, FileAccess.Read);
                                var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                                fileStream.Close();

                                atlas.Sources.Add(texture);
                                atlas.textures[name] = new MTexture(texture, new Vector2(-fx, -fy), fw, fh);
                            }
                        }
                    }
                    break;

                case AtlasDataFormat.CrunchXmlOrBinary:

                    if (File.Exists(Path.Combine(Engine.ContentDirectory, path + ".bin")))
                        ReadAtlasData(atlas, path + ".bin", AtlasDataFormat.CrunchBinary);
                    else
                        ReadAtlasData(atlas, path + ".xml", AtlasDataFormat.CrunchXml);

                    break;


                default:
                    throw new NotImplementedException();
            }
        }

        public static Atlas FromMultiAtlas(string rootPath, string[] dataPath, AtlasDataFormat format)
        {
            var atlas = new Atlas();
            atlas.Sources = new List<Texture2D>();
            
            for (int i = 0; i < dataPath.Length; i ++)
                ReadAtlasData(atlas, Path.Combine(rootPath, dataPath[i]), format);

            return atlas;
        }

        public static Atlas FromMultiAtlas(string rootPath, string filename, AtlasDataFormat format)
        {
            var atlas = new Atlas();
            atlas.Sources = new List<Texture2D>();

            var index = 0;
            while (true)
            {
                var dataPath = Path.Combine(rootPath, filename + index.ToString() + ".xml");

                if (!File.Exists(Path.Combine(Engine.ContentDirectory, dataPath)))
                    break;
                
                ReadAtlasData(atlas, dataPath, format);
                index++;
            }

            return atlas;
        }

        public static Atlas FromDirectory(string path)
        {
            var atlas = new Atlas();
            atlas.Sources = new List<Texture2D>();

            var contentDirectory = Engine.ContentDirectory;
            var contentDirectoryLength = contentDirectory.Length;
            var contentPath = Path.Combine(contentDirectory, path);
            var contentPathLength = contentPath.Length;

            foreach (var file in Directory.GetFiles(contentPath, "*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(file);
                if (ext != ".png" && ext != ".xnb")
                    continue;

                // get path and load
                var fileStream = new FileStream(file.Substring(contentDirectoryLength + 1), FileMode.Open, FileAccess.Read);
                var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                fileStream.Close();

                atlas.Sources.Add(texture);

                // make nice for dictionary
                var filepath = file.Substring(contentPathLength + 1);
                filepath = filepath.Substring(0, filepath.Length - 4);
                filepath = filepath.Replace('\\', '/');

                // load
                atlas.textures.Add(filepath, new MTexture(texture));
            }

            return atlas;
        }

        public MTexture this[string id]
        {
            get { return textures[id]; }
            set { textures[id] = value; }
        }

        public bool Has(string id)
        {
            return textures.ContainsKey(id);
        }

        public MTexture GetOrDefault(string id, MTexture defaultTexture)
        {
            if (String.IsNullOrEmpty(id) || !Has(id))
                return defaultTexture;
            return textures[id];
        }

        public List<MTexture> GetAtlasSubtextures(string key)
        {
            List<MTexture> list;

            if (!orderedTexturesCache.TryGetValue(key, out list))
            {
                list = new List<MTexture>();

                var index = 0;
                while (true)
                {
                    var texture = GetAtlasSubtextureFromAtlasAt(key, index);
                    if (texture != null)
                        list.Add(texture);
                    else
                        break;
                    index++;
                }

                orderedTexturesCache.Add(key, list);
            }

            return list;
        }

        private MTexture GetAtlasSubtextureFromCacheAt(string key, int index)
        {
            return orderedTexturesCache[key][index];
        }

        private MTexture GetAtlasSubtextureFromAtlasAt(string key, int index)
        {
            if (index == 0 && textures.ContainsKey(key))
                return textures[key];

            var indexString = index.ToString();
            var startLength = indexString.Length;
            while (indexString.Length < startLength + 6)
            {
                MTexture result;
                if (textures.TryGetValue(key + indexString, out result))
                    return result;
                indexString = "0" + indexString;
            }

            return null;
        }

        public MTexture GetAtlasSubtexturesAt(string key, int index)
        {
            List<MTexture> list;
            if (orderedTexturesCache.TryGetValue(key, out list))
                return list[index];
            else
                return GetAtlasSubtextureFromAtlasAt(key, index);
        }

        public void Dispose()
        {
            foreach (var texture in Sources)
                texture.Dispose();
            Sources.Clear();
            textures.Clear();
        }
    

    }
}
