using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;

namespace Boids.Core
{
    public class BoidsOptions
    {
        public int Count { get; set; }
        public BehaviorsOptions Behaviors { get; set; }
        public PartitionGridOptions PartitionGrid { get; set; }
        public GraphicsOptions Graphics { get; set; }
    }

    public class BehaviorsOptions
    {
        public bool Avoidance { get; set; }
        public bool AvoidPoints { get; set; }
        public bool Alignment { get; set; }
        public bool Cohesion { get; set; }
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

        private string _lineColorName;
        public string LineColorName
        {
            get => _lineColorName;
            set
            {
                _lineColorName = value;

                var clrColor = System.Drawing.Color.FromName(_lineColorName);

                if (clrColor == default(System.Drawing.Color))
                    clrColor = System.Drawing.Color.DodgerBlue;

                LineColor = new Microsoft.Xna.Framework.Color(clrColor.R, clrColor.G, clrColor.B, clrColor.A);
            }
        }

        public Microsoft.Xna.Framework.Color LineColor { get; set; }
    }
}