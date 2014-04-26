using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Monocle
{
    public class SpriteData
    {
        private Atlas atlas;
        private Dictionary<string, XmlElement> sprites;

        public SpriteData(string filename, Atlas atlas)
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

        public Sprite<string> GetSpriteString(string id)
        {
            XmlElement xml = sprites[id];

            Sprite<string> sprite = new Sprite<string>(atlas[xml.ChildText("Texture")], xml.ChildInt("FrameWidth"), xml.ChildInt("FrameHeight"));
            sprite.Origin = new Vector2(xml.ChildFloat("OriginX", 0), xml.ChildFloat("OriginY", 0));
            sprite.Position = new Vector2(xml.ChildFloat("X", 0), xml.ChildFloat("Y", 0));
            sprite.Color = xml.ChildHexColor("Color", Color.White);

            XmlElement anims = xml["Animations"];
            if (anims != null)
                foreach (XmlElement anim in anims.GetElementsByTagName("Anim"))
                    sprite.Add(anim.Attr("id"), anim.AttrFloat("delay", 0), anim.AttrBool("loop", true), Calc.ReadCSVInt(anim.Attr("frames"))); 

            return sprite;
        }

        public Sprite<int> GetSpriteInt(string id)
        {
            XmlElement xml = sprites[id];

            Sprite<int> sprite = new Sprite<int>(atlas[xml.ChildText("Texture")], xml.ChildInt("FrameWidth"), xml.ChildInt("FrameHeight"));
            sprite.Origin = new Vector2(xml.ChildFloat("OriginX", 0), xml.ChildFloat("OriginY", 0));
            sprite.Position = new Vector2(xml.ChildFloat("X", 0), xml.ChildFloat("Y", 0));
            sprite.Color = xml.ChildHexColor("Color", Color.White);

            XmlElement anims = xml["Animations"];
            if (anims != null)
                foreach (XmlElement anim in anims.GetElementsByTagName("Anim"))
                    sprite.Add(anim.AttrInt("id"), anim.AttrFloat("delay", 0), anim.AttrBool("loop", true), Calc.ReadCSVInt(anim.Attr("frames")));

            return sprite;
        }

        public MotionBlurSprite<int> GetMotionBlurSpriteInt(string id, int blurs)
        {
            XmlElement xml = sprites[id];

            MotionBlurSprite<int> sprite = new MotionBlurSprite<int>(atlas[xml.ChildText("Texture")], xml.ChildInt("FrameWidth"), xml.ChildInt("FrameHeight"), blurs);
            sprite.Origin = new Vector2(xml.ChildFloat("OriginX", 0), xml.ChildFloat("OriginY", 0));
            sprite.Position = new Vector2(xml.ChildFloat("X", 0), xml.ChildFloat("Y", 0));
            sprite.Color = xml.ChildHexColor("Color", Color.White);

            XmlElement anims = xml["Animations"];
            if (anims != null)
                foreach (XmlElement anim in anims.GetElementsByTagName("Anim"))
                    sprite.Add(anim.AttrInt("id"), anim.AttrFloat("delay", 0), anim.AttrBool("loop", true), Calc.ReadCSVInt(anim.Attr("frames")));

            return sprite;
        }

        public SpritePart<int> GetSpritePartInt(string id)
        {
            XmlElement xml = sprites[id];

            SpritePart<int> sprite = new SpritePart<int>(atlas[xml.ChildText("Texture")], xml.ChildInt("FrameWidth"), xml.ChildInt("FrameHeight"));
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

            if (xml.Name != "image")
                image.ClipRect = new Rectangle(image.ClipRect.X, image.ClipRect.Y, xml.ChildInt("FrameWidth"), xml.ChildInt("FrameHeight"));

            return image;
        }

        public Image GetAutoDetect(string id)
        {
            XmlElement xml = sprites[id];

            switch (xml.Name)
            {
                case "image":
                    return GetImage(id);
                case "sprite_int":
                    return GetSpriteInt(id);
                case "sprite_string":
                    return GetSpriteString(id);
                default:
                    throw new Exception("Sprite type '" + xml.Name + "' not recognized for auto-detect!");
            }
        }

    }
}
