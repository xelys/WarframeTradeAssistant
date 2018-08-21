namespace WarframeTradeAssistant.UI
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using MaterialSkin;
    using WarframeTradeAssistant.GameWindow;
    using WarframeTradeAssistant.Image;
    using WarframeTradeAssistant.Market;
    using WarframeTradeAssistant.Notifications;
    using WarframeTradeAssistant.Properties;
    using WarframeTradeAssistant.Utils;

    internal sealed class Main : IDisposable
    {
        private readonly ContextMenu contextMenu;

        private readonly NotificationsList notificationsList;

        private readonly NotificationsSender notificationsSender;

        private readonly TrayIcon trayIcon;

        private readonly WhispersDetector whispersDetector;

        private Settings settings;

        private MarketClient marketClient;

        private bool disposed = false;

        public Main()
        {
            InitializeMaterialSkinManager();
            this.trayIcon = new TrayIcon();
            this.contextMenu = new ContextMenu();
            this.trayIcon.SetContextMenuStrip(this.contextMenu.Create(this.ShowSettings, this.UpdateMarketStatus, this.DisconnectFromMarket));
            this.notificationsSender = new NotificationsSender();
            this.whispersDetector = new WhispersDetector(true, true);
            this.whispersDetector.NewWhisper += this.OnNewWhisper;
            this.whispersDetector.ChatStateChanged += this.OnChatStateChanged;
            this.notificationsList = new NotificationsList();
            this.whispersDetector.Start();

            if (Properties.Settings.Default.ConnectToMarketOnStartup)
            {
                this.ConnectToMarket().Forget();
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            if (this.marketClient != null)
            {
                this.marketClient.Dispose();
            }

            this.whispersDetector.Dispose();
            this.trayIcon.Dispose();
            this.notificationsSender.Dispose();
            if (this.settings != null)
            {
                this.settings.Dispose();
            }

            this.settings = null;
            this.disposed = true;
        }

        private static void InitializeMaterialSkinManager()
        {
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ROBOTO_MEDIUM_10 = FontLoader.CreateFont(9.0F);
            materialSkinManager.ROBOTO_MEDIUM_11 = FontLoader.CreateFont(11.0F);
            materialSkinManager.ROBOTO_MEDIUM_12 = FontLoader.CreateFont(12.0F);
            materialSkinManager.ROBOTO_REGULAR_11 = FontLoader.CreateFont(11.0F);
        }

        private async void OnChatStateChanged(object sender, WhispersDetector.ChatStateChangedEventArgs e)
        {
            if (Properties.Settings.Default.ShowInvisibleChatWarning && !e.IsValid)
            {
                await Task.Delay(5000);
                if (!this.whispersDetector.IsChatStateValid && NativeMethods.IsWarframeInBackground())
                {
                    this.notificationsList.ShowNotification(new ChatMessage { Name = "Warning", Text = Resources.ChatNotFoundWarning, IsWarning = true });
                }
            }
        }

        private async void OnNewWhisper(object sender, WhispersDetector.NewWhisperEventArgs e)
        {
            await this.Notify(e.Message);
        }

        private async Task Notify(ChatMessage message)
        {
            this.notificationsList.ShowNotification(message);
            await this.notificationsSender.Send(message);
        }

        private void OnUserStatusChanged(object sender, UserStatusChangedEventArgs e)
        {
            this.contextMenu.SetCheckbox(e.Status.ToString());
            this.trayIcon.DisplayStatusIcon(this.marketClient.Status);
        }

        private void ShowSettings()
        {
            if (this.settings == null)
            {
                this.settings = new Settings(this.notificationsSender);
                this.settings.FormClosed += (object sender, System.Windows.Forms.FormClosedEventArgs e) =>
                {
                    this.settings = null;
                };

                this.settings.Show();
            }
        }

        private async Task UpdateMarketStatus(Statuses status)
        {
            if (this.marketClient == null)
            {
                await this.ConnectToMarket();
            }

            if (this.marketClient != null)
            {
                this.marketClient.UpdateStatus(status);
            }
        }

        private async Task ConnectToMarket()
        {
            this.trayIcon.DisplaySpinner("Connecting to market...");
            this.contextMenu.Enabled = false;
            var token = await Storage.LoadToken();
            if (token == null)
            {
                this.notificationsList.ShowNotification(new ChatMessage { Name = "Warning", Text = Resources.SessionNotFoundWarning, IsWarning = true });
                this.SetOfflineMode();
            }
            else if (!await this.ConnectToMarket(token))
            {
                this.notificationsList.ShowNotification(new ChatMessage { Name = "Warning", Text = Resources.SessionExpiredWarning, IsWarning = true });
                this.SetOfflineMode();
            }

            this.contextMenu.Enabled = true;
        }

        private async Task<bool> ConnectToMarket(string token)
        {
            var market = await MarketClient.Connect(token);
            if (market != null)
            {
                this.marketClient = market;
                this.marketClient.UserStatusChanged += this.OnUserStatusChanged;
                return true;
            }

            return false;
        }

        private async Task DisconnectFromMarket()
        {
            this.contextMenu.Enabled = false;
            this.trayIcon.DisplaySpinner("Disconnecting from market...");
            this.contextMenu.UnsetCheckbox();

            if (this.marketClient != null)
            {
                await this.marketClient.CloseAsync();
                this.marketClient = null;
            }

            this.SetOfflineMode();
        }

        private void SetOfflineMode()
        {
            this.trayIcon.DisplayOfflineIcon();
            this.contextMenu.SetCheckbox("disconnected");
            this.contextMenu.Enabled = true;
        }
    }
}