using System;
using System.Collections.Generic;

namespace Monocle
{
    static public class Cache
    {
        static public Dictionary<Type, Stack<Entity>> cache;

        static private void Init<T>() where T : Entity, new()
        {
            if (cache == null)
                cache = new Dictionary<Type, Stack<Entity>>();
            if (!cache.ContainsKey(typeof(T)))
                cache.Add(typeof(T), new Stack<Entity>());
        }

        static public void Store<T>(T instance) where T : Entity, new()
        {
            Init<T>();
            cache[typeof(T)].Push(instance);
        }

        static public T Create<T>() where T : Entity, new()
        {
            Init<T>();
            if (cache[typeof(T)].Count > 0)
                return cache[typeof(T)].Pop() as T;
            else
                return new T();
        }

        static public void Clear<T>() where T : Entity, new()
        {
            if (cache != null && cache.ContainsKey(typeof(T)))
                cache[typeof(T)].Clear();
        }

        static public void ClearAll()
        {
            if (cache != null)
                foreach (var kv in cache)
                    kv.Value.Clear();
        }
    }
}
