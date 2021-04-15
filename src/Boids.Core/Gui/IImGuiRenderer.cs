using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Core.Gui
{
    public interface IImGuiRenderer
    {
        /// <summary>
        /// Creates a texture and loads the font data from ImGui. Should be called when the <see cref="GraphicsDevice" /> is initialized but before any rendering is done
        /// </summary>
        unsafe void RebuildFontAtlas();

        /// <summary>
        /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="MediaTypeNames.Image" />. That pointer is then used by ImGui to let us know what texture to draw
        /// </summary>
        IntPtr BindTexture(Texture2D texture);

        /// <summary>
        /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated
        /// </summary>
        void UnbindTexture(IntPtr textureId);

        /// <summary>
        /// Sets up ImGui for a new frame, should be called at frame start
        /// </summary>
        void BeforeLayout(GameTime gameTime);

        /// <summary>
        /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls
        /// </summary>
        void AfterLayout();
    }
}