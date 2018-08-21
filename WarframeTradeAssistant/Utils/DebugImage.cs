namespace WarframeTradeAssistant.Utils
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    internal static class DebugImage
    {
        public static int Index { get; private set; } = 0;

        public static void Next()
        {
            Index++;
        }

        public static void Save(Bitmap image, string prefix = "", Rectangle[] rects = null)
        {
            if (image == null)
            {
                return;
            }

            if (!Directory.Exists(@".\debug_output"))
            {
                Directory.CreateDirectory(@".\debug_output");
            }

            var fileName = @".\debug_output\" + prefix + Index + ".png";
            if (rects != null)
            {
                using (var output = new Bitmap(image))
                using (var g = Graphics.FromImage(output))
                {
                    g.DrawRectangles(Pens.Green, rects);
                    output.Save(fileName, ImageFormat.Png);
                }
            }
            else
            {
                image.Save(fileName, ImageFormat.Png);
            }
        }
    }
}
