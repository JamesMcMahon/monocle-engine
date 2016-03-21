using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public class NumberText : GraphicsComponent
    {
        private SpriteFont font;
        private int value;
        private string prefix;
        private string drawString;

        private bool centered;

        public Action<int> OnValueUpdate;

        public NumberText(SpriteFont font, string prefix, int value, bool centered = false)
            : base(false)
        {
            this.font = font;
            this.prefix = prefix;
            this.value = value;
            this.centered = centered;
            UpdateString();
        }

        public int Value
        {
            get
            {
                return value;
            }

            set
            {
                if (this.value != value)
                {
                    int oldValue = this.value;
                    this.value = value;
                    UpdateString();
                    if (OnValueUpdate != null)
                        OnValueUpdate(oldValue);
                }
            }
        }

        public void UpdateString()
        {
            drawString = prefix + value.ToString();

            if (centered)
                Origin = Calc.Floor(font.MeasureString(drawString) / 2);
        }

        public override void Render()
        {
            Draw.SpriteBatch.DrawString(font, drawString, RenderPosition, Color, Rotation, Origin, Scale, Effects, 0);
        }

        public float Width
        {
            get { return font.MeasureString(drawString).X; }
        }

        public float Height
        {
            get { return font.MeasureString(drawString).Y; }
        }
    }
}
