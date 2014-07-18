
namespace Monocle
{
    public class Component
    {
        public ComponentList Container { get; private set; }
        public bool Active;
        public bool Visible;

        public Component(bool active, bool visible)
        {
            Active = active;
            Visible = visible;
        }

        public virtual void Added(ComponentList container)
        {
            Container = container;
            if (Scene != null)
                Scene.Tracker.ComponentAdded(this);
        }

        public virtual void Removed(ComponentList container)
        {
            Container = null;
            if (Scene != null)
                Scene.Tracker.ComponentRemoved(this);
        }

        public virtual void EntityAdded()
        {
            if (Scene != null)
                Scene.Tracker.ComponentAdded(this);
        }

        public virtual void EntityRemoved()
        {
            Scene.Tracker.ComponentRemoved(this);
        }

        public virtual void Update()
        {

        }

        public virtual void Render()
        {

        }

        public virtual void DebugRender()
        {

        }

        public virtual void HandleGraphicsReset()
        {

        }

        public void RemoveSelf()
        {
            if (Container != null)
                Container.Remove(this);
        }

        public Entity Entity
        {
            get { return Container != null ? Container.Entity : null; }
        }

        public Scene Scene
        {
            get { return Container != null ? Container.Entity.Scene : null; }
        }
    }
}
