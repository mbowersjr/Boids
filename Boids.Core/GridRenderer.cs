using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Core
{
    public class GridRenderer
    {
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }
        public int CellCountX { get; set; }
        public int CellCountY { get; set; }
        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }
        public Color LineColor { get; set; } = Color.DodgerBlue;

        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphics;
        private Tuple<Vector2,Vector2>[] _gridPoints;

        public GridRenderer(GraphicsDevice graphics, int cellCountX, int cellCountY)
        {
            _graphics = graphics;

            CellCountX = cellCountX;
            CellCountY = cellCountY;
        }

        public void Initialize()
        {
            _spriteBatch = new SpriteBatch(_graphics);

            ViewportWidth = _graphics.Viewport.Width;
            ViewportHeight = _graphics.Viewport.Height;

            CellWidth = ViewportWidth / CellCountX;
            CellHeight = ViewportHeight / CellCountY;

            InitGridLines();
        }

        private void InitGridLines()
        {
            _gridPoints = new Tuple<Vector2,Vector2>[CellCountX + CellCountY];

            for (int x = 0; x < CellCountX; x++)
            {
                var points = new Tuple<Vector2,Vector2>(new Vector2(x * CellWidth, 0), new Vector2(x * CellWidth, ViewportHeight));
                _gridPoints[x] = points;
            }

            for (int y = 0; y < CellCountY; y++)
            {
                var points = new Tuple<Vector2,Vector2>(new Vector2(0, y * CellHeight), new Vector2(ViewportWidth, y * CellHeight));
                _gridPoints[CellCountX + y] = points;
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

        public void GetCellPosition(ref Vector2 position, out Vector2 cellPosition)
        {
            cellPosition.X = position.X / CellWidth;
            cellPosition.Y = position.Y / CellHeight;
        }
    }
}
