namespace Boids.Core.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
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