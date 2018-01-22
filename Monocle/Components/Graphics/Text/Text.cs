using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
    public class Text : GraphicsComponent
    {
        public enum HorizontalAlign { Left, Center, Right };
        public enum VerticalAlign { Top, Center, Bottom };

        private SpriteFont font;
        private string text;
        private HorizontalAlign horizontalOrigin;
        private VerticalAlign verticalOrigin;
        private Vector2 size;

        public Text(SpriteFont font, string text, Vector2 position, Color color, HorizontalAlign horizontalAlign = HorizontalAlign.Center, VerticalAlign verticalAlign = VerticalAlign.Center)
            : base(false)
        {
            this.font = font;
            this.text = text;
            Position = position;
            Color = color;
            this.horizontalOrigin = horizontalAlign;
            this.verticalOrigin = verticalAlign;
            UpdateSize();
        }

        public Text(SpriteFont font, string text, Vector2 position, HorizontalAlign horizontalAlign = HorizontalAlign.Center, VerticalAlign verticalAlign = VerticalAlign.Center)
            : this(font, text, position, Color.White, horizontalAlign, verticalAlign)
        {

        }

        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                UpdateSize();
            }
        }

        public string DrawText
        {
            get { return text; }
            set
            {
                text = value;
                UpdateSize();
            }
        }

        public HorizontalAlign HorizontalOrigin
        {
            get { return horizontalOrigin; }
            set
            {
                horizontalOrigin = value;
                UpdateCentering();
            }
        }

        public VerticalAlign VerticalOrigin
        {
            get { return verticalOrigin; }
            set
            {
                verticalOrigin = value;
                UpdateCentering();
            }
        }

        public float Width
        {
            get { return size.X; }
        }

        public float Height
        {
            get { return size.Y; }
        }

        private void UpdateSize()
        {
            size = font.MeasureString(text);
            UpdateCentering();
        }

        private void UpdateCentering()
        {
            if (horizontalOrigin == HorizontalAlign.Left)
                Origin.X = 0;
            else if (horizontalOrigin == HorizontalAlign.Center)
                Origin.X = size.X / 2;
            else
                Origin.X = size.X;

            if (verticalOrigin == VerticalAlign.Top)
                Origin.Y = 0;
            else if (verticalOrigin == VerticalAlign.Center)
                Origin.Y = size.Y / 2;
            else
                Origin.Y = size.Y;

            Origin = Origin.Floor();
        }

        public override void Render()
        {
            Draw.SpriteBatch.DrawString(font, text, RenderPosition, Color, Rotation, Origin, Scale, Effects, 0);
        }
    }
}
