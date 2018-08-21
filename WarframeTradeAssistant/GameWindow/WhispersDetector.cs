namespace WarframeTradeAssistant.GameWindow
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading.Tasks;
    using WarframeTradeAssistant.Image;
    using WarframeTradeAssistant.Utils;

    internal sealed class WhispersDetector : IDisposable
    {
        private const int SRCCOPY = 13369376;
        private const int TimerInterval = 1000;
        private readonly OCRWorker ocrWorker;
        private readonly DuplicateMessagesFilter duplicateMessagesFilter;
        private IntPtr handle;
        private IntPtr hdcSrc;
        private IntPtr destinationHandle;
        private IntPtr destinationBitmap;
        private int bitmapWidth, bitmapHeight;
        private Rectangle chatRectangle;
        private bool isValidChat;
        private bool wasInBackground;
        private bool wasWarframeInBackgroundOnLastTick = false;

        public WhispersDetector(bool onlyNew, bool onlyWhispers)
        {
            this.duplicateMessagesFilter = new DuplicateMessagesFilter();
            this.ocrWorker = new OCRWorker(onlyNew, onlyWhispers);
        }

        ~WhispersDetector()
        {
            this.Dispose(false);
        }

        public event EventHandler<NewWhisperEventArgs> NewWhisper;

        public event EventHandler<ChatStateChangedEventArgs> ChatStateChanged;

        public bool IsChatStateValid
        {
            get => !this.wasInBackground || this.isValidChat;
        }

        public void Start()
        {
            this.WatchLoop().Forget();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            this.Cleanup();
        }

        private bool InitScreenObjects()
        {
            IntPtr warframeWindowHandle = IntPtr.Zero;
            foreach (var process in Process.GetProcessesByName("Warframe.x64"))
            {
                warframeWindowHandle = process.MainWindowHandle;
            }

            if (this.handle != warframeWindowHandle)
            {
                this.Cleanup();
            }

            // Warframe not running
            if (warframeWindowHandle == IntPtr.Zero)
            {
                return false;
            }

            // Warframe was restarted
            if (this.handle == IntPtr.Zero)
            {
                this.handle = warframeWindowHandle;
            }

            if (this.hdcSrc == IntPtr.Zero)
            {
                this.hdcSrc = NativeMethods.GetWindowDC(this.handle);
            }

            if (this.destinationHandle == IntPtr.Zero)
            {
                this.destinationHandle = NativeMethods.CreateCompatibleDC(this.hdcSrc);
            }

            return true;
        }

        private bool InitDestinationBitmap()
        {
            if (this.chatRectangle == Rectangle.Empty)
            {
                this.UpdateChatLocation();
            }

            if (this.chatRectangle == Rectangle.Empty)
            {
                if (this.destinationBitmap != IntPtr.Zero)
                {
                    NativeMethods.DeleteObject(this.destinationBitmap);
                    this.destinationBitmap = IntPtr.Zero;
                }

                return false;
            }

            if (this.destinationBitmap != IntPtr.Zero && (this.bitmapWidth != this.chatRectangle.Width || this.bitmapHeight != this.chatRectangle.Height))
            {
                NativeMethods.DeleteObject(this.destinationBitmap);
                this.destinationBitmap = IntPtr.Zero;
            }

            if (this.destinationBitmap == IntPtr.Zero)
            {
                this.bitmapWidth = this.chatRectangle.Width;
                this.bitmapHeight = this.chatRectangle.Height;
                this.destinationBitmap = NativeMethods.CreateCompatibleBitmap(this.hdcSrc, this.bitmapWidth, this.bitmapHeight);
            }

            return true;
        }

        private void Cleanup()
        {
            if (this.destinationHandle != IntPtr.Zero)
            {
                NativeMethods.DeleteDC(this.destinationHandle);
                this.destinationHandle = IntPtr.Zero;
            }

            if (this.hdcSrc != IntPtr.Zero && this.handle != IntPtr.Zero)
            {
                NativeMethods.ReleaseDC(this.handle, this.hdcSrc);
                this.handle = IntPtr.Zero;
                this.hdcSrc = IntPtr.Zero;
            }

            if (this.destinationBitmap != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(this.destinationBitmap);
                this.destinationBitmap = IntPtr.Zero;
            }

            this.handle = IntPtr.Zero;
        }

        private async Task WatchLoop()
        {
            while (true)
            {
                var isWarframeInBackground = NativeMethods.IsWarframeInBackground();
                if (this.InitScreenObjects() && this.InitDestinationBitmap())
                {
                    var oldObject = NativeMethods.SelectObject(this.destinationHandle, this.destinationBitmap);
                    NativeMethods.BitBlt(this.destinationHandle, 0, 0, this.chatRectangle.Width, this.chatRectangle.Height, this.hdcSrc, this.chatRectangle.X, this.chatRectangle.Y, SRCCOPY);
                    NativeMethods.SelectObject(this.destinationHandle, oldObject);
                    var chatImage = System.Drawing.Image.FromHbitmap(this.destinationBitmap);
                    if (!ChatFinder.IsValidChatImage(chatImage))
                    {
                        this.chatRectangle = Rectangle.Empty;
                        await Task.Delay(TimerInterval);
                        continue;
                    }

                    if (!Properties.Settings.Default.ShowNotificationsOnlyOnBackgroundMessages || (isWarframeInBackground && this.wasWarframeInBackgroundOnLastTick))
                    {
                        var newChatMessages = await Task.Run(() => this.ocrWorker.ExtractNewMessages(chatImage));
                        foreach (var message in newChatMessages)
                        {
                            if (this.duplicateMessagesFilter.AppendMessage(message))
                            {
                                this.NewWhisper?.Invoke(this, new NewWhisperEventArgs(message));
                            }
                        }
                    }
                    else
                    {
                        this.ocrWorker.UpdateFrameAndIgnoreMessages(chatImage);
                    }
                }

                this.wasWarframeInBackgroundOnLastTick = isWarframeInBackground;

                await Task.Delay(TimerInterval);
            }
        }

        private void VerifyChatState(bool isValidChat)
        {
            var isInBackground = NativeMethods.IsWarframeInBackground();
            var chatStateChanged = isValidChat != this.isValidChat || isInBackground != this.wasInBackground;
            var isChatStateValid = !isInBackground || isValidChat;
            this.isValidChat = isValidChat;
            this.wasInBackground = isInBackground;
            if (chatStateChanged)
            {
                this.ChatStateChanged?.Invoke(this, new ChatStateChangedEventArgs(isChatStateValid));
            }
        }

        private void UpdateChatLocation()
        {
            using (Bitmap window = this.GetEntireWindow())
            {
                if (window != null)
                {
                    this.chatRectangle = ChatFinder.GetChatWindowRect(window);
                }
                else
                {
                    this.chatRectangle = Rectangle.Empty;
                }
            }

            this.VerifyChatState(this.chatRectangle != Rectangle.Empty);
        }

        private Bitmap GetEntireWindow()
        {
            if (this.handle == IntPtr.Zero || this.hdcSrc == IntPtr.Zero)
            {
                return null;
            }

            var windowRect = new Rect();
            NativeMethods.GetWindowRect(this.handle, ref windowRect);
            var width = windowRect.Right - windowRect.Left;
            var height = windowRect.Bottom - windowRect.Top;
            var hdcDest = NativeMethods.CreateCompatibleDC(this.hdcSrc);
            var bitmapHandle = NativeMethods.CreateCompatibleBitmap(this.hdcSrc, width, height);
            var oldObject = NativeMethods.SelectObject(hdcDest, bitmapHandle);
            NativeMethods.BitBlt(hdcDest, 0, 0, width, height, this.hdcSrc, 0, 0, SRCCOPY);
            Bitmap image = System.Drawing.Image.FromHbitmap(bitmapHandle);
            NativeMethods.SelectObject(hdcDest, oldObject);
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.DeleteObject(bitmapHandle);
            return image;
        }

        internal class ChatStateChangedEventArgs : EventArgs
        {
            public ChatStateChangedEventArgs(bool isValid)
            {
                this.IsValid = isValid;
            }

            public bool IsValid { get; private set; }
        }

        internal class NewWhisperEventArgs : EventArgs
        {
            public NewWhisperEventArgs(ChatMessage message)
            {
                this.Message = message;
            }

            public ChatMessage Message { get; private set; }
        }
    }
}
