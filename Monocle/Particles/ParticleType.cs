using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Monocle
{
    public class ParticleType
    {
        static private List<ParticleType> AllTypes = new List<ParticleType>();

        public Subtexture Source;
        public Color Color;
        public Color Color2;
        public int ColorSwitch;
        public bool ColorSwitchLoop;
        public float Speed;
        public float SpeedRange;
        public float SpeedMultiplier;
        public Vector2 Acceleration;
        public float Direction;
        public float DirectionRange;
        public int Life;
        public int LifeRange;
        public float Size;
        public float SizeRange;
        public bool Rotated;
        public bool RandomRotate;
        public bool ScaleOut;

        public ParticleType()
        {
            Color = Color2 = Color.White;
            ColorSwitch = 0;
            ColorSwitchLoop = true;
            Speed = SpeedRange = 0;
            SpeedMultiplier = 1;
            Acceleration = Vector2.Zero;
            Direction = DirectionRange = 0;
            Life = LifeRange = 0;
            Size = 2;
            SizeRange = 0;
            Rotated = true;

            AllTypes.Add(this);
        }

        public ParticleType(ParticleType copy)
        {
            Source = copy.Source;
            Color = copy.Color;
            Color2 = copy.Color2;
            ColorSwitch = copy.ColorSwitch;
            ColorSwitchLoop = copy.ColorSwitchLoop;
            Speed = copy.Speed;
            SpeedRange = copy.SpeedRange;
            SpeedMultiplier = copy.SpeedMultiplier;
            Acceleration = copy.Acceleration;
            Direction = copy.Direction;
            DirectionRange = copy.DirectionRange;
            Life = copy.Life;
            LifeRange = copy.LifeRange;
            Size = copy.Size;
            SizeRange = copy.SizeRange;
            Rotated = copy.Rotated;
            RandomRotate = copy.RandomRotate;
            ScaleOut = copy.ScaleOut;

            AllTypes.Add(this);
        }

        public Particle Create(ref Particle particle, Vector2 position)
        {
            return Create(ref particle, position, Direction);
        }

        public Particle Create(ref Particle particle, Vector2 position, float direction)
        {
            particle.Type = this;
            particle.Active = true;
            particle.Position = position;
            if (Source == null)
                particle.Size = (int)Calc.Random.Range(Size, SizeRange);
            else
                particle.Size = Calc.Random.Range(Size, SizeRange);
            particle.Color = Color;
            particle.Speed = Calc.AngleToVector(direction - DirectionRange / 2 + Calc.Random.NextFloat() * DirectionRange, Calc.Random.Range(Speed, SpeedRange));
            particle.Life = Calc.Random.Range(Life, LifeRange);
            particle.ColorSwitch = ColorSwitch;
            if (RandomRotate)
                particle.Rotation = Calc.Random.NextAngle();
            else if (Rotated)
                particle.Rotation = direction;

            if (ScaleOut)
                particle.SizeChange = -(particle.Size / (particle.Life * 2f));
            else
                particle.SizeChange = 0;

            return particle;
        }
    }
}
