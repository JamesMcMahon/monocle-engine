using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class CounterSet<T> : Component
    {
        private Dictionary<T, float> counters;
        private float timer;

        public CounterSet()
            : base(true, false)
        {
            counters = new Dictionary<T, float>();
        }

        public float this[T index]
        {
            get
            {
                float value;
                if (counters.TryGetValue(index, out value))
                    return Math.Max(value - timer, 0);
                else
                    return 0;
            }

            set
            {
                counters[index] = timer + value;
            }
        }

        public bool Check(T index)
        {
            float value;
            if (counters.TryGetValue(index, out value))
                return value - timer > 0;
            else
                return false;
        }

        public override void Update()
        {
            timer += Engine.DeltaTime;
        }
    }
}
