using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Monocle
{
    public class SpriteData
    {
        private MTexture atlas;
        private XmlDocument document;

        public Dictionary<string, XmlElement> SpriteXML;

        public SpriteData(MTexture atlas, XmlDocument xmlDocument)
        {
            this.atlas = atlas;
            this.document = xmlDocument;

            SpriteXML = new Dictionary<string, XmlElement>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var xml in document["Sprites"].ChildNodes)
            {
                if (xml is XmlElement)
                {
                    var element = xml as XmlElement;
                    if (SpriteXML.ContainsKey(element.Name))
                        throw new Exception("Duplicate animation name in SpriteData: '" + element.Name + "'!");
                    SpriteXML[element.Name] = element;
                }
            }

            ErrorCheck();
        }

        public SpriteData(MTexture atlas, string xmlPath)
            : this(atlas, Calc.LoadXML(xmlPath))
        {

        }

        public void ErrorCheck()
        {
            foreach (var kv in SpriteXML)
            {
                string prefix = "Sprite '" + kv.Key + "': ";
                var xml = kv.Value;

                //Path
                if (!xml.HasAttr("path"))
                    throw new Exception(prefix + "'path' is missing!");

                //Anims
                HashSet<string> ids = new HashSet<string>();
                foreach (XmlElement anim in xml.GetElementsByTagName("Anim"))
                    CheckAnimXML(anim, prefix, ids);
                foreach (XmlElement loop in xml.GetElementsByTagName("Loop"))
                    CheckAnimXML(loop, prefix, ids);

                //Start
                if (xml.HasAttr("start") && !ids.Contains(xml.Attr("start")))
                    throw new Exception(prefix + "starting animation '" + xml.Attr("start") + "' is missing!");

                //Origin
                if (xml.HasChild("Justify") && xml.HasChild("Origin"))
                    throw new Exception(prefix + "has both Origin and Justify tags!");
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

        public Sprite GetSprite(string id)
        {
            if (SpriteXML.ContainsKey(id))
            {
                var xml = SpriteXML[id];
                float masterDelay = xml.AttrFloat("delay", 0);

                Sprite sprite = new Sprite(atlas, xml.Attr("path", ""));

                //Build Animations
                foreach (XmlElement anim in xml.GetElementsByTagName("Anim"))
                {
                    Chooser<string> into;
                    if (anim.HasAttr("goto"))
                        into = Chooser<string>.FromString<string>(anim.Attr("goto"));
                    else
                        into = null;

                    if (anim.HasAttr("frames"))
                        sprite.Add(anim.Attr("id"), anim.Attr("path", ""), anim.AttrFloat("delay", masterDelay), into, Calc.ReadCSVIntWithTricks(anim.Attr("frames")));
                    else
                        sprite.Add(anim.Attr("id"), anim.Attr("path", ""), anim.AttrFloat("delay", masterDelay), into);
                }

                //Build Loops
                foreach (XmlElement loop in xml.GetElementsByTagName("Loop"))
                    sprite.AddLoop(loop.Attr("id"), loop.Attr("path", ""), loop.AttrFloat("delay", masterDelay));

                //Origin
                if (SpriteXML[id]["Justify"] != null)
                    sprite.JustifyOrigin(SpriteXML[id]["Justify"].Position());
                else if (SpriteXML[id]["Origin"] != null)
                    sprite.Origin = SpriteXML[id]["Origin"].Position();

                //Position
                if (SpriteXML[id]["Position"] != null)
                    sprite.Position = SpriteXML[id]["Position"].Position();

                //Start Animation
                if (SpriteXML[id].HasAttr("start"))
                    sprite.Play(SpriteXML[id].Attr("start"));

                return sprite;
            }
            else
                throw new Exception("Missing animation name in SpriteData: '" + id + "'!");
        }
    }
}
