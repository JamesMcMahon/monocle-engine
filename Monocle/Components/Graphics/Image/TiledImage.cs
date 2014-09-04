using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
    public class TiledImage : GraphicsComponent
    {
        private Canvas canvas;
        private Subtexture source;
        private int offsetX;
        private int offsetY;
        private Rectangle clipRect;

        public TiledImage(Subtexture source, Rectangle? clipRect, int width, int height, int offsetX = 0, int offsetY = 0)
            : base(false)
        {
            this.source = source;

            if (clipRect.HasValue)
                this.clipRect = source.GetAbsoluteClipRect(clipRect.Value);
            else
                this.clipRect = source.Rect;

            canvas = new Canvas(width, height);
            SetOffsets(offsetX, offsetY);
        }

        public TiledImage(Subtexture source, int width, int height, int offsetX = 0, int offsetY = 0)
            : this(source, null, width, height, offsetX, offsetY)
        {

        }

        public int OffsetX
        {
            get { return offsetX; }
            set
            {
                offsetX = (value + source.Width) % source.Width;
                UpdateBuffer();
            }
        }

        public int OffsetY
        {
            get { return offsetY; }
            set
            {
                offsetY = (value + source.Height) % source.Height;
                UpdateBuffer();
            }
        }

        public Rectangle ClipRect
        {
            get { return clipRect; }
            set
            {
                if (value != clipRect)
                {
                    clipRect = value;
                    UpdateBuffer();
                }
            }
        }

        public void SetOffsets(int offsetX, int offsetY)
        {
            this.offsetX = (offsetX + source.Width) % source.Width;
            this.offsetY = (offsetY + source.Height) % source.Height;
            UpdateBuffer();
        }

        private void UpdateBuffer()
        {
            Draw.SetTarget(canvas);
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            for (int x = -offsetX; x < canvas.Width; x += clipRect.Width)
                for (int y = -offsetY; y < canvas.Height; y += clipRect.Height)
                    Draw.SpriteBatch.Draw(source.Texture2D, new Vector2(x, y), clipRect, Color.White);
            Draw.SpriteBatch.End();
        }

        public override void Render()
        {
            Draw.SpriteBatch.Draw(canvas.Texture2D, RenderPosition, null, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }

        public override void HandleGraphicsReset()
        {
            UpdateBuffer();
        }

        public void Resize(int width, int height)
        {
            if (Width != width || Height != height)
            {
                canvas.Unload();
                canvas = new Canvas(width, height);
                UpdateBuffer();
            }
        }

        public void Resize(int width, int height, int offsetX, int offsetY)
        {
            if (Width != width || Height != height || OffsetX != offsetX || OffsetY != offsetY)
            {
                canvas.Unload();
                canvas = new Canvas(width, height);
                SetOffsets(offsetX, offsetY);
            }
        }

        public float Width
        {
            get { return canvas.Width; }
        }

        public float Height
        {
            get { return canvas.Height; }
        }
    }
}
