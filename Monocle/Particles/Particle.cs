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
            //Life
            Life -= Engine.DeltaTime;
            if (Life <= 0)
            {
                Active = false;
                return;
            }

            //Color switch
            if (ColorSwitch > 0)
            {
                ColorSwitch -= Engine.DeltaTime;
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
            Position += Speed * Engine.DeltaTime;
            Speed += Type.Acceleration * Engine.DeltaTime;
            if (Type.SpeedMultiplier != 1)
                Speed *= (float)Math.Pow(Type.SpeedMultiplier, Engine.DeltaTime);

            //Scale Out
            Size += SizeChange * Engine.DeltaTime;
        }

        public void Render()
        {
            if (Type.Source == null)
                Draw.SpriteBatch.Draw(Draw.Particle.Texture2D, RenderPosition, Draw.Particle.Rect, Color, 0, Vector2.One, Size * .5f, SpriteEffects.None, 0);
            else
                Draw.SpriteBatch.Draw(Type.Source.Texture2D, RenderPosition, Type.Source.Rect, Color, Rotation, Type.Source.Center, Size, SpriteEffects.None, 0);
        }

        public Vector2 RenderPosition { get { return Calc.Floor(Position); } }
    }
}
