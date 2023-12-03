using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Boids.Core.Gui
{
    public interface IGuiView
    {
        bool IsVisible { get; set; }
        GuiManager GuiManager { get; }
        void Initialize();
        void Update(GameTime gameTime);
        void LayoutView();
        void LoadContent();
        void UnloadContent();
    }
}
