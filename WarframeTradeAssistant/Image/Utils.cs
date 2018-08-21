namespace WarframeTradeAssistant.Image
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using WarframeTradeAssistant.Utils;

    internal enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    internal enum Corner
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }

    internal static class Utils
    {
        private static readonly int[] CornerMasks = new[] { 0b111100100, 0b111001001, 0b001001111, 0b100100111 };

        public static bool AreSimilar(Bitmap image1, Bitmap image2)
        {
            const int Padding = Constants.ChatWindowPadding;
            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                return false;
            }

            var diff = 0d;
            var area = image1.Width * image1.Height;
            BitmapData data1 = image1.LockBits(new Rectangle(Padding, Padding, image1.Width - (Padding * 2), image1.Height - (Padding * 2)), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData data2 = image2.LockBits(new Rectangle(Padding, Padding, image2.Width - (Padding * 2), image2.Height - (Padding * 2)), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            for (int y = 0; y < data1.Height; y++)
            {
                for (int x = 0; x < data1.Width; x++)
                {
                    var v1 = GetPixelValue(data1, x, y);
                    var v2 = GetPixelValue(data2, x, y);
                    v1 = v1 > 0.5 ? v1 : 0;
                    v2 = v2 > 0.5 ? v2 : 0;
                    diff += Math.Abs(v1 - v2);
                    if (diff > area * 0.001)
                    {
                        image1.UnlockBits(data1);
                        image2.UnlockBits(data2);
                        return false;
                    }
                }
            }

            image1.UnlockBits(data1);
            image2.UnlockBits(data2);
            return true;
        }

        public static bool AreSimilar(BitmapData template, BitmapData reference, Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.X + template.Width >= reference.Width || location.Y + template.Height >= reference.Height)
            {
                return false;
            }

            var diff = 0d;
            var area = template.Width * template.Height;
            for (int y = 0; y < template.Height; y++)
            {
                for (int x = 0; x < template.Width; x++)
                {
                    var v1 = GetPixelValue(template, x, y);
                    var v2 = GetPixelValue(reference, x + location.X, y + location.Y);
                    v1 = v1 > 0.5 ? v1 : 0;
                    v2 = v2 > 0.5 ? v2 : 0;
                    diff += Math.Abs(v1 - v2);
                    if (diff > area * 0.02)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static Bitmap CropImage(Bitmap image, Rectangle cropRect)
        {
            var destImage = new Bitmap(cropRect.Width, cropRect.Height);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.DrawImage(image, 0, 0, cropRect, GraphicsUnit.Pixel);
            }

            return destImage;
        }

        public static int FindSimilar(Bitmap templateSource, Rectangle sourceRect, Bitmap reference)
        {
            BitmapData templateData = templateSource.LockBits(sourceRect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData referenceData = reference.LockBits(new Rectangle(0, 0, reference.Width, reference.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            var height = -1;
            for (int i = referenceData.Height - templateData.Height; i >= 0; i--)
            {
                if (AreSimilar(templateData, referenceData, new Point(Constants.ChatWindowPadding, i)))
                {
                    height = i;
                    break;
                }
            }

            templateSource.UnlockBits(templateData);
            reference.UnlockBits(referenceData);
            return height;
        }

        public static unsafe void GetPixel(BitmapData data, int x, int y, out byte r, out byte g, out byte b)
        {
            byte* scan0 = (byte*)data.Scan0.ToPointer();
            int stride = data.Stride;
            byte* row = scan0 + (y * stride);
            int i = x * 3;
            r = row[i + 2];
            g = row[i + 1];
            b = row[i];
        }

        public static float GetPixelValue(BitmapData data, int x, int y)
        {
            if (x < 0 || x >= data.Width || y < 0 || y >= data.Height)
            {
                return 1;
            }

            GetPixel(data, x, y, out byte r, out byte g, out byte b);
            return Math.Max(r, Math.Max(g, b)) / 255f;
        }

        public static float GetPixelValue(BitmapData data, Point p)
        {
            return GetPixelValue(data, p.X, p.Y);
        }

        public static unsafe void GetPixelWithAlpha(BitmapData data, int x, int y, out byte a, out byte r, out byte g, out byte b)
        {
            byte* scan0 = (byte*)data.Scan0.ToPointer();
            int stride = data.Stride;
            byte* row = scan0 + (y * stride);
            int i = x * 4;
            a = row[i + 3];
            r = row[i + 2];
            g = row[i + 1];
            b = row[i];
        }

        public static bool IsCornerPixel(BitmapData data, Point point, Corner corner)
        {
            var threshold = 0.4f;
            var diff = 0.2f;
            var p = GetPixelValue(data, point.X, point.Y);
            var mask = CornerMasks[(int)corner];
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    var value = GetPixelValue(data, point.X + x, point.Y + y);
                    var i = ((y + 1) * 3) + (x + 1);
                    var lightPixel = ((1 << (8 - i)) & mask) > 0;
                    if (lightPixel && value - p < diff)
                    {
                        return false;
                    }

                    if (!lightPixel && (value > threshold || Math.Abs(value - p) > diff))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsHorizontalLine(BitmapData data, int aX, int bX, int y, bool fastTest = false)
        {
            if (y >= data.Height - 1)
            {
                return true;
            }

            int step = fastTest ? 10 : 1;
            float counter = 0;
            for (int x = aX; x <= bX; x += step)
            {
                var pixelValue = GetPixelValue(data, x, y);
                var bottomNeightborValue = GetPixelValue(data, x, y + 1);
                if (pixelValue < 0.4 && bottomNeightborValue - pixelValue > 0.2)
                {
                    counter++;
                }
            }

            return (counter * step) / (bX - aX) > 0.95;
        }

        public static bool IsNameColor(byte r, byte g, byte b, out bool isWhisper, int threshold = 5)
        {
            isWhisper = false;

            // whisper
            if (Math.Abs(r - 245) < threshold && Math.Abs(g - 133) < threshold && Math.Abs(b - 188) < threshold)
            {
                isWhisper = true;
                return true;
            }

            // clan
            if (Math.Abs(r - 42) < threshold && Math.Abs(g - 170) < threshold && Math.Abs(b - 140) < threshold)
            {
                return true;
            }

            // public
            if (Math.Abs(r - 132) < threshold && Math.Abs(g - 203) < threshold && Math.Abs(b - 206) < threshold)
            {
                return true;
            }

            // squad
            if (Math.Abs(r - 86) < threshold && Math.Abs(g - 157) < threshold && Math.Abs(b - 245) < threshold)
            {
                return true;
            }

            return false;
        }

        public static bool IsPixelColor(BitmapData data, int x, int y, byte r, byte g, byte b, int threshold = 5)
        {
            GetPixel(data, x, y, out byte r0, out byte g0, out byte b0);
            return Math.Abs(r - r0) < threshold && Math.Abs(g - g0) < threshold && Math.Abs(b - b0) < threshold;
        }

        public static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            var destImage = new Bitmap(width, height);
            var destRect = new Rectangle(0, 0, width, (int)Math.Ceiling((float)image.Height / image.Width * width));
            destImage.SetResolution(300, 300);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static unsafe void SetPixel(BitmapData data, int x, int y, byte r, byte g, byte b)
        {
            byte* scan0 = (byte*)data.Scan0.ToPointer();
            int stride = data.Stride;
            byte* row = scan0 + (y * stride);
            int i = x * 3;
            row[i + 2] = r;
            row[i + 1] = g;
            row[i] = b;
        }
    }
}
