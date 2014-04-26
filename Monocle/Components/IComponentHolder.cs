using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public interface IComponentHolder
    {
        Component Add(Component c);
        Component Remove(Component c);
    }
}
