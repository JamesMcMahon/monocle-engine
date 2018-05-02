using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    public class CollidableComponent : Component
    {
        public bool Collidable;

        private Collider collider;

        public CollidableComponent(bool active, bool visible, bool colllidable)
            : base(active, visible)
        {
            Collidable = colllidable;
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            if (collider != null)
                collider.Entity = entity;
        }

        public override void Removed(Entity entity)
        {
            if (collider != null)
                collider.Entity = null;
            base.Removed(entity);
        }

        public Collider Collider
        {
            get
            {
                if (collider == null)
                    return Entity.Collider;
                else
                    return collider;
            }

            set
            {
                if (value == collider)
                    return;
#if DEBUG
                if (value.Entity != null)
                    throw new Exception("Setting an Entity's Collider to a Collider already in use by another object");
#endif
                if (collider != null)
                    collider.Removed();
                collider = value;
                if (collider != null)
                    collider.Added(this);
            }
        }

        public float Width
        {
            get
            {
                if (collider == null)
                    return Entity.Width;
                else
                    return collider.Width;
            }
        }

        public float Height
        {
            get
            {
                if (collider == null)
                    return Entity.Height;
                else
                    return collider.Height;
            }
        }

        public float Left
        {
            get
            {
                if (collider == null)
                    return Entity.Left;
                else
                    return Entity.X + collider.Left;
            }

            set
            {
                if (collider == null)
                    Entity.Left = value;
                else
                    Entity.X = value - collider.Left;
            }
        }

        public float Right
        {
            get
            {
                if (collider == null)
                    return Entity.Right;
                else
                    return Entity.X + collider.Right;
            }

            set
            {
                if (collider == null)
                    Entity.Right = value;
                else
                    Entity.X = value - collider.Right;
            }
        }

        public float Top
        {
            get
            {
                if (collider == null)
                    return Entity.Top;
                else
                    return Entity.Y + collider.Top;
            }

            set
            {
                if (collider == null)
                    Entity.Top = value;
                else
                    Entity.Y = value - collider.Top;
            }
        }

        public float Bottom
        {
            get
            {
                if (collider == null)
                    return Entity.Bottom;
                else
                    return Entity.Y + collider.Bottom;
            }

            set
            {
                if (collider == null)
                    Entity.Bottom = value;
                else
                    Entity.Y = value - collider.Bottom;
            }
        }

        public float CenterX
        {
            get
            {
                if (collider == null)
                    return Entity.CenterX;
                else
                    return Entity.X + collider.CenterX;
            }

            set
            {
                if (collider == null)
                    Entity.CenterX = value;
                else
                    Entity.X = value - collider.CenterX;
            }
        }

        public float CenterY
        {
            get
            {
                if (collider == null)
                    return Entity.CenterY;
                else
                    return Entity.Y + collider.CenterY;
            }

            set
            {
                if (collider == null)
                    Entity.CenterY = value;
                else
                    Entity.Y = value - collider.CenterY;
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
    }
}
