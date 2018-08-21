namespace WarframeTradeAssistant.Market
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    internal sealed class MarketClient : IDisposable
    {
        private System.Windows.Forms.Timer timer;

        private ApiClient apiClient;

        private WebsocketClient websocketClient;

        private MarketClient()
        {
            this.timer = new System.Windows.Forms.Timer();
            this.timer.Tick += this.OnTimerTick;
            this.timer.Interval = 5000;
        }

        public event EventHandler<UserStatusChangedEventArgs> UserStatusChanged;

        public Statuses Status => this.websocketClient.Status;

        public static async Task<MarketClient> Connect(string token)
        {
            var client = new MarketClient();
            client.apiClient = await ApiClient.Connect(token);
            if (client.apiClient != null)
            {
                client.websocketClient = await WebsocketClient.Connect(token, client.apiClient.InitialStatus);
                client.websocketClient.UserStatusChanged += client.WebsocketClient_UserStatusChanged;
                client.timer.Start();
                return client;
            }
            else
            {
                return null;
            }
        }

        public async Task CloseAsync()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
            }

            if (this.websocketClient != null)
            {
                await this.websocketClient.CloseAsync();
            }

            this.websocketClient = null;
            if (this.apiClient != null)
            {
                this.apiClient.Dispose();
            }

            this.apiClient = null;
        }

        public void Dispose()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
            }

            this.timer = null;
            if (this.websocketClient != null)
            {
                this.websocketClient.Dispose();
            }

            this.websocketClient = null;
            if (this.apiClient != null)
            {
                this.apiClient.Dispose();
            }

            this.apiClient = null;
        }

        public void UpdateStatus(Statuses status)
        {
            this.websocketClient.UpdateStatus(status);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var isWarframeRunning = Process.GetProcessesByName("Warframe.x64").Length > 0;
            if (Properties.Settings.Default.AutomaticallyChangeStatus)
            {
                if (isWarframeRunning && this.Status != Statuses.ingame)
                {
                    this.UpdateStatus(Statuses.ingame);
                }

                if (!isWarframeRunning && this.Status != Statuses.invisible)
                {
                    this.UpdateStatus(Statuses.invisible);
                }
            }
        }

        private async void WebsocketClient_UserStatusChanged(object sender, UserStatusChangedEventArgs e)
        {
            this.UserStatusChanged?.Invoke(this, new UserStatusChangedEventArgs(e.Status));
            if (Properties.Settings.Default.ForceOrderStatusUpdates)
            {
                Console.Out.WriteLine("Status changed to " + e.Status + ", updating orders");
                await this.apiClient.UpdateOrders();
            }
        }
    }
}