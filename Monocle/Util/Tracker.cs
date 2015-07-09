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

        static public Dictionary<Type, List<Type>> TrackedEntityTypes { get; private set; }
        static public Dictionary<Type, List<Type>> TrackedComponentTypes { get; private set; }
        static public HashSet<Type> StoredEntityTypes { get; private set; }
        static public HashSet<Type> StoredComponentTypes { get; private set; }

        static public void Initialize()
        {
            TrackedEntityTypes = new Dictionary<Type, List<Type>>();
            TrackedComponentTypes = new Dictionary<Type, List<Type>>();
            StoredEntityTypes = new HashSet<Type>();
            StoredComponentTypes = new HashSet<Type>();

            foreach (var type in Assembly.GetEntryAssembly().GetTypes())
            {
                var attrs = type.GetCustomAttributes(typeof(Tracked), false);
                if (attrs.Length > 0)
                {
                    bool inherited = (attrs[0] as Tracked).Inherited;

                    if (typeof(Entity).IsAssignableFrom(type))
                    {
                        if (!type.IsAbstract)
                        {
                            if (!TrackedEntityTypes.ContainsKey(type))
                                TrackedEntityTypes.Add(type, new List<Type>());
                            TrackedEntityTypes[type].Add(type);
                        }

                        StoredEntityTypes.Add(type);

                        if (inherited)
                        {
                            foreach (var subclass in GetSubclasses(type))
                            {
                                if (!subclass.IsAbstract)
                                {
                                    if (!TrackedEntityTypes.ContainsKey(subclass))
                                        TrackedEntityTypes.Add(subclass, new List<Type>());
                                    TrackedEntityTypes[subclass].Add(type);
                                }
                            }
                        }
                    }
                    else if (typeof(Component).IsAssignableFrom(type))
                    {
                        if (!type.IsAbstract)
                        {
                            if (!TrackedComponentTypes.ContainsKey(type))
                                TrackedComponentTypes.Add(type, new List<Type>());
                            TrackedComponentTypes[type].Add(type);
                        }

                        StoredComponentTypes.Add(type);

                        if (inherited)
                        {
                            foreach (var subclass in GetSubclasses(type))
                            {
                                if (!subclass.IsAbstract)
                                {
                                    if (!TrackedComponentTypes.ContainsKey(subclass))
                                        TrackedComponentTypes.Add(subclass, new List<Type>());
                                    TrackedComponentTypes[subclass].Add(type);
                                }
                            }
                        }
                    }
                    else
                        throw new Exception("Type '" + type.Name + "' cannot be Tracked because it does not derive from Entity or Component");
                }
            }
        }

        static private List<Type> GetSubclasses(Type type)
        {
            List<Type> matches = new List<Type>();

            foreach (var check in Assembly.GetEntryAssembly().GetTypes())
                if (type != check && type.IsAssignableFrom(check))
                    matches.Add(check);

            return matches;
        }

        #endregion

        public Dictionary<Type, List<Entity>> Entities { get; private set; }
        public Dictionary<Type, List<Component>> Components { get; private set; }

        public Tracker()
        {
            Entities = new Dictionary<Type, List<Entity>>(TrackedEntityTypes.Count);
            foreach (var type in StoredEntityTypes)
                Entities.Add(type, new List<Entity>());

            Components = new Dictionary<Type, List<Component>>(TrackedComponentTypes.Count);
            foreach (var type in StoredComponentTypes)
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

        public List<Entity> GetEntities<T>() where T : Entity
        {
#if DEBUG
            if (!Entities.ContainsKey(typeof(T)))
                throw new Exception("Provided Entity type is not marked with the Tracked attribute!");
#endif

            return Entities[typeof(T)];
        }

        public List<Entity> GetEntitiesCopy<T>() where T : Entity
        {
            return new List<Entity>(GetEntities<T>());
        }

        public IEnumerator<T> EnumerateEntities<T>() where T : Entity
        {
#if DEBUG
            if (!Entities.ContainsKey(typeof(T)))
                throw new Exception("Provided Entity type is not marked with the Tracked attribute!");
#endif

            foreach (var e in Entities[typeof(T)])
                yield return e as T;
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

        public List<Component> GetComponents<T>() where T : Component
        {
#if DEBUG
            if (!Components.ContainsKey(typeof(T)))
                throw new Exception("Provided Component type is not marked with the Tracked attribute!");
#endif

            return Components[typeof(T)];
        }

        public List<Component> GetComponentsCopy<T>() where T : Component
        {
            return new List<Component>(GetComponents<T>());
        }

        public IEnumerator<T> EnumerateComponents<T>() where T : Component
        {
#if DEBUG
            if (!Components.ContainsKey(typeof(T)))
                throw new Exception("Provided Component type is not marked with the Tracked attribute!");
#endif

            foreach (var c in Components[typeof(T)])
                yield return c as T;
        }

        internal void EntityAdded(Entity entity)
        {
            var type = entity.GetType();
            List<Type> trackAs;

            if (TrackedEntityTypes.TryGetValue(type, out trackAs))
                foreach (var track in trackAs)
                    Entities[track].Add(entity);
        }

        internal void EntityRemoved(Entity entity)
        {
            var type = entity.GetType();
            List<Type> trackAs;

            if (TrackedEntityTypes.TryGetValue(type, out trackAs))
                foreach (var track in trackAs)
                    Entities[track].Remove(entity);
        }

        internal void ComponentAdded(Component component)
        {
            var type = component.GetType();
            List<Type> trackAs;

            if (TrackedComponentTypes.TryGetValue(type, out trackAs))
                foreach (var track in trackAs)
                    Components[track].Add(component);
        }

        internal void ComponentRemoved(Component component)
        {
            var type = component.GetType();
            List<Type> trackAs;

            if (TrackedComponentTypes.TryGetValue(type, out trackAs))
                foreach (var track in trackAs)
                    Components[track].Remove(component);
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
