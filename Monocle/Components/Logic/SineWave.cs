using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
    public class SineWave : Component
    {
        /*
         *    SINE WAVE:
         * 
         *  1       x      
         *  |    x     x   
         *  |  x         x 
         *  | x           x
         *  -x-------------x-------------x
         *  |               x           x
         *  |                x         x
         *  |                  x     x
         * -1                     x
         * 
         *     COS WAVE:
         * 
         *  1x                           x
         *  |   x                     x
         *  |     x                 x
         *  |      x               x
         *  --------x-------------x-------
         *  |        x           x
         *  |         x         x
         *  |           x     x
         * -1              x
         * 
         */

        public float Frequency = 1f;
        public float Rate = 1f;
        public float Value { get; private set; }
        public float ValueOverTwo { get; private set; }
        public float TwoValue { get; private set; }
        public Action<float> OnUpdate;
        public bool UseRawDeltaTime;

        private float counter;

        public SineWave()
            : base(true, false)
        {

        }

        public SineWave(float frequency)
            : this()
        {
            Frequency = frequency;
        }

        public override void Update()
        {
            Counter += MathHelper.TwoPi * Frequency * Rate * (UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime);
            if (OnUpdate != null)
                OnUpdate(Value);
        }

        public float ValueOffset(float offset)
        {
            return (float)Math.Sin(counter + offset);
        }

        public SineWave Randomize()
        {
            Counter = Calc.Random.NextFloat() * MathHelper.TwoPi * 2;
            return this;
        }

        public void Reset()
        {
            Counter = 0;
        }

        public void StartUp()
        {
            Counter = MathHelper.PiOver2;
        }

        public void StartDown()
        {
            Counter = MathHelper.PiOver2 * 3f;
        }

        public float Counter
        {
            get
            {
                return counter;
            }

            set
            {
                counter = (value + MathHelper.TwoPi * 4) % (MathHelper.TwoPi * 4);

                Value = (float)Math.Sin(counter);
                ValueOverTwo = (float)Math.Sin(counter / 2);
                TwoValue = (float)Math.Sin(counter * 2);
            }
        }
    }
}
