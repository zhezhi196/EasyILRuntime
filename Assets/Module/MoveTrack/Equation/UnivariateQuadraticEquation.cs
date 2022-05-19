namespace Module
{
    public struct UnivariateQuadraticEquation : IEquation
    {
        public float a;
        public float b;
        public float c;

        public UnivariateQuadraticEquation(float a, float b, float c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public float GetY(float x)
        {
            return a * x * x + b * x + c;
        }
    }
}