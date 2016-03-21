using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Monocle
{
    public class MTexture
    {
        public Texture2D Texture2D { get; protected set; }
        public Rectangle ClipRect { get; protected set; }
        public string ImagePath { get; private set; }
        public MTexture Parent { get; private set; }

        private Dictionary<string, MTexture> atlas;

        public MTexture(string imagePath)
        {
            ImagePath = imagePath;

#if DEBUG
            if (Engine.Instance.GraphicsDevice == null)
                throw new Exception("Cannot load until GraphicsDevice has been initialized");
            if (!File.Exists(Engine.Instance.Content.RootDirectory + ImagePath))
                throw new FileNotFoundException("Texture file does not exist!");
#endif

            FileStream stream = new FileStream(Engine.Instance.Content.RootDirectory + ImagePath, FileMode.Open);
            Texture2D = Texture2D.FromStream(Engine.Instance.GraphicsDevice, stream);
            stream.Close();

            ClipRect = new Rectangle(0, 0, Texture2D.Width, Texture2D.Height);
        }

        public MTexture(int width, int height, Color color)
        {
            Texture2D = new Texture2D(Engine.Instance.GraphicsDevice, width, height);
            var data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;
            Texture2D.SetData<Color>(data);
            ClipRect = new Rectangle(0, 0, width, height);
        }

        public MTexture(MTexture parent, int x, int y, int width, int height)
        {
            Parent = parent;
            Texture2D = parent.Texture2D;
            ImagePath = parent.ImagePath;
            ClipRect = parent.GetRelativeClipRect(x, y, width, height);
        }

        public MTexture(MTexture parent, Rectangle clipRect)
        {
            Parent = parent;
            Texture2D = parent.Texture2D;
            ImagePath = parent.ImagePath;
            ClipRect = parent.GetRelativeClipRect(clipRect);
        }

        public virtual void Unload()
        {
            Texture2D.Dispose();
            Texture2D = null;
            atlas = null;
        }

        #region Atlas

        public MTexture this[string id]
        {
            get
            {
                if (atlas == null)
                    return null;
                else
                    return atlas[id];
            }

            set
            {
                if (atlas == null)
                    atlas = new Dictionary<string, MTexture>(StringComparer.OrdinalIgnoreCase);
                atlas[id] = value;
            }
        }

        public enum AtlasDataFormat { TexturePacker_Sparrow };

        public void LoadAtlasData(string dataPath, AtlasDataFormat format)
        {
            switch (format)
            {
                case AtlasDataFormat.TexturePacker_Sparrow:
                    {
                        XmlDocument xml = Calc.LoadContentXML(dataPath);
                        XmlElement at = xml["TextureAtlas"];
                        var subtextures = at.GetElementsByTagName("SubTexture");

                        atlas = new Dictionary<string, MTexture>(subtextures.Count, StringComparer.OrdinalIgnoreCase);
                        foreach (XmlElement sub in subtextures)
                            atlas.Add(sub.Attr("name"), new MTexture(this, sub.X(), sub.Y(), sub.Width(), sub.Height()));
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Helpers

        public Rectangle GetRelativeClipRect(Rectangle rect)
        {
            return GetRelativeClipRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public Rectangle GetRelativeClipRect(int x, int y, int width, int height)
        {
            return new Rectangle(ClipRect.X + x, ClipRect.Y + y, Math.Min(width, ClipRect.Width - x), Math.Min(height, ClipRect.Height - y));
        }

        public bool Loaded
        {
            get { return Texture2D != null && !Texture2D.IsDisposed; }
        }

        public int Width
        {
            get { return ClipRect.Width; }
        }

        public int Height
        {
            get { return ClipRect.Height; }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(Width / 2, Height / 2);
            }
        }

        public int TotalPixels
        {
            get { return Width * Height; }
        }

        #endregion
    }
}
