using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class Counter : Component
    {
        private float counter;

        public Counter()
            : base(true, false)
        {

        }

        public Counter(int setTo)
            : this()
        {
            Set(setTo);
        }

        public override void Update()
        {
            counter -= Engine.DeltaTime;
        }

        public void Set(float to)
        {
            counter = to;
        }

        public void SetMax(float to)
        {
            counter = MathHelper.Max(to, counter);
        }

        public bool JustPassed(float num)
        {
            return counter <= num && counter + Engine.DeltaTime > num;
        }

        static public implicit operator bool(Counter c)
        {
            return c.counter > 0;
        }

        static public implicit operator int(Counter c)
        {
            return (int)c.counter;
        }
    }
}
