namespace WarframeTradeAssistant.Image
{
    using System.Drawing;
    using System.Linq;

    internal class ChatComparer
    {
        private readonly bool onlyNewMessages;

        private Bitmap previousFrame;

        private Rectangle previousFrameLastLine;

        public ChatComparer(bool onlyNewMessages)
        {
            this.onlyNewMessages = onlyNewMessages;
        }

        public ChatLineInfo[] NewLines { get; private set; }

        public void UpdateFrame(Bitmap chatImage)
        {
            var currentFrameLines = TextLinesFinder.GetTextLineRects(chatImage);
            if (this.previousFrame != null)
            {
                if (Utils.AreSimilar(this.previousFrame, chatImage))
                {
                    this.NewLines = new ChatLineInfo[0];
                }
                else
                {
                    var previousLastLinePositionInNewFrame = this.previousFrameLastLine != Rectangle.Empty ? Utils.FindSimilar(this.previousFrame, this.previousFrameLastLine, chatImage) : -1;
                    if (previousLastLinePositionInNewFrame == -1)
                    {
                        this.NewLines = currentFrameLines.Select(r => new ChatLineInfo(chatImage, r)).ToArray();
                    }
                    else
                    {
                        var previousFrameBottom = previousLastLinePositionInNewFrame + this.previousFrameLastLine.Height;
                        this.NewLines = currentFrameLines.Where(l => l.Y > previousFrameBottom).Select(r => new ChatLineInfo(chatImage, r)).ToArray();
                    }
                }
            }
            else
            {
                this.NewLines = this.onlyNewMessages ? new ChatLineInfo[0] : currentFrameLines.Select(r => new ChatLineInfo(chatImage, r)).ToArray();
            }

            if (this.previousFrame != null)
            {
                this.previousFrame.Dispose();
            }

            this.previousFrame = chatImage;
            if (currentFrameLines.Count > 0)
            {
                this.previousFrameLastLine = currentFrameLines[currentFrameLines.Count - 1];
            }
            else
            {
                this.previousFrameLastLine = Rectangle.Empty;
            }
        }
    }
}
