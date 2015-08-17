using System;
using System.Collections.Generic;

namespace Monocle
{
    /// <summary>
    /// A virtual input that is represented as a int that is either -1, 0, or 1
    /// </summary>
    public class VirtualIntegerAxis : VirtualInput
    {
        public List<VirtualAxis.Node> Nodes;

        public VirtualIntegerAxis()
            : base()
        {
            Nodes = new List<VirtualAxis.Node>();
        }

        public VirtualIntegerAxis(params VirtualAxis.Node[] nodes)
            : base()
        {
            Nodes = new List<VirtualAxis.Node>(nodes);
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

        static public implicit operator int(VirtualIntegerAxis axis)
        {
            return axis.Value;
        }
    }
}
