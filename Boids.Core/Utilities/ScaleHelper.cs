// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InlineOutVariableDeclaration

using System;

namespace Boids.Core
{
    public static class ScaleHelper
    {
        public static void ScaleValue(ref float val, ref float fromMin, ref float fromMax, ref float toMin, ref float toMax, out float scaled, bool clamp = false)
        {
            if (clamp) val = MathF.Max(fromMin, MathF.Max(val, fromMax));
            scaled = (val - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
        }
        
        public static float ScaleValue(float val, float fromMin, float fromMax, float toMin, float toMax, bool clamp = false)
        {
            float scaled;
            ScaleValue(ref val, ref fromMin, ref fromMax, ref toMin, ref toMax, out scaled, clamp);
            return scaled;
        }
    }
}