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
        public static Renderer Renderer { get; internal set; }

        /// <summary>
        /// All 2D rendering is done through this SpriteBatch instance
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// The default Monocle font (Consolas 12). Loaded automatically by Monocle at startup
        /// </summary>
        public static SpriteFont DefaultFont { get; private set; }

        /// <summary>
        /// A subtexture used to draw particle systems.
        /// Will be generated at startup, but you can replace this with a subtexture from your Atlas to reduce texture swaps.
        /// Should be a 2x2 white pixel
        /// </summary>
        public static MTexture Particle;

        /// <summary>
        /// A subtexture used to draw rectangles and lines. 
        /// Will be generated at startup, but you can replace this with a subtexture from your Atlas to reduce texture swaps.
        /// Use the top left pixel of your Particle Subtexture if you replace it!
        /// Should be a 1x1 white pixel
        /// </summary>
        public static MTexture Pixel;

        private static Rectangle rect;

        internal static void Initialize(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
            DefaultFont = Engine.Instance.Content.Load<SpriteFont>(@"Monocle\MonocleDefault");
            UseDebugPixelTexture();
        }

        public static void UseDebugPixelTexture()
        {
            MTexture texture = new MTexture(2, 2, Color.White);
            Pixel = new MTexture(texture, 0, 0, 1, 1);
            Particle = new MTexture(texture, 0, 0, 2, 2);
        }

        public static void Point(Vector2 at, Color color)
        {
            SpriteBatch.Draw(Pixel.Texture, at, Pixel.ClipRect, color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        #region Line

        public static void Line(Vector2 start, Vector2 end, Color color)
        {
            LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color);
        }

        public static void Line(Vector2 start, Vector2 end, Color color, float thickness)
        {
            LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color, thickness);
        }

        public static void Line(float x1, float y1, float x2, float y2, Color color)
        {
            Line(new Vector2(x1, y1), new Vector2(x2, y2), color);
        }

        #endregion

        #region Line Angle

        public static void LineAngle(Vector2 start, float angle, float length, Color color)
        {
            SpriteBatch.Draw(Pixel.Texture, start, Pixel.ClipRect, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        public static void LineAngle(Vector2 start, float angle, float length, Color color, float thickness)
        {
            SpriteBatch.Draw(Pixel.Texture, start, Pixel.ClipRect, color, angle, new Vector2(0, .5f), new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        public static void LineAngle(float startX, float startY, float angle, float length, Color color)
        {
            LineAngle(new Vector2(startX, startY), angle, length, color);
        }

        #endregion

        #region Circle

        public static void Circle(Vector2 position, float radius, Color color, int resolution)
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

        public static void Circle(float x, float y, float radius, Color color, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, resolution);
        }

        public static void Circle(Vector2 position, float radius, Color color, float thickness, int resolution)
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

        public static void Circle(float x, float y, float radius, Color color, float thickness, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, thickness, resolution);
        }

        #endregion

        #region Rect

        public static void Rect(float x, float y, float width, float height, Color color)
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = (int)height;
            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);
        }

        public static void Rect(Vector2 position, float width, float height, Color color)
        {
            Rect(position.X, position.Y, width, height, color);
        }

        public static void Rect(Rectangle rect, Color color)
        {
            Draw.rect = rect;
            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);
        }

        public static void Rect(Collider collider, Color color)
        {
            Rect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);
        }

        #endregion

        #region Hollow Rect

        public static void HollowRect(float x, float y, float width, float height, Color color)
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = 1;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);

            rect.Y += (int)height - 1;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);

            rect.Y -= (int)height - 1;
            rect.Width = 1;
            rect.Height = (int)height;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);

            rect.X += (int)width - 1;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);
        }

        public static void HollowRect(Vector2 position, float width, float height, Color color)
        {
            HollowRect(position.X, position.Y, width, height, color);
        }

        public static void HollowRect(Rectangle rect, Color color)
        {
            HollowRect(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public static void HollowRect(Collider collider, Color color)
        {
            HollowRect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);
        }

        #endregion

        #region Text

        public static void Text(SpriteFont font, string text, Vector2 position, Color color)
        {
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color);
        }

        public static void Text(SpriteFont font, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation)
        {
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        public static void TextJustified(SpriteFont font, string text, Vector2 position, Color color, Vector2 justify)
        {
            Vector2 origin = font.MeasureString(text);
            origin.X *= justify.X;
            origin.Y *= justify.Y;

            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void TextJustified(SpriteFont font, string text, Vector2 position, Color color, float scale, Vector2 justify)
        {
            Vector2 origin = font.MeasureString(text);
            origin.X *= justify.X;
            origin.Y *= justify.Y;
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public static void TextCentered(SpriteFont font, string text, Vector2 position)
        {
            Text(font, text, position - font.MeasureString(text) * .5f, Color.White);
        }

        public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color)
        {
            Text(font, text, position - font.MeasureString(text) * .5f, color);
        }

        public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
        {
            Text(font, text, position, color, font.MeasureString(text) * .5f, Vector2.One * scale, 0);
        }

        public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale, float rotation)
        {
            Text(font, text, position, color, font.MeasureString(text) * .5f, Vector2.One * scale, rotation);
        }

        public static void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
        {
            Vector2 origin = font.MeasureString(text) / 2;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), Color.Black, 0, origin, scale, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public static void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor)
        {
            Vector2 origin = font.MeasureString(text) / 2;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), outlineColor, 0, origin, 1, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, float scale)
        {
            Vector2 origin = font.MeasureString(text) / 2;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), outlineColor, 0, origin, scale, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public static void OutlineTextJustify(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, Vector2 justify)
        {
            Vector2 origin = font.MeasureString(text) * justify;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position) + new Vector2(i, j), outlineColor, 0, origin, 1, SpriteEffects.None, 0);
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void OutlineTextJustify(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, Vector2 justify, float scale)
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

        public static void SineTextureH(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            position = Calc.Floor(position);
            Rectangle clip = tex.ClipRect;
            clip.Width = sliceSize;

            int num = 0;
            while (clip.X < tex.ClipRect.X + tex.ClipRect.Width)
            {
                Vector2 add = new Vector2(sliceSize * num, (float)Math.Round(Math.Sin(sineCounter + sliceAdd * num) * amplitude));
                Draw.SpriteBatch.Draw(tex.Texture, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.X += sliceSize;
                clip.Width = Math.Min(sliceSize, tex.ClipRect.X + tex.ClipRect.Width - clip.X);
            }
        }

        public static void SineTextureV(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            position = Calc.Floor(position);
            Rectangle clip = tex.ClipRect;
            clip.Height = sliceSize;

            int num = 0;
            while (clip.Y < tex.ClipRect.Y + tex.ClipRect.Height)
            {
                Vector2 add = new Vector2((float)Math.Round(Math.Sin(sineCounter + sliceAdd * num) * amplitude), sliceSize * num);
                Draw.SpriteBatch.Draw(tex.Texture, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.Y += sliceSize;
                clip.Height = Math.Min(sliceSize, tex.ClipRect.Y + tex.ClipRect.Height - clip.Y);
            }
        }

        public static void TextureBannerV(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
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
                Draw.SpriteBatch.Draw(tex.Texture, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.Y += clip.Height;
            }
        }

        #endregion
    }
}
