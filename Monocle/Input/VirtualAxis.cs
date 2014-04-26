using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class VirtualAxis : VirtualInput
    {
        public List<VirtualAxisNode> Nodes;

        public VirtualAxis()
            : base()
        {
            Nodes = new List<VirtualAxisNode>();
        }

        public VirtualAxis(params VirtualAxisNode[] nodes)
            : base()
        {
            Nodes = new List<VirtualAxisNode>(nodes);
        }

        public override void Update()
        {
            foreach (var node in Nodes)
                node.Update();
        }

        public float Value
        {
            get
            {
                foreach (var node in Nodes)
                {
                    float value = node.Value;
                    if (value != 0)
                        return value;
                }

                return 0;
            }
        }
    }

    public abstract class VirtualAxisNode : VirtualInputNode
    {
        public abstract float Value { get; }
    }

    public class VirtualAxisPadLeftStickX : VirtualAxisNode
    {
        public int GamepadIndex;
        public float Deadzone;

        public VirtualAxisPadLeftStickX(int gamepadIndex, float deadzone)
        {

        }

        public override float Value
        {
            get
            {
                return Calc.SignThreshold(MInput.GamePads[GamepadIndex].GetLeftStick().X, Deadzone);
            }
        }
    }

    public class VirtualAxisPadLeftStickY : VirtualAxisNode
    {
        public int GamepadIndex;
        public float Deadzone;

        public VirtualAxisPadLeftStickY(int gamepadIndex, float deadzone)
        {

        }

        public override float Value
        {
            get
            {
                return Calc.SignThreshold(MInput.GamePads[GamepadIndex].GetLeftStick().Y, Deadzone);
            }
        }
    }

    public class VirtualAxisPadRightStickX : VirtualAxisNode
    {
        public int GamepadIndex;
        public float Deadzone;

        public VirtualAxisPadRightStickX(int gamepadIndex, float deadzone)
        {

        }

        public override float Value
        {
            get
            {
                return Calc.SignThreshold(MInput.GamePads[GamepadIndex].GetRightStick().X, Deadzone);
            }
        }
    }

    public class VirtualAxisPadRightStickY : VirtualAxisNode
    {
        public int GamepadIndex;
        public float Deadzone;

        public VirtualAxisPadRightStickY(int gamepadIndex, float deadzone)
        {

        }

        public override float Value
        {
            get
            {
                return Calc.SignThreshold(MInput.GamePads[GamepadIndex].GetRightStick().Y, Deadzone);
            }
        }
    }

    public class VirtualAxisKeys : VirtualAxisNode
    {
        public enum OverlapBehaviors { CancelOut, TakeOlder, TakeNewer };

        public OverlapBehaviors OverlapBehavior;
        public Keys Positive;
        public Keys Negative;

        private float value;
        private bool turned;

        public VirtualAxisKeys(OverlapBehaviors overlapBehavior, Keys negative, Keys positive)
        {
            OverlapBehavior = overlapBehavior;
            Negative = negative;
            Positive = positive;
        }

        public override void Update()
        {
            if (MInput.Keyboard.Check(Positive))
            {
                if (MInput.Keyboard.Check(Negative))
                {
                    switch (OverlapBehavior)
                    {
                        default:
                        case OverlapBehaviors.CancelOut:
                            value = 0;
                            break;

                        case OverlapBehaviors.TakeNewer:
                            if (!turned)
                            {
                                value *= -1;
                                turned = true;
                            }
                            break;

                        case OverlapBehaviors.TakeOlder:
                            //value stays the same
                            break;
                    }
                }
                else
                {
                    turned = false;
                    value = 1;
                }
            }
            else if (MInput.Keyboard.Check(Negative))
            {
                turned = false;
                value = -1;
            }
            else
            {
                turned = false;
                value = 0;
            }
        }

        public override float Value
        {
            get { return value; }
        }
    }
}
