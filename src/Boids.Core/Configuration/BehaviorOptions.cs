namespace Boids.Core.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BehaviorOptions
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public float? Coefficient { get; set; }
        public float? Radius { get; set; }
        public int? Order { get; set; }
    }
}