using System;
using System.Collections;
using System.Collections.Generic;

namespace Monocle
{
    public class CompositeComponent : Component, IEnumerable<Component>, IEnumerable
    {
        public ComponentList Components { get; private set; }

        public CompositeComponent(bool active, bool visible)
            : base(active, visible)
        {
            Components = new ComponentList(Entity);
        }

        public override void Added(ComponentList container)
        {
            base.Added(container);
            Components.Entity = Entity;
        }

        public override void Removed(ComponentList container)
        {
            base.Removed(container);
            Components.Entity = null;
        }

        override public void Update()
        {
            Components.Update();
        }

        override public void Render()
        {
            Components.Render();
        }

        public override void DebugRender()
        {
            Components.DebugRender();
        }

        public override void HandleGraphicsReset()
        {
            Components.HandleGraphicsReset();
        }

        #region Components Shortcuts

        /// <summary>
        /// Shortcut function for adding a Component to the CompositeComponent's Components list
        /// </summary>
        /// <param name="component">The Component to add</param>
        public void Add(Component component)
        {
            Components.Add(component);
        }

        /// <summary>
        /// Shortcut function for removing an Component from the CompositeComponent's Components list
        /// </summary>
        /// <param name="component">The Component to remove</param>
        public void Remove(Component component)
        {
            Components.Remove(component);
        }

        /// <summary>
        /// Shortcut function for adding a set of Components from the CompositeComponent's Components list
        /// </summary>
        /// <param name="components">The Components to add</param>
        public void Add(IEnumerable<Component> components)
        {
            Components.Add(components);
        }

        /// <summary>
        /// Shortcut function for removing a set of Components from the CompositeComponent's Components list
        /// </summary>
        /// <param name="components">The Components to remove</param>
        public void Remove(IEnumerable<Component> components)
        {
            Components.Remove(components);
        }

        /// <summary>
        /// Shortcut function for adding a set of Components from the CompositeComponent's Components list
        /// </summary>
        /// <param name="components">The Components to add</param>
        public void Add(params Component[] components)
        {
            Components.Add(components);
        }

        /// <summary>
        /// Shortcut function for removing a set of Components from the CompositeComponent's Components list
        /// </summary>
        /// <param name="components">The Components to remove</param>
        public void Remove(params Component[] components)
        {
            Components.Remove(components);
        }

        /// <summary>
        /// Allows you to iterate through all Components in the CompositeComponent
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Component> GetEnumerator()
        {
            return Components.GetEnumerator();
        }

        /// <summary>
        /// Allows you to iterate through all Components in the CompositeComponent
        /// </summary>
        /// <returns></returns>
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
