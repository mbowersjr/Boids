using Boids.Core;

namespace Boids.Core.Behaviors
{
    public class FlockBehaviors
    {
        public AvoidanceBehavior Avoidance { get; private set; }
        public AvoidPointsBehavior AvoidPoints { get; private set; }
        public CohesionBehavior Cohesion { get; private set; }
        public AlignmentBehavior Alignment { get; private set; }

        public FlockBehaviors()
        {
            Avoidance = new AvoidanceBehavior();
            AvoidPoints = new AvoidPointsBehavior();
            Cohesion = new CohesionBehavior();
            Alignment = new AlignmentBehavior();
        }
    }
}
