using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class Tracker
    {
        public Dictionary<Type, List<Entity>> Entities { get; private set; }
        public Dictionary<Type, List<Component>> Components { get; private set; }

        public Tracker()
        {
            Entities = new Dictionary<Type, List<Entity>>(Tracked.EntityTypes.Count);
            foreach (var type in Tracked.EntityTypes)
                Entities.Add(type, new List<Entity>());

            Components = new Dictionary<Type, List<Component>>(Tracked.ComponentTypes.Count);
            foreach (var type in Tracked.ComponentTypes)
                Components.Add(type, new List<Component>());
        }

        public void LogEntities()
        {
            foreach (var kv in Entities)
            {
                string output = kv.Key.Name + " : " + kv.Value.Count;
                Engine.Commands.Log(output);
            }
        }

        public void LogComponents()
        {
            foreach (var kv in Components)
            {
                string output = kv.Key.Name + " : " + kv.Value.Count;
                Engine.Commands.Log(output);
            }
        }

        public T GetEntity<T>() where T : Entity
        {
#if DEBUG
            if (!Tracked.EntityTypes.Contains(typeof(T)))
                throw new Exception("Provided Entity type is not marked with the Tracked attribute!");
#endif

            var list = Entities[typeof(T)];
            if (list.Count == 0)
                return null;
            else
                return list[0] as T;
        }

        public List<T> GetEntities<T>() where T : Entity
        {
#if DEBUG
            if (!Tracked.EntityTypes.Contains(typeof(T)))
                throw new Exception("Provided Entity type is not marked with the Tracked attribute!");
#endif

            return Entities[typeof(T)] as List<T>;
        }

        public T GetComponent<T>() where T : Component
        {
#if DEBUG
            if (!Tracked.EntityTypes.Contains(typeof(T)))
                throw new Exception("Provided Component type is not marked with the Tracked attribute!");
#endif

            var list = Components[typeof(T)];
            if (list.Count == 0)
                return null;
            else
                return list[0] as T;
        }

        public List<T> GetComponents<T>() where T : Component
        {
#if DEBUG
            if (!Tracked.EntityTypes.Contains(typeof(T)))
                throw new Exception("Provided Component type is not marked with the Tracked attribute!");
#endif

            return Entities[typeof(T)] as List<T>;
        }

        internal void EntityAdded(Entity entity)
        {
            var type = entity.GetType();
            if (Tracked.EntityTypes.Contains(type))
                Entities[type].Add(entity);
        }

        internal void EntityRemoved(Entity entity)
        {
            var type = entity.GetType();
            if (Tracked.EntityTypes.Contains(type))
                Entities[type].Remove(entity);
        }

        internal void ComponentAdded(Component component)
        {
            var type = component.GetType();
            if (Tracked.ComponentTypes.Contains(type))
                Components[type].Add(component);
        }

        internal void ComponentRemoved(Component component)
        {
            var type = component.GetType();
            if (Tracked.ComponentTypes.Contains(type))
                Components[type].Remove(component);
        }
    }
}
