namespace WarframeTradeAssistant.Image
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Tesseract;

#pragma warning disable S101 // Types should be named in camel case
    internal sealed class OCRWorker
    {
#pragma warning restore S101 // Types should be named in camel case
        private const int UpscalingTargetHeight = 67;

        private const int UpscalingTargetWidth = 2043;

        private readonly ChatComparer chatComparer;

        private readonly bool onlyWhispers;

        public OCRWorker(bool onlyNew, bool onlyWhispers)
        {
            this.onlyWhispers = onlyWhispers;
            this.chatComparer = new ChatComparer(onlyNew);
        }

        public List<ChatMessage> ExtractNewMessages(Bitmap chatImage)
        {
            this.chatComparer.UpdateFrame(chatImage);
            var newLines = this.chatComparer.NewLines;
            UnsharpMask unsharpMask = null;
            TesseractEngine engine = null;
            if (newLines.Length > 0)
            {
                unsharpMask = new UnsharpMask(UpscalingTargetWidth, UpscalingTargetHeight);
                engine = new TesseractEngine(@"./", "eng", EngineMode.TesseractAndCube);
            }

            List<ChatMessage> messages = new List<ChatMessage>();
            var messageLines = new List<ChatLineInfo>();
            foreach (var line in newLines)
            {
                if (line.IsFirstLineOfMessage)
                {
                    this.ConcatenateLinesIntoMessages(unsharpMask, engine, messages, chatImage, messageLines);
                }

                messageLines.Add(line);
            }

            this.ConcatenateLinesIntoMessages(unsharpMask, engine, messages, chatImage, messageLines);
            if (engine != null)
            {
                engine.Dispose();
            }

            return messages;
        }

        public void UpdateFrameAndIgnoreMessages(Bitmap chatImage)
        {
            this.chatComparer.UpdateFrame(chatImage);
        }

        private Bitmap ClearColon(Bitmap source, int nameWidth)
        {
            if (nameWidth == 0)
            {
                return source;
            }

            var rect = new Rectangle(0, 0, source.Width, source.Height);
            Bitmap output = new Bitmap(source);
            var data = source.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            var outputData = output.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = nameWidth; x < nameWidth + 8; x++)
                {
                    Utils.GetPixel(data, x, y, out byte r, out byte g, out byte b);
                    var color = Color.FromArgb(r, g, b);
                    var s = color.GetSaturation();
                    var br = color.GetBrightness();
                    if (s < 0.2f && br > 0.3)
                    {
                        Utils.SetPixel(outputData, x, y, 0, 0, 0);
                    }
                }
            }

            source.UnlockBits(data);
            output.UnlockBits(outputData);
            return output;
        }

        private void ConcatenateLinesIntoMessages(UnsharpMask unsharpMask, TesseractEngine engine, List<ChatMessage> messages, Bitmap chatImage, List<ChatLineInfo> messageLines)
        {
            if (messageLines.Count == 0 || (this.onlyWhispers && !messageLines[0].IsWhisper))
            {
                messageLines.Clear();
                return;
            }

            foreach (var line in messageLines)
            {
                this.RunOCR(unsharpMask, engine, chatImage, line);
            }

            messages.Add(new ChatMessage { Name = messageLines[0].Name, Text = string.Join(" ", messageLines.Select(l => l.Text)) });
            messageLines.Clear();
        }

        private void PreprocessImage(UnsharpMask unsharpMask, Bitmap source, Rectangle sourceRect, int nameWidth, MemoryStream outputStream)
        {
            using (var croppedLineImage = Utils.CropImage(source, sourceRect))
            using (var cleanedUpImage = this.ClearColon(croppedLineImage, nameWidth))
            using (var upscaledChatImage = Utils.ResizeImage(cleanedUpImage, UpscalingTargetWidth, UpscalingTargetHeight))
            using (var sharpenedImage = unsharpMask.Perform(upscaledChatImage, 0.3f))
            {
                sharpenedImage.Save(outputStream, System.Drawing.Imaging.ImageFormat.Tiff);
            }
        }

        private void RunOCR(UnsharpMask unsharpMask, TesseractEngine engine, Bitmap chatImage, ChatLineInfo line)
        {
            var resizedNameWidth = ((double)line.NameWidth) / line.LineRect.Width * UpscalingTargetWidth;
            List<string> nameWords = new List<string>();
            List<string> textWords = new List<string>();
            using (var ms = new MemoryStream())
            {
                this.PreprocessImage(unsharpMask, chatImage, line.LineRect, line.NameWidth, ms);
                using (var tesseractImage = Pix.LoadTiffFromMemory(ms.ToArray()))
                using (var page = engine.Process(tesseractImage, PageSegMode.SingleLine))
                {
                    using (var iter = page.GetIterator())
                    {
                        int i = 0;
                        iter.Begin();
                        do
                        {
                            var boundingBoxFound = iter.TryGetBoundingBox(PageIteratorLevel.Word, out Rect bounds);

                            if (boundingBoxFound && bounds.X1 <= resizedNameWidth)
                            {
                                nameWords.Add(iter.GetText(PageIteratorLevel.Word));
                            }
                            else
                            {
                                textWords.Add(iter.GetText(PageIteratorLevel.Word));
                                i++;
                            }
                        }
                        while (iter.Next(PageIteratorLevel.Word));
                    }
                }
            }

            line.Name = string.Join(string.Empty, nameWords);
            line.Text = string.Join(" ", textWords);
        }
    }
}
