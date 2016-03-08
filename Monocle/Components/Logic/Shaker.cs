using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class Shaker : Component
    {
        public Vector2 Value;
        public float Interval = .05f;
        public float Timer;

        private bool on;

        public Shaker(bool on = true)
            : base(true, false)
        {
            this.on = on;
        }

        public bool On
        {
            get
            {
                return on;
            }

            set
            {
                on = value;
                if (!on)
                    Timer = 0;
            }
        }

        public void ShakeFor(float seconds)
        {
            on = true;
            Timer = seconds;
        }

        public override void Update()
        {
            if (on && Timer > 0)
            {
                Timer -= Engine.DeltaTime;
                if (Timer <= 0)
                    on = false;
            }

            if (on)
            {
                if (Entity.Scene.OnInterval(Interval))
                    Value = Calc.Random.ShakeVector();
            }
            else
                Value = Vector2.Zero;
        }
    }
}
