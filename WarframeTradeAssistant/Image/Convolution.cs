namespace WarframeTradeAssistant.Image
{
    using System;
    using System.Threading.Tasks;

    internal class Convolution
    {
        private const float PI = (float)Math.PI;

        private readonly int cellSizeX, cellSizeY, kernelSize, order;

        private readonly Complex[,] complexImage, complexKernel;

        private readonly Complex[][] exponents;

        public Convolution(int width, int height, float[,] kernel)
        {
            this.kernelSize = kernel.GetLength(0);
            this.cellSizeX = NextPowerOfTwo(width + this.kernelSize);
            this.cellSizeY = NextPowerOfTwo(height + this.kernelSize);
            this.order = Log2(Math.Max(this.cellSizeX, this.cellSizeY));
            this.exponents = new Complex[this.order][];
            this.complexKernel = new Complex[this.cellSizeX, this.cellSizeY];
            this.PrecomputeExponents();
            this.PrecomputeKernel(kernel);
            this.complexImage = new Complex[this.cellSizeX, this.cellSizeY];
        }

        public void FFTConvolution2D(float[,] image)
        {
            for (int x = 0; x < this.cellSizeX; x++)
            {
                for (int y = 0; y < this.cellSizeY; y++)
                {
                    if (x < image.GetLength(0) && y < image.GetLength(1))
                    {
                        this.complexImage[x + (this.kernelSize / 2), y + (this.kernelSize / 2)] = new Complex(image[x, y], 0);
                    }
                }
            }

            this.FFT2D(this.complexImage, false);

            for (int x = 0; x < this.cellSizeX; x++)
            {
                for (int y = 0; y < this.cellSizeY; y++)
                {
                    this.complexImage[x, y] = this.complexImage[x, y] * this.complexKernel[x, y];
                }
            }

            this.FFT2D(this.complexImage, true);

            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    image[x, y] = this.complexImage[x, y].Real;
                }
            }
        }

        private static int Log2(int N)
        {
            switch (N)
            {
                case 8192: return 13;
                case 4096: return 12;
                case 2048: return 11;
                case 1024: return 10;
                case 512: return 9;
                case 256: return 8;
                case 128: return 7;
                case 64: return 6;
                case 32: return 5;
                case 16: return 4;
                case 8: return 3;
                case 4: return 2;
                case 2: return 1;
                default: return 0;
            }
        }

        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        private static int NextPowerOfTwo(int n)
        {
            return (int)Math.Pow(2, Math.Ceiling(Math.Log(n) / (float)Math.Log(2)));
        }

        private Complex[] FFT(Complex[] x, bool inverse)
        {
            var X = new Complex[x.Length];
            this.FFT(x, X, x.Length, 1, 0, 0, inverse);
            if (inverse)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    X[i] = X[i] / x.Length;
                }
            }

            return X;
        }

        private void FFT(Complex[] x, Complex[] X, int N, int m, int a, int offset, bool inverse)
        {
            if (N == 1)
            {
                X[offset] = x[a];
                return;
            }

            this.FFT(x, X, N / 2, 2 * m, a, offset, inverse);
            this.FFT(x, X, N / 2, 2 * m, a + m, offset + (N / 2), inverse);

            for (int k = 0; k < N / 2; k++)
            {
                var c = this.exponents[Log2(N) - 1][k];
                if (inverse)
                {
                    c = new Complex(c.Real, -c.Imag);
                }

                var e = X[k + offset];
                var d = X[k + offset + (N / 2)];

                var dxcReal = (d.Real * c.Real) - (d.Imag * c.Imag);
                var dxcImag = (d.Real * c.Imag) + (d.Imag * c.Real);
                X[k + offset] = new Complex(e.Real + dxcReal, e.Imag + dxcImag);
                X[k + offset + (N / 2)] = new Complex(e.Real - dxcReal, e.Imag - dxcImag);
            }
        }

        private void FFT2D(Complex[,] x, bool inverse)
        {
            int N1 = x.GetLength(0);
            int N2 = x.GetLength(1);
            Parallel.For(
                0,
                N1,
                (i) =>
            {
                var xi = new Complex[N2];
                for (int j = 0; j < N2; j++)
                {
                    xi[j] = x[i, j];
                }

                var Xi = FFT(xi, inverse);
                for (int j = 0; j < N2; j++)
                {
                    x[i, j] = Xi[j];
                }
            });

            Parallel.For(
                0,
                N2,
                (j) =>
            {
                var xi = new Complex[N1];
                for (int i = 0; i < N1; i++)
                {
                    xi[i] = x[i, j];
                }

                var Xi = FFT(xi, inverse);
                for (int i = 0; i < N1; i++)
                {
                    x[i, j] = Xi[i];
                }
            });
        }

        private void PrecomputeExponents()
        {
            for (int i = this.order; i > 0; i--)
            {
                var N = (int)Math.Pow(2, i);
                this.exponents[i - 1] = new Complex[N / 2];
                for (int k = 0; k < N / 2; k++)
                {
                    this.exponents[i - 1][k] = Complex.Exponent(-2 * PI * k / N);
                }
            }
        }

        private void PrecomputeKernel(float[,] kernel)
        {
            for (int x = 0; x < this.kernelSize; x++)
            {
                for (int y = 0; y < this.kernelSize; y++)
                {
                    this.complexKernel[Mod(this.cellSizeX - x, this.cellSizeX), Mod(this.cellSizeY - y, this.cellSizeY)] = new Complex(kernel[x, y], 0);
                }
            }

            this.FFT2D(this.complexKernel, false);
        }
    }
}
