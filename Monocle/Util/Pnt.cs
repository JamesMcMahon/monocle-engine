using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    public struct Pnt
    {
        public static readonly Pnt Zero = new Pnt(0, 0);
        public static readonly Pnt UnitX = new Pnt(1, 0);
        public static readonly Pnt UnitY = new Pnt(0, 1);
        public static readonly Pnt One = new Pnt(1, 1);

        public int X;
        public int Y;

        public Pnt(int x, int y)
        {
            X = x;
            Y = y;
        }

        #region Pnt operators

        public static bool operator ==(Pnt a, Pnt b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Pnt a, Pnt b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Pnt operator +(Pnt a, Pnt b)
        {
            return new Pnt(a.X + b.X, a.Y + b.Y);
        }

        public static Pnt operator -(Pnt a, Pnt b)
        {
            return new Pnt(a.X - b.X, a.Y - b.Y);
        }

        public static Pnt operator *(Pnt a, Pnt b)
        {
            return new Pnt(a.X * b.X, a.Y * b.Y);
        }

        public static Pnt operator /(Pnt a, Pnt b)
        {
            return new Pnt(a.X / b.X, a.Y / b.Y);
        }

        public static Pnt operator %(Pnt a, Pnt b)
        {
            return new Pnt(a.X % b.X, a.Y % b.Y);
        }

        #endregion

        #region int operators

        public static bool operator ==(Pnt a, int b)
        {
            return a.X == b && a.Y == b;
        }

        public static bool operator !=(Pnt a, int b)
        {
            return a.X != b || a.Y != b;
        }

        public static Pnt operator +(Pnt a, int b)
        {
            return new Pnt(a.X + b, a.Y + b);
        }

        public static Pnt operator -(Pnt a, int b)
        {
            return new Pnt(a.X - b, a.Y - b);
        }

        public static Pnt operator *(Pnt a, int b)
        {
            return new Pnt(a.X * b, a.Y * b);
        }

        public static Pnt operator /(Pnt a, int b)
        {
            return new Pnt(a.X / b, a.Y / b);
        }

        public static Pnt operator %(Pnt a, int b)
        {
            return new Pnt(a.X % b, a.Y % b);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return X * 10000 + Y;
        }

        public override string ToString()
        {
            return "{ X: " + X + ", Y: " + Y + " }";
        }
    }
}
