using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Monocle
{
    public class Atlas : Texture
    {
        public Dictionary<string, Subtexture> SubTextures { get; private set; }

        private string xmlPath;

        public Atlas(string xmlPath, string imagePath, bool load)
        {
            XmlPath = xmlPath;
            ImagePath = imagePath;

            XmlDocument xml = Calc.LoadXML(XmlPath);
            XmlElement atlas = xml["TextureAtlas"];
            
            var subTextures = atlas.GetElementsByTagName("SubTexture");
            SubTextures = new Dictionary<string, Subtexture>(subTextures.Count);

            foreach (XmlElement subTexture in subTextures)
            {
                var a = subTexture.Attributes;
                SubTextures.Add(a["name"].Value, new Subtexture(
                    this,
                    Convert.ToInt32(a["x"].Value),
                    Convert.ToInt32(a["y"].Value),
                    Convert.ToInt32(a["width"].Value),
                    Convert.ToInt32(a["height"].Value))
                    );
            }

            if (load)
                Load();
        }

        public Subtexture this[string name]
        {
            get
            {
#if DEBUG
                if (!SubTextures.ContainsKey(name))
                    throw new Exception("SubTexture does not exist: " + name);
#endif
                return SubTextures[name];
            }
        }

        public Subtexture this[string name, Rectangle subRect]
        {
            get
            {
                return new Subtexture(this[name], subRect.X, subRect.Y, subRect.Width, subRect.Height);
            }
        }

        public bool Contains(string name)
        {
            return SubTextures.ContainsKey(name);
        }

        public string XmlPath
        {
            get { return xmlPath; }
            internal set
            {
                xmlPath = value;
#if DEBUG && DESKTOP
                if (!File.Exists(xmlPath))
                    throw new Exception("File does not exist: " + xmlPath);
#endif
            }
        }
    }
}
