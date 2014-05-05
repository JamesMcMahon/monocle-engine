
namespace Monocle
{
    public abstract class VirtualInput
    {
        public enum OverlapBehaviors { CancelOut, TakeOlder, TakeNewer };

        public VirtualInput()
        {
            MInput.VirtualInputs.Add(this);
        }

        public void Dettach()
        {
            MInput.VirtualInputs.Remove(this);
        }

        public abstract void Update();
    }

    public abstract class VirtualInputNode
    {
        public virtual void Update()
        {

        }
    }
}
