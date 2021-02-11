using System;
using Microsoft.Xna.Framework;

namespace Boids.Core
{
    public static class RandomStatic
    {
        private static readonly Random _rand;
        private static int _state;

        static RandomStatic()
        {
            _state = 1;
            _rand = new Random();
        }

        public static int Next()
        {
            _state = 214013*_state + 2531011;
            return (_state >> 16) & 0x7FFF;
        }

        public static int Next(int maxValue)
        {
            return (int)(maxValue * NextSingle() + 0.5f);
        }

        public static int Next(int minValue, int maxValue)
        {
            return (int)((maxValue - minValue) * NextSingle() + 0.5f) + minValue;
        }

        public static void NextBytes(Span<byte> buffer) => _rand.NextBytes(buffer);
        public static void NextBytes(byte[] buffer) => _rand.NextBytes(buffer);

        public static int Next_Original() => _rand.Next();
        public static int Next_Original(int maxValue) => _rand.Next(maxValue);
        public static int Next_Original(int minValue, int maxValue) => _rand.Next(minValue, maxValue);

        public static double NextDouble() => _rand.NextDouble();

        public static float NextSingle(float minValue, float maxValue)
        {
            return (maxValue - minValue) * NextSingle() + minValue;
        }

        public static float NextSingle(float maxValue)
        {
            return NextSingle() * maxValue;
        }

        public static float NextSingle()
        {
            return Next() / (float)short.MaxValue;
        }

        public static float NextAngle()
        {
            return NextSingle(-MathHelper.Pi, MathHelper.Pi);
        }

        public static Vector2 NextUnitVector()
        {
            var angle = NextAngle();
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }
    }
}