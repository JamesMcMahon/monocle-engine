using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monocle
{
    public class EntityList : IEnumerable<Entity>, IEnumerable
    {
        public Scene Scene { get; private set; }

        private List<Entity> entities;
        private List<Entity> toAdd;
        private List<Entity> toAwake;
        private List<Entity> toRemove;

        private HashSet<Entity> current;
        private HashSet<Entity> adding;
        private HashSet<Entity> removing;

        private bool unsorted;

        internal EntityList(Scene scene)
        {
            Scene = scene;

            entities = new List<Entity>();
            toAdd = new List<Entity>();
            toAwake = new List<Entity>();
            toRemove = new List<Entity>();

            current = new HashSet<Entity>();
            adding = new HashSet<Entity>();
            removing = new HashSet<Entity>();
        }

        internal void MarkUnsorted()
        {
            unsorted = true;
        }

        public void UpdateLists()
        {
            if (toAdd.Count > 0)
            {
                for (int i = 0; i < toAdd.Count; i++)
                {
                    var entity = toAdd[i];
                    if (!current.Contains(entity))
                    {
                        current.Add(entity);
                        entities.Add(entity);

                        if (Scene != null)
                        {
                            Scene.TagLists.EntityAdded(entity);
                            Scene.Tracker.EntityAdded(entity);
                            entity.Added(Scene);
                        }
                    }
                }

                unsorted = true;
            }

            if (toRemove.Count > 0)
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    var entity = toRemove[i];
                    if (entities.Contains(entity))
                    {
                        current.Remove(entity);
                        entities.Remove(entity);

                        if (Scene != null)
                        {
                            entity.Removed(Scene);
                            Scene.TagLists.EntityRemoved(entity);
                            Scene.Tracker.EntityRemoved(entity);
                            Engine.Pooler.EntityRemoved(entity);
                        }
                    }
                }

                toRemove.Clear();
                removing.Clear();
            }

            if (unsorted)
            {
                unsorted = false;
                entities.Sort(CompareDepth);
            }

            if (toAdd.Count > 0)
            {
                toAwake.AddRange(toAdd);
                toAdd.Clear();
                adding.Clear();

                foreach (var entity in toAwake)
                    if (entity.Scene == Scene)
                        entity.Awake(Scene);
                toAwake.Clear();
            }
        }

        public void Add(Entity entity)
        {
            if (!adding.Contains(entity) && !current.Contains(entity))
            {
                adding.Add(entity);
                toAdd.Add(entity);
            }
        }

        public void Remove(Entity entity)
        {
            if (!removing.Contains(entity) && current.Contains(entity))
            {
                removing.Add(entity);
                toRemove.Add(entity);
            }
        }

        public void Add(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                Add(entity);
        }

        public void Remove(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                Remove(entity);
        }

        public void Add(params Entity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
                Add(entities[i]);
        }

        public void Remove(params Entity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
                Remove(entities[i]);
        }

        public int Count
        {
            get
            {
                return entities.Count;
            }
        }

        public Entity this[int index]
        {
            get
            {
                if (index < 0 || index >= entities.Count)
                    throw new IndexOutOfRangeException();
                else
                    return entities[index];
            }
        }

        public int AmountOf<T>() where T : Entity
        {
            int count = 0;
            foreach (var e in entities)
                if (e is T)
                    count++;

            return count;
        }

        public T FindFirst<T>() where T : Entity
        {
            foreach (var e in entities)
                if (e is T)
                    return e as T;

            return null;
        }

        public List<T> FindAll<T>() where T : Entity
        {
            List<T> list = new List<T>();

            foreach (var e in entities)
                if (e is T)
                    list.Add(e as T);

            return list;
        }

        public void With<T>(Action<T> action) where T : Entity
        {
            foreach (var e in entities)
                if (e is T)
                    action(e as T);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Entity[] ToArray()
        {
            return entities.ToArray<Entity>();
        }

        public bool HasVisibleEntities(int matchTags)
        {
            foreach (var entity in entities)
                if (entity.Visible && entity.TagCheck(matchTags))
                    return true;
            return false;
        }

        internal void Update()
        {
            foreach (var entity in entities)
                if (entity.Active)
                    entity.Update();
        }

        public void Render()
        {
            foreach (var entity in entities)
                if (entity.Visible)
                    entity.Render();
        }

        public void RenderOnly(int matchTags)
        {
            foreach (var entity in entities)
                if (entity.Visible && entity.TagCheck(matchTags))
                    entity.Render();
        }

        public void RenderOnlyFullMatch(int matchTags)
        {
            foreach (var entity in entities)
                if (entity.Visible && entity.TagFullCheck(matchTags))
                    entity.Render();
        }

        public void RenderExcept(int excludeTags)
        {
            foreach (var entity in entities)
                if (entity.Visible && !entity.TagCheck(excludeTags))
                    entity.Render();
        }

        public void DebugRender(Camera camera)
        {
            foreach (var entity in entities)
                entity.DebugRender(camera);
        }

        internal void HandleGraphicsReset()
        {
            foreach (var entity in entities)
                entity.HandleGraphicsReset();
        }

        internal void HandleGraphicsCreate()
        {
            foreach (var entity in entities)
                entity.HandleGraphicsCreate();
        }

        public static Comparison<Entity> CompareDepth = (a, b) => { return Math.Sign(b.actualDepth - a.actualDepth); };
    }
}
