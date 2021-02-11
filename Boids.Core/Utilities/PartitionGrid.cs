using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xna.Framework;

namespace Boids.Core
{
    public class PartitionGrid
    {
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }
        public int CellCountX { get; set; }
        public int CellCountY { get; set; }
        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        private readonly ILogger<PartitionGrid> _logger;

        public PartitionGrid(int viewportWidth, int viewportHeight, int cellCountX, int cellCountY, ILogger<PartitionGrid> logger = null)
        {
            _logger = logger ?? NullLogger<PartitionGrid>.Instance;

            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
            CellCountX = cellCountX;
            CellCountY = cellCountY;

            CellWidth = ViewportWidth / CellCountX;
            CellHeight = ViewportHeight / CellCountY;

            _logger.LogInformation("Determined cell dimensions: {CellWidth} x {CellHeight}", CellWidth, CellHeight);
        }

        public Vector2 GetCellPosition(Vector2 position)
        {
            return new Vector2(position.X / CellWidth, position.Y / CellHeight);
        }

        public void GetCellPosition(ref Vector2 position, out Vector2 cellPosition)
        {
            cellPosition.X = position.X / CellWidth;
            cellPosition.Y = position.Y / CellHeight;
        }
    }
}
