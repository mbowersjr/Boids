using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Boids.Core
{
    public static class Vector2Extensions
    {
        public static float ToDegrees(this Vector2 vector)
        {
            var theta = MathHelper.ToDegrees(vector.ToRadians());
            // if (theta < 0f)
            //     theta += 360f;
            return theta;
        }

        public static float ToRadians(this Vector2 vector) => MathHelper.WrapAngle(MathF.Atan2(vector.Y, vector.X));

        public static Vector2 NextVector2Within(this FastRandom random, ref RectangleF rect)
        {
            var x = random.NextSingle(rect.Left, rect.Right);
            var y = random.NextSingle(rect.Top, rect.Bottom);
            return new Vector2(x, y);
        }
        
        public static void Limit(this Vector2 vector, float max)
        {
            if (vector.LengthSquared() > (max * max))
            {
                var result = vector.NormalizedCopy();
                result *= max;

                vector.X = result.X;
                vector.Y = result.Y;
            }
        }
    }
}
