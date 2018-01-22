using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    public class Wiggler : Component
    {
        private static Stack<Wiggler> cache = new Stack<Wiggler>();

        public float Counter { get; private set; }
        public float Value { get; private set; }
        public bool StartZero;
        public bool UseRawDeltaTime;

        private float sineCounter;

        private float increment;
        private float sineAdd;
        private Action<float> onChange;
        private bool removeSelfOnFinish;

        public static Wiggler Create(float duration, float frequency,  Action<float> onChange = null, bool start = false, bool removeSelfOnFinish = false)
        {
            Wiggler wiggler;

            if (cache.Count > 0)
                wiggler = cache.Pop();
            else
                wiggler = new Wiggler();
            wiggler.Init(duration, frequency, onChange, start, removeSelfOnFinish);

            return wiggler;
        }

        private Wiggler()
            : base(false, false)
        {

        }

        private void Init(float duration, float frequency, Action<float> onChange, bool start, bool removeSelfOnFinish)
        {
            Counter = sineCounter = 0;
            UseRawDeltaTime = false;

            increment = 1f / duration;
            sineAdd = MathHelper.TwoPi * frequency;
            this.onChange = onChange;
            this.removeSelfOnFinish = removeSelfOnFinish;

            if (start)
                Start();
            else
                Active = false;
        }

        public override void Removed(Entity entity)
        {
            base.Removed(entity);
            cache.Push(this);
        }

        public void Start()
        {
            Counter = 1f;

            if (StartZero)
            {
                sineCounter = MathHelper.PiOver2;
                Value = 0;
                if (onChange != null)
                    onChange(0);
            }
            else
            {
                sineCounter = 0;
                Value = 1f;
                if (onChange != null)
                    onChange(1f);
            }

            Active = true;
        }

        public void Start(float duration, float frequency)
        {
            increment = 1f / duration;
            sineAdd = MathHelper.TwoPi * frequency;
            Start();
        }

        public void Stop()
        {
            Active = false;
        }

        public void StopAndClear()
        {
            Stop();
            Value = 0;
        }

        public override void Update()
        {
            if (UseRawDeltaTime)
            {
                sineCounter += sineAdd * Engine.RawDeltaTime;
                Counter -= increment * Engine.RawDeltaTime;
            }
            else
            {
                sineCounter += sineAdd * Engine.DeltaTime;
                Counter -= increment * Engine.DeltaTime;
            }

            if (Counter <= 0)
            {
                Counter = 0;
                Active = false;
                if (removeSelfOnFinish)
                    RemoveSelf();
            }

            Value = (float)Math.Cos(sineCounter) * Counter;

            if (onChange != null)
                onChange(Value);
        }
    }
}
