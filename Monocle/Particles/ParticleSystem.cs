using Microsoft.Xna.Framework;

namespace Monocle
{
    public class ParticleSystem : Entity
    {
        private Particle[] particles;
        private int nextSlot;

        public ParticleSystem(int depth, int maxParticles)
            : base()
        {
            particles = new Particle[maxParticles];
            Depth = depth;
        }

        public override void Update()
        {
            for (int i = 0; i < particles.Length; i++)
                if (particles[i].Active)
                    particles[i].Update();
        }

        public override void Render()
        {
            foreach (var p in particles)
                if (p.Active)
                    p.Render();
        }

        public void Emit(ParticleType type, Vector2 position)
        {        
            type.Create(ref particles[nextSlot], position);
            nextSlot = (nextSlot + 1) % particles.Length;
        }

        public void Emit(ParticleType type, int amount, Vector2 position, Vector2 positionRange)
        {
            for (int i = 0; i < amount; i++)
                Emit(type, Calc.Random.Range(position - positionRange, positionRange * 2));
        }

        public void Emit(ParticleType type, Vector2 position, float direction)
        {
            type.Create(ref particles[nextSlot], position, direction);
            nextSlot = (nextSlot + 1) % particles.Length;
        }

        public void Emit(ParticleType type, int amount, Vector2 position, Vector2 positionRange, float direction)
        {
            for (int i = 0; i < amount; i++)
                Emit(type, Calc.Random.Range(position - positionRange, positionRange * 2), direction);
        }
    }
}
