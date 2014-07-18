using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Monocle
{
    public class Tracked : Attribute
    {
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

    }
}
