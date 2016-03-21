using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Monocle
{
    public class SpriteData
    {
        private MTexture atlas;
        private Dictionary<string, XmlElement> sprites;

        public SpriteData(string filename, MTexture atlas)
        {
            this.atlas = atlas;

            XmlDocument xml = Calc.LoadXML(filename);
            sprites = new Dictionary<string, XmlElement>();
            foreach (var e in xml["SpriteData"])
                if (e is XmlElement)
                    sprites.Add((e as XmlElement).Attr("id"), e as XmlElement);
        }

        public bool Contains(string id)
        {
            return sprites.ContainsKey(id);
        }

        public XmlElement GetXML(string id)
        {
            return sprites[id];
        }

        public Spritesheet<string> GetSpritesheetString(string id)
        {
            XmlElement xml = sprites[id];

            Spritesheet<string> sprite = new Spritesheet<string>(atlas[xml.ChildText("Texture")], xml.ChildInt("FrameWidth"), xml.ChildInt("FrameHeight"));
            sprite.Origin = new Vector2(xml.ChildFloat("OriginX", 0), xml.ChildFloat("OriginY", 0));
            sprite.Position = new Vector2(xml.ChildFloat("X", 0), xml.ChildFloat("Y", 0));
            sprite.Color = xml.ChildHexColor("Color", Color.White);

            XmlElement anims = xml["Animations"];
            if (anims != null)
                foreach (XmlElement anim in anims.GetElementsByTagName("Anim"))
                    sprite.Add(anim.Attr("id"), anim.AttrFloat("delay", 0), anim.AttrBool("loop", true), Calc.ReadCSVInt(anim.Attr("frames"))); 

            return sprite;
        }

        public Spritesheet<int> GetSpritesheetInt(string id)
        {
            XmlElement xml = sprites[id];

            Spritesheet<int> sprite = new Spritesheet<int>(atlas[xml.ChildText("Texture")], xml.ChildInt("FrameWidth"), xml.ChildInt("FrameHeight"));
            sprite.Origin = new Vector2(xml.ChildFloat("OriginX", 0), xml.ChildFloat("OriginY", 0));
            sprite.Position = new Vector2(xml.ChildFloat("X", 0), xml.ChildFloat("Y", 0));
            sprite.Color = xml.ChildHexColor("Color", Color.White);

            XmlElement anims = xml["Animations"];
            if (anims != null)
                foreach (XmlElement anim in anims.GetElementsByTagName("Anim"))
                    sprite.Add(anim.AttrInt("id"), anim.AttrFloat("delay", 0), anim.AttrBool("loop", true), Calc.ReadCSVInt(anim.Attr("frames")));

            return sprite;
        }

        public Image GetImage(string id)
        {
            XmlElement xml = sprites[id];

            Image image = new Image(atlas[xml.ChildText("Texture")]);
            image.Origin = new Vector2(xml.ChildFloat("OriginX", 0), xml.ChildFloat("OriginY", 0));
            image.Position = new Vector2(xml.ChildFloat("X", 0), xml.ChildFloat("Y", 0));
            image.Color = xml.ChildHexColor("Color", Color.White);

            return image;
        }

        public Image GetAutoDetect(string id)
        {
            XmlElement xml = sprites[id];

            switch (xml.Name)
            {
                case "image":
                    return GetImage(id);
                case "spritesheet_int":
                    return GetSpritesheetInt(id);
                case "spritesheet_string":
                    return GetSpritesheetString(id);
                default:
                    throw new Exception("Image type '" + xml.Name + "' not recognized for auto-detect!");
            }
        }

    }
}
