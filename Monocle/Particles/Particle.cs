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
        public float Life;
        public float ColorSwitch;
        public float Rotation;
        public float SizeChange;

        public void Update()
        {
			var dt = (Type.UseActualDeltaTime ? Engine.ActualDeltaTime : Engine.DeltaTime);

			//Life
			Life -= dt;
            if (Life <= 0)
            {
                Active = false;
                return;
            }

            //Color switch
            if (ColorSwitch > 0)
            {
                ColorSwitch -= dt;
                if (ColorSwitch <= 0)
                {
                    if (Type.ColorSwitchLoop)
                        ColorSwitch = Type.ColorSwitch;

                    if (Color == Type.Color)
                        Color = Type.Color2;
                    else
                        Color = Type.Color;
                }
            }

            //Speed
            Position += Speed * dt;
            Speed += Type.Acceleration * dt;
            if (Type.SpeedMultiplier != 1)
                Speed *= (float)Math.Pow(Type.SpeedMultiplier, dt);

            //Scale Out
            Size += SizeChange * dt;
        }

        public void Render()
        {
            if (Type.Source == null)
                Draw.SpriteBatch.Draw(Draw.Particle.Texture2D, RenderPosition, Draw.Particle.ClipRect, Color, 0, Vector2.One, Size * 0.5f, SpriteEffects.None, 0);
            else
                Draw.SpriteBatch.Draw(Type.Source.Texture2D, RenderPosition, Type.Source.ClipRect, Color, Rotation, Type.Source.Center, Size, SpriteEffects.None, 0);
        }

        public Vector2 RenderPosition { get { return Calc.Floor(Position); } }
    }
}
