using System.Numerics;

namespace Boids.Core.Gui.Views
{
    public interface IGuiWindow : IGuiView
    {
        string WindowTitle { get; set; }
    }
}