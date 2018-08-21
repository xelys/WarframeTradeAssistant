namespace WarframeTradeAssistant.Image
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    internal sealed class TextLinesFinder : IDisposable
    {
        private readonly Rectangle textArea;

        private Bitmap chatImage;

        private BitmapData data;

        private TextLinesFinder(Bitmap chatImage, int threshold = 5)
        {
            this.chatImage = chatImage;
            this.data = chatImage.LockBits(new Rectangle(0, 0, chatImage.Width, chatImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            if (ChatFinder.IsValidChatImage(this.data, threshold))
            {
                this.textArea = ChatFinder.GetChatTextAreaRect(this.data, threshold);
            }
        }

        public static List<Rectangle> GetTextLineRects(Bitmap chatImage, int threshold = 5)
        {
            using (var finder = new TextLinesFinder(chatImage, threshold))
            {
                var lines = finder.GetTextLines();
                if (lines.Count > 0)
                {
                    lines.RemoveAt(0);
                }

                return lines;
            }
        }

        public void Dispose()
        {
            if (this.chatImage != null && this.data != null)
            {
                this.chatImage.UnlockBits(this.data);
            }

            this.chatImage = null;
            this.data = null;
        }

        private List<Rectangle> GetTextLines()
        {
            int lineTop = 0;
            int lineBottom = 0;
            var textLines = new List<Rectangle>();
            bool onLine = false;
            int emptyLinesCount = 0;
            for (int y = this.textArea.Top; y < this.textArea.Bottom; y++)
            {
                var isTextLine = this.PixelLineContainsText(y);

                if (!onLine && isTextLine)
                {
                    lineTop = y;
                    onLine = true;
                    emptyLinesCount = 0;
                    continue;
                }

                if (!isTextLine)
                {
                    emptyLinesCount++;
                }
                else
                {
                    emptyLinesCount = 0;
                }

                if (onLine && !isTextLine && emptyLinesCount > 3)
                {
                    lineBottom = y - emptyLinesCount;
                    onLine = false;
                    var textLineHeight = lineBottom - lineTop + 2;
                    if (textLineHeight >= 7)
                    {
                        textLines.Add(new Rectangle(this.textArea.Left, lineTop - 1, this.textArea.Width, textLineHeight));
                    }
                }
            }

            return textLines;
        }

        private bool IsTextPixel(int x, int y)
        {
            Utils.GetPixel(this.data, x, y, out byte r, out byte g, out byte b);
            var value = Math.Max(r, Math.Max(g, b)) / 255f;
            return value > 0.5;
        }

        private bool PixelLineContainsText(int y)
        {
            for (int x = this.textArea.Left; x < this.textArea.Right; x++)
            {
                if (this.IsTextPixel(x, y))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
