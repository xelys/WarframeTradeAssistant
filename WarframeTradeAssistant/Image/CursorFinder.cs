﻿namespace WarframeTradeAssistant.Image
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    internal static class CursorFinder
    {
        private const int CursorWidth = 19;
        private const int CursorHeight = 28;
        private static readonly byte[] Cursor = new byte[]
        {
            249, 245, 8, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            249, 253, 249, 13, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            247, 254, 249, 249, 26, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            245, 254, 248, 247, 247, 40, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            241, 253, 253, 244, 244, 244, 51, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            236, 253, 253, 242, 240, 240, 240, 195, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            232, 252, 252, 250, 237, 237, 237, 237, 209, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            229, 252, 251, 252, 233, 233, 233, 233, 233, 219, 3, 2, 2, 2, 2, 2, 2, 2, 2,
            223, 251, 250, 251, 219, 229, 229, 229, 229, 229, 223, 5, 2, 2, 2, 2, 2, 2, 2,
            219, 251, 250, 249, 248, 226, 225, 225, 225, 225, 225, 223, 9, 2, 2, 2, 2, 2, 2,
            214, 250, 249, 249, 251, 184, 222, 221, 221, 221, 221, 221, 221, 15, 2, 2, 2, 2, 2,
            210, 250, 249, 248, 248, 232, 223, 217, 217, 217, 217, 217, 217, 217, 25, 2, 2, 2, 2,
            206, 249, 248, 248, 247, 250, 161, 220, 212, 212, 212, 212, 212, 212, 212, 37, 2, 2, 2,
            199, 249, 248, 247, 246, 247, 202, 213, 208, 208, 208, 208, 208, 208, 208, 208, 49, 2, 2,
            193, 248, 247, 246, 245, 245, 240, 153, 223, 204, 204, 204, 204, 204, 204, 204, 204, 58, 2,
            188, 247, 246, 246, 245, 244, 246, 172, 198, 202, 199, 199, 199, 199, 199, 199, 199, 199, 196,
            182, 247, 246, 245, 244, 243, 244, 217, 151, 224, 196, 195, 195, 195, 195, 195, 195, 195, 195,
            176, 246, 245, 244, 243, 243, 243, 229, 153, 181, 221, 193, 191, 191, 191, 191, 191, 191, 190,
            170, 246, 244, 243, 243, 242, 243, 228, 180, 150, 172, 215, 189, 187, 187, 187, 187, 187, 174,
            165, 245, 244, 243, 242, 241, 242, 228, 171, 160, 157, 154, 208, 186, 183, 183, 183, 183, 19,
            162, 244, 243, 242, 241, 241, 241, 229, 163, 161, 173, 150, 176, 203, 183, 179, 179, 179, 5,
            160, 244, 243, 242, 241, 240, 240, 231, 164, 150, 163, 150, 205, 182, 196, 180, 175, 174, 2,
            155, 244, 242, 241, 240, 240, 239, 234, 167, 208, 151, 150, 203, 203, 185, 190, 178, 160, 2,
            150, 244, 241, 240, 240, 239, 238, 237, 168, 223, 219, 157, 202, 201, 201, 188, 183, 12, 2,
            140, 240, 242, 240, 239, 238, 238, 239, 170, 222, 222, 222, 200, 200, 200, 200, 186, 2, 2,
            1, 0, 46, 236, 242, 238, 237, 239, 170, 220, 221, 221, 213, 198, 198, 194, 6, 4, 2,
            2, 1, 1, 0, 35, 229, 241, 241, 170, 217, 220, 220, 220, 197, 187, 2, 1, 2, 2,
            2, 2, 2, 1, 1, 0, 25, 223, 170, 213, 220, 217, 208, 174, 1, 2, 2, 2, 2
        };

        public static Point GetCursorLocation(BitmapData imageData)
        {
            for (int y = 0; y < imageData.Height - CursorHeight; y++)
            {
                for (int x = 0; x < imageData.Width - CursorWidth; x++)
                {
                    Utils.GetPixel(imageData, x, y, out byte r, out byte g, out byte b);
                    if (Math.Abs(r - 250) < 2 && Math.Abs(g - 250) < 2 && Math.Abs(b - 250) < 2 && Compare(imageData, x - 1, y - 1))
                    {
                        return new Point(x, y);
                    }
                }
            }

            return Point.Empty;
        }

        private static bool Compare(BitmapData imageData, int x0, int y0)
        {
            if (x0 < 0 || y0 < 0 || x0 + CursorWidth >= imageData.Width || y0 + CursorHeight >= imageData.Height)
            {
                return false;
            }

            var diffSum = 0d;
            var threshold = CursorWidth * CursorHeight * 0.05d;
            for (int y = 0; y < CursorHeight; y++)
            {
                for (int x = 0; x < CursorWidth; x++)
                {
                    var value = Utils.GetPixelValue(imageData, x0 + x, y0 + y);
                    bool opaquePixel = (Cursor[(y * 19) + x] & 128) != 0;
                    float maskValue = (Cursor[(y * 19) + x] & 127) * 2 / 255f;
                    var diff = opaquePixel ? Math.Abs(maskValue - value) : 0;
                    diffSum += diff;
                    if (diffSum > threshold)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
