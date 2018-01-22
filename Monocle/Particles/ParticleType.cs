using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    public class ParticleType
    {
        public enum ColorModes { Static, Choose, Blink, Fade };
        public enum FadeModes { None, Linear, Late, InAndOut };
        public enum RotationModes { None, Random, SameAsDirection };

        private static List<ParticleType> AllTypes = new List<ParticleType>();

        public MTexture Source;
        public Chooser<MTexture> SourceChooser;
        public Color Color;
        public Color Color2;
        public ColorModes ColorMode;
        public FadeModes FadeMode;
        public float SpeedMin;
        public float SpeedMax;
        public float SpeedMultiplier;
        public Vector2 Acceleration;
        public float Friction;
        public float Direction;
        public float DirectionRange;
        public float LifeMin;
        public float LifeMax;
        public float Size;
        public float SizeRange;
        public float SpinMin;
        public float SpinMax;
        public bool SpinFlippedChance;
        public RotationModes RotationMode;
        public bool ScaleOut;
        public bool UseActualDeltaTime;

        public ParticleType()
        {
            Color = Color2 = Color.White;
            ColorMode = ColorModes.Static;
            FadeMode = FadeModes.None;
            SpeedMin = SpeedMax = 0;
            SpeedMultiplier = 1;
            Acceleration = Vector2.Zero;
            Friction = 0f;
            Direction = DirectionRange = 0;
            LifeMin = LifeMax = 0;
            Size = 2;
            SizeRange = 0;
            SpinMin = SpinMax = 0;
            SpinFlippedChance = false;
            RotationMode = RotationModes.None;

            AllTypes.Add(this);
        }

        public ParticleType(ParticleType copyFrom)
        {
            Source = copyFrom.Source;
            SourceChooser = copyFrom.SourceChooser;
            Color = copyFrom.Color;
            Color2 = copyFrom.Color2;
            ColorMode = copyFrom.ColorMode;
            FadeMode = copyFrom.FadeMode;
            SpeedMin = copyFrom.SpeedMin;
            SpeedMax = copyFrom.SpeedMax;
            SpeedMultiplier = copyFrom.SpeedMultiplier;
            Acceleration = copyFrom.Acceleration;
            Friction = copyFrom.Friction;
            Direction = copyFrom.Direction;
            DirectionRange = copyFrom.DirectionRange;
            LifeMin = copyFrom.LifeMin;
            LifeMax = copyFrom.LifeMax;
            Size = copyFrom.Size;
            SizeRange = copyFrom.SizeRange;
            RotationMode = copyFrom.RotationMode;
            SpinMin = copyFrom.SpinMin;
            SpinMax = copyFrom.SpinMax;
            SpinFlippedChance = copyFrom.SpinFlippedChance;
            ScaleOut = copyFrom.ScaleOut;
            UseActualDeltaTime = copyFrom.UseActualDeltaTime;

            AllTypes.Add(this);
        }

        public Particle Create(ref Particle particle, Vector2 position)
        {
            return Create(ref particle, position, Direction);
        }

        public Particle Create(ref Particle particle, Vector2 position, Color color)
        {
            return Create(ref particle, null, position, Direction, color);
        }

        public Particle Create(ref Particle particle, Vector2 position, float direction)
        {
            return Create(ref particle, null, position, direction, Color);
        }

        public Particle Create(ref Particle particle, Vector2 position, Color color, float direction)
        {
            return Create(ref particle, null, position, direction, color);
        }

        public Particle Create(ref Particle particle, Entity entity, Vector2 position, float direction, Color color)
        {
            particle.Track = entity;
            particle.Type = this;
            particle.Active = true;
            particle.Position = position;

            // source texture
            if (SourceChooser != null)
                particle.Source = SourceChooser.Choose();
            else if (Source != null)
                particle.Source = Source;
            else
                particle.Source = Draw.Particle; 
                 
            // size
            if (SizeRange != 0)
                particle.StartSize = particle.Size = Size - SizeRange * .5f + Calc.Random.NextFloat(SizeRange);
            else
                particle.StartSize = particle.Size = Size;

            // color
            if (ColorMode == ColorModes.Choose)
                particle.StartColor = particle.Color = Calc.Random.Choose(color, Color2);
            else
                particle.StartColor = particle.Color = color;

            // speed / direction
            var moveDirection = direction - DirectionRange / 2 + Calc.Random.NextFloat() * DirectionRange;
            particle.Speed = Calc.AngleToVector(moveDirection, Calc.Random.Range(SpeedMin, SpeedMax));

            // life
            particle.StartLife = particle.Life = Calc.Random.Range(LifeMin, LifeMax);

            // rotation
            if (RotationMode == RotationModes.Random)
                particle.Rotation = Calc.Random.NextAngle();
            else if (RotationMode == RotationModes.SameAsDirection)
                particle.Rotation = moveDirection;
            else
                particle.Rotation = 0;

            // spin
            particle.Spin = Calc.Random.Range(SpinMin, SpinMax);
            if (SpinFlippedChance)
                particle.Spin *= Calc.Random.Choose(1, -1);

            return particle;
        }

    }
}
