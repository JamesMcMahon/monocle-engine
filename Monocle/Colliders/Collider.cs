using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
    public abstract class Collider
    {
        public Entity Entity { get; internal set; }
        public Component Component { get; private set; }
        public Vector2 Position;

        internal virtual void Added(Entity entity)
        {
            Entity = entity;
            Component = null;
        }

        internal virtual void Added(Component component)
        {
            Entity = component.Entity;
            Component = component;
        }

        internal virtual void Removed()
        {
            Entity = null;
            Component = null;
        }

        public bool Collide(Entity entity)
        {
            return Collide(entity.Collider);
        }

        public bool Collide(CollidableComponent component)
        {
            return Collide(component.Collider);
        }

        public bool Collide(Collider collider)
        {
            if (collider is Hitbox)
            {
                return Collide(collider as Hitbox);
            }
            else if (collider is Grid)
            {
                return Collide(collider as Grid);
            }
            else if (collider is ColliderList)
            {
                return Collide(collider as ColliderList);
            }
            else if (collider is Circle)
            {
                return Collide(collider as Circle);
            }
            else
                throw new Exception("Collisions against the collider type are not implemented!");
        }

        public abstract bool Collide(Vector2 point);
        public abstract bool Collide(Rectangle rect);
        public abstract bool Collide(Vector2 from, Vector2 to);
        public abstract bool Collide(Hitbox hitbox);
        public abstract bool Collide(Grid grid);
        public abstract bool Collide(Circle circle);
        public abstract bool Collide(ColliderList list);
        public abstract Collider Clone();
        public abstract void Render(Camera camera, Color color);
        public abstract float Width { get; set; }
        public abstract float Height { get; set; }
        public abstract float Top { get; set; }
        public abstract float Bottom { get; set; }
        public abstract float Left { get; set; }
        public abstract float Right { get; set; }

        public void CenterOrigin()
        {
            Position.X = -Width / 2;
            Position.Y = -Height / 2;
        }

        public float CenterX
        {
            get
            {
                return Left + Width / 2f;
            }

            set
            {
                Left = value - Width / 2f;
            }
        }

        public float CenterY
        {
            get
            {
                return Top + Height / 2f;
            }

            set
            {
                Top = value - Height / 2f;
            }
        }

        public Vector2 TopLeft
        {
            get
            {
                return new Vector2(Left, Top);
            }

            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Vector2 TopCenter
        {
            get
            {
                return new Vector2(CenterX, Top);
            }

            set
            {
                CenterX = value.X;
                Top = value.Y;
            }
        }

        public Vector2 TopRight
        {
            get
            {
                return new Vector2(Right, Top);
            }

            set
            {
                Right = value.X;
                Top = value.Y;
            }
        }

        public Vector2 CenterLeft
        {
            get
            {
                return new Vector2(Left, CenterY);
            }

            set
            {
                Left = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(CenterX, CenterY);
            }

            set
            {
                CenterX = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }

        public Vector2 HalfSize
        {
            get
            {
                return Size * .5f;
            }
        }

        public Vector2 CenterRight
        {
            get
            {
                return new Vector2(Right, CenterY);
            }

            set
            {
                Right = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 BottomLeft
        {
            get
            {
                return new Vector2(Left, Bottom);
            }

            set
            {
                Left = value.X;
                Bottom = value.Y;
            }
        }

        public Vector2 BottomCenter
        {
            get
            {
                return new Vector2(CenterX, Bottom);
            }

            set
            {
                CenterX = value.X;
                Bottom = value.Y;
            }
        }

        public Vector2 BottomRight
        {
            get
            {
                return new Vector2(Right, Bottom);
            }

            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }

        public void Render(Camera camera)
        {
            Render(camera, Color.Red);
        }

        public Vector2 AbsolutePosition
        {
            get
            {
                if (Entity != null)
                    return Entity.Position + Position;
                else
                    return Position;
            }
        }

        public float AbsoluteX
        {
            get
            {
                if (Entity != null)
                    return Entity.Position.X + Position.X;
                else
                    return Position.X;
            }
        }

        public float AbsoluteY
        {
            get
            {
                if (Entity != null)
                    return Entity.Position.Y + Position.Y;
                else
                    return Position.Y;
            }
        }

        public float AbsoluteTop
        {
            get
            {
                if (Entity != null)
                    return Top + Entity.Position.Y;
                else
                    return Top;
            }
        }

        public float AbsoluteBottom
        {
            get
            {
                if (Entity != null)
                    return Bottom + Entity.Position.Y;
                else
                    return Bottom;
            }
        }

        public float AbsoluteLeft
        {
            get
            {
                if (Entity != null)
                    return Left + Entity.Position.X;
                else
                    return Left;
            }
        }

        public float AbsoluteRight
        {
            get
            {
                if (Entity != null)
                    return Right + Entity.Position.X;
                else
                    return Right;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)AbsoluteLeft, (int)AbsoluteTop, (int)Width, (int)Height);
            }
        }
    }
}
