namespace Src.Scripts.Utils
{
    public static class NumericalExtensions
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static float Remap(this int value, float from1, float to1, float from2, float to2)
        {
            return ((float) value).Remap(from1, to1, from2, to2);
        }
    }
}