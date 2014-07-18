using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Monocle
{
    public class MethodHandle<T> where T : Entity
    {
        private MethodInfo info;

        public MethodHandle(string methodName)
        {
            info = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic);
        }

        public void Call(T instance)
        {
            info.Invoke(instance, null);
        }
    }
}
