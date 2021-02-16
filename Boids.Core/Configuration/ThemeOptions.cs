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

        public string Boid { get; set; }
        public Lazy<Color> BoidColor => new Lazy<Color>(() => ConvertToColor(Boid));

        public string BoidPropertiesText { get; set; }
        public Lazy<Color> BoidPropertiesTextColor => new Lazy<Color>(() => ConvertToColor(BoidPropertiesText));

        public string BoidDirectionLine { get; set; }
        public Lazy<Color> BoidDirectionLineColor => new Lazy<Color>(() => ConvertToColor(BoidDirectionLine));

        private static Microsoft.Xna.Framework.Color ConvertToColor(string optionsValue)
        {
            string hex;
            float alpha = 1f;
            Microsoft.Xna.Framework.Color color;
            
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

            var clrColor = System.Drawing.ColorTranslator.FromHtml(hex);
            var xnaColor = clrColor.ToXnaColor(alpha);

            return xnaColor;
        }
    }
}