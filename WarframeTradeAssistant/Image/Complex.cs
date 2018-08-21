namespace WarframeTradeAssistant.Image
{
    using System;

    [Serializable]
    public struct Complex
    {
        public Complex(float real)
        {
            this.Real = real;
            this.Imag = 0;
        }

        public Complex(float real, float imag)
        {
            this.Real = real;
            this.Imag = imag;
        }

        public float Imag { get; }

        public float Real { get; }

        public static Complex Add(Complex left, Complex right)
        {
            return new Complex(left.Real + right.Real, left.Imag + right.Imag);
        }

        public static Complex Exponent(float x)
        {
            return new Complex((float)Math.Cos(x), (float)Math.Sin(x));
        }

        public static Complex Subtract(Complex left, Complex right)
        {
            return new Complex(left.Real - right.Real, left.Imag - right.Imag);
        }

        public static Complex operator +(Complex left, Complex right)
        {
            return new Complex(left.Real + right.Real, left.Imag + right.Imag);
        }

        public static Complex operator -(Complex left, Complex right)
        {
            return new Complex(left.Real - right.Real, left.Imag - right.Imag);
        }

        public static Complex operator *(Complex left, Complex right)
        {
            return new Complex((left.Real * right.Real) - (left.Imag * right.Imag), (left.Real * right.Imag) + (left.Imag * right.Real));
        }

        public static Complex operator *(float left, Complex right)
        {
            return new Complex(left * right.Real, left * right.Imag);
        }

        public static Complex operator *(Complex left, float right)
        {
            return new Complex(right * left.Real, right * left.Imag);
        }

        public static Complex operator /(Complex left, float right)
        {
            return new Complex(left.Real / right, left.Imag / right);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.Real, this.Imag);
        }
    }
}
