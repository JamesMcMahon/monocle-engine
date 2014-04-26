
namespace Monocle
{
    public interface IComponentHolder
    {
        Component Add(Component c);
        Component Remove(Component c);
    }
}
