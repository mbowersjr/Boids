using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Boids.Core
{
    public static class FastRandomInst
    {
        private static FastRandom _instance = new FastRandom();
        public static FastRandom Instance => _instance;

        public static void Seed(int seed) => _instance = new FastRandom(seed);

        public static int Next() => _instance.Next();
        public static int Next(int max) => _instance.Next(max);
        public static int Next(int min, int max) => _instance.Next(min, max);
        public static int Next(Range<int> range) => _instance.Next(range);
        public static float NextSingle() => _instance.NextSingle();
        public static float NextSingle(float max) => _instance.NextSingle(max);
        public static float NextSingle(float min, float max) => _instance.NextSingle(min, max);
        public static float NextSingle(Range<float> range) => _instance.NextSingle(range);
        public static float NextAngle() => _instance.NextAngle();
        public static Vector2 NextUnitVector(out Vector2 vector)
        {
            _instance.NextUnitVector(out vector);
            return vector;
        }
        public static Vector2 NextVector2Within(ref RectangleF rect) => _instance.NextVector2Within(ref rect);

    }
}
