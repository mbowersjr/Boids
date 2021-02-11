using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Core
{

    public class PartitionGridRenderer
    {
        public Color LineColor { get; set; } = Color.DodgerBlue;

        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;
        private Tuple<Vector2,Vector2>[] _gridPoints;

        public PartitionGrid _grid;
        public PartitionGrid Grid => _grid;

        private readonly ILogger<PartitionGridRenderer> _logger;

        public PartitionGridRenderer(GraphicsDevice graphics, int cellCountX, int cellCountY, ILogger<PartitionGridRenderer> logger = null)
        {
            _logger = logger ?? NullLogger<PartitionGridRenderer>.Instance;

            _logger.LogInformation("Initializing partition grid with cell: {CellCountX} x {CellCountY}", cellCountX, cellCountY);

            _graphics = graphics;
            _grid = new PartitionGrid(_graphics.Viewport.Width, _graphics.Viewport.Height, cellCountX, cellCountY);
        }

        public void Initialize()
        {
            _spriteBatch = new SpriteBatch(_graphics);
            InitGridLines();
        }

        private void InitGridLines()
        {
            _logger.LogInformation("Caching points for grid lines ...");

            _gridPoints = new Tuple<Vector2,Vector2>[_grid.CellCountX + _grid.CellCountY];

            for (int x = 0; x < _grid.CellCountX; x++)
            {
                var points = new Tuple<Vector2,Vector2>(new Vector2(x * _grid.CellWidth, 0),
                                                        new Vector2(x * _grid.CellWidth, _grid.ViewportHeight));
                _gridPoints[x] = points;
            }

            for (int y = 0; y < _grid.CellCountY; y++)
            {
                var points = new Tuple<Vector2,Vector2>(new Vector2(0, y * _grid.CellHeight),
                                                        new Vector2(_grid.ViewportWidth, y * _grid.CellHeight));
                _gridPoints[_grid.CellCountX + y] = points;
            }
        }

        public void Draw()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            for (int i = 0; i < _gridPoints.Length; i++)
            {
                _spriteBatch.DrawLine(point1: _gridPoints[i].Item1,
                                      point2: _gridPoints[i].Item2,
                                      color: LineColor);
            }

            _spriteBatch.End();
        }
    }
}
