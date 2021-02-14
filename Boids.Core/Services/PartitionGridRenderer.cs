using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.VectorDraw;

namespace Boids.Core.Services
{
    public class PartitionGridRenderer
    {
        private PrimitiveBatch _primitiveBatch;
        private PrimitiveDrawing _primitiveDrawing;
        private Tuple<Vector2,Vector2>[] _gridPoints;
        private GridCell[,] _cells;
        private PartitionGrid _parentPartitionGrid;
        
        private readonly ILogger<PartitionGridRenderer> _logger;

        public PartitionGridRenderer(ILogger<PartitionGridRenderer> logger)
        {
            _logger = logger;
        }

        public ref GridCell GetCellRef(int x, int y)
        {
            return ref _cells[x, y];
        }

        public void ClearActiveCells()
        {
            for (var x = 0; x < _cells.GetLength(0); x++)
            {
                for (var y = 0; y < _cells.GetLength(1); y++)
                {
                    _cells[x, y].IsActive = false;
                }
            }
        }
        
        public void Initialize(PartitionGrid parentPartitionGrid)
        {
            _parentPartitionGrid = parentPartitionGrid;
            
            _logger.LogDebug("Initializing grid renderer ...");
            
            _primitiveBatch = new PrimitiveBatch(MainGame.Graphics.GraphicsDevice);
            _primitiveDrawing = new PrimitiveDrawing(_primitiveBatch);

            InitGridCells();
            InitGridLines();
        }

        private void InitGridCells()
        {
            _cells = new GridCell[_parentPartitionGrid.CellCountX, _parentPartitionGrid.CellCountY];

            for (var x = 0; x < _parentPartitionGrid.CellCountX; x++)
            {
                for (var y = 0; y < _parentPartitionGrid.CellCountY; y++)
                {
                    var cell = new GridCell();
                    cell.Bounds = new Rectangle(x * _parentPartitionGrid.CellWidth, y * _parentPartitionGrid.CellHeight, _parentPartitionGrid.CellWidth, _parentPartitionGrid.CellHeight);
                    _cells[x,y] = cell;
                }
            }
        }

        private void InitGridLines()
        {
            _logger.LogDebug("Caching points for grid lines ...");

            _gridPoints = new Tuple<Vector2,Vector2>[_parentPartitionGrid.CellCountX + _parentPartitionGrid.CellCountY];

            for (var x = 0; x < _parentPartitionGrid.CellCountX; x++)
            {
                var points = new Tuple<Vector2,Vector2>(new Vector2(x * _parentPartitionGrid.CellWidth, 0),
                                                        new Vector2(x * _parentPartitionGrid.CellWidth, _parentPartitionGrid.ViewportHeight));
                _gridPoints[x] = points;
            }

            for (var y = 0; y < _parentPartitionGrid.CellCountY; y++)
            {
                var points = new Tuple<Vector2,Vector2>(new Vector2(0, y * _parentPartitionGrid.CellHeight),
                                                        new Vector2(_parentPartitionGrid.ViewportWidth, y * _parentPartitionGrid.CellHeight));
                _gridPoints[_parentPartitionGrid.CellCountX + y] = points;
            }
        }

        private Matrix _localProjection;
        private Matrix _localView;

        public void Draw(GameTime gameTime)
        {
            if (!MainGame.Options.PartitionGrid.Visible)
                return;

            _localProjection = Matrix.CreateOrthographicOffCenter(0f, _parentPartitionGrid.ViewportWidth, _parentPartitionGrid.ViewportHeight, 0f, 0f, 1f);
            _localView = Matrix.Identity;

            _primitiveBatch.Begin(ref _localProjection, ref _localView);

            if (MainGame.Options.PartitionGrid.LinesVisible)
            {
                for (var i = 0; i < _gridPoints.Length; i++)
                {
                    _primitiveDrawing.DrawSegment(_gridPoints[i].Item1, _gridPoints[i].Item2, MainGame.Options.PartitionGrid.LineColor);
                }
            }

            if (MainGame.Options.PartitionGrid.HighlightActiveCells)
            {
                for (var x = 0; x < _cells.GetLength(0); x++)
                {
                    for (var y = 0; y < _cells.GetLength(1); y++)
                    {
                        if (_cells[x, y].IsActive)
                        {
                            _primitiveDrawing.DrawSolidRectangle(_cells[x, y].Bounds.Position,
                                                                 _cells[x, y].Bounds.Width,
                                                                 _cells[x, y].Bounds.Height,
                                                                 MainGame.Options.PartitionGrid.CellHighlightColor);
                        }
                    }
                }
            }

            _primitiveBatch.End();
        }

        public struct GridCell
        {
            public bool IsActive;
            public RectangleF Bounds;
        }
    }
    
}
