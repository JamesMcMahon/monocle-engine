using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Monocle
{
    /// <summary>
    /// A virtual input that is represented as a boolean. As well as simply checking the current button state, you can ask whether it was just pressed or released this frame. You can also keep the button press stored in a buffer for a limited time, or until it is consumed by calling ConsumeBuffer()
    /// </summary>
    public class VirtualButton : VirtualInput
    {
        public List<Node> Nodes;
        public float BufferTime;
        public float FirstRepeatTime;
        public float MultiRepeatTime;
        public bool Repeating { get; private set; }

        private float bufferCounter;
        private float repeatCounter;
        private bool willRepeat;

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

        public void SetRepeat(float repeatTime)
        {
            SetRepeat(repeatTime, repeatTime);
        }
        
        public void SetRepeat(float firstRepeatTime, float multiRepeatTime)
        {
            FirstRepeatTime = firstRepeatTime;
            MultiRepeatTime = multiRepeatTime;
            willRepeat = (FirstRepeatTime > 0);
            if (!willRepeat)
                Repeating = false;
        }

        public override void Update()
        {
            bufferCounter -= Engine.DeltaTime;

            bool check = false;
            foreach (var node in Nodes)
            {
                node.Update();
                if (node.Pressed)
                {
                    bufferCounter = BufferTime;
                    check = true;
                }
                else if (node.Check)
                    check = true;
            }

            if (!check)
            {
                repeatCounter = 0;
                bufferCounter = 0;
            }
            else if (willRepeat)
            {
                Repeating = false;
                if (repeatCounter == 0)
                    repeatCounter = FirstRepeatTime;
                else
                {
                    repeatCounter -= Engine.DeltaTime;
                    if (repeatCounter <= 0)
                    {
                        Repeating = true;
                        repeatCounter = MultiRepeatTime;
                    }
                }
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
                if (bufferCounter > 0 || Repeating)
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
            bufferCounter = 0;
        }

        static public implicit operator bool(VirtualButton button)
        {
            return button.Check;
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

        #region Pad Left Stick

        public class PadLeftStickRight : Node
        {
            public int GamepadIndex;
            public float Deadzone;

            public PadLeftStickRight(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickRightCheck(Deadzone); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickRightPressed(Deadzone); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickRightReleased(Deadzone); }
            }
        }

        public class PadLeftStickLeft : Node
        {
            public int GamepadIndex;
            public float Deadzone;

            public PadLeftStickLeft(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickLeftCheck(Deadzone); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickLeftPressed(Deadzone); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickLeftReleased(Deadzone); }
            }
        }

        public class PadLeftStickUp : Node
        {
            public int GamepadIndex;
            public float Deadzone;

            public PadLeftStickUp(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickUpCheck(Deadzone); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickUpPressed(Deadzone); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickUpReleased(Deadzone); }
            }
        }

        public class PadLeftStickDown : Node
        {
            public int GamepadIndex;
            public float Deadzone;

            public PadLeftStickDown(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickDownCheck(Deadzone); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickDownPressed(Deadzone); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].LeftStickDownReleased(Deadzone); }
            }
        }

        #endregion

        #region Pad Right Stick

        public class PadRightStickRight : Node
        {
            public int GamepadIndex;
            public float Deadzone;

            public PadRightStickRight(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].RightStickRightCheck(Deadzone); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].RightStickRightPressed(Deadzone); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].RightStickRightReleased(Deadzone); }
            }
        }

        public class PadRightStickLeft : Node
        {
            public int GamepadIndex;
            public float Deadzone;

            public PadRightStickLeft(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].RightStickLeftCheck(Deadzone); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].RightStickLeftPressed(Deadzone); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].RightStickLeftReleased(Deadzone); }
            }
        }

        public class PadRightStickUp : Node
        {
            public int GamepadIndex;
            public float Deadzone;

            public PadRightStickUp(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].RightStickUpCheck(Deadzone); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].RightStickUpPressed(Deadzone); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].RightStickUpReleased(Deadzone); }
            }
        }

        public class PadRightStickDown : Node
        {
            public int GamepadIndex;
            public float Deadzone;

            public PadRightStickDown(int gamepadindex, float deadzone)
            {
                GamepadIndex = gamepadindex;
                Deadzone = deadzone;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].RightStickDownCheck(Deadzone); }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].RightStickDownPressed(Deadzone); }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].RightStickDownReleased(Deadzone); }
            }
        }

        #endregion

        #region Pad Triggers

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

        #endregion

        #region Pad DPad

        public class PadDPadRight : Node
        {
            public int GamepadIndex;

            public PadDPadRight(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].DPadRightCheck; }
            }

            public override bool Pressed
            {
	            get { return MInput.GamePads[GamepadIndex].DPadRightPressed; }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].DPadRightReleased; }
            }
        }

        public class PadDPadLeft : Node
        {
            public int GamepadIndex;

            public PadDPadLeft(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].DPadLeftCheck; }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].DPadLeftPressed; }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].DPadLeftReleased; }
            }
        }

        public class PadDPadUp : Node
        {
            public int GamepadIndex;

            public PadDPadUp(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].DPadUpCheck; }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].DPadUpPressed; }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].DPadUpReleased; }
            }
        }

        public class PadDPadDown : Node
        {
            public int GamepadIndex;

            public PadDPadDown(int gamepadIndex)
            {
                GamepadIndex = gamepadIndex;
            }

            public override bool Check
            {
                get { return MInput.GamePads[GamepadIndex].DPadDownCheck; }
            }

            public override bool Pressed
            {
                get { return MInput.GamePads[GamepadIndex].DPadDownPressed; }
            }

            public override bool Released
            {
                get { return MInput.GamePads[GamepadIndex].DPadDownReleased; }
            }
        }

        #endregion

        #region Mouse

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

        #endregion
    }
}
