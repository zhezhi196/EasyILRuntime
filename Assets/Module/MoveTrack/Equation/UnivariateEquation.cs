using UnityEngine;

namespace Module
{
    /// <summary>
    /// 一元一次方程
    /// </summary>
    public struct UnivariateEquation : IEquation
    {
        public float a;
        public float b;
        
        public UnivariateEquation(float a, float b)
        {
            this.a = a;
            this.b = b;
        }
        public float GetY(float x)
        {
            return a * x + b;
        }
    }
}