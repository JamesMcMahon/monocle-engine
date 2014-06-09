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

        public void CenterOrigin()
        {
            Position.X = -Width / 2;
            Position.Y = -Height / 2;
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

        public override void Render(Color color)
        {
            Draw.HollowRect(AbsoluteX, AbsoluteY, Width, Height, color);
        }

        public void SetFromRectangle(Rectangle rect)
        {
            Position = new Vector2(rect.X, rect.Y);
            Width = rect.Width;
            Height = rect.Height;
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
            return point.X >= AbsoluteLeft && point.Y >= AbsoluteTop && point.X < AbsoluteRight && point.Y < AbsoluteBottom;
        }

        public override bool Collide(Rectangle rect)
        {
            return AbsoluteRight > rect.Left && AbsoluteBottom > rect.Top && AbsoluteLeft < rect.Right && AbsoluteTop < rect.Bottom;
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            PointSectors fromSector = GetSector(from);
            PointSectors toSector = GetSector(to);

            if (fromSector == PointSectors.Center || toSector == PointSectors.Center)
                return true;
            else if ((fromSector & toSector) != 0)
                return false;
            else
            {
                PointSectors both = fromSector | toSector;

                //Do line checks against the edges
                Vector2 edgeFrom;
                Vector2 edgeTo;

                if ((both & PointSectors.Top) != 0)
                {
                    GetTopEdge(out edgeFrom, out edgeTo);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Bottom) != 0)
                {
                    GetBottomEdge(out edgeFrom, out edgeTo);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Left) != 0)
                {
                    GetLeftEdge(out edgeFrom, out edgeTo);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Right) != 0)
                {
                    GetRightEdge(out edgeFrom, out edgeTo);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }
            }

            return false;
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
            //Check if the circle's centerpoint is within the hitbox
            if (Collide(circle.AbsolutePosition))
                return true;

            //Check the circle against the relevant edges
            Vector2 edgeFrom;
            Vector2 edgeTo;
            PointSectors sector = GetSector(circle.AbsolutePosition);

            if ((sector & PointSectors.Top) != 0)
            {
                GetTopEdge(out edgeFrom, out edgeTo);
                if (circle.Collide(edgeFrom, edgeTo))
                    return true;
            }

            if ((sector & PointSectors.Bottom) != 0)
            {
                GetBottomEdge(out edgeFrom, out edgeTo);
                if (circle.Collide(edgeFrom, edgeTo))
                    return true;
            }

            if ((sector & PointSectors.Left) != 0)
            {
                GetLeftEdge(out edgeFrom, out edgeTo);
                if (circle.Collide(edgeFrom, edgeTo))
                    return true;
            }

            if ((sector & PointSectors.Right) != 0)
            {
                GetRightEdge(out edgeFrom, out edgeTo);
                if (circle.Collide(edgeFrom, edgeTo))
                    return true;
            }

            return false;
        }

        public override bool Collide(ColliderList list)
        {
            return list.Collide(this);
        }

        public override bool Collide(SlopeHitbox slope)
        {
            return slope.Collide(this);
        }

        /*
         *  Bitflags and helpers for using the Cohen–Sutherland algorithm
         *  http://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
         *  
         * Sector bitflags:
         *      1001  1000  1010
         *      0001  0000  0010
         *      0101  0100  0110
         */

        [Flags]
        private enum PointSectors { Center = 0, Top = 1, Bottom = 2, TopLeft = 9, TopRight = 5, Left = 8, Right = 4, BottomLeft = 10, BottomRight = 6 };

        private PointSectors GetSector(Vector2 point)
        {
            PointSectors sector = PointSectors.Center;

            if (point.X < AbsoluteLeft)
                sector |= PointSectors.Left;
            else if (point.X >= AbsoluteRight)
                sector |= PointSectors.Right;

            if (point.Y < AbsoluteTop)
                sector |= PointSectors.Top;
            else if (point.Y >= AbsoluteBottom)
                sector |= PointSectors.Bottom;

            return sector;
        }
    }
}
