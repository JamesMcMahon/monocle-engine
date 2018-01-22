using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Monocle
{
    public static class MInput
    {
        public static KeyboardData Keyboard { get; private set; }
        public static MouseData Mouse { get; private set; }
        public static GamePadData[] GamePads { get; private set; }

        internal static List<VirtualInput> VirtualInputs;

        public static bool Active = true;
        public static bool Disabled = false;

        internal static void Initialize()
        {
            //Init devices
            Keyboard = new KeyboardData();
            Mouse = new MouseData();
            GamePads = new GamePadData[4];
            for (int i = 0; i < 4; i++)
                GamePads[i] = new GamePadData((PlayerIndex)i);
            VirtualInputs = new List<VirtualInput>();
        }

        internal static void Shutdown()
        {
            foreach (var gamepad in GamePads)
                gamepad.StopRumble();
        }

        internal static void Update()
        {
            if (Engine.Instance.IsActive && Active)
            {
                if (Engine.Commands.Open)
                {
                    Keyboard.UpdateNull();
                    Mouse.UpdateNull();
                }
                else
                {
                    Keyboard.Update();
                    Mouse.Update();
                }

                for (int i = 0; i < 4; i++)
                    GamePads[i].Update();
            }
            else
            {
                Keyboard.UpdateNull();
                Mouse.UpdateNull();
                for (int i = 0; i < 4; i++)
                    GamePads[i].UpdateNull();
            }

            UpdateVirtualInputs();
        }

        public static void UpdateNull()
        {
            Keyboard.UpdateNull();
            Mouse.UpdateNull();
            for (int i = 0; i < 4; i++)
                GamePads[i].UpdateNull();

            UpdateVirtualInputs();
        }

        private static void UpdateVirtualInputs()
        {
            foreach (var virtualInput in VirtualInputs)
                virtualInput.Update();
        }

        #region Keyboard

        public class KeyboardData
        {
            public KeyboardState PreviousState;
            public KeyboardState CurrentState;

            internal KeyboardData()
            {

            }

            internal void Update()
            {
                PreviousState = CurrentState;
                CurrentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            }

            internal void UpdateNull()
            {
                PreviousState = CurrentState;
                CurrentState = new KeyboardState();
            }

            #region Basic Checks

            public bool Check(Keys key)
            {
                if (Disabled)
                    return false;

                return CurrentState.IsKeyDown(key);
            }

            public bool Pressed(Keys key)
            {
                if (Disabled)
                    return false;

                return CurrentState.IsKeyDown(key) && !PreviousState.IsKeyDown(key);
            }

            public bool Released(Keys key)
            {
                if (Disabled)
                    return false;

                return !CurrentState.IsKeyDown(key) && PreviousState.IsKeyDown(key);
            }

            #endregion

            #region Convenience Checks

            public bool Check(Keys keyA, Keys keyB)
            {
                return Check(keyA) || Check(keyB);
            }

            public bool Pressed(Keys keyA, Keys keyB)
            {
                return Pressed(keyA) || Pressed(keyB);
            }

            public bool Released(Keys keyA, Keys keyB)
            {
                return Released(keyA) || Released(keyB);
            }

            public bool Check(Keys keyA, Keys keyB, Keys keyC)
            {
                return Check(keyA) || Check(keyB) || Check(keyC);
            }

            public bool Pressed(Keys keyA, Keys keyB, Keys keyC)
            {
                return Pressed(keyA) || Pressed(keyB) || Pressed(keyC);
            }

            public bool Released(Keys keyA, Keys keyB, Keys keyC)
            {
                return Released(keyA) || Released(keyB) || Released(keyC);
            }

            #endregion

            #region Axis

            public int AxisCheck(Keys negative, Keys positive)
            {
                if (Check(negative))
                {
                    if (Check(positive))
                        return 0;
                    else
                        return -1;
                }
                else if (Check(positive))
                    return 1;
                else
                    return 0;
            }

            public int AxisCheck(Keys negative, Keys positive, int both)
            {
                if (Check(negative))
                {
                    if (Check(positive))
                        return both;
                    else
                        return -1;
                }
                else if (Check(positive))
                    return 1;
                else
                    return 0;
            }

            #endregion
        }

        #endregion

        #region Mouse

        public class MouseData
        {
            public MouseState PreviousState;
            public MouseState CurrentState;

            internal MouseData()
            {
                PreviousState = new MouseState();
                CurrentState = new MouseState();
            }

            internal void Update()
            {
                PreviousState = CurrentState;
                CurrentState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            }

            internal void UpdateNull()
            {
                PreviousState = CurrentState;
                CurrentState = new MouseState();
            }

            #region Buttons

            public bool CheckLeftButton
            {
                get { return CurrentState.LeftButton == ButtonState.Pressed; }
            }

            public bool CheckRightButton
            {
                get { return CurrentState.RightButton == ButtonState.Pressed; }
            }

            public bool CheckMiddleButton
            {
                get { return CurrentState.MiddleButton == ButtonState.Pressed; }
            }

            public bool PressedLeftButton
            {
                get { return CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released; }
            }

            public bool PressedRightButton
            {
                get { return CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released; }
            }

            public bool PressedMiddleButton
            {
                get { return CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released; }
            }

            public bool ReleasedLeftButton
            {
                get { return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed; }
            }

            public bool ReleasedRightButton
            {
                get { return CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed; }
            }

            public bool ReleasedMiddleButton
            {
                get { return CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed; }
            }

            #endregion

            #region Wheel

            public int Wheel
            {
                get { return CurrentState.ScrollWheelValue; }
            }

            public int WheelDelta
            {
                get { return CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue; }
            }

            #endregion

            #region Position

            public bool WasMoved
            {
                get
                {
                    return CurrentState.X != PreviousState.X
                        || CurrentState.Y != PreviousState.Y;
                }
            }

            public float X
            {
                get { return Position.X; }
                set { Position = new Vector2(value, Position.Y); }
            }

            public float Y
            {
                get { return Position.Y; }
                set { Position = new Vector2(Position.X, value); }
            }

            public Vector2 Position
            {
                get
                {
                    return Vector2.Transform(new Vector2(CurrentState.X, CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix));
                }

                set
                {
                    var vector = Vector2.Transform(value, Engine.ScreenMatrix);
                    Microsoft.Xna.Framework.Input.Mouse.SetPosition((int)Math.Round(vector.X), (int)Math.Round(vector.Y));
                }
            }

            #endregion
        }

        #endregion

        #region GamePads

        public class GamePadData
        {
            public PlayerIndex PlayerIndex { get; private set; }
            public GamePadState PreviousState;
            public GamePadState CurrentState;
            public bool Attached;

            private float rumbleStrength;
            private float rumbleTime;

            internal GamePadData(PlayerIndex playerIndex)
            {
                PlayerIndex = playerIndex;
            }

            public void Update()
            {
                PreviousState = CurrentState;
                CurrentState = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex);
                Attached = CurrentState.IsConnected;

                if (rumbleTime > 0)
                {
                    rumbleTime -= Engine.DeltaTime;
                    if (rumbleTime <= 0)
                        GamePad.SetVibration(PlayerIndex, 0, 0);
                }
            }

            public void UpdateNull()
            {
                PreviousState = CurrentState;
                CurrentState = new GamePadState();
                Attached = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex).IsConnected;

                if (rumbleTime > 0)
                    rumbleTime -= Engine.DeltaTime;

                GamePad.SetVibration(PlayerIndex, 0, 0);
            }

            public void Rumble(float strength, float time)
            {
                if (rumbleTime <= 0 || strength > rumbleStrength || (strength == rumbleStrength && time > rumbleTime))
                {
                    GamePad.SetVibration(PlayerIndex, strength, strength);
                    rumbleStrength = strength;
                    rumbleTime = time;
                }
            }

            public void StopRumble()
            {
                GamePad.SetVibration(PlayerIndex, 0, 0);
                rumbleTime = 0;
            }

            #region Buttons

            public bool Check(Buttons button)
            {
                if (Disabled)
                    return false;

                return CurrentState.IsButtonDown(button);
            }

            public bool Pressed(Buttons button)
            {
                if (Disabled)
                    return false;

                return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
            }

            public bool Released(Buttons button)
            {
                if (Disabled)
                    return false;

                return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
            }

            public bool Check(Buttons buttonA, Buttons buttonB)
            {
                return Check(buttonA) || Check(buttonB);
            }

            public bool Pressed(Buttons buttonA, Buttons buttonB)
            {
                return Pressed(buttonA) || Pressed(buttonB);
            }

            public bool Released(Buttons buttonA, Buttons buttonB)
            {
                return Released(buttonA) || Released(buttonB);
            }

            public bool Check(Buttons buttonA, Buttons buttonB, Buttons buttonC)
            {
                return Check(buttonA) || Check(buttonB) || Check(buttonC);
            }

            public bool Pressed(Buttons buttonA, Buttons buttonB, Buttons buttonC)
            {
                return Pressed(buttonA) || Pressed(buttonB) || Check(buttonC);
            }

            public bool Released(Buttons buttonA, Buttons buttonB, Buttons buttonC)
            {
                return Released(buttonA) || Released(buttonB) || Check(buttonC);
            }

            #endregion

            #region Sticks

            public Vector2 GetLeftStick()
            {
                Vector2 ret = CurrentState.ThumbSticks.Left;
                ret.Y = -ret.Y;
                return ret;
            }

            public Vector2 GetLeftStick(float deadzone)
            {
                Vector2 ret = CurrentState.ThumbSticks.Left;
                if (ret.LengthSquared() < deadzone * deadzone)
                    ret = Vector2.Zero;
                else
                    ret.Y = -ret.Y;
                return ret;
            }

            public Vector2 GetRightStick()
            {
                Vector2 ret = CurrentState.ThumbSticks.Right;
                ret.Y = -ret.Y;
                return ret;
            }

            public Vector2 GetRightStick(float deadzone)
            {
                Vector2 ret = CurrentState.ThumbSticks.Right;
                if (ret.LengthSquared() < deadzone * deadzone)
                    ret = Vector2.Zero;
                else
                    ret.Y = -ret.Y;
                return ret;
            }

            #region Left Stick Directions

            public bool LeftStickLeftCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X <= -deadzone;
            }

            public bool LeftStickLeftPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X <= -deadzone && PreviousState.ThumbSticks.Left.X > -deadzone;
            }

            public bool LeftStickLeftReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X > -deadzone && PreviousState.ThumbSticks.Left.X <= -deadzone;
            }

            public bool LeftStickRightCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X >= deadzone;
            }

            public bool LeftStickRightPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X >= deadzone && PreviousState.ThumbSticks.Left.X < deadzone;
            }

            public bool LeftStickRightReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.X < deadzone && PreviousState.ThumbSticks.Left.X >= deadzone;
            }

            public bool LeftStickDownCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y <= -deadzone;
            }

            public bool LeftStickDownPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y <= -deadzone && PreviousState.ThumbSticks.Left.Y > -deadzone;
            }

            public bool LeftStickDownReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y > -deadzone && PreviousState.ThumbSticks.Left.Y <= -deadzone;
            }

            public bool LeftStickUpCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y >= deadzone;
            }

            public bool LeftStickUpPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y >= deadzone && PreviousState.ThumbSticks.Left.Y < deadzone;
            }

            public bool LeftStickUpReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Left.Y < deadzone && PreviousState.ThumbSticks.Left.Y >= deadzone;
            }

            public float LeftStickHorizontal(float deadzone)
            {
                float h = CurrentState.ThumbSticks.Left.X;
                if (Math.Abs(h) < deadzone)
                    return 0;
                else
                    return h;
            }

            public float LeftStickVertical(float deadzone)
            {
                float v = CurrentState.ThumbSticks.Left.Y;
                if (Math.Abs(v) < deadzone)
                    return 0;
                else
                    return -v;
            }

            #endregion

            #region Right Stick Directions

            public bool RightStickLeftCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X <= -deadzone;
            }

            public bool RightStickLeftPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X <= -deadzone && PreviousState.ThumbSticks.Right.X > -deadzone;
            }

            public bool RightStickLeftReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X > -deadzone && PreviousState.ThumbSticks.Right.X <= -deadzone;
            }

            public bool RightStickRightCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X >= deadzone;
            }

            public bool RightStickRightPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X >= deadzone && PreviousState.ThumbSticks.Right.X < deadzone;
            }

            public bool RightStickRightReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.X < deadzone && PreviousState.ThumbSticks.Right.X >= deadzone;
            }

            public bool RightStickUpCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y <= -deadzone;
            }

            public bool RightStickUpPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y <= -deadzone && PreviousState.ThumbSticks.Right.Y > -deadzone;
            }

            public bool RightStickUpReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y > -deadzone && PreviousState.ThumbSticks.Right.Y <= -deadzone;
            }

            public bool RightStickDownCheck(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y >= deadzone;
            }

            public bool RightStickDownPressed(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y >= deadzone && PreviousState.ThumbSticks.Right.Y < deadzone;
            }

            public bool RightStickDownReleased(float deadzone)
            {
                return CurrentState.ThumbSticks.Right.Y < deadzone && PreviousState.ThumbSticks.Right.Y >= deadzone;
            }

            public float RightStickHorizontal(float deadzone)
            {
                float h = CurrentState.ThumbSticks.Right.X;
                if (Math.Abs(h) < deadzone)
                    return 0;
                else
                    return h;
            }

            public float RightStickVertical(float deadzone)
            {
                float v = CurrentState.ThumbSticks.Right.Y;
                if (Math.Abs(v) < deadzone)
                    return 0;
                else
                    return -v;
            }

            #endregion

            #endregion

            #region DPad

            public int DPadHorizontal
            {
                get
                {
                    return CurrentState.DPad.Right == ButtonState.Pressed ? 1 : (CurrentState.DPad.Left == ButtonState.Pressed ? -1 : 0);
                }
            }

            public int DPadVertical
            {
                get
                {
                    return CurrentState.DPad.Down == ButtonState.Pressed ? 1 : (CurrentState.DPad.Up == ButtonState.Pressed ? -1 : 0);
                }
            }

            public Vector2 DPad
            {
                get
                {
                    return new Vector2(DPadHorizontal, DPadVertical);
                }
            }

            public bool DPadLeftCheck
            {
                get
                {
                    return CurrentState.DPad.Left == ButtonState.Pressed;
                }
            }

            public bool DPadLeftPressed
            {
                get
                {
                    return CurrentState.DPad.Left == ButtonState.Pressed && PreviousState.DPad.Left == ButtonState.Released;
                }
            }

            public bool DPadLeftReleased
            {
                get
                {
                    return CurrentState.DPad.Left == ButtonState.Released && PreviousState.DPad.Left == ButtonState.Pressed;
                }
            }

            public bool DPadRightCheck
            {
                get
                {
                    return CurrentState.DPad.Right == ButtonState.Pressed;
                }
            }

            public bool DPadRightPressed
            {
                get
                {
                    return CurrentState.DPad.Right == ButtonState.Pressed && PreviousState.DPad.Right == ButtonState.Released;
                }
            }

            public bool DPadRightReleased
            {
                get
                {
                    return CurrentState.DPad.Right == ButtonState.Released && PreviousState.DPad.Right == ButtonState.Pressed;
                }
            }

            public bool DPadUpCheck
            {
                get
                {
                    return CurrentState.DPad.Up == ButtonState.Pressed;
                }
            }

            public bool DPadUpPressed
            {
                get
                {
                    return CurrentState.DPad.Up == ButtonState.Pressed && PreviousState.DPad.Up == ButtonState.Released;
                }
            }

            public bool DPadUpReleased
            {
                get
                {
                    return CurrentState.DPad.Up == ButtonState.Released && PreviousState.DPad.Up == ButtonState.Pressed;
                }
            }

            public bool DPadDownCheck
            {
                get
                {
                    return CurrentState.DPad.Down == ButtonState.Pressed;
                }
            }

            public bool DPadDownPressed
            {
                get
                {
                    return CurrentState.DPad.Down == ButtonState.Pressed && PreviousState.DPad.Down == ButtonState.Released;
                }
            }

            public bool DPadDownReleased
            {
                get
                {
                    return CurrentState.DPad.Down == ButtonState.Released && PreviousState.DPad.Down == ButtonState.Pressed;
                }
            }

            #endregion

            #region Triggers

            public bool LeftTriggerCheck(float threshold)
            {
                if (Disabled)
                    return false;

                return CurrentState.Triggers.Left >= threshold;
            }

            public bool LeftTriggerPressed(float threshold)
            {
                if (Disabled)
                    return false;

                return CurrentState.Triggers.Left >= threshold && PreviousState.Triggers.Left < threshold;
            }

            public bool LeftTriggerReleased(float threshold)
            {
                if (Disabled)
                    return false;

                return CurrentState.Triggers.Left < threshold && PreviousState.Triggers.Left >= threshold;
            }

            public bool RightTriggerCheck(float threshold)
            {
                if (Disabled)
                    return false;

                return CurrentState.Triggers.Right >= threshold;
            }

            public bool RightTriggerPressed(float threshold)
            {
                if (Disabled)
                    return false;

                return CurrentState.Triggers.Right >= threshold && PreviousState.Triggers.Right < threshold;
            }

            public bool RightTriggerReleased(float threshold)
            {
                if (Disabled)
                    return false;

                return CurrentState.Triggers.Right < threshold && PreviousState.Triggers.Right >= threshold;
            }

            #endregion
        }

        #endregion

        #region Helpers

        public static void RumbleFirst(float strength, float time)
        {
            GamePads[0].Rumble(strength, time);
        }

        public static int Axis(bool negative, bool positive, int bothValue)
        {
            if (negative)
            {
                if (positive)
                    return bothValue;
                else
                    return -1;
            }
            else if (positive)
                return 1;
            else
                return 0;
        }

        public static int Axis(float axisValue, float deadzone)
        {
            if (Math.Abs(axisValue) >= deadzone)
                return Math.Sign(axisValue);
            else
                return 0;
        }

        public static int Axis(bool negative, bool positive, int bothValue, float axisValue, float deadzone)
        {
            int ret = Axis(axisValue, deadzone);
            if (ret == 0)
                ret = Axis(negative, positive, bothValue);
            return ret;
        }

        #endregion
    }
}
