using System;
using System.Collections.Generic;

namespace Monocle
{
    public class CompositeComponent : Component, IComponentHolder
    {
        public List<Component> Components { get; private set; }
        public bool Updating { get; private set; }

        private HashSet<Component> toAdd;
        private int toRemove;

        public CompositeComponent(bool active, bool visible)
            : base(active, visible)
        {
            Components = new List<Component>();
            toAdd = new HashSet<Component>();
        }

        public override void Added()
        {
            base.Added();

            foreach (var c in Components)
                c.Entity = Entity;
        }

        public override void Removed()
        {
            base.Removed();

            foreach (var c in Components)
                c.Entity = null;
        }

        override public void Update()
        {
            if (Components != null)
            {
                Updating = true;
                foreach (var c in Components)
                    if (c.Active)
                        c.Update();
                Updating = false;
                UpdateComponentList();
            }
        }

        override public void Render()
        {
            if (Components != null)
                foreach (var c in Components)
                    if (c.Visible)
                        c.Render();
        }

        #region Components

        public Component Add(Component component)
        {
#if DEBUG
            if (component == null)
                throw new Exception("Adding a null Component");
            if (component.Entity != null)
                throw new Exception("Component added that is already in an Entity");
#endif
            if (!Updating)
            {
                Components.Add(component);
                component.Entity = Entity;
                component.Parent = this;
                component.Added();
            }
            else
                toAdd.Add(component);
            return component;
        }

        public void Add(params Component[] components)
        {
            foreach (var component in components)
                Add(component);
        }

        public Component Remove(Component component)
        {
#if DEBUG
            if (Components == null || component.Entity != this)
                throw new Exception("Removing Component that is not in the Entity");
#endif
            if (!Updating)
            {
                Components.Remove(component);
                component.Removed();
                component.Entity = null;
                component.Parent = null;
                component.MarkedForRemoval = false;
            }
            else if (!component.MarkedForRemoval)
            {
                toRemove++;
                component.MarkedForRemoval = true;
            }
            return component;
        }

        public void Remove(params Component[] components)
        {
            foreach (var component in components)
                Remove(component);
        }

        public Component Remove(int index)
        {
#if DEBUG
            if (index < 0 || index >= Components.Count)
                throw new Exception("Component index out of bounds");
#endif
            return Remove(Components[index]);
        }

        public void Remove<T>() where T : Component
        {
            for (int i = 0; i < Components.Count; i++)
                if (Components[i] is T)
                    Remove(i);
        }

        public void RemoveAll()
        {
            if (Components != null)
            {
                if (!Updating)
                {
                    foreach (var c in Components)
                    {
                        c.Removed();
                        c.Entity = null;
                        c.Parent = null;
                        c.MarkedForRemoval = false;
                    }
                    Components.Clear();
                }
                else
                {
                    foreach (var c in Components)
                        Remove(c);
                }
            }
        }

        public int ComponentCount
        {
            get
            {
                if (Components == null)
                    return 0;
                else
                    return Components.Count;
            }
        }

        public bool Contains(Component component)
        {
            return Components != null && Components.Contains(component);
        }

        public bool Contains<T>()
        {
            if (Components != null)
                for (int i = 0; i < Components.Count; i++)
                    if (Components[i] is T)
                        return true;
            return false;
        }

        public T GetFirst<T>() where T : Component
        {
            if (Components != null)
                for (int i = 0; i < Components.Count; i++)
                    if (Components[i] is T)
                        return (T)Components[i];

            return null;
        }

        public List<T> GetList<T>(List<T> list) where T : Component
        {
            if (Components != null)
                for (int i = 0; i < Components.Count; i++)
                    if (Components[i] is T)
                        list.Add((T)Components[i]);
            return list;
        }

        public List<T> GetList<T>() where T : Component
        {
            return GetList<T>(new List<T>());
        }

        public void UpdateComponentList()
        {
            if (toRemove > 0)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i].MarkedForRemoval)
                    {
                        Component c = Components[i];
                        Components.RemoveAt(i);

                        c.Removed();
                        c.MarkedForRemoval = false;
                        c.Entity = null;
                        c.Parent = null;

                        toRemove--;
                        if (toRemove <= 0)
                            break;
                        i--;
                    }
                }
            }

            if (toAdd.Count > 0)
            {
                foreach (var c in toAdd)
                {
                    Components.Add(c);
                    c.Entity = Entity;
                    c.Parent = this;
                    c.Added();
                }

                toAdd.Clear();
            }
        }

        #endregion
    }
}
