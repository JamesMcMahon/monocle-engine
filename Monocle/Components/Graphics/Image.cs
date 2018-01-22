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
            if (Texture != null)
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

        public Image SetOrigin(float x, float y)
        {
            Origin.X = x;
            Origin.Y = y;
            return this;
        }

        public Image CenterOrigin()
        {
            Origin.X = Width / 2f;
            Origin.Y = Height / 2f;
            return this;
        }

        public Image JustifyOrigin(Vector2 at)
        {
            Origin.X = Width * at.X;
            Origin.Y = Height * at.Y;
            return this;
        }

        public Image JustifyOrigin(float x, float y)
        {
            Origin.X = Width * x;
            Origin.Y = Height * y;
            return this;
        }
    }
}
