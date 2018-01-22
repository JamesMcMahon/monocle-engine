using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class ShakerList : Component
    {
        public Vector2[] Values;
        public float Interval = .05f;
        public float Timer;
        public bool RemoveOnFinish;
        public Action<Vector2[]> OnShake;

        private bool on;

        public ShakerList(int length, bool on = true, Action<Vector2[]> onShake = null)
            : base(true, false)
        {
            Values = new Vector2[length];
            this.on = on;
            OnShake = onShake;
        }

        public ShakerList(int length, float time, bool removeOnFinish, Action<Vector2[]> onShake = null)
            : this(length, true, onShake)
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
                    if (Values[0] != Vector2.Zero)
                    {
                        for (var i = 0; i < Values.Length; i++)
                            Values[i] = Vector2.Zero;
                        if (OnShake != null)
                            OnShake(Values);
                    }
                }
            }
        }

        public ShakerList ShakeFor(float seconds, bool removeOnFinish)
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
                    for (var i = 0; i < Values.Length; i++)
                        Values[i] = Vector2.Zero;
                    if (OnShake != null)
                        OnShake(Values);
                    if (RemoveOnFinish)
                        RemoveSelf();
                    return;
                }
            }

            if (on && Scene.OnInterval(Interval))
            {
                for (var i = 0; i < Values.Length; i++)
                    Values[i] = Calc.Random.ShakeVector();
                if (OnShake != null)
                    OnShake(Values);
            }
        }
    }
}
