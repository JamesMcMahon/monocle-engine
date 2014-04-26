using Microsoft.Xna.Framework;

namespace Monocle
{
    public struct Bezier
    {
        public Vector2 Begin;
        public Vector2 End;
        public Vector2 Control;

        public Bezier(Vector2 begin, Vector2 end, Vector2 control)
        {
            Begin = begin;
            End = end;
            Control = control;
        }

        public void DoubleControl()
        {
            Vector2 axis = End - Begin;
            Vector2 mid = Begin + axis/2;
            Vector2 diff = Control - mid;
            Control += diff;
        }

        public Vector2 GetPoint(float percent)
        {
            float reverse = 1 - percent;
            return (reverse * reverse * Begin) + (2f * reverse * percent * Control) + (percent * percent * End);
        }
    }
}
