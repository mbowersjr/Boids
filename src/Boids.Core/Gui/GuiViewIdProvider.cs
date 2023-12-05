using Microsoft.Extensions.Logging;

namespace Boids.Core.Gui
{
    public interface IGuiViewIdProvider
    {
        string GenerateId<T>(T view) where T : class, IGuiView;
    }

    public class GuiViewIdProvider : IGuiViewIdProvider
    {
        private readonly ILogger<GuiViewIdProvider> _logger;
        
        public GuiViewIdProvider(ILogger<GuiViewIdProvider> logger)
        {
            _logger = logger;
        }

        public string GenerateId<T>(T view) where T : class, IGuiView
        {
            var viewType = typeof(T);
            var viewName = view.Name;

            _logger.LogInformation("Generating Id for instance of view \"{ViewName}\" ({ViewType})", view.Name, viewType.Name);
            
            if (viewName.Contains(' '))
            {
                _logger.LogInformation("Removing whitespace from ViewName \"{ViewName}\"", view.Name);
                
                viewName = viewName.Replace(" ", string.Empty);
            }

            var id = $"{viewType.Name}_{viewName}";

            if (viewType is IGuiWindow)
            {
                _logger.LogDebug("View type {ViewType} implements IGuiWindow, adding \"WINDOW_\" prefix", viewType.Name);
                id = $"WINDOW_{id}";
            }

            _logger.LogDebug("Generated ID \"{Id}\"", id);

            return id;
        }
    }
}