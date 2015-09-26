using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Monocle
{
    public class Scene : IEnumerable<Entity>, IEnumerable
    {
        public float TimeActive { get; private set; }
        public bool Focused { get; private set; }
        public EntityList Entities { get; private set; }
        public TagLists TagLists { get; private set; }
        public List<Renderer> Renderers { get; private set; }
        public Entity HelperEntity { get; private set; }
        public Tracker Tracker { get; private set; }

        private Dictionary<int, double> actualDepthLookup;

        public Scene()
        {
            Tracker = new Tracker();
            Entities = new EntityList(this);
            TagLists = new TagLists();
            Renderers = new List<Renderer>();

            actualDepthLookup = new Dictionary<int, double>();

            HelperEntity = new Entity();
            Entities.Add(HelperEntity);
        }

        public virtual void Begin()
        {
            Focused = true;
            foreach (var entity in Entities)
                entity.SceneBegin();
        }

        public virtual void End()
        {
            Focused = false;
            foreach (var entity in Entities)
                entity.SceneEnd();
        }

        public virtual void BeforeUpdate()
        {
            TimeActive += Engine.DeltaTime;

            Entities.UpdateLists();
            TagLists.UpdateLists();
        }

        public virtual void Update()
        {
            Entities.Update();
        }

        public virtual void AfterUpdate()
        {

        }

        public virtual void BeforeRender()
        {
            for (int i = 0; i < Renderers.Count; i++)
            {
                Draw.Renderer = Renderers[i];
                Renderers[i].BeforeRender(this);
            }
        }

        public virtual void Render()
        {
            for (int i = 0; i < Renderers.Count; i++)
            {
                Draw.Renderer = Renderers[i];
                Renderers[i].Render(this);
            }
        }

        public virtual void AfterRender()
        {
            for (int i = 0; i < Renderers.Count; i++)
            {
                Draw.Renderer = Renderers[i];
                Renderers[i].AfterRender(this);
            }

            Draw.Renderer = null;
        }

        public virtual void HandleGraphicsReset()
        {
            Entities.HandleGraphicsReset();
        }

        public virtual void HandleGraphicsCreate()
        {
            Entities.HandleGraphicsCreate();
        }

        /// <summary>
        /// Returns whether the Scene timer has passed the given time interval since the last frame. Ex: given 2.0f, this will return true once every 2 seconds
        /// </summary>
        /// <param name="interval">The time interval to check for</param>
        /// <returns></returns>
        public bool OnInterval(float interval)
        {
            return (int)((TimeActive - Engine.DeltaTime) / interval) < (int)(TimeActive / interval);
        }

        /// <summary>
        /// Returns whether the Scene timer has passed the given time interval since the last frame. Ex: given 2.0f, this will return true once every 2 seconds
        /// </summary>
        /// <param name="interval">The time interval to check for</param>
        /// <returns></returns>
        public bool OnInterval(float interval, float offset)
        {
            return Math.Floor((TimeActive - offset - Engine.DeltaTime) / interval) < Math.Floor((TimeActive - offset) / interval);
        }

        #region Collisions

        public bool CollideCheck(Vector2 point, int tag)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollidePoint(point))
                    return true;
            return false;
        }

        public bool CollideCheck(Vector2 from, Vector2 to, int tag)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollideLine(from, to))
                    return true;
            return false;
        }

        public bool CollideCheck(Rectangle rect, int tag)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollideRect(rect))
                    return true;
            return false;
        }

        public Entity CollideFirst(Vector2 point, int tag)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollidePoint(point))
                    return list[i];
            return null;
        }

        public Entity CollideFirst(Vector2 from, Vector2 to, int tag)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollideLine(from, to))
                    return list[i];
            return null;
        }

        public Entity CollideFirst(Rectangle rect, int tag)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollideRect(rect))
                    return list[i];
            return null;
        }

        public void CollideInto(Vector2 point, int tag, List<Entity> hits)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollidePoint(point))
                    hits.Add(list[i]);
        }

        public void CollideInto(Vector2 from, Vector2 to, int tag, List<Entity> hits)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollideLine(from, to))
                    hits.Add(list[i]);
        }

        public void CollideInto(Rectangle rect, int tag, List<Entity> hits)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollideRect(rect))
                    list.Add(list[i]);
        }

        public List<Entity> CollideAll(Vector2 point, int tag)
        {
            List<Entity> list = new List<Entity>();
            CollideInto(point, tag, list);
            return list;
        }

        public List<Entity> CollideAll(Vector2 from, Vector2 to, int tag)
        {
            List<Entity> list = new List<Entity>();
            CollideInto(from, to, tag, list);
            return list;
        }

        public List<Entity> CollideAll(Rectangle rect, int tag)
        {
            List<Entity> list = new List<Entity>();
            CollideInto(rect, tag, list);
            return list;
        }

        public void CollideDo(Vector2 point, int tag, Action<Entity> action)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollidePoint(point))
                    action(list[i]);
        }

        public void CollideDo(Vector2 from, Vector2 to, int tag, Action<Entity> action)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollideLine(from, to))
                    action(list[i]);
        }

        public void CollideDo(Rectangle rect, int tag, Action<Entity> action)
        {
            var list = TagLists[(int)tag];

            for (int i = 0; i < list.Count; i++)
                if (list[i].Collidable && list[i].CollideRect(rect))
                    action(list[i]);
        }

        public Vector2 LineWalkCheck(Vector2 from, Vector2 to, int tag, float precision)
        {
            Vector2 add = to - from;
            add.Normalize();
            add *= precision;

            int amount = (int)Math.Floor((from - to).Length() / precision);
            Vector2 prev = from;
            Vector2 at = from + add;

            for (int i = 0; i <= amount; i++)
            {
                if (CollideCheck(at, tag))
                    return prev;
                prev = at;
                at += add;
            }

            return to;
        }

        #endregion

        #region Utils

        internal void SetActualDepth(Entity entity)
        {
            const double theta = .000001f;

            double add = 0;
            if (actualDepthLookup.TryGetValue(entity.depth, out add))
                actualDepthLookup[entity.depth] += theta;
            else
                actualDepthLookup.Add(entity.depth, theta);
            entity.actualDepth = entity.depth - add;

            //Mark lists unsorted
            Entities.MarkUnsorted();
            foreach (var tag in entity.Tags)
                TagLists.MarkUnsorted(tag);
        }

        #endregion

        #region Entity Shortcuts

        /// <summary>
        /// Shortcut to call Engine.Pooler.Create, add the Entity to this Scene, and return it. Entity type must be marked as Pooled
        /// </summary>
        /// <typeparam name="T">Pooled Entity type to create</typeparam>
        /// <returns></returns>
        public T CreateAndAdd<T>() where T : Entity, new()
        {
            var entity = Engine.Pooler.Create<T>();
            Add(entity);
            return entity;
        }

        /// <summary>
        /// Quick access to entire tag lists of Entities. Result will never be null
        /// </summary>
        /// <param name="tag">The tag list to fetch</param>
        /// <returns></returns>
        public List<Entity> this[int tag]
        {
            get
            {
                return TagLists[(int)tag];
            }
        }

        /// <summary>
        /// Shortcut function for adding an Entity to the Scene's Entities list
        /// </summary>
        /// <param name="entity">The Entity to add</param>
        public void Add(Entity entity)
        {
            Entities.Add(entity);
        }

        /// <summary>
        /// Shortcut function for removing an Entity from the Scene's Entities list
        /// </summary>
        /// <param name="entity">The Entity to remove</param>
        public void Remove(Entity entity)
        {
            Entities.Remove(entity);
        }

        /// <summary>
        /// Shortcut function for adding a set of Entities from the Scene's Entities list
        /// </summary>
        /// <param name="entities">The Entities to add</param>
        public void Add(IEnumerable<Entity> entities)
        {
            Entities.Add(entities);
        }

        /// <summary>
        /// Shortcut function for removing a set of Entities from the Scene's Entities list
        /// </summary>
        /// <param name="entities">The Entities to remove</param>
        public void Remove(IEnumerable<Entity> entities)
        {
            Entities.Remove(entities);
        }

        /// <summary>
        /// Shortcut function for adding a set of Entities from the Scene's Entities list
        /// </summary>
        /// <param name="entities">The Entities to add</param>
        public void Add(params Entity[] entities)
        {
            Entities.Add(entities);
        }

        /// <summary>
        /// Shortcut function for removing a set of Entities from the Scene's Entities list
        /// </summary>
        /// <param name="entities">The Entities to remove</param>
        public void Remove(params Entity[] entities)
        {
            Entities.Remove(entities);
        }

        /// <summary>
        /// Allows you to iterate through all Entities in the Scene
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Entity> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        /// <summary>
        /// Allows you to iterate through all Entities in the Scene
        /// </summary>
        /// <returns></returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Renderer Shortcuts

        /// <summary>
        /// Shortcut function to add a Renderer to the Renderer list
        /// </summary>
        /// <param name="renderer">The Renderer to add</param>
        public void Add(Renderer renderer)
        {
            Renderers.Add(renderer);
        }

        /// <summary>
        /// Shortcut function to remove a Renderer from the Renderer list
        /// </summary>
        /// <param name="renderer">The Renderer to remove</param>
        public void Remove(Renderer renderer)
        {
            Renderers.Add(renderer);
        }

        #endregion
    }
}
