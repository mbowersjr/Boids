using System;

namespace Boids.Core
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string str1, string str2) => string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
    }
}
