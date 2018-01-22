using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Monocle
{
    public class SpriteDataSource
    {
        public XmlElement XML;
        public string Path;
        public string OverridePath;
    }

    public class SpriteData
    {
        public List<SpriteDataSource> Sources = new List<SpriteDataSource>();
        public Sprite Sprite;
        public Atlas Atlas;

        public SpriteData(Atlas atlas)
        {
            Sprite = new Sprite(atlas, "");
            Atlas = atlas;
        }

        public void Add(XmlElement xml, string overridePath = null)
        {
            var source = new SpriteDataSource();
            source.XML = xml;
            source.Path = source.XML.Attr("path");
            source.OverridePath = overridePath;

            //Error Checking
            {
                var prefix = "Sprite '" + source.XML.Name + "': ";

                //Path
                if (!source.XML.HasAttr("path") && string.IsNullOrEmpty(overridePath))
                    throw new Exception(prefix + "'path' is missing!");

                //Anims
                var ids = new HashSet<string>();
                foreach (XmlElement anim in source.XML.GetElementsByTagName("Anim"))
                    CheckAnimXML(anim, prefix, ids);
                foreach (XmlElement loop in source.XML.GetElementsByTagName("Loop"))
                    CheckAnimXML(loop, prefix, ids);

                //Start
                if (source.XML.HasAttr("start") && !ids.Contains(source.XML.Attr("start")))
                    throw new Exception(prefix + "starting animation '" + source.XML.Attr("start") + "' is missing!");

                //Origin
                if (source.XML.HasChild("Justify") && source.XML.HasChild("Origin"))
                    throw new Exception(prefix + "has both Origin and Justify tags!");
            }

            //Create the Sprite
            {
                var normalPath = source.XML.Attr("path", "");
                var masterDelay = source.XML.AttrFloat("delay", 0);

                //Build Animations
                foreach (XmlElement anim in source.XML.GetElementsByTagName("Anim"))
                {
                    Chooser<string> into;
                    if (anim.HasAttr("goto"))
                        into = Chooser<string>.FromString<string>(anim.Attr("goto"));
                    else
                        into = null;

                    var id = anim.Attr("id");
                    var path = anim.Attr("path", "");
                    var frames = Calc.ReadCSVIntWithTricks(anim.Attr("frames", ""));

                    if (!string.IsNullOrEmpty(overridePath) && HasFrames(Atlas, overridePath + path, frames))
                        path = overridePath + path;
                    else
                        path = normalPath + path;
                    
                    Sprite.Add(id, path, anim.AttrFloat("delay", masterDelay), into, frames);
                }

                //Build Loops
                foreach (XmlElement loop in source.XML.GetElementsByTagName("Loop"))
                {
                    var id = loop.Attr("id");
                    var path = loop.Attr("path", "");
                    var frames = Calc.ReadCSVIntWithTricks(loop.Attr("frames", ""));

                    if (!string.IsNullOrEmpty(overridePath) && HasFrames(Atlas, overridePath + path, frames))
                        path = overridePath + path;
                    else
                        path = normalPath + path;
                    
                    Sprite.AddLoop(id, path, loop.AttrFloat("delay", masterDelay), frames);
                }

                //Origin
                if (source.XML.HasChild("Center"))
                {
                    Sprite.CenterOrigin();
                    Sprite.Justify = new Vector2(.5f, .5f);
                }
                else if (source.XML.HasChild("Justify"))
                {
                    Sprite.JustifyOrigin(source.XML.ChildPosition("Justify"));
                    Sprite.Justify = source.XML.ChildPosition("Justify");
                }
                else if (source.XML.HasChild("Origin"))
                    Sprite.Origin = source.XML.ChildPosition("Origin");

                //Position
                if (source.XML.HasChild("Position"))
                    Sprite.Position = source.XML.ChildPosition("Position");

                //Start Animation
                if (source.XML.HasAttr("start"))
                    Sprite.Play(source.XML.Attr("start"));
            }

            Sources.Add(source);
        }

        private bool HasFrames(Atlas atlas, string path, int[] frames = null)
        {
            if (frames == null || frames.Length <= 0)
                return atlas.GetAtlasSubtexturesAt(path, 0) != null;
            else
            {
                for (int i = 0; i < frames.Length; i++)
                    if (atlas.GetAtlasSubtexturesAt(path, frames[i]) == null)
                        return false;

                return true;
            }
        }

        private void CheckAnimXML(XmlElement xml, string prefix, HashSet<string> ids)
        {
            if (!xml.HasAttr("id"))
                throw new Exception(prefix + "'id' is missing on " + xml.Name + "!");

            if (ids.Contains(xml.Attr("id")))
                throw new Exception(prefix + "multiple animations with id '" + xml.Attr("id") + "'!");

            ids.Add(xml.Attr("id"));
        }

        public Sprite Create()
        {
            return Sprite.CreateClone();
        }

        public Sprite CreateOn(Sprite sprite)
        {
            return Sprite.CloneInto(sprite);
        }
    }
}
