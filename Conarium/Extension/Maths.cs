namespace Conarium.Extension
{
    public static class Maths
    {
        public static float LerpF(this float a, float b, float alpha)
            => a + (b-a) * alpha;

        public static double LerpD(this double a, double b, double alpha)
            => a + (b-a) * alpha;

        
    }
}