using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public abstract class VirtualInput
    {
        public VirtualInput()
        {
            MInput.VirtualInputs.Add(this);
        }

        public void Dettach()
        {
            MInput.VirtualInputs.Remove(this);
        }

        public abstract void Update();
    }

    public abstract class VirtualInputNode
    {
        public virtual void Update()
        {

        }
    }
}
