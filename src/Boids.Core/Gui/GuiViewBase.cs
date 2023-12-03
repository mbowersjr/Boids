using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace Boids.Core.Gui
{
    public abstract class GuiViewBase : IGuiView
    {
        public GuiManager GuiManager { get; private set; }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        public ref bool IsVisibleRef => ref _isVisible;

        protected ILogger<GuiViewBase> Logger { get; private set; }

        protected GuiViewBase(GuiManager guiManager, ILogger<GuiViewBase> logger)
        {
            GuiManager = guiManager;
            Logger = logger;
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public abstract void LayoutView();
    }
}