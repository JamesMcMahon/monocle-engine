using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monocle
{
    public class EntityList : IEnumerable<Entity>, IEnumerable
    {
        public enum LockModes { Open, Locked, Error };

        private List<Entity> entities;
        private List<Entity> toAdd;
        private List<Entity> toRemove;
        private LockModes lockMode;
        private bool unsorted;
        private Scene entityEventScene;

        internal EntityList()
        {
            entities = new List<Entity>();
            toAdd = new List<Entity>();
            toRemove = new List<Entity>();
        }

        internal EntityList(Scene entityEventScene)
            : this()
        {
            this.entityEventScene = entityEventScene;
        }

        internal void MarkUnsorted()
        {
            unsorted = true;
        }

        internal LockModes LockMode
        {
            get
            {
                return lockMode;
            }

            set
            {
                lockMode = value;

                if (toAdd.Count > 0)
                {
                    foreach (var entity in toAdd)
                    {
                        if (!entities.Contains(entity))
                        {
                            entities.Add(entity);
                            if (entityEventScene != null)
                                entity.Added(entityEventScene);
                        }
                    }

                    toAdd.Clear();
                    unsorted = true;
                }

                if (toRemove.Count > 0)
                {
                    foreach (var entity in toRemove)
                    {
                        if (entities.Contains(entity))
                        {
                            entities.Remove(entity);
                            if (entityEventScene != null)
                                entity.Removed(entityEventScene);
                        }
                    }

                    toRemove.Clear();
                }

                if (unsorted)
                {
                    unsorted = false;
                    entities.Sort(compareDepth);
                }
            }
        }

        public void Add(Entity entity)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    unsorted = true;
                    if (!entities.Contains(entity))
                    {
                        entities.Add(entity);
                        if (entityEventScene != null)
                            entity.Added(entityEventScene);
                    }
                    break;

                case LockModes.Locked:
                    if (!toAdd.Contains(entity) && !entities.Contains(entity))
                        toAdd.Add(entity);
                    break;

                case LockModes.Error:
                    throw new Exception("Cannot add or remove Entities at this time!");
            }
        }

        public void Remove(Entity entity)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (entities.Contains(entity))
                    {
                        entities.Remove(entity);
                        if (entityEventScene != null)
                            entity.Removed(entityEventScene);
                    }
                    break;

                case LockModes.Locked:
                    if (!toRemove.Contains(entity) && entities.Contains(entity))
                        toRemove.Add(entity);
                    break;

                case LockModes.Error:
                    throw new Exception("Cannot add or remove Entities at this time!");
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
            foreach (var entity in entities)
                Add(entity);
        }

        public void Remove(params Entity[] entities)
        {
            foreach (var entity in entities)
                Remove(entity);
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

        #region Static

        static EntityList()
        {
            compareDepth = CompareDepth;
        }

        static private Comparison<Entity> compareDepth;

        static private int CompareDepth(Entity a, Entity b)
        {
            return Math.Sign(b.actualDepth - a.actualDepth);
        }

        #endregion
    }
}
