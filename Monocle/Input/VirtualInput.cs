
namespace Monocle
{
    /// <summary>
    /// Represents a virtual button, axis or joystick whose state is determined by the state of its VirtualInputNodes
    /// </summary>
    public abstract class VirtualInput
    {
        public enum OverlapBehaviors { CancelOut, TakeOlder, TakeNewer };
        public enum ThresholdModes { LargerThan, LessThan, EqualTo };

        public VirtualInput()
        {
            MInput.VirtualInputs.Add(this);
        }

        public void Deregister()
        {
            MInput.VirtualInputs.Remove(this);
        }

        public abstract void Update();
    }

    /// <summary>
    /// Add these to your VirtualInput to define how it determines its current input state. 
    /// For example, if you want to check whether a keyboard key is pressed, create a VirtualButton and add to it a VirtualButton.KeyboardKey
    /// </summary>
    public abstract class VirtualInputNode
    {
        public virtual void Update()
        {

        }
    }
}
