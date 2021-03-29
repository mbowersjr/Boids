namespace Boids.Core
{
    public static class ColorExtensions
    {
        public static Microsoft.Xna.Framework.Color ToXnaColor(this System.Drawing.Color clrColor, float alpha = 1f) => new Microsoft.Xna.Framework.Color(clrColor.R, clrColor.G, clrColor.B, clrColor.A);
        public static System.Drawing.Color ToClrColor(this Microsoft.Xna.Framework.Color xnaColor) => System.Drawing.Color.FromArgb(xnaColor.A, xnaColor.R, xnaColor.G, xnaColor.B);
    }
}
