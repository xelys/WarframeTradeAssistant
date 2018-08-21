namespace WarframeTradeAssistant.Image
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using WarframeTradeAssistant.Utils;

    internal sealed class ChatFinder : IDisposable
    {
        private static readonly Size[] DirectionalOffsets;

        private readonly Rectangle cursorRect = Rectangle.Empty;

        private readonly int threshold;

        private BitmapData data;

        private Bitmap image;

        static ChatFinder()
        {
            DirectionalOffsets = new Size[4];
            DirectionalOffsets[(int)Direction.Left] = new Size(-1, 0);
            DirectionalOffsets[(int)Direction.Up] = new Size(0, -1);
            DirectionalOffsets[(int)Direction.Right] = new Size(1, 0);
            DirectionalOffsets[(int)Direction.Down] = new Size(0, 1);
        }

        private ChatFinder(Bitmap image, int threshold = 5)
        {
            this.image = image;
            this.data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            this.threshold = threshold;

            var p = CursorFinder.GetCursorLocation(this.data);
            if (p != Point.Empty)
            {
                this.cursorRect = new Rectangle(p.X - 10, p.Y - 10, 19 + 20, 28 + 20);
            }
        }

        private ChatFinder(BitmapData data, int threshold = 5)
        {
            this.data = data;
            this.threshold = threshold;
        }

        public static Rectangle GetChatTextAreaRect(Bitmap chatImage, int threshold = 5)
        {
            using (var locator = new ChatFinder(chatImage, threshold))
            {
                return locator.GetChatTextAreaRect();
            }
        }

        public static Rectangle GetChatTextAreaRect(BitmapData chatImageData, int threshold = 5)
        {
            using (var locator = new ChatFinder(chatImageData, threshold))
            {
                return locator.GetChatTextAreaRect();
            }
        }

        public static Rectangle GetChatWindowRect(Bitmap windowImage, int threshold = 5)
        {
            using (var locator = new ChatFinder(windowImage, threshold))
            {
                return locator.GetChatWindowRect();
            }
        }

        public static bool IsValidChatImage(Bitmap chatImage, int threshold = 5)
        {
            using (var locator = new ChatFinder(chatImage, threshold))
            {
                return locator.IsValidChatImage();
            }
        }

        public static bool IsValidChatImage(BitmapData chatImageData, int threshold = 5)
        {
            using (var locator = new ChatFinder(chatImageData, threshold))
            {
                return locator.IsValidChatImage();
            }
        }

        public void Dispose()
        {
            if (this.image != null && this.data != null)
            {
                this.image.UnlockBits(this.data);
            }

            this.image = null;
            this.data = null;
        }

        private Point FindBorderPoint(Point startingPoint, Direction direction)
        {
            return this.FindBorderPoint(startingPoint, DirectionalOffsets[(int)direction]);
        }

        private Point FindBorderPoint(Point startingPoint, Size offset)
        {
            var p = startingPoint;
            while (p.X > 0 && p.X < this.data.Width && p.Y > 0 && p.Y < this.data.Height)
            {
                if (this.cursorRect.Contains(p))
                {
                    return Point.Empty;
                }

                var v0 = Utils.GetPixelValue(this.data, p.X, p.Y);
                var v = Utils.GetPixelValue(this.data, p + offset);
                if (Math.Abs(v - v0) > 0.2)
                {
                    break;
                }

                p += offset;
            }

            return p;
        }

        private Line FindBottomBorder(Point topLeft, int windowWidth)
        {
            const int CornerMargin = Constants.ChatCornerMargin;
            var y = topLeft.Y;
            while (y < this.data.Height)
            {
                if (this.cursorRect.IntersectsWith(new Rectangle(topLeft.X + CornerMargin, y, windowWidth - (2 * CornerMargin), 1)))
                {
                    return new Line(Point.Empty, Point.Empty);
                }

                if (Utils.IsHorizontalLine(this.data, topLeft.X + CornerMargin, topLeft.X + windowWidth - CornerMargin, y))
                {
                    break;
                }

                y++;
            }

            return new Line(new Point(topLeft.X, y), new Point(topLeft.X + windowWidth, y));
        }

        private IEnumerable<Point> FindColorPixels(Color color)
        {
            for (int y = 0; y < this.data.Height; y++)
            {
                for (int x = 0; x < this.data.Width; x++)
                {
                    Utils.GetPixel(this.data, x, y, out byte r, out byte g, out byte b);
                    if (Math.Abs(r - color.R) < this.threshold && Math.Abs(g - color.G) < this.threshold && Math.Abs(b - color.B) < this.threshold)
                    {
                        yield return new Point(x, y);
                    }
                }
            }
        }

        private Rectangle FindSolidColorRect(Color color, int minWidth, int minHeight)
        {
            foreach (var p in this.FindColorPixels(color))
            {
                var rect = this.GetSolidColorRectSize(p.X, p.Y);
                if (rect.Width >= minWidth && rect.Height >= minHeight)
                {
                    return rect;
                }
            }

            return Rectangle.Empty;
        }

        private Rectangle GetChatTextAreaRect()
        {
            const int Padding = Constants.ChatWindowPadding;
            var scrollBarWidth = this.GetScrollBarWidth();
            return new Rectangle(Padding, Padding, this.data.Width - (Padding * 2) - scrollBarWidth, this.data.Height - (Padding * 2));
        }

        private Rectangle GetChatWindowRect()
        {
            const int Padding = Constants.ChatWindowPadding;

            var tabHighlight = this.FindSolidColorRect(Color.FromArgb(126, 161, 227), 30, 2);
            if (tabHighlight == Rectangle.Empty)
            {
                return Rectangle.Empty;
            }

            var startingPoint = new Point(tabHighlight.Left, tabHighlight.Bottom + tabHighlight.Height - 1);
            var leftBorderPoint = this.FindBorderPoint(startingPoint, Direction.Left);
            if (leftBorderPoint == Point.Empty)
            {
                return Rectangle.Empty;
            }

            var bottomLeftCorner = this.FindBorderPoint(leftBorderPoint, Direction.Down);
            if (bottomLeftCorner == Point.Empty)
            {
                return Rectangle.Empty;
            }

            var chatTopLeftCorner = this.FindBorderPoint(bottomLeftCorner + new Size(0, 1), Direction.Down) + new Size(0, 1);
            if (chatTopLeftCorner == Point.Empty)
            {
                return Rectangle.Empty;
            }

            var chatTopRightCorner = this.FindBorderPoint(chatTopLeftCorner, Direction.Right);
            if (chatTopRightCorner == Point.Empty)
            {
                return Rectangle.Empty;
            }

            var windowWidth = chatTopRightCorner.X - chatTopLeftCorner.X;
            var bottomBorder = this.FindBottomBorder(chatTopLeftCorner, windowWidth);
            if (bottomBorder.Start == Point.Empty)
            {
                return Rectangle.Empty;
            }

            var width = chatTopRightCorner.X - chatTopLeftCorner.X;
            var height = bottomBorder.Start.Y - chatTopLeftCorner.Y;
            return new Rectangle(chatTopLeftCorner.X - Padding, chatTopLeftCorner.Y - Padding, width + (Padding * 2), height + (Padding * 2));
        }

        private int GetScrollBarWidth()
        {
            const int Padding = Constants.ChatWindowPadding;
            if (!this.IsScrollBarVisible())
            {
                return 0;
            }

            var maxWidth = 0;
            for (int y = Padding; y < this.data.Height - Padding; y++)
            {
                var counter = 0;
                for (int x = this.data.Width - Padding; x > this.data.Width - Padding - 20; x--)
                {
                    Utils.GetPixel(this.data, x, y, out byte r, out byte g, out byte b);
                    if ((Math.Abs(r - 255) < this.threshold && Math.Abs(g - 255) < this.threshold && Math.Abs(b - 255) < this.threshold) ||
                        (Math.Abs(r - 144) < this.threshold && Math.Abs(g - 144) < this.threshold && Math.Abs(b - 144) < this.threshold))
                    {
                        counter++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (counter > maxWidth)
                {
                    maxWidth = counter;
                }
            }

            return maxWidth;
        }

        private Rectangle GetSolidColorRectSize(int x0, int y0)
        {
            Utils.GetPixel(this.data, x0, y0, out byte r0, out byte g0, out byte b0);
            int x = x0;
            int y = y0;
            int maxX = this.data.Width;
            while (y < this.data.Height)
            {
                if (!Utils.IsPixelColor(this.data, x, y, r0, g0, b0))
                {
                    return new Rectangle(x0, y0, maxX - x0, y - y0);
                }

                while (x < maxX)
                {
                    if (!Utils.IsPixelColor(this.data, x, y, r0, g0, b0))
                    {
                        maxX = x;
                        break;
                    }

                    x++;
                }

                x = x0;
                y++;
            }

            return new Rectangle(x0, y0, 1, 1);
        }

        private bool IsScrollBarVisible()
        {
            const int Padding = Constants.ChatWindowPadding;
            var x = this.data.Width - Padding - 5;
            var counter = 0;
            for (int y = Padding; y < this.data.Height - Padding; y++)
            {
                Utils.GetPixel(this.data, x, y, out byte r, out byte g, out byte b);
                if (Math.Abs(r - 255) < this.threshold && Math.Abs(g - 255) < this.threshold && Math.Abs(b - 255) < this.threshold)
                {
                    counter++;
                }

                if (counter > 20)
                {
                    return true;
                }
            }

            return counter > 20;
        }

        private bool IsValidChatImage()
        {
            const int Padding = Constants.ChatWindowPadding;
            const int CornerMargin = Constants.ChatCornerMargin;
            if (this.cursorRect != Rectangle.Empty)
            {
                return false;
            }

            if (!Utils.IsCornerPixel(this.data, new Point(Padding, Padding), Corner.TopLeft))
            {
                return false;
            }

            if (!Utils.IsCornerPixel(this.data, new Point(this.data.Width - Padding, Padding), Corner.TopRight))
            {
                return false;
            }

            if (!Utils.IsHorizontalLine(this.data, Padding + CornerMargin, this.data.Width - Padding - CornerMargin, this.data.Height - Padding, true))
            {
                return false;
            }

            return true;
        }

        private struct Line
        {
            public Line(Point start, Point end) : this()
            {
                this.Start = start;
                this.End = end;
            }

            public Point Start { get; private set; }

            public Point End { get; private set; }
        }
    }
}
