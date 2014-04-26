
namespace Monocle
{
    public class Component
    {
        public Entity Entity { get; internal set; }
        public IComponentHolder Parent { get; internal set; }
        public bool MarkedForRemoval { get; internal set; }
        public bool Active;
        public bool Visible;

        public Component(bool active, bool visible)
        {
            Active = active;
            Visible = visible;
        }

        public virtual void Added()
        {

        }

        public virtual void Removed()
        {

        }

        public virtual void EntityAdded()
        {

        }

        public virtual void EntityRemoved()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Render()
        {

        }

        public virtual void HandleGraphicsReset()
        {

        }

        public void RemoveSelf()
        {
            if (Parent != null)
                Parent.Remove(this);
        }

        public Scene Scene
        {
            get { return Entity != null ? Entity.Scene : Engine.Instance.Scene; }
        }

        static public implicit operator bool(Component component)
        {
            return component != null;
        }
    }
}
