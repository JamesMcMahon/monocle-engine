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
        public Texture2D Texture2D { get; private set; }
        public Rectangle ClipRect { get; private set; }
        public string ImagePath { get; private set; }
        public Vector2 DrawOffset { get; private set; }
        public int Width { get; private set; }

        public int Height { get; private set; }

        private Dictionary<string, MTexture> atlas;

        public MTexture(string imagePath)
        {
            ImagePath = imagePath;
            atlas = null;

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
            DrawOffset = Vector2.Zero;
            Width = ClipRect.Width;
            Height = ClipRect.Height;
        }

        public MTexture(int width, int height, Color color)
        {
            ImagePath = null;
            atlas = null;

            Texture2D = new Texture2D(Engine.Instance.GraphicsDevice, width, height);
            var data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;
            Texture2D.SetData<Color>(data);

            ClipRect = new Rectangle(0, 0, width, height);
            DrawOffset = Vector2.Zero;
            Width = width;
            Height = height;
        }

        public MTexture(MTexture parent, int x, int y, int width, int height)
        {
            Texture2D = parent.Texture2D;
            ImagePath = parent.ImagePath;
            atlas = null;

            ClipRect = parent.GetRelativeRect(x, y, width, height);
            DrawOffset = new Vector2(-Math.Min(x - parent.DrawOffset.X, 0), -Math.Min(y - parent.DrawOffset.Y, 0));
            Width = width;
            Height = height;
        }

        public MTexture(MTexture parent, Rectangle clipRect)
            : this(parent, clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height)
        {

        }

        private MTexture(MTexture parent, Rectangle clipRect, Vector2 drawOffset, int width, int height)
        {
            Texture2D = parent.Texture2D;
            ImagePath = parent.ImagePath;
            atlas = null;

            ClipRect = parent.GetRelativeRect(clipRect);
            DrawOffset = drawOffset;
            Width = width;
            Height = height;
        }

        public void Unload()
        {
            Texture2D.Dispose();
            Texture2D = null;
            atlas = null;
        }

        public MTexture GetSubtexture(int x, int y, int width, int height)
        {
            return new MTexture(this, x, y, width, height);
        }

        public MTexture GetRect(Rectangle rect)
        {
            return new MTexture(this, rect);
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
                        {
                            var clipRect = sub.Rect();
                            if (sub.HasAttr("frameX"))
                                atlas.Add(sub.Attr("name"), new MTexture(this, clipRect, new Vector2(-sub.AttrInt("frameX"), -sub.AttrInt("frameY")), sub.AttrInt("frameWidth"), sub.AttrInt("frameHeight")));
                            else
                                atlas.Add(sub.Attr("name"), new MTexture(this, clipRect));
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Helpers

        private Rectangle GetRelativeRect(Rectangle rect)
        {
            return GetRelativeRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private Rectangle GetRelativeRect(int x, int y, int width, int height)
        {
            int atX = (int)(ClipRect.X - DrawOffset.X + x);
            int atY = (int)(ClipRect.Y - DrawOffset.Y + y);

            int rX = (int)MathHelper.Clamp(atX, ClipRect.Left, ClipRect.Right);
            int rY = (int)MathHelper.Clamp(atY, ClipRect.Top, ClipRect.Bottom);
            int rW = Math.Max(0, Math.Min(atX + width, ClipRect.Right) - rX);
            int rH = Math.Max(0, Math.Min(atY + height, ClipRect.Bottom) - rY);

            return new Rectangle(rX, rY, rW, rH);
        }

        public bool Loaded
        {
            get { return Texture2D != null && !Texture2D.IsDisposed; }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(Width / 2f, Height / 2f);
            }
        }

        public int TotalPixels
        {
            get { return Width * Height; }
        }

        #endregion

        #region Draw

        public void Draw(Vector2 position)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, Color.White, 0, -DrawOffset, 1f, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, Color.White, 0, origin - DrawOffset, 1f, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, origin - DrawOffset, 1f, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color, float scale)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, origin - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color, float scale, float rotation)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, origin - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color, float scale, float rotation, SpriteEffects flip)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, origin - DrawOffset, scale, flip, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, origin - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale, float rotation)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, origin - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale, float rotation, SpriteEffects flip)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, origin - DrawOffset, scale, flip, 0);
        }

        #endregion

        #region Draw Centered

        public void DrawCentered(Vector2 position)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, Color.White, 0, Center - DrawOffset, 1f, SpriteEffects.None, 0);
        }

        public void DrawCentered(Vector2 position, Color color)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, Center - DrawOffset, 1f, SpriteEffects.None, 0);
        }

        public void DrawCentered(Vector2 position, Color color, float scale)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, Center - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void DrawCentered(Vector2 position, Color color, float scale, float rotation)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, Center - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void DrawCentered(Vector2 position, Color color, float scale, float rotation, SpriteEffects flip)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, Center - DrawOffset, scale, flip, 0);
        }

        public void DrawCentered(Vector2 position, Color color, Vector2 scale)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, Center - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void DrawCentered(Vector2 position, Color color, Vector2 scale, float rotation)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, Center - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void DrawCentered(Vector2 position, Color color, Vector2 scale, float rotation, SpriteEffects flip)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, Center - DrawOffset, scale, flip, 0);
        }

        #endregion

        #region Draw Justified

        public void DrawJustified(Vector2 position, Vector2 justify)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, Color.White, 0, new Vector2(Width * justify.X, Height * justify.Y) - DrawOffset, 1f, SpriteEffects.None, 0);
        }

        public void DrawJustified(Vector2 position, Vector2 justify, Color color)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, new Vector2(Width * justify.X, Height * justify.Y) - DrawOffset, 1f, SpriteEffects.None, 0);
        }

        public void DrawJustified(Vector2 position, Vector2 justify, Color color, float scale)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, new Vector2(Width * justify.X, Height * justify.Y) - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void DrawJustified(Vector2 position, Vector2 justify, Color color, float scale, float rotation)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, new Vector2(Width * justify.X, Height * justify.Y) - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void DrawJustified(Vector2 position, Vector2 justify, Color color, float scale, float rotation, SpriteEffects flip)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, new Vector2(Width * justify.X, Height * justify.Y) - DrawOffset, scale, flip, 0);
        }

        public void DrawJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, 0, new Vector2(Width * justify.X, Height * justify.Y) - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void DrawJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale, float rotation)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, new Vector2(Width * justify.X, Height * justify.Y) - DrawOffset, scale, SpriteEffects.None, 0);
        }

        public void DrawJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale, float rotation, SpriteEffects flip)
        {
            Monocle.Draw.SpriteBatch.Draw(Texture2D, position, ClipRect, color, rotation, new Vector2(Width * justify.X, Height * justify.Y) - DrawOffset, scale, flip, 0);
        }

        #endregion
    }
}
