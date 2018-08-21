namespace WarframeTradeAssistant.Image
{
    using System.Drawing;
    using System.Drawing.Imaging;

    public struct ChatMessage
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public bool IsWarning { get; set; }
    }

    public class ChatLineInfo
    {
        public ChatLineInfo(Bitmap chatImage, Rectangle lineRect)
        {
            this.Name = null;
            this.Text = null;
            this.IsFirstLineOfMessage = false;
            this.IsWhisper = false;
            this.NameWidth = 0;
            this.LineRect = lineRect;
            var data = chatImage.LockBits(lineRect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    Utils.GetPixel(data, x, y, out byte r, out byte g, out byte b);
                    var isNameColor = Utils.IsNameColor(r, g, b, out bool isWhisper);
                    if (isNameColor && x > this.NameWidth)
                    {
                        this.NameWidth = x;
                    }

                    if (isNameColor)
                    {
                        this.IsFirstLineOfMessage = true;
                    }

                    if (isWhisper)
                    {
                        this.IsWhisper = true;
                    }
                }
            }

            chatImage.UnlockBits(data);
        }

        public int Height { get => this.LineRect.Height; }

        public bool IsFirstLineOfMessage { get; private set; }

        public bool IsWhisper { get; private set; }

        public Rectangle LineRect { get; private set; }

        public string Name { get; set; }

        public int NameWidth { get; private set; }

        public string Text { get; set; }

        public int Width { get => this.LineRect.Width; }
    }
}
