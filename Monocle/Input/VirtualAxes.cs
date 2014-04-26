using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class VirtualAxes
    {
        public List<VirtualAxesNode> Nodes;

        public VirtualAxes()
        {
            Nodes = new List<VirtualAxesNode>();
        }

        public VirtualAxes(params VirtualAxesNode[] nodes)
            : this()
        {
            foreach (var node in nodes)
                Nodes.Add(node);
        }

        public Vector2 Value
        {
            get
            {
                Vector2 value = Vector2.Zero;

                foreach (var node in Nodes)
                {
                    Vector2 nodeValue = node.Value;
                    if (nodeValue != Vector2.Zero)
                    {
                        value = nodeValue;
                        break;
                    }
                }

                return value;
            }
        }
    }

    public abstract class VirtualAxesNode
    {
        public abstract Vector2 Value { get; }
    }

    public class VirtualAxesPadLeftStick : VirtualAxesNode
    {
        public int GamepadIndex;
        public float Deadzone;

        public VirtualAxesPadLeftStick(int gamepadIndex, float deadzone)
        {
            GamepadIndex = gamepadIndex;
            Deadzone = deadzone;
        }

        public override Vector2 Value
        {
            get
            {
                return MInput.GamePads[GamepadIndex].GetLeftStick(Deadzone);
            }
        }
    }

    public class VirtualAxesKeys : VirtualAxesNode
    {
        public enum OverlapBehaviors { CancelOut, TakeOlder, TakeNewer };

        public OverlapBehaviors OverlapBehavior;
        public Keys Left;
        public Keys Right;
        public Keys Up;
        public Keys Down;

        private Vector2 previousValue;

        public VirtualAxesKeys(OverlapBehaviors overlapBehavior, Keys left, Keys right, Keys up, Keys down)
        {
            OverlapBehavior = overlapBehavior;
            Left = left;
            Right = right;
            Up = up;
            Down = down;
        }

        public override Vector2 Value
        {
            get
            {
                Vector2 value = Vector2.Zero;

                //X Axis
                if (MInput.Keyboard.Check(Left))
                {
                    if (MInput.Keyboard.Check(Right))
                    {
                        switch (OverlapBehavior)
                        {
                            case OverlapBehaviors.CancelOut:
                                value.X = 0;
                                break;

                            case OverlapBehaviors.TakeNewer:
                                value.X = -previousValue.X;
                                break;

                            case OverlapBehaviors.TakeOlder:
                                value.X = previousValue.X;
                                break;
                        }
                    }
                    else
                        value.X = -1;
                }
                else if (MInput.Keyboard.Check(Right))
                    value.X = 1;
                else
                    value.X = 0;

                //Y Axis
                if (MInput.Keyboard.Check(Up))
                {
                    if (MInput.Keyboard.Check(Down))
                    {
                        switch (OverlapBehavior)
                        {
                            case OverlapBehaviors.CancelOut:
                                value.Y = 0;
                                break;

                            case OverlapBehaviors.TakeNewer:
                                value.Y = -previousValue.Y;
                                break;

                            case OverlapBehaviors.TakeOlder:
                                value.Y = previousValue.Y;
                                break;
                        }
                    }
                    else
                        value.Y = -1;
                }
                else if (MInput.Keyboard.Check(Down))
                    value.Y = 1;
                else
                    value.Y = 0;

                previousValue = value;
                return value;
            }
        }
    }
}
