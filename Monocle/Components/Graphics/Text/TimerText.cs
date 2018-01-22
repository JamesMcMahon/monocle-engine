using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public class TimerText : GraphicsComponent
    {
        private const float DELTA_TIME = 1 / 60f;

        public enum CountModes { Down, Up };
        public enum TimerModes { SecondsMilliseconds };

        private SpriteFont font;
        private int frames;
        private TimerModes timerMode;
        private Vector2 justify;

        public string Text { get; private set; }
        public Action OnComplete;
        public CountModes CountMode;

        public TimerText(SpriteFont font, TimerModes mode, CountModes countMode, int frames, Vector2 justify, Action onComplete = null)
            : base(true)
        {
            this.font = font;
            this.timerMode = mode;
            this.CountMode = countMode;
            this.frames = frames;
            this.justify = justify;

#if DEBUG
            if (frames < 0)
                throw new Exception("Frames must be larger than or equal to zero!");
#endif

            OnComplete = onComplete;

            UpdateText();
            CalculateOrigin();
        }

        private void UpdateText()
        {
            switch (timerMode)
            {
                case TimerModes.SecondsMilliseconds:
                    float seconds = (frames / 60) + (frames % 60) * DELTA_TIME;
                    Text = seconds.ToString("0.00");
                    break;
            }
        }

        private void CalculateOrigin()
        {
            Origin = (font.MeasureString(Text) * justify).Floor();
        }

        public override void Update()
        {
            base.Update();

            if (CountMode == CountModes.Down)
            {
                if (frames > 0)
                {
                    frames--;
                    if (frames == 0 && OnComplete != null)
                        OnComplete();

                    UpdateText();
                    CalculateOrigin();
                }
            }
            else
            {
                frames++;
                UpdateText();
                CalculateOrigin();
            }
        }

        public override void Render()
        {
            Draw.SpriteBatch.DrawString(font, Text, RenderPosition, Color, Rotation, Origin, Scale, Effects, 0);
        }

        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                CalculateOrigin();
            }
        }

        public int Frames
        {
            get { return frames; }
            set
            {
#if DEBUG
                if (value < 0)
                    throw new Exception("Frames must be larger than or equal to zero!");
#endif
                if (frames != value)
                {
                    frames = value;
                    UpdateText();
                    CalculateOrigin();
                }
            }
        }

        public Vector2 Justify
        {
            get { return justify; }
            set
            {
                justify = value;
                CalculateOrigin();
            }
        }

        public float Width
        {
            get { return font.MeasureString(Text).X; }
        }

        public float Height
        {
            get { return font.MeasureString(Text).Y; }
        }
    }
}
