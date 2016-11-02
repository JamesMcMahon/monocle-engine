using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public struct Particle
    {
        public ParticleType Type;

        public bool Active;
        public Color Color;
        public Vector2 Position;
        public Vector2 Speed;
        public float Size;
        public float StartSize;
        public float Life;
        public float StartLife;
        public float ColorSwitch;
        public float Rotation;

        public void Update()
        {
			var dt = (Type.UseActualDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime);
            var ease = Life / StartLife;

            //Life
            Life -= dt;
            if (Life <= 0)
            {
                Active = false;
                return;
            }

            //Color switch
            if (Type.ColorMode == ParticleType.ColorModes.Fade)
                Color = Color.Lerp(Type.Color2, Type.Color, ease);
            else if (Type.ColorMode == ParticleType.ColorModes.Blink)
                Color = Calc.BetweenInterval(Life, .1f) ? Type.Color : Type.Color2;

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
            if (Type.Source == null)
                Draw.SpriteBatch.Draw(Draw.Particle.Texture2D, RenderPosition - new Vector2((int)(Size * 0.5f), (int)(Size * 0.5f)), Draw.Particle.ClipRect, Color, 0, Vector2.Zero, Size, SpriteEffects.None, 0);
            else
                Draw.SpriteBatch.Draw(Type.Source.Texture2D, RenderPosition, Type.Source.ClipRect, Color, Rotation, Type.Source.Center, Size, SpriteEffects.None, 0);
        }

        public Vector2 RenderPosition
        {
            get
            {
                return Calc.Floor(Position);
            }
        }
    }
}
