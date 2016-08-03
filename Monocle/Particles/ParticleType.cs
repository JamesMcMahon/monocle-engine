using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    public class ParticleType
    {
        static private List<ParticleType> AllTypes = new List<ParticleType>();

        public MTexture Source;
        public Color Color;
        public Color Color2;
        public bool FadeColor;
        public float Speed;
        public float SpeedRange;
        public float SpeedMultiplier;
        public Vector2 Acceleration;
		public float Friction;
        public float Direction;
        public float DirectionRange;
        public float Life;
        public float LifeRange;
        public float Size;
        public float SizeRange;
        public bool Rotated;
        public bool RandomRotate;
        public bool ScaleOut;
		public bool UseActualDeltaTime;

        public ParticleType()
        {
            Color = Color2 = Color.White;
            FadeColor = false;
            Speed = SpeedRange = 0;
            SpeedMultiplier = 1;
            Acceleration = Vector2.Zero;
			Friction = 0f;
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
            FadeColor = copy.FadeColor;
            Speed = copy.Speed;
            SpeedRange = copy.SpeedRange;
            SpeedMultiplier = copy.SpeedMultiplier;
            Acceleration = copy.Acceleration;
			Friction = copy.Friction;
            Direction = copy.Direction;
            DirectionRange = copy.DirectionRange;
            Life = copy.Life;
            LifeRange = copy.LifeRange;
            Size = copy.Size;
            SizeRange = copy.SizeRange;
            Rotated = copy.Rotated;
            RandomRotate = copy.RandomRotate;
            ScaleOut = copy.ScaleOut;
			UseActualDeltaTime = copy.UseActualDeltaTime;

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
            particle.Size = Calc.Random.Range(Size, Size + SizeRange);
            particle.Color = Color;
            particle.Speed = Calc.AngleToVector(direction - DirectionRange / 2 + Calc.Random.NextFloat() * DirectionRange, Calc.Random.Range(Speed, SpeedRange));
            particle.MaxLife = particle.Life = Calc.Random.Range(Life, LifeRange);
            if (RandomRotate)
                particle.Rotation = Calc.Random.NextAngle();
            else if (Rotated)
                particle.Rotation = direction;

			if (ScaleOut)
				particle.SizeChange = -particle.Size / particle.Life;
			else
				particle.SizeChange = 0;

            return particle;
        }
    }
}
