using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Monocle
{
    public class VirtualButton : VirtualInput
    {
        public List<Node> Nodes;
        public float BufferTime;

        private float bufferCounter;

        public VirtualButton(float bufferTime)
            : base()
        {
            Nodes = new List<Node>();
            BufferTime = bufferTime;
        }

        public VirtualButton()
            : this(0)
        {

        }

        public VirtualButton(float bufferTime, params Node[] nodes)
            : base()
        {
            Nodes = new List<Node>(nodes);
            BufferTime = bufferTime;
        }

        public VirtualButton(params Node[] nodes)
            : this(0, nodes)
        {

        }

        public override void Update()
        {
            if (bufferCounter > 0)
                bufferCounter -= Engine.DeltaTime;

            foreach (var node in Nodes)
            {
                node.Update();
                if (node.Pressed)
                    bufferCounter = BufferTime;
            }
        }

        public bool Check
        {
            get
            {
                foreach (var node in Nodes)
                    if (node.Check)
                        return true;
                return false;
            }
        }

        public bool Pressed
        {
            get
            {
                if (bufferCounter > 0)
                    return true;

                foreach (var node in Nodes)
                    if (node.Pressed)
                        return true;
                return false;
            }
        }

        public bool Released
        {
            get
            {
                foreach (var node in Nodes)
                    if (node.Released)
                        return true;
                return false;
            }
        }

        public void ConsumeBuffer()
        {
            BufferTime = 0;
        }

        public abstract class Node : VirtualInputNode
        {
            public abstract bool Check { get; }
            public abstract bool Pressed { get; }
            public abstract bool Released { get; }
        }

        public class KeyboardKey : Node
        {
            public Keys Key;

            public KeyboardKey(Keys key)
            {
                Key = key;
            }

            public override bool Check
            {
                get { return MInput.Keyboard.Check(Key); }
            }

            public override bool Pressed
            {
                get { return MInput.Keyboard.Pressed(Key); }
            }

            public override bool Released
            {
                get { return MInput.Keyboard.Released(Key); }
            }
        }

        public class PadButton : Node
        {
            public int GamepadIndex;
            public Buttons Button;

            public PadButton(int gamepadIndex, Buttons button)
            {
                GamepadIndex = gamepadIndex;
                Button = button;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].Check(Button); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].Pressed(Button); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].Released(Button); }
            }
        }

        public class PadLeftTrigger : Node
        {
            public int GamepadIndex;
            public float Threshold;

            public PadLeftTrigger(int gamepadIndex, float threshold)
            {
                GamepadIndex = gamepadIndex;
                Threshold = threshold;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].LeftTriggerCheck(Threshold); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].LeftTriggerPressed(Threshold); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].LeftTriggerReleased(Threshold); }
            }
        }

        public class PadRightTrigger : Node
        {
            public int GamepadIndex;
            public float Threshold;

            public PadRightTrigger(int gamepadIndex, float threshold)
            {
                GamepadIndex = gamepadIndex;
                Threshold = threshold;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].RightTriggerCheck(Threshold); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].RightTriggerPressed(Threshold); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].RightTriggerReleased(Threshold); }
            }
        }

        public class MouseLeftButton : Node
        {
            public override bool Check
            {
                get { return MInput.Mouse.CheckLeftButton; }
            }

            public override bool Pressed
            {
                get { return MInput.Mouse.PressedLeftButton; }
            }

            public override bool Released
            {
                get { return MInput.Mouse.ReleasedLeftButton; }
            }
        }

        public class MouseRightButton : Node
        {
            public override bool Check
            {
                get { return MInput.Mouse.CheckRightButton; }
            }

            public override bool Pressed
            {
                get { return MInput.Mouse.PressedRightButton; }
            }

            public override bool Released
            {
                get { return MInput.Mouse.ReleasedRightButton; }
            }
        }

        public class MouseMiddleButton : Node
        {
            public override bool Check
            {
                get { return MInput.Mouse.CheckMiddleButton; }
            }

            public override bool Pressed
            {
                get { return MInput.Mouse.PressedMiddleButton; }
            }

            public override bool Released
            {
                get { return MInput.Mouse.ReleasedMiddleButton; }
            }
        }
    }
}
