using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class VirtualButton
    {
        public List<VirtualButtonNode> Nodes { get; private set; }

        public VirtualButton()
        {
            Nodes = new List<VirtualButtonNode>();
        }

        public VirtualButton(params VirtualButtonNode[] nodes)
            : this()
        {
            foreach (var node in nodes)
                Nodes.Add(node);
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
    }

    public abstract class VirtualButtonNode
    {
        public abstract bool Check { get; }
        public abstract bool Pressed { get; }
        public abstract bool Released { get; }
    }

    public class VirtualButtonKey : VirtualButtonNode
    {
        public Keys Key;

        public VirtualButtonKey(Keys key)
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

    public class VirtualButtonPadButton : VirtualButtonNode
    {
        public int GamepadIndex;
        public Buttons Button;

        public VirtualButtonPadButton(int gamepadIndex, Buttons button)
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
}
