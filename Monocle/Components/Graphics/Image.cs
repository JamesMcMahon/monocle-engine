using Microsoft.Xna.Framework;

namespace Monocle
{
    public class Image : GraphicsComponent
    {
        public MTexture Texture { get; protected set; }
        public Rectangle ClipRect;

        public Image(MTexture texture, Rectangle? clipRect = null)
            : this(texture, clipRect, false)
        {

        }

        public Image(MTexture texture)
            : base(false)
        {
            SetTexture(texture);
        }

        internal Image(MTexture texture, Rectangle? clipRect, bool active)
            : base(active)
        {
            SetTexture(texture, clipRect);
        }

        public void SetTexture(MTexture texture, Rectangle? clipRect = null)
        {
            Texture = texture;
            if (clipRect.HasValue)
                ClipRect = texture.GetRelativeClipRect(clipRect.Value);
            else
                ClipRect = texture.ClipRect;
        }

        public override void Render()
        {
            Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition, ClipRect, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }

        public virtual float Width
        {
            get { return ClipRect.Width; }
        }

        public virtual float Height
        {
            get { return ClipRect.Height; }
        }

        public void CenterOrigin()
        {
            Origin.X = Width / 2f;
            Origin.Y = Height / 2f;
        }

        public void JustifyOrigin(Vector2 at)
        {
            Origin.X = Width * at.X;
            Origin.Y = Height * at.Y;
        }

        public void JustifyOrigin(float x, float y)
        {
            Origin.X = Width * x;
            Origin.Y = Height * y;
        }
    }
}
