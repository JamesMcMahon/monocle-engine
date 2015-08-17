
namespace Monocle
{
    public class Component
    {
        public Entity Entity { get; private set; }
        public bool Active;
        public bool Visible;

        public Component(bool active, bool visible)
        {
            Active = active;
            Visible = visible;
        }

        public virtual void Added(Entity entity)
        {
            Entity = entity;
            if (Scene != null)
                Scene.Tracker.ComponentAdded(this);
        }

        public virtual void Removed(Entity entity)
        {
            Entity = null;
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

        public virtual void HandleGraphicsCreate()
        {

        }

        public void RemoveSelf()
        {
            if (Entity != null)
                Entity.Remove(this);
        }

        public Scene Scene
        {
            get { return Entity != null ? Entity.Scene : null; }
        }
    }
}
