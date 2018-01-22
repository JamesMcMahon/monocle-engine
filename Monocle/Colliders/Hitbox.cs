using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
    public class Hitbox : Collider
    {
        private float width;
        private float height;

        public Hitbox(float width, float height, float x = 0, float y = 0)
        {
            this.width = width;
            this.height = height;

            Position.X = x;
            Position.Y = y;
        }

        public override float Width
        {
            get { return width; }
            set { width = value; }
        }

        public override float Height
        {
            get { return height; }
            set { height = value; }
        }

        public override float Left
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public override float Top
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public override float Right
        {
            get { return Position.X + Width; }
            set { Position.X = value - Width; }
        }

        public override float Bottom
        {
            get { return Position.Y + Height; }
            set { Position.Y = value - Height; }
        }

        public bool Intersects(Hitbox hitbox)
        {
            return AbsoluteLeft < hitbox.AbsoluteRight && AbsoluteRight > hitbox.AbsoluteLeft && AbsoluteBottom > hitbox.AbsoluteTop && AbsoluteTop < hitbox.AbsoluteBottom;
        }

        public bool Intersects(float x, float y, float width, float height)
        {
            return AbsoluteRight > x && AbsoluteBottom > y && AbsoluteLeft < x + width && AbsoluteTop < y + height;
        }

        public override Collider Clone()
        {
            return new Hitbox(width, height, Position.X, Position.Y);
        }

        public override void Render(Camera camera, Color color)
        {
            Draw.HollowRect(AbsoluteX, AbsoluteY, Width, Height, color);
        }

        public void SetFromRectangle(Rectangle rect)
        {
            Position = new Vector2(rect.X, rect.Y);
            Width = rect.Width;
            Height = rect.Height;
        }

        public void Set(float x, float y, float w, float h)
        {
            Position = new Vector2(x, y);
            Width = w;
            Height = h;
        }

        /*
         *  Get edges
         */

        public void GetTopEdge(out Vector2 from, out Vector2 to)
        {
            from.X = AbsoluteLeft;
            to.X = AbsoluteRight;
            from.Y = to.Y = AbsoluteTop;
        }

        public void GetBottomEdge(out Vector2 from, out Vector2 to)
        {
            from.X = AbsoluteLeft;
            to.X = AbsoluteRight;
            from.Y = to.Y = AbsoluteBottom;
        }

        public void GetLeftEdge(out Vector2 from, out Vector2 to)
        {
            from.Y = AbsoluteTop;
            to.Y = AbsoluteBottom;
            from.X = to.X = AbsoluteLeft;
        }

        public void GetRightEdge(out Vector2 from, out Vector2 to)
        {
            from.Y = AbsoluteTop;
            to.Y = AbsoluteBottom;
            from.X = to.X = AbsoluteRight;
        }

        /*
         *  Checking against other colliders
         */

        public override bool Collide(Vector2 point)
        {
            return Monocle.Collide.RectToPoint(AbsoluteLeft, AbsoluteTop, Width, Height, point);
        }

        public override bool Collide(Rectangle rect)
        {
            return AbsoluteRight > rect.Left && AbsoluteBottom > rect.Top && AbsoluteLeft < rect.Right && AbsoluteTop < rect.Bottom;
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            return Monocle.Collide.RectToLine(AbsoluteLeft, AbsoluteTop, Width, Height, from, to);
        }

        public override bool Collide(Hitbox hitbox)
        {
            return Intersects(hitbox);
        }

        public override bool Collide(Grid grid)
        {
            return grid.Collide(Bounds);
        }

        public override bool Collide(Circle circle)
        {
            return Monocle.Collide.RectToCircle(AbsoluteLeft, AbsoluteTop, Width, Height, circle.AbsolutePosition, circle.Radius);
        }

        public override bool Collide(ColliderList list)
        {
            return list.Collide(this);
        }
    }
}
