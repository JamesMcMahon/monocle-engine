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
        public bool RemoveOnFinish;
        public Action<Vector2> OnShake;

        private bool on;

        public Shaker(bool on = true, Action<Vector2> onShake = null)
            : base(true, false)
        {
            this.on = on;
            OnShake = onShake;
        }

        public Shaker(float time, bool removeOnFinish, Action<Vector2> onShake = null)
            : this(true, onShake)
        {
            Timer = time;
            RemoveOnFinish = removeOnFinish;
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
                {
                    Timer = 0;
                    if (Value != Vector2.Zero)
                    {
                        Value = Vector2.Zero;
                        if (OnShake != null)
                            OnShake(Vector2.Zero);
                    }
                }
            }
        }

        public Shaker ShakeFor(float seconds, bool removeOnFinish)
        {
            on = true;
            Timer = seconds;
            RemoveOnFinish = removeOnFinish;

            return this;
        }

        public override void Update()
        {
            if (on && Timer > 0)
            {
                Timer -= Engine.DeltaTime;
                if (Timer <= 0)
                {
                    on = false;
                    Value = Vector2.Zero;
                    if (OnShake != null)
                        OnShake(Vector2.Zero);
                    if (RemoveOnFinish)
                        RemoveSelf();
                    return;
                }
            }

            if (on && Scene.OnInterval(Interval))
            {
                Value = Calc.Random.ShakeVector();
                if (OnShake != null)
                    OnShake(Value);
            }
        }
    }
}
