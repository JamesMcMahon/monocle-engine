using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public static class Draw
    {
        /// <summary>
        /// The currently-rendering Renderer
        /// </summary>
        static public Renderer Renderer { get; internal set; }

        /// <summary>
        /// All 2D rendering is done through this SpriteBatch instance
        /// </summary>
        static public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// The default Monocle font (Consolas 12). Loaded automatically by Monocle at startup
        /// </summary>
        static public SpriteFont DefaultFont { get; private set; }

        /// <summary>
        /// A subtexture used to draw particle systems.
        /// Will be generated at startup, but you can replace this with a subtexture from your Atlas to reduce texture swaps.
        /// Should be a 2x2 white pixel
        /// </summary>
        static public MTexture Particle;

        /// <summary>
        /// A subtexture used to draw rectangles and lines. 
        /// Will be generated at startup, but you can replace this with a subtexture from your Atlas to reduce texture swaps.
        /// Use the top left pixel of your Particle Subtexture if you replace it!
        /// Should be a 1x1 white pixel
        /// </summary>
        static public MTexture Pixel;

        static private Rectangle rect;

        static internal void Initialize(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
            DefaultFont = Engine.Instance.Content.Load<SpriteFont>(@"Monocle\MonocleDefault");

#if DEBUG
            UseDebugPixelTexture();
#endif
            UseDebugPixelTexture();
        }

        static public void UseDebugPixelTexture()
        {
            MTexture texture = new MTexture(2, 2, Color.White);
            Pixel = new MTexture(texture, 0, 0, 1, 1);
            Particle = new MTexture(texture, 0, 0, 2, 2);
        }

        static public void Point(Vector2 at, Color color)
        {
            SpriteBatch.Draw(Pixel.Texture2D, at, Pixel.ClipRect, color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        #region Line

        static public void Line(Vector2 start, Vector2 end, Color color)
        {
            LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color);
        }

        static public void Line(Vector2 start, Vector2 end, Color color, float thickness)
        {
            LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color, thickness);
        }

        static public void Line(float x1, float y1, float x2, float y2, Color color)
        {
            Line(new Vector2(x1, y1), new Vector2(x2, y2), color);
        }

        #endregion

        #region Line Angle

        static public void LineAngle(Vector2 start, float angle, float length, Color color)
        {
            SpriteBatch.Draw(Pixel.Texture2D, start, Pixel.ClipRect, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        static public void LineAngle(Vector2 start, float angle, float length, Color color, float thickness)
        {
            SpriteBatch.Draw(Pixel.Texture2D, start, Pixel.ClipRect, color, angle, new Vector2(0, .5f), new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        static public void LineAngle(float startX, float startY, float angle, float length, Color color)
        {
            LineAngle(new Vector2(startX, startY), angle, length, color);
        }

        #endregion

        #region Circle

        static public void Circle(Vector2 position, float radius, Color color, int resolution)
        {
            Vector2 last = Vector2.UnitX * radius;
            Vector2 lastP = last.Perpendicular();
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 at = Calc.AngleToVector(i * MathHelper.PiOver2 / resolution, radius);
                Vector2 atP = at.Perpendicular();

                Draw.Line(position + last, position + at, color);
                Draw.Line(position - last, position - at, color);
                Draw.Line(position + lastP, position + atP, color);
                Draw.Line(position - lastP, position - atP, color);

                last = at;
                lastP = atP;
            }
        }

        static public void Circle(float x, float y, float radius, Color color, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, resolution);
        }

        static public void Circle(Vector2 position, float radius, Color color, float thickness, int resolution)
        {
            Vector2 last = Vector2.UnitX * radius;
            Vector2 lastP = last.Perpendicular();
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 at = Calc.AngleToVector(i * MathHelper.PiOver2 / resolution, radius);
                Vector2 atP = at.Perpendicular();

                Draw.Line(position + last, position + at, color, thickness);
                Draw.Line(position - last, position - at, color, thickness);
                Draw.Line(position + lastP, position + atP, color, thickness);
                Draw.Line(position - lastP, position - atP, color, thickness);

                last = at;
                lastP = atP;
            }
        }

        static public void Circle(float x, float y, float radius, Color color, float thickness, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, thickness, resolution);
        }

        #endregion

        #region Rect

        static public void Rect(float x, float y, float width, float height, Color color)
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = (int)height;
            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.ClipRect, color);
        }

        static public void Rect(Vector2 position, float width, float height, Color color)
        {
            Rect(position.X, position.Y, width, height, color);
        }

        static public void Rect(Rectangle rect, Color color)
        {
            Draw.rect = rect;
            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.ClipRect, color);
        }

        static public void Rect(Collider collider, Color color)
        {
            Rect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);
        }

        #endregion

        #region Hollow Rect

        static public void HollowRect(float x, float y, float width, float height, Color color)
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = 1;

            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.ClipRect, color);

            rect.Y += (int)height - 1;

            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.ClipRect, color);

            rect.Y -= (int)height - 1;
            rect.Width = 1;
            rect.Height = (int)height;

            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.ClipRect, color);

            rect.X += (int)width - 1;

            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.ClipRect, color);
        }

        static public void HollowRect(Vector2 position, float width, float height, Color color)
        {
            HollowRect(position.X, position.Y, width, height, color);
        }

        static public void HollowRect(Rectangle rect, Color color)
        {
            HollowRect(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        static public void HollowRect(Collider collider, Color color)
        {
            HollowRect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);
        }

        #endregion

        #region Text

        static public void Text(SpriteFont font, string text, Vector2 position, Color color)
        {
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color);
        }

        static public void Text(SpriteFont font, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation)
        {
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        static public void TextJustified(SpriteFont font, string text, Vector2 position, Color color, Vector2 justify)
        {
            Vector2 origin = font.MeasureString(text);
            origin.X *= justify.X;
            origin.Y *= justify.Y;

            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        static public void TextJustified(SpriteFont font, string text, Vector2 position, Color color, float scale, Vector2 justify)
        {
            Vector2 origin = font.MeasureString(text);
            origin.X *= justify.X;
            origin.Y *= justify.Y;
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        static public void TextCentered(SpriteFont font, string text, Vector2 position)
        {
            Text(font, text, position - font.MeasureString(text) * .5f, Color.White);
        }

        static public void TextCentered(SpriteFont font, string text, Vector2 position, Color color)
        {
            Text(font, text, position - font.MeasureString(text) * .5f, color);
        }

        static public void TextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
        {
            Text(font, text, position, color, font.MeasureString(text) * .5f, Vector2.One * scale, 0);
        }

        static public void TextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale, float rotation)
        {
            Text(font, text, position, color, font.MeasureString(text) * .5f, Vector2.One * scale, rotation);
        }

        static public void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
        {
            Vector2 origin = font.MeasureString(text) / 2;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), Color.Black, 0, origin, scale, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        static public void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor)
        {
            Vector2 origin = font.MeasureString(text) / 2;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), outlineColor, 0, origin, 1, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        static public void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, float scale)
        {
            Vector2 origin = font.MeasureString(text) / 2;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), outlineColor, 0, origin, scale, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        static public void OutlineTextJustify(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, Vector2 justify)
        {
            Vector2 origin = font.MeasureString(text) * justify;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), outlineColor, 0, origin, 1, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        static public void OutlineTextJustify(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, Vector2 justify, float scale)
        {
            Vector2 origin = font.MeasureString(text) * justify;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), outlineColor, 0, origin, scale, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        #endregion

        #region Weird Textures

        static public void SineTextureH(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            position = Calc.Floor(position);
            Rectangle clip = tex.ClipRect;
            clip.Width = sliceSize;

            int num = 0;
            while (clip.X < tex.ClipRect.X + tex.ClipRect.Width)
            {
                Vector2 add = new Vector2(sliceSize * num, (float)Math.Round(Math.Sin(sineCounter + sliceAdd * num) * amplitude));
                Draw.SpriteBatch.Draw(tex.Texture2D, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.X += sliceSize;
                clip.Width = Math.Min(sliceSize, tex.ClipRect.X + tex.ClipRect.Width - clip.X);
            }
        }

        static public void SineTextureV(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            position = Calc.Floor(position);
            Rectangle clip = tex.ClipRect;
            clip.Height = sliceSize;

            int num = 0;
            while (clip.Y < tex.ClipRect.Y + tex.ClipRect.Height)
            {
                Vector2 add = new Vector2((float)Math.Round(Math.Sin(sineCounter + sliceAdd * num) * amplitude), sliceSize * num);
                Draw.SpriteBatch.Draw(tex.Texture2D, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.Y += sliceSize;
                clip.Height = Math.Min(sliceSize, tex.ClipRect.Y + tex.ClipRect.Height - clip.Y);
            }
        }

        static public void TextureBannerV(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            position = Calc.Floor(position);
            Rectangle clip = tex.ClipRect;
            clip.Height = sliceSize;

            int num = 0;
            while (clip.Y < tex.ClipRect.Y + tex.ClipRect.Height)
            {
                float fade = (clip.Y - tex.ClipRect.Y) / (float)tex.ClipRect.Height;
                clip.Height = (int)MathHelper.Lerp(sliceSize, 1, fade);
                clip.Height = Math.Min(sliceSize, tex.ClipRect.Y + tex.ClipRect.Height - clip.Y);

                Vector2 add = new Vector2((float)Math.Round(Math.Sin(sineCounter + sliceAdd * num) * amplitude * fade), clip.Y - tex.ClipRect.Y);
                Draw.SpriteBatch.Draw(tex.Texture2D, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.Y += clip.Height;
            }
        }

        #endregion
    }
}
