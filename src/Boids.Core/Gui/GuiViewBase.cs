using System;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace Boids.Core.Gui
{
    public abstract class GuiViewBase : IGuiView, IDisposable
    {
        public abstract string Name { get; }

        public virtual string Id { get; private set; }

        // public GuiManager GuiManager { get; private set; }
        private readonly GuiManager _guiManager;

        #region IsVisible

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        public ref bool IsVisibleRef => ref _isVisible;

        #endregion

        protected ILogger<GuiViewBase> Logger { get; private set; }

        protected GuiViewBase(GuiManager guiManager, ILogger<GuiViewBase> logger)
        {
            _guiManager = guiManager;
            Logger = logger;

            Id = _guiManager.GenerateId(this);
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

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private bool _isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                UnloadContent();
            }

            _isDisposed = true;
        }

        #endregion

    }
}