using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    static public class Cache
    {
        static public Dictionary<Type, Stack<Entity>> cache;

        static public void Init<T>() where T : Entity, new()
        {
            if (cache == null)
                cache = new Dictionary<Type, Stack<Entity>>();
            if (!cache.ContainsKey(typeof(T)))
                cache.Add(typeof(T), new Stack<Entity>());
            else
                cache[typeof(T)].Clear();
        }

        static public void Store<T>(T instance) where T : Entity, new()
        {
#if DEBUG
            if (!cache.ContainsKey(typeof(T)))
                throw new Exception("Cache not initialized for type " + typeof(T).GetType().ToString());
#endif
            cache[typeof(T)].Push(instance);
        }

        static public T Create<T>() where T : Entity, new()
        {
#if DEBUG
            if (!cache.ContainsKey(typeof(T)))
                throw new Exception("Cache not initialized for type " + typeof(T).ToString());
#endif
            if (cache[typeof(T)].Count > 0)
                return cache[typeof(T)].Pop() as T;
            else
                return new T();
        }
    }
}
