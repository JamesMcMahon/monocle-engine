using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
    public class Circle : Collider
    {
        public float Radius;

        public Circle(float radius, float x = 0, float y = 0)
        {
            Radius = radius;
            Position.X = x;
            Position.Y = y;
        }

        public override float Width
        {
            get { return Radius * 2; }
            set { Radius = value / 2; }
        }

        public override float Height
        {
            get { return Radius * 2; }
            set { Radius = value / 2; }
        }

        public override float Left
        {
            get { return Position.X - Radius; }
            set { Position.X = value + Radius; }
        }

        public override float Top
        {
            get { return Position.Y - Radius; }
            set { Position.Y = value + Radius; }
        }

        public override float Right
        {
            get { return Position.X + Radius; }
            set { Position.X = value - Radius; }
        }

        public override float Bottom
        {
            get { return Position.Y + Radius; }
            set { Position.Y = value - Radius; }
        }

        public override Collider Clone()
        {
            return new Circle(Radius, Position.X, Position.Y);
        }

        public override void Render(Color color)
        {
            throw new NotImplementedException();
        }

        /*
         *  Checking against other colliders
         */

        public override bool Collide(Vector2 point)
        {
            return Vector2.DistanceSquared(AbsolutePosition, point) < Radius * Radius;
        }

        public override bool Collide(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            return Vector2.DistanceSquared(AbsolutePosition, Calc.ClosestPointOnLine(from, to, AbsolutePosition)) < Radius * Radius;
        }

        public override bool Collide(Circle circle)
        {
            return Vector2.DistanceSquared(AbsolutePosition, circle.AbsolutePosition) < (Radius + circle.Radius) * (Radius + circle.Radius);
        }

        public override bool Collide(Hitbox hitbox)
        {
            return hitbox.Collide(this);
        }

        public override bool Collide(Grid grid)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(ColliderList list)
        {
            return list.Collide(this);
        }

    }
}
