using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public struct Particle
    {
        public Entity Track;
        public ParticleType Type;
        public MTexture Source;

        public bool Active;
        public Color Color;
        public Color StartColor;
        public Vector2 Position;
        public Vector2 Speed;
        public float Size;
        public float StartSize;
        public float Life;
        public float StartLife;
        public float ColorSwitch;
        public float Rotation;
        public float Spin;
        
        public bool SimulateFor(float duration)
        {
            if (duration > Life)
            {
                Life = 0;
                Active = false;
                return false;
            }
            else
            {
                var dt = Engine.TimeRate * (Engine.Instance.TargetElapsedTime.Milliseconds / 1000f);
                if (dt > 0)
                    for (var t = 0f; t < duration; t += dt)
                        Update(dt);

                return true;
            }
        }

        public void Update(float? delta = null)
        {
            var dt = 0f;
            if (delta.HasValue)
                dt = delta.Value;
            else
                dt = (Type.UseActualDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime);

            var ease = Life / StartLife;

            //Life
            Life -= dt;
            if (Life <= 0)
            {
                Active = false;
                return;
            }

            //Spin
            if (Type.RotationMode == ParticleType.RotationModes.SameAsDirection)
            {
                if (Speed != Vector2.Zero)
                    Rotation = Speed.Angle();
            }
            else
                Rotation += Spin * dt;

            //Fade
            float alpha;
            if (Type.FadeMode == ParticleType.FadeModes.Linear)
                alpha = ease;
            else if (Type.FadeMode == ParticleType.FadeModes.Late)
                alpha = Math.Min(1f, ease / .25f);
            else if (Type.FadeMode == ParticleType.FadeModes.InAndOut)
            {
                if (ease > .75f)
                    alpha = 1 - ((ease - .75f) / .25f);
                else if (ease < .25f)
                    alpha = ease / .25f;
                else
                    alpha = 1f;
            }
            else
                alpha = 1f;

            
            //Color switch with alpha
            if (alpha == 0)
                Color = Color.Transparent;
            else
            {
                if (Type.ColorMode == ParticleType.ColorModes.Static)
                    Color = StartColor;
                else if (Type.ColorMode == ParticleType.ColorModes.Fade)
                    Color = Color.Lerp(Type.Color2, StartColor, ease);
                else if (Type.ColorMode == ParticleType.ColorModes.Blink)
                    Color = (Calc.BetweenInterval(Life, .1f) ? StartColor : Type.Color2);
                else if (Type.ColorMode == ParticleType.ColorModes.Choose)
                    Color = StartColor;

                if (alpha < 1f)
                    Color *= alpha;
            }
            
            //Speed
            Position += Speed * dt;
            Speed += Type.Acceleration * dt;
            Speed = Calc.Approach(Speed, Vector2.Zero, Type.Friction * dt);
            if (Type.SpeedMultiplier != 1)
                Speed *= (float)Math.Pow(Type.SpeedMultiplier, dt);

            //Scale Out
            if (Type.ScaleOut)
                Size = StartSize * Ease.CubeOut(ease);
        }

        public void Render()
        {
            var renderAt = new Vector2((int)Position.X, (int)Position.Y);
            if (Track != null)
                renderAt += Track.Position;

            Draw.SpriteBatch.Draw(Source.Texture, renderAt, Source.ClipRect, Color, Rotation, Source.Center, Size, SpriteEffects.None, 0);
        }

        public void Render(float alpha)
        {
            var renderAt = new Vector2((int)Position.X, (int)Position.Y);
            if (Track != null)
                renderAt += Track.Position;
            
            Draw.SpriteBatch.Draw(Source.Texture, renderAt, Source.ClipRect, Color * alpha, Rotation, Source.Center, Size, SpriteEffects.None, 0);
        }
    }
}
