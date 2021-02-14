using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using Boids.Core;

namespace Boids.Core
{
    public class BoidsOptions
    {
        public int Count { get; set; }
        public List<BehaviorOptions> Behaviors { get; set; }
        public PartitionGridOptions PartitionGrid { get; set; }
        public GraphicsOptions Graphics { get; set; }
    }

    public class BehaviorOptions
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
    }

    public class GraphicsOptions
    {
        public ResolutionOptions Resolution { get; set; }
        public bool VSync { get; set; }
    }

    public class ResolutionOptions
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class PartitionGridOptions
    {
        public int CellsX { get; set; }
        public int CellsY { get; set; }
        public bool Visible { get; set; }
        public bool LinesVisible { get; set; }
        public bool HighlightActiveCells { get; set; }
        public Microsoft.Xna.Framework.Color CellHighlightColor { get; set; }
        public Microsoft.Xna.Framework.Color LineColor { get; set; }

        private string _cellHighlightColorName;
        public string CellHighlightColorName
        {
            get => _cellHighlightColorName;
            set
            {
                _cellHighlightColorName = value;
                CellHighlightColor = GetColorFromColorName(_cellHighlightColorName);
            }
        }

        private string _lineColorName;
        public string LineColorName
        {
            get => _lineColorName;
            set
            {
                _lineColorName = value;
                LineColor = GetColorFromColorName(_lineColorName);
            }
        }

        private static Microsoft.Xna.Framework.Color GetColorFromColorName(string colorName)
        {
            var color = System.Drawing.Color.FromName(colorName);

            if (color == default(System.Drawing.Color))
            {
                color = System.Drawing.Color.DodgerBlue;
            }

            return color.ToXnaColor();
        }
    }
}
