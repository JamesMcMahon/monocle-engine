using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public class Screen
    {
        public Engine Engine { get; private set; }
        public GraphicsDeviceManager Graphics { get { return Engine.Graphics; } }
        public GraphicsDevice GraphicsDevice { get { return Engine.GraphicsDevice; } }
        public RenderTarget2D RenderTarget { get; private set; }

        public Color ClearColor = Color.Black;
        public SamplerState SamplerState = SamplerState.PointClamp;
        public Effect Effect = null;
        public Vector2 Offset = Vector2.Zero;
        public Vector2 OffsetAdd = Vector2.Zero;
        public Matrix Matrix;

        private Viewport viewport;
        private float scale = 1.0f;
        private Rectangle screenRect;
        private Rectangle drawRect;
        private int width;
        private int height;

        public Screen(Engine engine, int width, int height, float scale)
        {
            Engine = engine;
            screenRect = drawRect = new Rectangle(0, 0, width, height);
            viewport = new Viewport();
            this.width = width;
            this.height = height;
            this.scale = scale;

            drawRect.Width = viewport.Width = (int)(screenRect.Width * scale);
            drawRect.Height = viewport.Height = (int)(screenRect.Height * scale);
            SetWindowSize(drawRect.Width, drawRect.Height);
        }

        public void Initialize()
        {
            if (RenderTarget != null)
                RenderTarget.Dispose();
            RenderTarget = new RenderTarget2D(GraphicsDevice, screenRect.Width, screenRect.Height);
        }

        public void Resize(int width, int height, float scale)
        {
            screenRect = drawRect = new Rectangle(0, 0, width, height);
            viewport = new Viewport();
            this.width = width;
            this.width = height;
            this.scale = scale;

            drawRect.Width = viewport.Width = (int)(screenRect.Width * scale);
            drawRect.Height = viewport.Height = (int)(screenRect.Height * scale);
            if (IsFullscreen)
            {
                Scale = Math.Min(GraphicsDevice.DisplayMode.Width / (float)screenRect.Width, GraphicsDevice.DisplayMode.Height / (float)screenRect.Height);
                HandleFullscreenViewport();
            }
            else
                SetWindowSize(drawRect.Width, drawRect.Height);

            Initialize();
        }

        public void Render()
        {
            GraphicsDevice.Viewport = viewport;
            Draw.SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState, DepthStencilState.None, RasterizerState.CullNone, Effect);

            Vector2 offset = Offset + OffsetAdd;

            if (offset == Vector2.Zero)
                Draw.SpriteBatch.Draw(RenderTarget, drawRect, screenRect, Color.White);
            else
            {
                offset.X = (offset.X + 320) % 320;
                offset.Y = (offset.Y + 240) % 240;

                if (offset.X == 0)
                {
                    int addY = (int)Math.Round(offset.Y * Scale, MidpointRounding.AwayFromZero);
                    Draw.SpriteBatch.Draw(RenderTarget, new Rectangle(drawRect.X, drawRect.Y + addY, drawRect.Width, drawRect.Height), screenRect, Color.White);

                    if (addY < 0)
                        addY += (int)(Height * Scale);
                    else
                        addY -= (int)(Height * Scale);
                    Draw.SpriteBatch.Draw(RenderTarget, new Rectangle(drawRect.X, drawRect.Y + addY, drawRect.Width, drawRect.Height), screenRect, Color.White);
                }
                else if (offset.Y == 0)
                {
                    int addX = (int)Math.Round(offset.X * Scale, MidpointRounding.AwayFromZero);
                    Draw.SpriteBatch.Draw(RenderTarget, new Rectangle(drawRect.X + addX, drawRect.Y, drawRect.Width, drawRect.Height), screenRect, Color.White);

                    if (addX < 0)
                        addX += (int)(Width * Scale);
                    else
                        addX -= (int)(Width * Scale);
                    Draw.SpriteBatch.Draw(RenderTarget, new Rectangle(drawRect.X + addX, drawRect.Y, drawRect.Width, drawRect.Height), screenRect, Color.White);
                }
                else
                {
                    int addX = (int)Math.Round(offset.X * Scale, MidpointRounding.AwayFromZero);
                    int addY = (int)Math.Round(offset.Y * Scale, MidpointRounding.AwayFromZero);
                    int addX2 = addX < 0 ? addX + (int)(Width * Scale) : addX - (int)(Width * Scale);
                    int addY2 = addY < 0 ? addY + (int)(Height * Scale) : addY - (int)(Height * Scale);

                    Draw.SpriteBatch.Draw(RenderTarget, new Rectangle(drawRect.X + addX, drawRect.Y + addY, drawRect.Width, drawRect.Height), screenRect, Color.White);
                    Draw.SpriteBatch.Draw(RenderTarget, new Rectangle(drawRect.X + addX, drawRect.Y + addY2, drawRect.Width, drawRect.Height), screenRect, Color.White);
                    Draw.SpriteBatch.Draw(RenderTarget, new Rectangle(drawRect.X + addX2, drawRect.Y + addY, drawRect.Width, drawRect.Height), screenRect, Color.White);
                    Draw.SpriteBatch.Draw(RenderTarget, new Rectangle(drawRect.X + addX2, drawRect.Y + addY2, drawRect.Width, drawRect.Height), screenRect, Color.White);
                }
            }

            Draw.SpriteBatch.End();
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    drawRect.Width = viewport.Width = (int)(screenRect.Width * scale);
                    drawRect.Height = viewport.Height = (int)(screenRect.Height * scale);

                    if (IsFullscreen)
                        HandleFullscreenViewport();
                    else
                        SetWindowSize(ScaledWidth, ScaledHeight);
                }
            }
        }

        private void SetWindowSize(int width, int height)
        {
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.ApplyChanges();

            viewport.Width = width;
            viewport.Height = ScaledHeight;
            viewport.X = 0;
            viewport.Y = (height - ScaledHeight) / 2;

            drawRect.X = width / 2 - ScaledWidth / 2;

            Matrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(drawRect.X, 0, 0);
        }

        private void HandleFullscreenViewport()
        {
            viewport.Width = GraphicsDevice.DisplayMode.Width;
            viewport.Height = ScaledHeight;
            viewport.X = 0;
            viewport.Y = (GraphicsDevice.DisplayMode.Height - ScaledHeight) / 2;

            drawRect.X = viewport.Width / 2 - ScaledWidth / 2;

            Matrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(drawRect.X, 0, 0);
        }

        public enum FullscreenMode { KeepScale, LargestScale, LargestIntegerScale };
        public void EnableFullscreen(FullscreenMode mode = FullscreenMode.LargestScale)
        {
            Graphics.IsFullScreen = true;
            Graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            Graphics.ApplyChanges();

            if (mode == FullscreenMode.LargestScale)
                Scale = Math.Min(GraphicsDevice.DisplayMode.Width / (float)screenRect.Width, GraphicsDevice.DisplayMode.Height / (float)screenRect.Height);
            else if (mode == FullscreenMode.LargestIntegerScale)
                Scale = (float)Math.Floor(Math.Min(GraphicsDevice.DisplayMode.Width / (float)screenRect.Width, GraphicsDevice.DisplayMode.Height / (float)screenRect.Height));

            HandleFullscreenViewport();
        }

        public void DisableFullscreen(float newScale)
        {
            Graphics.IsFullScreen = false;
            Scale = newScale;
        }

        public void DisableFullscreen()
        {
            DisableFullscreen(scale);
        }

        public int ScaledWidth { get { return (int)(width * scale); } }
        public int ScaledHeight { get { return (int)(height * scale); } }
        public bool IsFullscreen { get { return Graphics.IsFullScreen; } }
        public Vector2 Size { get { return new Vector2(Width, Height); } }
        public int Width { get { return RenderTarget.Width; } }
        public int Height { get { return RenderTarget.Height; } }
        public Vector2 Center { get { return Size * 0.5f; } }
    }
}
