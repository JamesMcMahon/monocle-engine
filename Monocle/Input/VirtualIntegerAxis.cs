using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle.Input
{
    public class VirtualIntegerAxis : VirtualInput
    {
        public List<VirtualAxisNode> Nodes;

        public VirtualIntegerAxis()
            : base()
        {
            Nodes = new List<VirtualAxisNode>();
        }

        public VirtualIntegerAxis(params VirtualAxisNode[] nodes)
            : base()
        {
            Nodes = new List<VirtualAxisNode>(nodes);
        }

        public override void Update()
        {
            foreach (var node in Nodes)
                node.Update();
        }

        public int Value
        {
            get
            {
                foreach (var node in Nodes)
                {
                    float value = node.Value;
                    if (value != 0)
                        return (int)Math.Sign(value);
                }

                return 0;
            }
        }
    }
}
