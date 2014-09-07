using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
    static public class Ease
    {
        public delegate float Easer(float t);

        static public readonly Easer SineIn = (float t) => { return -(float)Math.Cos(MathHelper.PiOver2 * t) + 1; };
        static public readonly Easer SineOut = (float t) => { return (float)Math.Sin(MathHelper.PiOver2 * t); };
        static public readonly Easer SineInOut = (float t) => { return -(float)Math.Cos(MathHelper.Pi * t) / 2f + .5f; };

        static public readonly Easer QuadIn = (float t) => { return t * t; };
        static public readonly Easer QuadOut = Invert(QuadIn);
        static public readonly Easer QuadInOut = Follow(QuadIn, QuadOut);

        static public readonly Easer CubeIn = (float t) => { return t * t * t; };
        static public readonly Easer CubeOut = Invert(CubeIn);
        static public readonly Easer CubeInOut = Follow(CubeIn, CubeOut);

        static public readonly Easer QuintIn = (float t) => { return t * t * t * t * t; };
        static public readonly Easer QuintOut = Invert(QuintIn);
        static public readonly Easer QuintInOut = Follow(QuintIn, QuintOut);

        static public readonly Easer ExpoIn = (float t) => { return (float)Math.Pow(2, 10 * (t - 1)); };
        static public readonly Easer ExpoOut = Invert(ExpoIn);
        static public readonly Easer ExpoInOut = Follow(ExpoIn, ExpoOut);

        static public readonly Easer BackIn = (float t) => { return t * t * (2.70158f * t - 1.70158f); };
        static public readonly Easer BackOut = Invert(BackIn);
        static public readonly Easer BackInOut = Follow(BackIn, BackOut);

        static public readonly Easer BigBackIn = (float t) => { return t * t * (4f * t - 3f); };
        static public readonly Easer BigBackOut = Invert(BigBackIn);
        static public readonly Easer BigBackInOut = Follow(BigBackIn, BigBackOut);

        static public readonly Easer ElasticIn = (float t) =>
            {
                var ts = t * t;
                var tc = ts * t;
                return (33 * tc * ts + -59 * ts * ts + 32 * tc + -5 * ts);
            };
        static public readonly Easer ElasticOut = (float t) =>
            {
                var ts = t * t;
                var tc = ts * t;
                return (33 * tc * ts + -106 * ts * ts + 126 * tc + -67 * ts + 15 * t);
            };
        static public readonly Easer ElasticInOut = Follow(ElasticIn, ElasticOut);

        private const float B1 = 1f / 2.75f;
        private const float B2 = 2f / 2.75f;
        private const float B3 = 1.5f / 2.75f;
        private const float B4 = 2.5f / 2.75f;
        private const float B5 = 2.25f / 2.75f;
        private const float B6 = 2.625f / 2.75f;

        static public readonly Easer BounceIn = (float t) =>
            {
                t = 1 - t;
                if (t < B1)
                    return 1 - 7.5625f * t * t;
                if (t < B2)
                    return 1 - (7.5625f * (t - B3) * (t - B3) + .75f);
                if (t < B4)
                    return 1 - (7.5625f * (t - B5) * (t - B5) + .9375f);
                return 1 - (7.5625f * (t - B6) * (t - B6) + .984375f);
            };

        static public readonly Easer BounceOut = (float t) =>
            {
                if (t < B1)
                    return 7.5625f * t * t;
                if (t < B2)
                    return 7.5625f * (t - B3) * (t - B3) + .75f;
                if (t < B4)
                    return 7.5625f * (t - B5) * (t - B5) + .9375f;
                return 7.5625f * (t - B6) * (t - B6) + .984375f;
            };

        static public readonly Easer BounceInOut = (float t) =>
            {
                if (t < .5f)
                {
                    t = 1 - t * 2;
                    if (t < B1)
                        return (1 - 7.5625f * t * t) / 2;
                    if (t < B2)
                        return (1 - (7.5625f * (t - B3) * (t - B3) + .75f)) / 2;
                    if (t < B4)
                        return (1 - (7.5625f * (t - B5) * (t - B5) + .9375f)) / 2;
                    return (1 - (7.5625f * (t - B6) * (t - B6) + .984375f)) / 2;
                }
                t = t * 2 - 1;
                if (t < B1)
                    return (7.5625f * t * t) / 2 + .5f;
                if (t < B2)
                    return (7.5625f * (t - B3) * (t - B3) + .75f) / 2 + .5f;
                if (t < B4)
                    return (7.5625f * (t - B5) * (t - B5) + .9375f) / 2 + .5f;
                return (7.5625f * (t - B6) * (t - B6) + .984375f) / 2 + .5f;
            };

        static public Easer Invert(Easer easer)
        {
            return (float t) => { return 1 - easer(1 - t); };
        }

        static public Easer Follow(Easer first, Easer second)
        {
            return (float t) => { return (t <= 0.5f) ? first(t * 2) / 2 : second(t * 2 - 1) / 2 + 0.5f; };
        }

        static public float UpDown(float eased)
        {
            if (eased <= .5f)
                return eased * 2;
            else
                return 1 - (eased - .5f) * 2;
        }
    }
}
