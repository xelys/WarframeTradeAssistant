namespace WarframeTradeAssistant.Image
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class UnsharpMask
    {
        private readonly Convolution convolution;

        public UnsharpMask(int width, int height, int kernelSize = 5, int sigma = 5)
        {
            this.convolution = new Convolution(width, height, CreateKernel(kernelSize, sigma));
        }

        public static float[,] CreateKernel(int size, float sigma)
        {
            var pi = (float)Math.PI;
            float[,] kernel = new float[size, size];
            int center = size / 2;
            for (int i = 0; i < size; i++)
            {
                int x = i - center;
                for (int j = 0; j < size; j++)
                {
                    int y = j - center;
                    kernel[i, j] = (1.0f / ((2 * pi) * sigma * sigma)) * ((float)Math.Exp(((-x * x) + (-y * y)) / (2 * sigma * sigma)));
                }
            }

            return kernel;
        }

        public Bitmap Perform(Bitmap image, float threshold)
        {
            var values = this.ImageToValuesMatrix(image, threshold);
            var copy = this.Copy(values);
            this.convolution.FFTConvolution2D(values);
            this.SubtractThresholdInvert(copy, values, 1.5f);
            return this.ValuesMatrixToImage(copy);
        }

        private static int Clamp(int x, int min, int max)
        {
            if (x < min)
            {
                return min;
            }

            if (x > max)
            {
                return max;
            }

            return x;
        }

        private float[,] Copy(float[,] array)
        {
            var copy = new float[array.GetLength(0), array.GetLength(1)];
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    copy[x, y] = array[x, y];
                }
            }

            return copy;
        }

        private unsafe float[,] ImageToValuesMatrix(Bitmap image, float threshold)
        {
            float[,] output = new float[image.Width, image.Height];
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;
            byte* scan0 = (byte*)imageData.Scan0.ToPointer();
            int stride = imageData.Stride;
            for (int y = 0; y < imageData.Height; y++)
            {
                byte* row = scan0 + (y * stride);
                for (int x = 0; x < imageData.Width; x++)
                {
                    int blueIndex = x * bytesPerPixel;
                    int greenIndex = blueIndex + 1;
                    int redIndex = blueIndex + 2;

                    byte pixelR = row[redIndex];
                    byte pixelG = row[greenIndex];
                    byte pixelB = row[blueIndex];

                    var br = Color.FromArgb(pixelR, pixelG, pixelB).GetBrightness();
                    output[x, y] = br > threshold ? br : 0f;
                }
            }

            image.UnlockBits(imageData);
            return output;
        }

        private void SubtractThresholdInvert(float[,] source, float[,] blurred, float factor = 1.0f)
        {
            for (int y = 0; y < source.GetLength(1); y++)
            {
                for (int x = 0; x < source.GetLength(0); x++)
                {
                    var v = source[x, y] + ((source[x, y] - blurred[x, y]) * factor);
                    source[x, y] = v > 0.5 ? 0 : 1;
                }
            }
        }

        private unsafe Bitmap ValuesMatrixToImage(float[,] values)
        {
            Bitmap output = new Bitmap(values.GetLength(0), values.GetLength(1), PixelFormat.Format24bppRgb);
            BitmapData imageData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;
            byte* scan0 = (byte*)imageData.Scan0.ToPointer();
            int stride = imageData.Stride;
            for (int y = 0; y < imageData.Height; y++)
            {
                byte* row = scan0 + (y * stride);

                for (int x = 0; x < imageData.Width; x++)
                {
                    int blueIndex = x * bytesPerPixel;
                    int greenIndex = blueIndex + 1;
                    int redIndex = blueIndex + 2;

                    var value = (int)Math.Round(values[x, y] * 255);
                    value = Clamp(value, 0, 255);
                    var color = (byte)value;
                    row[redIndex] = color;
                    row[greenIndex] = color;
                    row[blueIndex] = color;
                }
            }

            output.UnlockBits(imageData);
            return output;
        }
    }
}
