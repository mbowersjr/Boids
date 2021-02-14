using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using Boids.Core.Entities;
    
namespace Boids.Core.Services
{
    public class PartitionGrid
    {
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }
        public int CellCountX { get; set; }
        public int CellCountY { get; set; }
        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }
        
        private PartitionGridRenderer _gridRenderer;
        private readonly ILogger<PartitionGrid> _logger;
        
        public PartitionGrid(PartitionGridRenderer gridRenderer, ILogger<PartitionGrid> logger)
        {
            _gridRenderer = gridRenderer;
            _logger = logger;
        }

        public void Initialize()
        {
            ViewportWidth = MainGame.Options.Graphics.Resolution.X;
            ViewportHeight = MainGame.Options.Graphics.Resolution.Y;
            CellCountX = MainGame.Options.PartitionGrid.CellsX;
            CellCountY = MainGame.Options.PartitionGrid.CellsY;

            CellWidth = ViewportWidth / CellCountX;
            CellHeight = ViewportHeight / CellCountY;
            
            _logger.LogDebug("Initializing partition grid with cell counts: {CellCountX} x {CellCountY}", CellCountX, CellCountY);
            _logger.LogDebug("Determined cell dimensions: {CellWidth} x {CellHeight}", CellWidth, CellHeight);
            
            _gridRenderer.Initialize(this);
        }

        public void UpdateActiveCells(IEnumerable<Boid> boids)
        {
            _gridRenderer.ClearActiveCells();
            
            foreach (var boid in boids)
            {
                if (!boid.IsActive)
                    continue;

                var cellPosition = GetCellPosition(boid.Position);
                ref var cell = ref _gridRenderer.GetCellRef(cellPosition.X, cellPosition.Y);
                
                cell.IsActive = true;
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            _gridRenderer.Draw(gameTime);
        }
        
        public Vector2 GetCellPosition(Boid boid)
        {
            return new Vector2(boid.Position.X / CellWidth, boid.Position.Y / CellHeight);
        }
        
        public (int X, int Y) GetCellPosition(Vector2 position)
        {
            return ((int)(position.X / CellWidth), (int)(position.Y / CellHeight));
        }
    }
}
