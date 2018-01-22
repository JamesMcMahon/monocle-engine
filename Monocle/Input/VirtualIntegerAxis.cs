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

        public int Value;
        public int PreviousValue { get; private set; }

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

            PreviousValue = Value;
            Value = 0;
            foreach (var node in Nodes)
            {
                float value = node.Value;
                if (value != 0)
                {
                    Value = Math.Sign(value);
                    break;
                }
            }
        }

        public static implicit operator int(VirtualIntegerAxis axis)
        {
            return axis.Value;
        }
    }
}
