
namespace Monocle
{
    public abstract class Renderer
    {
        public abstract void BeforeRender(Scene scene);
        public abstract void Render(Scene scene);
        public abstract void AfterRender(Scene scene);
    }
}
