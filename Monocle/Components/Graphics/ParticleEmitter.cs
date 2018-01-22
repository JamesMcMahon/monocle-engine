using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Monocle
{
    public class ParticleEmitter : Component
    {

        public ParticleSystem System;
        public ParticleType Type;

        public Entity Track;
        public float Interval;
        public Vector2 Position;
        public Vector2 Range;
        public int Amount;
        public float? Direction;

        private float timer = 0f;

        public ParticleEmitter(ParticleSystem system, ParticleType type, Vector2 position, Vector2 range, int amount, float interval) : base(true, false)
        {
            System = system;
            Type = type;
            Position = position;
            Range = range;
            Amount = amount;
            Interval = interval;
        }

        public ParticleEmitter(ParticleSystem system, ParticleType type, Vector2 position, Vector2 range, float direction, int amount, float interval) 
            : this(system, type, position, range, amount, interval)
        {
            Direction = direction;
        }

        public ParticleEmitter(ParticleSystem system, ParticleType type, Entity track, Vector2 position, Vector2 range, float direction, int amount, float interval)
            : this(system, type, position, range, amount, interval)
        {
            Direction = direction;
            Track = track;
        }

        public void SimulateCycle()
        {
            Simulate(Type.LifeMax);
        }

        public void Simulate(float duration)
        {
            var steps = duration / Interval;
            for (var i = 0; i < steps; i++)
            {
                for (int j = 0; j < Amount; j++)
                {
                    // create the particle
                    var particle = new Particle();
                    var pos = Entity.Position + Position + Calc.Random.Range(-Range, Range);
                    if (Direction.HasValue)
                        particle = Type.Create(ref particle, pos, Direction.Value);
                    else
                        particle = Type.Create(ref particle, pos);
                    particle.Track = Track;

                    // simulate for a duration
                    var simulateFor = duration - Interval * i;
                    if (particle.SimulateFor(simulateFor))
                        System.Add(particle);
                }
            }
        }

        public void Emit()
        {
            if (Direction.HasValue)
                System.Emit(Type, Amount, Entity.Position + Position, Range, Direction.Value);
            else
                System.Emit(Type, Amount, Entity.Position + Position, Range);
        }

        public override void Update()
        {
            timer -= Engine.DeltaTime;
            if (timer <= 0)
            {
                timer = Interval;
                Emit();
            }
        }

    }
}
