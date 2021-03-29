using System;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;

namespace Boids.Core.Configuration
{
    public class ThemeOptions
    {
        public string Background { get; set; }
        public Lazy<Color> BackgroundColor => new Lazy<Color>(() => ConvertToColor(Background));

        public string PartitionGridLine { get; set; }
        public Lazy<Color> PartitionGridLineColor => new Lazy<Color>(() => ConvertToColor(PartitionGridLine));

        public string PartitionGridHighlight { get; set; }
        public Lazy<Color> PartitionGridHighlightColor => new Lazy<Color>(() => ConvertToColor(PartitionGridHighlight));

        public string AvoidedPointLine { get; set; }
        public Lazy<Color> AvoidedPointLineColor => new Lazy<Color>(() => ConvertToColor(AvoidedPointLine));
        
        public string AvoidedPointActiveLine { get; set; }
        public Lazy<Color> AvoidedPointActiveLineColor => new Lazy<Color>(() => ConvertToColor(AvoidedPointActiveLine));

        public string DistanceReferenceCircle { get; set; }
        public Lazy<Color> DistanceReferenceCircleColor => new Lazy<Color>(() => ConvertToColor(DistanceReferenceCircle));

        public string Boid { get; set; }
        public Lazy<Color> BoidColor => new Lazy<Color>(() => ConvertToColor(Boid));

        public string BoidDirectionLine { get; set; }
        public Lazy<Color> BoidDirectionLineColor => new Lazy<Color>(() => ConvertToColor(BoidDirectionLine));

        public string BoidForceLine { get; set; }
        public Lazy<Color> BoidForceLineColor => new Lazy<Color>(() => ConvertToColor(BoidForceLine));

        public string BoidPropertiesText { get; set; }
        public Lazy<Color> BoidPropertiesTextColor => new Lazy<Color>(() => ConvertToColor(BoidPropertiesText));

        private static Color ConvertToColor(string optionsValue)
        {
            string hex;
            float alpha = 1f;
            
            if (optionsValue.IndexOf(',', StringComparison.OrdinalIgnoreCase) < 0)
            {
                hex = optionsValue;
            }
            else
            {
                var split = optionsValue.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                hex = split[0];
                float.TryParse(split[1], out alpha);
            }

            var clrColor = ColorTranslator.FromHtml(hex);
            var xnaColor = clrColor.ToXnaColor(alpha);

            return xnaColor;
        }
    }
}