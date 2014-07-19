using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Monocle
{
    public class Tracker
    {
        #region Static

        static public HashSet<Type> EntityTypes { get; private set; }
        static public HashSet<Type> ComponentTypes { get; private set; }

        static public void Initialize()
        {
            EntityTypes = new HashSet<Type>();
            ComponentTypes = new HashSet<Type>();

            foreach (var type in Assembly.GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(Tracked), false).Length > 0)
                {
                    if (typeof(Entity).IsAssignableFrom(type))
                        EntityTypes.Add(type);
                    else if (typeof(Component).IsAssignableFrom(type))
                        ComponentTypes.Add(type);
                    else
                        throw new Exception("Type '" + type.Name + "' cannot be Tracked because it does not derive from Entity or Component");
                }
            }
        }

        #endregion

        public Dictionary<Type, List<Entity>> Entities { get; private set; }
        public Dictionary<Type, List<Component>> Components { get; private set; }

        public Tracker()
        {
            Entities = new Dictionary<Type, List<Entity>>(EntityTypes.Count);
            foreach (var type in EntityTypes)
                Entities.Add(type, new List<Entity>());

            Components = new Dictionary<Type, List<Component>>(ComponentTypes.Count);
            foreach (var type in ComponentTypes)
                Components.Add(type, new List<Component>());
        }

        public T GetEntity<T>() where T : Entity
        {
#if DEBUG
            if (!Entities.ContainsKey(typeof(T)))
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
            if (!Entities.ContainsKey(typeof(T)))
                throw new Exception("Provided Entity type is not marked with the Tracked attribute!");
#endif

            return Entities[typeof(T)] as List<T>;
        }

        public T GetComponent<T>() where T : Component
        {
#if DEBUG
            if (!Components.ContainsKey(typeof(T)))
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
            if (!Components.ContainsKey(typeof(T)))
                throw new Exception("Provided Component type is not marked with the Tracked attribute!");
#endif

            return Components[typeof(T)] as List<T>;
        }

        internal void EntityAdded(Entity entity)
        {
            var type = entity.GetType();
            if (EntityTypes.Contains(type))
                Entities[type].Add(entity);
        }

        internal void EntityRemoved(Entity entity)
        {
            var type = entity.GetType();
            if (EntityTypes.Contains(type))
                Entities[type].Remove(entity);
        }

        internal void ComponentAdded(Component component)
        {
            var type = component.GetType();
            if (ComponentTypes.Contains(type))
                Components[type].Add(component);
        }

        internal void ComponentRemoved(Component component)
        {
            var type = component.GetType();
            if (ComponentTypes.Contains(type))
                Components[type].Remove(component);
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
    }

    public class Tracked : Attribute
    {
        public bool Inherited;

        public Tracked(bool inherited = false)
        {
            Inherited = inherited;
        }
    }
}
