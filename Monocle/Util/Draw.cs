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
        /// Matrix that handles scaling for fullscreen. Automatically set by Monocle when the game switches from fullscreen to windowed mode.
        /// All your rendering should use this Matrix and the default Renderers use it
        /// </summary>
        static public Matrix MasterRenderMatrix { get; internal set; }

        /// <summary>
        /// A subtexture used to draw particle systems.
        /// Will be generated at startup, but you can replace this with a subtexture from your Atlas to reduce texture swaps.
        /// Should be a 2x2 white pixel
        /// </summary>
        static public Subtexture Particle;

        /// <summary>
        /// A subtexture used to draw rectangles and lines. 
        /// Will be generated at startup, but you can replace this with a subtexture from your Atlas to reduce texture swaps.
        /// Use the top left pixel of your Particle Subtexture if you replace it!
        /// Should be a 1x1 white pixel
        /// </summary>
        static public Subtexture Pixel;

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
            Texture texture = new Texture(2, 2, Color.White);
            Pixel = new Subtexture(texture, 0, 0, 1, 1);
            Particle = new Subtexture(texture, 0, 0, 2, 2);
        }

        static public void BeginCanvas(Canvas canvas)
        {
            SetTarget(canvas);
            Clear();
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
        }

        static public void BeginCanvas(Canvas canvas, Color clearColor, BlendState blendState)
        {
            SetTarget(canvas);
            Clear(clearColor);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
        }

        static public void SetTarget(Canvas canvas)
        {
            Engine.Instance.GraphicsDevice.SetRenderTarget(canvas.RenderTarget2D);
        }

        static public void ResetTarget()
        {
            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
        }

        static public void Clear(Color color)
        {
            Engine.Instance.GraphicsDevice.Clear(color);
        }

        static public void Clear()
        {
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
        }

        static public void Point(Vector2 at, Color color)
        {
            SpriteBatch.Draw(Pixel.Texture2D, at, Pixel.Rect, color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
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
            SpriteBatch.Draw(Pixel.Texture2D, start, Pixel.Rect, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        static public void LineAngle(Vector2 start, float angle, float length, Color color, float thickness)
        {
            SpriteBatch.Draw(Pixel.Texture2D, start, Pixel.Rect, color, angle, new Vector2(0, .5f), new Vector2(length, thickness), SpriteEffects.None, 0);
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

        #endregion

        #region Rect

        static public void Rect(float x, float y, float width, float height, Color color)
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = (int)height;
            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.Rect, color);
        }

        static public void Rect(Vector2 position, float width, float height, Color color)
        {
            Rect(position.X, position.Y, width, height, color);
        }

        static public void Rect(Rectangle rect, Color color)
        {
            Draw.rect = rect;
            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.Rect, color);
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

            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.Rect, color);

            rect.Y += (int)height - 1;

            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.Rect, color);

            rect.Y -= (int)height - 1;
            rect.Width = 1;
            rect.Height = (int)height;

            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.Rect, color);

            rect.X += (int)width - 1;

            SpriteBatch.Draw(Pixel.Texture2D, rect, Pixel.Rect, color);
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

        static public void Text(SpriteFont font, string text, Vector2 position, Color color)
        {
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color);
        }

        static public void Text(SpriteFont font, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation)
        {
            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        static public void TextJustify(SpriteFont font, string text, Vector2 position, Color color, Vector2 justify)
        {
            Vector2 origin = font.MeasureString(text);
            origin.X *= justify.X;
            origin.Y *= justify.Y;

            Draw.SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        static public void TextJustify(SpriteFont font, string text, Vector2 position, Color color, float scale, Vector2 justify)
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

        static public void TextRight(SpriteFont font, string text, Vector2 position, Color color)
        {
            Vector2 origin = font.MeasureString(text);
            origin.Y /= 2;

            Text(font, text, position - origin, color);
        }

        static public void TextRight(SpriteFont font, string text, Vector2 position, Color color, Vector2 scale, float rotation)
        {
            Vector2 origin = font.MeasureString(text);
            origin.Y /= 2f;

            Text(font, text, position, color, origin, scale, rotation);
        }

        static public void Texture(Texture texture, Vector2 position)
        {
            SpriteBatch.Draw(texture.Texture2D, Calc.Floor(position), null, Color.White);
        }

        static public void Texture(Texture texture, Vector2 position, Color color)
        {
            SpriteBatch.Draw(texture.Texture2D, Calc.Floor(position), null, color);
        }

        static public void Texture(Texture texture, Vector2 position, Color color, float scale)
        {
            SpriteBatch.Draw(texture.Texture2D, Calc.Floor(position), null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        static public void Texture(Texture texture, Rectangle clipRect, Vector2 position, Color color)
        {
            SpriteBatch.Draw(texture.Texture2D, Calc.Floor(position), clipRect, color);
        }

        static public void Texture(Texture texture, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation)
        {
            SpriteBatch.Draw(texture.Texture2D, Calc.Floor(position), null, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        static public void Texture(Subtexture subTexture, Vector2 position)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, Color.White);
        }

        static public void Texture(Subtexture subTexture, Vector2 position, Color color)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color);
        }

        static public void Texture(Subtexture subTexture, Vector2 position, Color color, Vector2 origin, Vector2 scale)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        static public void Texture(Subtexture subTexture, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        static public void Texture(Subtexture subTexture, Vector2 position, Color color, Vector2 origin, float scale, float rotation)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        static public void Texture(Subtexture subTexture, Vector2 position, Color color, Vector2 origin, float scale, float rotation, SpriteEffects effects)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, rotation, origin, scale, effects, 0);
        }

        static public void Texture(Subtexture subTexture, Rectangle clipRect, Vector2 position, Color color)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.GetAbsoluteClipRect(clipRect), color);
        }

        static public void TextureCentered(Subtexture subTexture, Vector2 position)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, Color.White, 0, new Vector2(subTexture.Rect.Width / 2, subTexture.Rect.Height / 2), 1, SpriteEffects.None, 0);
        }

        static public void TextureCentered(Subtexture subTexture, Vector2 position, Color color)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, 0, new Vector2(subTexture.Rect.Width / 2, subTexture.Rect.Height / 2), 1, SpriteEffects.None, 0);
        }

        static public void OutlineTextureCentered(Subtexture subTexture, Vector2 position, Color color)
        {
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (i != 0 || j != 0)
                        SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position) + new Vector2(i, j), subTexture.Rect, Color.Black, 0, new Vector2(subTexture.Rect.Width / 2, subTexture.Rect.Height / 2), 1, SpriteEffects.None, 0);

            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, 0, new Vector2(subTexture.Rect.Width / 2, subTexture.Rect.Height / 2), 1, SpriteEffects.None, 0);
        }

        static public void OutlineTextureCentered(Subtexture subTexture, Vector2 position, Color color, Vector2 scale)
        {
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (i != 0 || j != 0)
                        SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position) + new Vector2(i, j), subTexture.Rect, Color.Black, 0, new Vector2(subTexture.Rect.Width / 2, subTexture.Rect.Height / 2), scale, SpriteEffects.None, 0);

            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, 0, new Vector2(subTexture.Rect.Width / 2, subTexture.Rect.Height / 2), scale, SpriteEffects.None, 0);
        }

        static public void TextureCentered(Subtexture subTexture, Rectangle clipRect, Vector2 position, Color color)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.GetAbsoluteClipRect(clipRect), color, 0, new Vector2(clipRect.Width / 2, clipRect.Height / 2), 1, SpriteEffects.None, 0);
        }

        static public void TextureCentered(Subtexture subTexture, Vector2 position, Color color, float scale, float rotation)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, rotation, new Vector2(subTexture.Rect.Width / 2, subTexture.Rect.Height / 2), scale, SpriteEffects.None, 0);
        }

        static public void TextureCentered(Subtexture subTexture, Vector2 position, Color color, Vector2 scale, float rotation)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.Rect, color, rotation, new Vector2(subTexture.Rect.Width / 2, subTexture.Rect.Height / 2), scale, SpriteEffects.None, 0);
        }

        static public void TextureCentered(Subtexture subTexture, Rectangle clipRect, Vector2 position, Color color, float scale, float rotation)
        {
            SpriteBatch.Draw(subTexture.Texture.Texture2D, Calc.Floor(position), subTexture.GetAbsoluteClipRect(clipRect), color, rotation, new Vector2(clipRect.Width / 2, clipRect.Height / 2), scale, SpriteEffects.None, 0);
        }

        static public void TextureJustify(Texture texture, Vector2 position, Color color, Vector2 scale, float rotation, Vector2 justify)
        {
            SpriteBatch.Draw(texture.Texture2D, Calc.Floor(position), texture.Rect, color, rotation, new Vector2(texture.Width, texture.Height) * justify, scale, SpriteEffects.None, 0);
        }

        static public void TextureJustify(Subtexture subtexture, Vector2 position, Color color, Vector2 scale, float rotation, Vector2 justify)
        {
            SpriteBatch.Draw(subtexture.Texture.Texture2D, Calc.Floor(position), subtexture.Rect, color, rotation, subtexture.Size * justify, scale, SpriteEffects.None, 0);
        }

        static public void TextureJustify(Subtexture subtexture, Rectangle clipRect, Vector2 position, Color color, Vector2 justify)
        {
            Rectangle drawRect = subtexture.Rect;
            drawRect.X += clipRect.X;
            drawRect.Y += clipRect.Y;
            drawRect.Width = clipRect.Width;
            drawRect.Height = clipRect.Height;

            Vector2 origin = new Vector2(drawRect.Width * justify.X, drawRect.Height * justify.Y);

            SpriteBatch.Draw(subtexture.Texture.Texture2D, Calc.Floor(position), drawRect, color, 0, origin, Vector2.One, SpriteEffects.None, 0);
        }

        static public void SineTextureV(Texture texture, Rectangle clipRect, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            position = Calc.Floor(position);
            Rectangle clip = clipRect;
            clip.Height = sliceSize;

            int num = 0;
            while (clip.Y < clipRect.Y + clipRect.Height)
            {
                Vector2 add = new Vector2((float)Math.Round(Math.Sin(sineCounter + sliceAdd * num) * amplitude), sliceSize * num);
                Draw.SpriteBatch.Draw(texture.Texture2D, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.Y += sliceSize;
                clip.Height = Math.Min(sliceSize, clipRect.Y + clipRect.Height - clip.Y);
            }
        }

        static public void SineTextureV(Subtexture subtexture, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            SineTextureV(subtexture.Texture, subtexture.Rect, position, origin, scale, rotation, color, effects, sineCounter, amplitude, sliceSize, sliceAdd);
        }

        static public void TextureBannerV(Texture texture, Rectangle clipRect, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            position = Calc.Floor(position);
            Rectangle clip = clipRect;
            clip.Height = sliceSize;

            int num = 0;
            while (clip.Y < clipRect.Y + clipRect.Height)
            {
                float fade = (clip.Y - clipRect.Y) / (float)clipRect.Height;
                clip.Height = (int)MathHelper.Lerp(sliceSize, 1, fade);
                clip.Height = Math.Min(sliceSize, clipRect.Y + clipRect.Height - clip.Y);

                Vector2 add = new Vector2((float)Math.Round(Math.Sin(sineCounter + sliceAdd * num) * amplitude * fade), clip.Y - clipRect.Y);
                Draw.SpriteBatch.Draw(texture.Texture2D, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.Y += clip.Height;
            }
        }

        static public void TextureBannerV(Subtexture subtexture, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            TextureBannerV(subtexture.Texture, subtexture.Rect, position, origin, scale, rotation, color, effects, sineCounter, amplitude, sliceSize, sliceAdd);
        }

        static public void SineTextureH(Texture texture, Rectangle clipRect, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            position = Calc.Floor(position);
            Rectangle clip = clipRect;
            clip.Width = sliceSize;

            int num = 0;
            while (clip.X < clipRect.X + clipRect.Width)
            {
                Vector2 add = new Vector2(sliceSize * num, (float)Math.Round(Math.Sin(sineCounter + sliceAdd * num) * amplitude));
                Draw.SpriteBatch.Draw(texture.Texture2D, position, clip, color, rotation, origin - add, scale, effects, 0);

                num++;
                clip.X += sliceSize;
                clip.Width = Math.Min(sliceSize, clipRect.X + clipRect.Width - clip.X);
            }
        }

        static public void SineTextureH(Subtexture subtexture, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2, int sliceSize = 2, float sliceAdd = MathHelper.TwoPi / 8)
        {
            SineTextureH(subtexture.Texture, subtexture.Rect, position, origin, scale, rotation, color, effects, sineCounter, amplitude, sliceSize, sliceAdd);
        }

        static public void TextureFill(Texture texture, Rectangle clipRect, Rectangle fillArea)
        {
            Rectangle currentDraw = Rectangle.Empty;

            for (currentDraw.X = fillArea.X; currentDraw.X < fillArea.X + fillArea.Width; currentDraw.X += clipRect.Width)
            {
                for (currentDraw.Y = fillArea.Y; currentDraw.Y < fillArea.Y + fillArea.Height; currentDraw.Y += clipRect.Height)
                {
                    currentDraw.Width = Math.Min(fillArea.X + fillArea.Width - currentDraw.X, clipRect.Width);
                    currentDraw.Height = Math.Min(fillArea.Y + fillArea.Height - currentDraw.Y, clipRect.Height);
                    SpriteBatch.Draw(texture.Texture2D, currentDraw, clipRect, Color.White);
                }
            }
        }

        //TODO: FIX THIS
        static public void TextureFill(Texture texture, Rectangle clipRect, Rectangle fillArea, int offsetX, int offsetY)
        {
            offsetX %= clipRect.Width;
            offsetY %= clipRect.Height;

            Rectangle currentDraw = Rectangle.Empty;
            Rectangle currentClip = clipRect;
            currentClip.X = clipRect.X + offsetX;
            currentClip.Width -= offsetX;

            for (currentDraw.X = fillArea.X; currentDraw.X < fillArea.X + fillArea.Width; currentDraw.X += clipRect.Width)
            {
                currentClip.Y = clipRect.Y + offsetY;
                currentClip.Height = clipRect.Height - offsetY;

                for (currentDraw.Y = fillArea.Y; currentDraw.Y < fillArea.Y + fillArea.Height; currentDraw.Y += clipRect.Height)
                {
                    currentDraw.Width = Math.Min(fillArea.X + fillArea.Width - currentDraw.X, currentClip.Width);
                    currentDraw.Height = Math.Min(fillArea.Y + fillArea.Height - currentDraw.Y, currentClip.Height);
                    SpriteBatch.Draw(texture.Texture2D, currentDraw, currentClip, Color.White);

                    currentClip.Y = clipRect.Y;
                    currentClip.Height = clipRect.Height;
                }

                currentClip.X = clipRect.X;
                currentClip.Width = clipRect.Width;
            }
        }

        static public void TextureFill(Subtexture subTexture, Rectangle fillArea, int offsetX, int offsetY)
        {
            TextureFill(subTexture.Texture, subTexture.Rect, fillArea, offsetX, offsetY);
        }

        static public void TextureFill(Subtexture subTexture, Rectangle fillArea)
        {
            TextureFill(subTexture.Texture, subTexture.Rect, fillArea);
        }
    }
}
