using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Monocle
{
    public class Entity : IEnumerable<Component>, IEnumerable
    {
        public bool Active = true;
        public bool Visible = true;
        public bool Collidable = true;
        public Vector2 Position;

        public Scene Scene { get; private set; }
        public ComponentList Components { get; private set; }

        private int tag;
        private Collider collider;
        internal int depth = 0;
        internal double actualDepth = 0;

        public Entity(Vector2 position)
        {
            Position = position;
            Components = new ComponentList(this);
        }

        public Entity()
            : this(Vector2.Zero)
        {

        }

        /// <summary>
        /// Called when the containing Scene Begins
        /// </summary>
        public virtual void SceneBegin(Scene scene)
        {

        }

        /// <summary>
        /// Called when the containing Scene Ends
        /// </summary>
        public virtual void SceneEnd(Scene scene)
        {
            if (Components != null)
                foreach (var c in Components)
                    c.SceneEnd(scene);
        }

        /// <summary>
        /// Called before the frame starts, after Entities are added and removed, on the frame that the Entity was added
        /// Useful if you added two Entities in the same frame, and need them to detect each other before they start Updating
        /// </summary>
        /// <param name="scene"></param>
        public virtual void Awake(Scene scene)
        {
            if (Components != null)
                foreach (var c in Components)
                    c.EntityAwake();
        }

        /// <summary>
        /// Called when this Entity is added to a Scene, which only occurs immediately before each Update. 
        /// Keep in mind, other Entities to be added this frame may be added after this Entity. 
        /// See Awake() for after all Entities are added, but still before the frame Updates.
        /// </summary>
        /// <param name="scene"></param>
        public virtual void Added(Scene scene)
        {
            Scene = scene;
            if (Components != null)
                foreach (var c in Components)
                    c.EntityAdded(scene);
            Scene.SetActualDepth(this);
        }

        /// <summary>
        /// Called when the Entity is removed from a Scene
        /// </summary>
        /// <param name="scene"></param>
        public virtual void Removed(Scene scene)
        {
            if (Components != null)
                foreach (var c in Components)
                    c.EntityRemoved(scene);
            Scene = null;
        }

        /// <summary>
        /// Do game logic here, but do not render here. Not called if the Entity is not Active
        /// </summary>
        public virtual void Update()
        {
            Components.Update();
        }

        /// <summary>
        /// Draw the Entity here. Not called if the Entity is not Visible
        /// </summary>
        public virtual void Render()
        {
            Components.Render();
        }

        /// <summary>
        /// Draw any debug visuals here. Only called if the console is open, but still called even if the Entity is not Visible
        /// </summary>
        public virtual void DebugRender(Camera camera)
        {
            if (Collider != null)
                Collider.Render(camera, Collidable ? Color.Red : Color.DarkRed);

            Components.DebugRender(camera);
        }

        /// <summary>
        /// Called when the graphics device resets. When this happens, any RenderTargets or other contents of VRAM will be wiped and need to be regenerated
        /// </summary>
        public virtual void HandleGraphicsReset()
        {
            Components.HandleGraphicsReset();
        }

        public virtual void HandleGraphicsCreate()
        {
            Components.HandleGraphicsCreate();
        }

        public void RemoveSelf()
        {
            if (Scene != null)
                Scene.Entities.Remove(this);
        }

        public int Depth
        {
            get { return depth; }
            set
            {
                if (depth != value)
                {
                    depth = value;
                    if (Scene != null)
                        Scene.SetActualDepth(this);
                }
            }
        }

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        #region Collider

        public Collider Collider
        {
            get { return collider; }
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
                    return 0;
                else
                    return collider.Width;
            }
        }

        public float Height
        {
            get
            {
                if (collider == null)
                    return 0;
                else
                    return collider.Height;
            }
        }

        public float Left
        {
            get
            {
                if (collider == null)
                    return X;
                else
                    return Position.X + collider.Left;
            }

            set
            {
                if (collider == null)
                    Position.X = value;
                else
                    Position.X = value - collider.Left;
            }
        }

        public float Right
        {
            get
            {
                if (collider == null)
                    return Position.X;
                else
                    return Position.X + collider.Right;
            }

            set
            {
                if (collider == null)
                    Position.X = value;
                else
                    Position.X = value - collider.Right;
            }
        }

        public float Top
        {
            get
            {
                if (collider == null)
                    return Position.Y;
                else
                    return Position.Y + collider.Top;
            }

            set
            {
                if (collider == null)
                    Position.Y = value;
                else
                    Position.Y = value - collider.Top;
            }
        }

        public float Bottom
        {
            get
            {
                if (collider == null)
                    return Position.Y;
                else
                    return Position.Y + collider.Bottom;
            }

            set
            {
                if (collider == null)
                    Position.Y = value;
                else
                    Position.Y = value - collider.Bottom;
            }
        }

        public float CenterX
        {
            get
            {
                if (collider == null)
                    return Position.X;
                else
                    return Position.X + collider.CenterX;
            }

            set
            {
                if (collider == null)
                    Position.X = value;
                else
                    Position.X = value - collider.CenterX;
            }
        }

        public float CenterY
        {
            get
            {
                if (collider == null)
                    return Position.Y;
                else
                    return Position.Y + collider.CenterY;
            }

            set
            {
                if (collider == null)
                    Position.Y = value;
                else
                    Position.Y = value - collider.CenterY;
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

        #endregion

        #region Tag

        public int Tag
        {
            get
            {
                return tag;
            }

            set
            {
                if (tag != value)
                {
                    if (Scene != null)
                    {
                        for (int i = 0; i < Monocle.BitTag.TotalTags; i++)
                        {
                            int check = 1 << i;
                            bool add = (value & check) != 0;
                            bool has = (Tag & check) != 0;

                            if (has != add)
                            {
                                if (add)
                                    Scene.TagLists[i].Add(this);
                                else
                                    Scene.TagLists[i].Remove(this);
                            }
                        }
                    }

                    tag = value;
                }
            }
        }

        public bool TagFullCheck(int tag)
        {
            return (this.tag & tag) == tag;
        }

        public bool TagCheck(int tag)
        {
            return (this.tag & tag) != 0;
        }

        public void AddTag(int tag)
        {
            Tag |= tag;
        }

        public void RemoveTag(int tag)
        {
            Tag &= ~tag;
        }

        #endregion

        #region Collision Shortcuts

        #region Collide Check

        public bool CollideCheck(Entity other)
        {
            return Collide.Check(this, other);
        }

        public bool CollideCheck(Entity other, Vector2 at)
        {
            return Collide.Check(this, other, at);
        }

        public bool CollideCheck(CollidableComponent other)
        {
            return Collide.Check(this, other);
        }

        public bool CollideCheck(CollidableComponent other, Vector2 at)
        {
            return Collide.Check(this, other, at);
        }

        public bool CollideCheck(BitTag tag)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.Check(this, Scene[tag]);
        }

        public bool CollideCheck(BitTag tag, Vector2 at)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.Check(this, Scene[tag], at);
        }

        public bool CollideCheck<T>() where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            return Collide.Check(this, Scene.Tracker.Entities[typeof(T)]);
        }

        public bool CollideCheck<T>(Vector2 at) where T : Entity
        {
            return Collide.Check(this, Scene.Tracker.Entities[typeof(T)], at);
        }

        public bool CollideCheck<T, Exclude>() where T : Entity where Exclude : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked objects when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(Exclude)))
                throw new Exception("Excluded type is an untracked Entity type!");
#endif

            var exclude = Scene.Tracker.Entities[typeof(Exclude)];
            foreach (var e in Scene.Tracker.Entities[typeof(T)])
                if (!exclude.Contains(e))
                    if (Collide.Check(this, e))
                        return true;
            return false;
        }

        public bool CollideCheck<T, Exclude>(Vector2 at) where T : Entity where Exclude : Entity
        {
            var was = Position;
            Position = at;
            var ret = CollideCheck<T, Exclude>();
            Position = was;
            return ret;
        }

        public bool CollideCheckByComponent<T>() where T : CollidableComponent
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            else if (!Scene.Tracker.Components.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif
            
            foreach (var c in Scene.Tracker.CollidableComponents[typeof(T)])
                if (Collide.Check(this, c))
                    return true;
            return false;
        }

        public bool CollideCheckByComponent<T>(Vector2 at) where T : CollidableComponent
        {
            Vector2 old = Position;
            Position = at;
            bool ret = CollideCheckByComponent<T>();
            Position = old;
            return ret;
        }

        #endregion

        #region Collide CheckOutside

        public bool CollideCheckOutside(Entity other, Vector2 at)
        {
            return !Collide.Check(this, other) && Collide.Check(this, other, at);
        }

        public bool CollideCheckOutside(BitTag tag, Vector2 at)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            foreach (var entity in Scene[tag])
                if (!Collide.Check(this, entity) && Collide.Check(this, entity, at))
                    return true;

            return false;
        }

        public bool CollideCheckOutside<T>(Vector2 at) where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            foreach (var entity in Scene.Tracker.Entities[typeof(T)])
                if (!Collide.Check(this, entity) && Collide.Check(this, entity, at))
                    return true;
            return false;
        }

        public bool CollideCheckOutsideByComponent<T>(Vector2 at) where T : CollidableComponent
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            else if (!Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach (var component in Scene.Tracker.CollidableComponents[typeof(T)])
                if (!Collide.Check(this, component) && Collide.Check(this, component, at))
                    return true;
            return false;
        }

        #endregion

        #region Collide First

        public Entity CollideFirst(BitTag tag)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.First(this, Scene[tag]);
        }

        public Entity CollideFirst(BitTag tag, Vector2 at)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.First(this, Scene[tag], at);
        }

        public T CollideFirst<T>() where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif
            return Collide.First(this, Scene.Tracker.Entities[typeof(T)]) as T;
        }

        public T CollideFirst<T>(Vector2 at) where T : Entity
        {
#if DEBUG
            if (Scene == null)
                 throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif
            return Collide.First(this, Scene.Tracker.Entities[typeof(T)], at) as T;
        }

        public T CollideFirstByComponent<T>() where T : CollidableComponent
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            else if (!Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach (var component in Scene.Tracker.CollidableComponents[typeof(T)])
                if (Collide.Check(this, component))
                    return component as T;
            return null;
        }

        public T CollideFirstByComponent<T>(Vector2 at) where T : CollidableComponent
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            else if (!Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach (var component in Scene.Tracker.CollidableComponents[typeof(T)])
                if (Collide.Check(this, component, at))
                    return component as T;
            return null;
        }

        #endregion

        #region Collide FirstOutside

        public Entity CollideFirstOutside(BitTag tag, Vector2 at)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            foreach (var entity in Scene[tag])
                if (!Collide.Check(this, entity) && Collide.Check(this, entity, at))
                    return entity;
            return null;
        }

        public T CollideFirstOutside<T>(Vector2 at) where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            foreach (var entity in Scene.Tracker.Entities[typeof(T)])
                if (!Collide.Check(this, entity) && Collide.Check(this, entity, at))
                    return entity as T;
            return null;
        }

        public T CollideFirstOutsideByComponent<T>(Vector2 at) where T : CollidableComponent
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            else if (!Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach (var component in Scene.Tracker.CollidableComponents[typeof(T)])
                if (!Collide.Check(this, component) && Collide.Check(this, component, at))
                    return component as T;
            return null;
        }

        #endregion

        #region Collide All

        public List<Entity> CollideAll(BitTag tag)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.All(this, Scene[tag]);
        }

        public List<Entity> CollideAll(BitTag tag, Vector2 at)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.All(this, Scene[tag], at);
        }

        public List<Entity> CollideAll<T>() where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            return Collide.All(this, Scene.Tracker.Entities[typeof(T)]);
        }

        public List<Entity> CollideAll<T>(Vector2 at) where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            return Collide.All(this, Scene.Tracker.Entities[typeof(T)], at);
        }

        public List<Entity> CollideAll<T>(Vector2 at, List<Entity> into) where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            into.Clear();
            return Collide.All(this, Scene.Tracker.Entities[typeof(T)], into, at);
        }

        public List<T> CollideAllByComponent<T>() where T : CollidableComponent
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            else if (!Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            List<T> list = new List<T>();
            foreach (var component in Scene.Tracker.CollidableComponents[typeof(T)])
                if (Collide.Check(this, component))
                    list.Add(component as T);
            return list;
        }

        public List<T> CollideAllByComponent<T>(Vector2 at) where T : CollidableComponent
        {
            Vector2 old = Position;
            Position = at;
            var ret = CollideAllByComponent<T>();
            Position = old;
            return ret;
        }

        #endregion

        #region Collide Do

        public bool CollideDo(BitTag tag, Action<Entity> action)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            bool hit = false;
            foreach (var other in Scene[tag])
            {
                if (CollideCheck(other))
                {
                    action(other);
                    hit = true;
                }
            }
            return hit;
        }

        public bool CollideDo(BitTag tag, Action<Entity> action, Vector2 at)
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            bool hit = false;
            var was = Position;
            Position = at;

            foreach (var other in Scene[tag])
            {
                if (CollideCheck(other))
                {
                    action(other);
                    hit = true;
                }
            }

            Position = was;
            return hit;
        }

        public bool CollideDo<T>(Action<T> action) where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            bool hit = false;
            foreach (var other in Scene.Tracker.Entities[typeof(T)])
            {
                if (CollideCheck(other))
                {
                    action(other as T);
                    hit = true;
                }
            }
            return hit;
        }

        public bool CollideDo<T>(Action<T> action, Vector2 at) where T : Entity
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            else if (!Scene.Tracker.Entities.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            bool hit = false;
            var was = Position;
            Position = at;

            foreach (var other in Scene.Tracker.Entities[typeof(T)])
            {
                if (CollideCheck(other))
                {
                    action(other as T);
                    hit = true;
                }
            }

            Position = was;
            return hit;
        }

        public bool CollideDoByComponent<T>(Action<T> action) where T : CollidableComponent
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            else if (!Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            bool hit = false;
            foreach (var component in Scene.Tracker.CollidableComponents[typeof(T)])
            {
                if (CollideCheck(component))
                {
                    action(component as T);
                    hit = true;
                }
            }
            return hit;
        }

        public bool CollideDoByComponent<T>(Action<T> action, Vector2 at) where T : CollidableComponent
        {
#if DEBUG
            if (Scene == null)
                throw new Exception("Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            else if (!Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)))
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            bool hit = false;
            var was = Position;
            Position = at;

            foreach (var component in Scene.Tracker.CollidableComponents[typeof(T)])
            {
                if (CollideCheck(component))
                {
                    action(component as T);
                    hit = true;
                }
            }

            Position = was;
            return hit;
        }

        #endregion

        #region Collide Geometry

        public bool CollidePoint(Vector2 point)
        {
            return Collide.CheckPoint(this, point);
        }

        public bool CollidePoint(Vector2 point, Vector2 at)
        {
            return Collide.CheckPoint(this, point, at);
        }

        public bool CollideLine(Vector2 from, Vector2 to)
        {
            return Collide.CheckLine(this, from, to);
        }

        public bool CollideLine(Vector2 from, Vector2 to, Vector2 at)
        {
            return Collide.CheckLine(this, from, to, at);
        }

        public bool CollideRect(Rectangle rect)
        {
            return Collide.CheckRect(this, rect);
        }

        public bool CollideRect(Rectangle rect, Vector2 at)
        {
            return Collide.CheckRect(this, rect, at);
        }

        #endregion

        #endregion

        #region Components Shortcuts

        /// <summary>
        /// Shortcut function for adding a Component to the Entity's Components list
        /// </summary>
        /// <param name="component">The Component to add</param>
        public void Add(Component component)
        {
            Components.Add(component);
        }

        /// <summary>
        /// Shortcut function for removing an Component from the Entity's Components list
        /// </summary>
        /// <param name="component">The Component to remove</param>
        public void Remove(Component component)
        {
            Components.Remove(component);
        }

        /// <summary>
        /// Shortcut function for adding a set of Components from the Entity's Components list
        /// </summary>
        /// <param name="components">The Components to add</param>
        public void Add(params Component[] components)
        {
            Components.Add(components);
        }

        /// <summary>
        /// Shortcut function for removing a set of Components from the Entity's Components list
        /// </summary>
        /// <param name="components">The Components to remove</param>
        public void Remove(params Component[] components)
        {
            Components.Remove(components);
        }

        public T Get<T>() where T : Component
        {
            return Components.Get<T>();
        }

        /// <summary>
        /// Allows you to iterate through all Components in the Entity
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Component> GetEnumerator()
        {
            return Components.GetEnumerator();
        }

        /// <summary>
        /// Allows you to iterate through all Components in the Entity
        /// </summary>
        /// <returns></returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Misc Utils

        public Entity Closest(params Entity[] entities)
        {
            Entity closest = entities[0];
            float dist = Vector2.DistanceSquared(Position, closest.Position);

            for (int i = 1; i < entities.Length; i++)
            {
                float current = Vector2.DistanceSquared(Position, entities[i].Position);
                if (current < dist)
                {
                    closest = entities[i];
                    dist = current;
                }
            }

            return closest;
        }

        public Entity Closest(BitTag tag)
        {
            var list = Scene[tag];
            Entity closest = null;
            float dist;

            if (list.Count >= 1)
            {
                closest = list[0];
                dist = Vector2.DistanceSquared(Position, closest.Position);

                for (int i = 1; i < list.Count; i++)
                {
                    float current = Vector2.DistanceSquared(Position, list[i].Position);
                    if (current < dist)
                    {
                        closest = list[i];
                        dist = current;
                    }
                }
            }

            return closest;
        }

        public T SceneAs<T>() where T : Scene
        {
            return Scene as T;
        }

        #endregion
    }
}
