using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    public struct Pnt
    {
        static public readonly Pnt Zero = new Pnt(0, 0);
        static public readonly Pnt UnitX = new Pnt(1, 0);
        static public readonly Pnt UnitY = new Pnt(0, 1);
        static public readonly Pnt One = new Pnt(1, 1);

        public int X;
        public int Y;

        public Pnt(int x, int y)
        {
            X = x;
            Y = y;
        }

        #region Pnt operators

        static public bool operator ==(Pnt a, Pnt b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        static public bool operator !=(Pnt a, Pnt b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        static public Pnt operator +(Pnt a, Pnt b)
        {
            return new Pnt(a.X + b.X, a.Y + b.Y);
        }

        static public Pnt operator -(Pnt a, Pnt b)
        {
            return new Pnt(a.X - b.X, a.Y - b.Y);
        }

        static public Pnt operator *(Pnt a, Pnt b)
        {
            return new Pnt(a.X * b.X, a.Y * b.Y);
        }

        static public Pnt operator /(Pnt a, Pnt b)
        {
            return new Pnt(a.X / b.X, a.Y / b.Y);
        }

        static public Pnt operator %(Pnt a, Pnt b)
        {
            return new Pnt(a.X % b.X, a.Y % b.Y);
        }

        #endregion

        #region int operators

        static public bool operator ==(Pnt a, int b)
        {
            return a.X == b && a.Y == b;
        }

        static public bool operator !=(Pnt a, int b)
        {
            return a.X != b || a.Y != b;
        }

        static public Pnt operator +(Pnt a, int b)
        {
            return new Pnt(a.X + b, a.Y + b);
        }

        static public Pnt operator -(Pnt a, int b)
        {
            return new Pnt(a.X - b, a.Y - b);
        }

        static public Pnt operator *(Pnt a, int b)
        {
            return new Pnt(a.X * b, a.Y * b);
        }

        static public Pnt operator /(Pnt a, int b)
        {
            return new Pnt(a.X / b, a.Y / b);
        }

        static public Pnt operator %(Pnt a, int b)
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
