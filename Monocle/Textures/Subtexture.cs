using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
    public class Subtexture
    {
        public Texture Texture { get; private set; }
        public Rectangle Rect;

        public Subtexture(Texture texture, int x, int y, int width, int height)
        {
            Texture = texture;
            Rect = new Rectangle(x, y, width, height);
        }

        public Subtexture(Subtexture sub, int x, int y, int width, int height)
        {
            Texture = sub.Texture;
            Rect = sub.Rect;
            Rect.X += x;
            Rect.Y += y;
            Rect.Width = width;
            Rect.Height = height;
        }

        public Texture2D Texture2D
        {
            get { return Texture.Texture2D; }
        }

        public bool Loaded
        {
            get { return Texture.Loaded; }
        }

        public int X
        {
            get { return Rect.X; }
        }

        public int Y
        {
            get { return Rect.Y; }
        }

        public int Width
        {
            get { return Rect.Width; }
        }

        public int Height
        {
            get { return Rect.Height; }
        }

        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
        }

        public Vector2 Center
        {
            get { return Size * .5f; }
        }

        public Rectangle GetFrame(int index, int frameWidth, int frameHeight)
        {
            int x = index * frameWidth;
            int y = (x / Rect.Width) * frameHeight;
            x %= Rect.Width;
            return new Rectangle(X + x, Y + y, frameWidth, frameHeight);
        }

        public Rectangle GetAbsoluteClipRect(Rectangle relativeClipRect)
        {
            return new Rectangle(relativeClipRect.X + Rect.X, relativeClipRect.Y + Rect.Y, relativeClipRect.Width, relativeClipRect.Height);
        }
    }
}
