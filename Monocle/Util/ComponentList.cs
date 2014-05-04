using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monocle
{
    public class ComponentList : IEnumerable<Component>, IEnumerable
    {
        public enum LockModes { Open, Locked, Error };
        public Entity Entity { get; internal set; }

        private List<Component> components;
        private List<Component> toAdd;
        private List<Component> toRemove;
        private LockModes lockMode;

        internal ComponentList(Entity entity)
        {
            Entity = entity;

            components = new List<Component>();
            toAdd = new List<Component>();
            toRemove = new List<Component>();
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
                    foreach (var component in toAdd)
                    {
                        if (!components.Contains(component))
                        {
                            components.Add(component);
                            component.Added(this);
                        }
                    }

                    toAdd.Clear();
                }

                if (toRemove.Count > 0)
                {
                    foreach (var component in toRemove)
                    {
                        if (components.Contains(component))
                        {
                            components.Remove(component);
                            component.Removed(this);
                        }
                    }

                    toRemove.Clear();
                }
            }
        }

        public void Add(Component component)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (!components.Contains(component))
                    {
                        components.Add(component);
                        component.Added(this);
                    }
                    break;

                case LockModes.Locked:
                    if (!toAdd.Contains(component) && !components.Contains(component))
                        toAdd.Add(component);
                    break;

                case LockModes.Error:
                    throw new Exception("Cannot add or remove Entities at this time!");
            }
        }

        public void Remove(Component component)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (components.Contains(component))
                    {
                        components.Remove(component);
                        component.Removed(this);
                    }
                    break;

                case LockModes.Locked:
                    if (!toRemove.Contains(component) && components.Contains(component))
                        toRemove.Add(component);
                    break;

                case LockModes.Error:
                    throw new Exception("Cannot add or remove Entities at this time!");
            }
        }

        public void Add(IEnumerable<Component> components)
        {
            foreach (var component in components)
                Add(component);
        }

        public void Remove(IEnumerable<Component> components)
        {
            foreach (var component in components)
                Remove(component);
        }

        public void Add(params Component[] components)
        {
            foreach (var component in components)
                Add(component);
        }

        public void Remove(params Component[] components)
        {
            foreach (var component in components)
                Remove(component);
        }

        public int Count
        {
            get
            {
                return components.Count;
            }
        }

        public Component this[int index]
        {
            get
            {
                if (index < 0 || index >= components.Count)
                    throw new IndexOutOfRangeException();
                else
                    return components[index];
            }
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return components.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Component[] ToArray()
        {
            return components.ToArray<Component>();
        }

        internal void Update()
        {
            LockMode = ComponentList.LockModes.Locked;
            for (int i = 0; i < components.Count; i++)
                if (components[i].Active)
                    components[i].Update();
            LockMode = ComponentList.LockModes.Open;
        }

        internal void Render()
        {
            LockMode = ComponentList.LockModes.Error;
            for (int i = 0; i < components.Count; i++)
                if (components[i].Visible)
                    components[i].Render();
            LockMode = ComponentList.LockModes.Open;
        }

        internal void HandleGraphicsReset()
        {
            LockMode = ComponentList.LockModes.Error;
            for (int i = 0; i < components.Count; i++)
                components[i].HandleGraphicsReset();
            LockMode = ComponentList.LockModes.Open;
        }
    }
}
