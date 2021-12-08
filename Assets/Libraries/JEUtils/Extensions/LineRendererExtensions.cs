using UnityEngine;

namespace Src.Scripts.Utils
{
    public static class LineRendererExtensions
    {
        public static void SetSolidColor(this LineRenderer self, Color color)
        {
            self.endColor = color;
            self.startColor = color;
        }
        
        public static void SetConstantWidth(this LineRenderer self, float width)
        {
            self.endWidth = width;
            self.startWidth = width;
        }
    }
}