using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class SlopeHitbox : Collider
    {
        public enum AnchorPoints { TopLeft, TopRight, BottomLeft, BottomRight };

        private float width;
        private float height;

        public AnchorPoints AnchorPoint { get; private set; }

        public SlopeHitbox(AnchorPoints anchorPoint, float width, float height, float x = 0, float y = 0)
        {
            AnchorPoint = anchorPoint;
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

        public override Collider Clone()
        {
            return new SlopeHitbox(AnchorPoint, width, height, Position.X, Position.Y);
        }

        public override void Render(Color color)
        {
            Vector2 topLeft = new Vector2(AbsoluteLeft, AbsoluteTop);
            Vector2 topRight = new Vector2(AbsoluteRight, AbsoluteTop);
            Vector2 bottomLeft = new Vector2(AbsoluteLeft, AbsoluteBottom);
            Vector2 bottomRight = new Vector2(AbsoluteRight, AbsoluteBottom);

            switch (AnchorPoint)
            {
                case AnchorPoints.BottomLeft:
                    Draw.Line(topLeft, bottomLeft, color);
                    Draw.Line(bottomLeft, bottomRight, color);
                    Draw.Line(bottomRight, topLeft, color);
                    break;

                case AnchorPoints.BottomRight:
                    Draw.Line(topRight, bottomRight, color);
                    Draw.Line(bottomRight, bottomLeft, color);
                    Draw.Line(bottomLeft, topRight, color);
                    break;

                case AnchorPoints.TopLeft:
                    Draw.Line(bottomLeft, topLeft, color);
                    Draw.Line(topLeft, topRight, color);
                    Draw.Line(topRight, bottomLeft, color);
                    break;

                case AnchorPoints.TopRight:
                    Draw.Line(topLeft, topRight, color);
                    Draw.Line(topRight, bottomRight, color);
                    Draw.Line(bottomRight, topLeft, color);
                    break;
            }
        }

        /*
         *  Checking against other colliders
         */

        public override bool Collide(Vector2 point)
        {
            if (point.X >= AbsoluteLeft && point.Y >= AbsoluteTop && point.X < AbsoluteRight && point.Y < AbsoluteBottom)
            {
                switch (AnchorPoint)
                {
                    case AnchorPoints.BottomLeft:

                        break;
                }
            }
            else
                return false;
        }

        public override bool Collide(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Hitbox hitbox)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Circle circle)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Grid grid)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(SlopeHitbox slope)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(ColliderList list)
        {
            return list.Collide(this);
        }
    }
}
