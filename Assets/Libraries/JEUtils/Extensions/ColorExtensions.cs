using System;
using UnityEngine;

namespace Src.Scripts.Utils
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Rearrange the components of a color
        /// <code>
        /// someColor.Swizzle((r,g,b,a,f) => f(b,r,g,0.6f))
        /// </code>
        /// </summary>
        public static Color Swizzle(this Color self, Func<float, float, float, float, Func<float, float, float, float, Color>, Color> fn)
        {
            // todo a version of swizzle that doesn't care about alpha
            return fn(self.r, self.g, self.b, self.a, (r, g, b, a) => new Color(r, g, b, a));
        }

        public static Color Alpha(this Color self, float alpha)
        {
            return new Color(self.r, self.g, self.b, alpha);
        }

        public static Color Red(this Color self, float red)
        {
            return new Color(red, self.g, self.b, self.a);
        }

        public static Color Green(this Color self, float green)
        {
            return new Color(self.r, green, self.b, self.a);
        }

        public static Color Blue(this Color self, float blue)
        {
            return new Color(self.r, self.g, blue, self.a);
        }

        public static Color Multiply(this Color self, float multiplier)
        {
            return new Color(self.r * multiplier, self.g * multiplier, self.b * multiplier, self.a);
        }
    }
}