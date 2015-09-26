using Microsoft.Xna.Framework;

namespace Monocle
{
    public class Image : GraphicsComponent
    {
        public Texture Texture { get; protected set; }
        public Rectangle ClipRect;

        public Image(Texture texture, Rectangle? clipRect = null)
            : this(texture, clipRect, false)
        {

        }

        public Image(Subtexture subTexture, Rectangle? clipRect = null)
            : this(subTexture, clipRect, false)
        {

        }

        public Image(Texture texture)
            : base(false)
        {
            Texture = texture;
            ClipRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        internal Image(Texture texture, Rectangle? clipRect, bool active)
            : base(active)
        {
            Texture = texture;
            ClipRect = clipRect ?? texture.Rect;
        }

        internal Image(Subtexture subTexture, Rectangle? clipRect, bool active)
            : base(active)
        {
            Texture = subTexture.Texture;

            if (clipRect.HasValue)
                ClipRect = subTexture.GetAbsoluteClipRect(clipRect.Value);
            else
                ClipRect = subTexture.Rect;
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

        public void SwapSubtexture(Subtexture subtexture, Rectangle? clipRect = null)
        {
            Texture = subtexture.Texture;
            ClipRect = clipRect ?? subtexture.Rect;
        }
    }
}
