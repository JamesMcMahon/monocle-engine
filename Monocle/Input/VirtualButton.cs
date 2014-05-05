using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Monocle
{
    public class VirtualButton : VirtualInput
    {
        public List<Node> Nodes;

        public VirtualButton()
            : base()
        {
            Nodes = new List<Node>();
        }

        public VirtualButton(params Node[] nodes)
            : base()
        {
            Nodes = new List<Node>(nodes);
        }

        public override void Update()
        {
            foreach (var node in Nodes)
                node.Update();
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
    }
}
