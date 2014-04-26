using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class VirtualAxes : VirtualInput
    {
        public List<VirtualAxesNode> Nodes;
        public bool Normalized;

        public VirtualAxes(bool normalized)
            : base()
        {
            Nodes = new List<VirtualAxesNode>();
            Normalized = normalized;
        }

        public VirtualAxes(bool normalized, params VirtualAxesNode[] nodes)
            : base()
        {
            Nodes = new List<VirtualAxesNode>(nodes);
            Normalized = normalized;
        }

        public override void Update()
        {
            foreach (var node in Nodes)
                node.Update();
        }

        public Vector2 Value
        {
            get
            {
                foreach (var node in Nodes)
                {
                    Vector2 value = node.Value;
                    if (value != Vector2.Zero)
                    {
                        if (Normalized)
                            value.Normalize();
                        return value;
                    }
                }

                return Vector2.Zero;
            }
        }
    }

    public abstract class VirtualAxesNode : VirtualInputNode
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

    public class VirtualAxesPadRightStick : VirtualAxesNode
    {
        public int GamepadIndex;
        public float Deadzone;

        public VirtualAxesPadRightStick(int gamepadIndex, float deadzone)
        {
            GamepadIndex = gamepadIndex;
            Deadzone = deadzone;
        }

        public override Vector2 Value
        {
            get
            {
                return MInput.GamePads[GamepadIndex].GetRightStick(Deadzone);
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

        private bool turnedX;
        private bool turnedY;
        private Vector2 value;

        public VirtualAxesKeys(OverlapBehaviors overlapBehavior, Keys left, Keys right, Keys up, Keys down)
        {
            OverlapBehavior = overlapBehavior;
            Left = left;
            Right = right;
            Up = up;
            Down = down;
        }

        public override void Update()
        {
            //X Axis
            if (MInput.Keyboard.Check(Left))
            {
                if (MInput.Keyboard.Check(Right))
                {
                    switch (OverlapBehavior)
                    {
                        default:
                        case OverlapBehaviors.CancelOut:
                            value.X = 0;
                            break;

                        case OverlapBehaviors.TakeNewer:
                            if (!turnedX)
                            {
                                value.X *= -1;
                                turnedX = true;
                            }
                            break;

                        case OverlapBehaviors.TakeOlder:
                            //X stays the same
                            break;
                    }
                }
                else
                {
                    turnedX = false;
                    value.X = -1;
                }
            }
            else if (MInput.Keyboard.Check(Right))
            {
                turnedX = false;
                value.X = 1;
            }
            else
            {
                turnedX = false;
                value.X = 0;
            }

            //Y Axis
            if (MInput.Keyboard.Check(Up))
            {
                if (MInput.Keyboard.Check(Down))
                {
                    switch (OverlapBehavior)
                    {
                        default:
                        case OverlapBehaviors.CancelOut:
                            value.Y = 0;
                            break;

                        case OverlapBehaviors.TakeNewer:
                            if (!turnedY)
                            {
                                value.Y *= -1;
                                turnedY = true;
                            }
                            break;

                        case OverlapBehaviors.TakeOlder:
                            //Y stays the same
                            break;
                    }
                }
                else
                {
                    turnedY = false;
                    value.Y = -1;
                }
            }
            else if (MInput.Keyboard.Check(Down))
            {
                turnedY = false;
                value.Y = 1;
            }
            else
            {
                turnedY = false;
                value.Y = 0;
            }
        }

        public override Vector2 Value
        {
            get { return value; }
        }
    }
}
