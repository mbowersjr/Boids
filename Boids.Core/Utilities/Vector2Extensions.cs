using System;
using Microsoft.Xna.Framework;

namespace Boids.Core
{
    public static class Vector2Extensions
    {
        public static float ToDegrees(this Vector2 vector)
        {
            var theta = MathHelper.ToDegrees(vector.ToRadians());
            if (theta < 0f)
                theta += 360f;
            return theta;
        }

        public static float ToRadians(this Vector2 vector) => MathF.Atan2(-vector.Y, vector.X);
        
        public static float ToAngle(this Vector2 vector) => MathF.Atan2(vector.Y, vector.X) * MathHelper.TwoPi;
    }
}
