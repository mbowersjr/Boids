using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Boids.Core.Gui.Views
{
    public interface IGuiView
    {
        string Id { get; }
        string Name { get; }
        bool IsVisible { get; set; }
        ref bool IsVisibleRef { get; }
        void Initialize();
        void Update(GameTime gameTime);
        void LoadContent();
        void UnloadContent();
        void LayoutView();
    }
}
