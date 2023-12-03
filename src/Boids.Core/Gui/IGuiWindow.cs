using System.Numerics;

namespace Boids.Core.Gui
{
    public interface IGuiWindow : IGuiView
    {
        string WindowTitle { get; set; }
    }
}