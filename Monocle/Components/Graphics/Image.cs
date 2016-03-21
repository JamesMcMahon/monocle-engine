using Microsoft.Xna.Framework;

namespace Monocle
{
    public class Image : GraphicsComponent
    {
        public MTexture Texture;

        public Image(MTexture texture)
            : base(false)
        {
            Texture = texture;
        }

        internal Image(MTexture texture, bool active)
            : base(active)
        {
            Texture = texture;
        }

        public override void Render()
        {
            Texture.Draw(RenderPosition, Origin, Color, Scale, Rotation, Effects);
        }

        public virtual float Width
        {
            get { return Texture.Width; }
        }

        public virtual float Height
        {
            get { return Texture.Height; }
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
