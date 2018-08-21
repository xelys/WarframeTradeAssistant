namespace WarframeTradeAssistant.Market
{
    using System;
    using System.Dynamic;
    using System.Threading.Tasks;
    using System.Timers;
    using Newtonsoft.Json;
    using WebSocketSharp;

    internal sealed class WebsocketClient : IDisposable
    {
        private const string SocketURL = "wss://warframe.market/socket";

        private readonly TaskCompletionSource<WebsocketClient> connectTaskSource;

        private TaskCompletionSource<object> disconnectTaskSource;

        private Statuses? targetStatus;

        private Statuses? status;

        private Timer timer;

        private WebSocket webSocket;

        private WebsocketClient(string token)
        {
            this.connectTaskSource = new TaskCompletionSource<WebsocketClient>();
            this.webSocket = new WebSocket(SocketURL);
            this.webSocket.SetCookie(new WebSocketSharp.Net.Cookie("JWT", token));
            this.webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            this.webSocket.OnOpen += this.OnOpen;
            this.webSocket.OnError += this.OnError;
            this.webSocket.OnMessage += this.OnMessage;
            this.webSocket.OnClose += this.OnClose;
            this.timer = new Timer { AutoReset = true, Interval = 60000 };
            this.timer.Elapsed += this.Timer_Elapsed;
            this.webSocket.ConnectAsync();
            this.timer.Start();
        }

        public event EventHandler<UserStatusChangedEventArgs> UserStatusChanged;

        public Statuses Status => this.status ?? Statuses.invisible;

        public static Task<WebsocketClient> Connect(string token, Statuses initialStatus)
        {
            var client = new WebsocketClient(token);
            client.targetStatus = initialStatus;
            return client.connectTaskSource.Task;
        }

        public Task CloseAsync()
        {
            this.DisposeTimer();
            this.disconnectTaskSource = new TaskCompletionSource<object>();
            if (this.webSocket != null)
            {
                this.webSocket.CloseAsync(CloseStatusCode.Normal, string.Empty);
            }

            this.webSocket = null;
            return this.disconnectTaskSource.Task;
        }

        public void Close()
        {
            this.DisposeTimer();
            if (this.webSocket != null)
            {
                this.webSocket.Close(CloseStatusCode.Normal, string.Empty);
            }

            this.webSocket = null;
        }

        public void Dispose()
        {
            this.Close();
        }

        public void UpdateStatus(Statuses status)
        {
            this.targetStatus = status;
            this.SendStatusUpdateCommand();
        }

        private void DisposeTimer()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Dispose();
            }

            this.timer = null;
        }

        private void SendStatusUpdateCommand()
        {
            if (this.targetStatus.HasValue && this.webSocket != null && this.webSocket.ReadyState == WebSocketState.Open)
            {
                dynamic command = new ExpandoObject();
                command.type = "@WS/USER/SET_STATUS";
                command.payload = this.targetStatus.Value.ToString();
                var json = JsonConvert.SerializeObject(command);
                Console.Out.WriteLine("Send: " + json);
                this.webSocket.Send(json);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.SendStatusUpdateCommand();
        }

        private void OnOpen(object source, EventArgs args)
        {
            this.SendStatusUpdateCommand();
            this.connectTaskSource?.TrySetResult(this);
        }

        private void OnClose(object source, CloseEventArgs closeEvent)
        {
            if (closeEvent.Code != (ushort)CloseStatusCode.Normal)
            {
                Common.Logger.Write($"Code: {closeEvent.Code}, Reason: {closeEvent.Reason}, Clean: {closeEvent.WasClean}");
                this.webSocket.Connect();
            }
            else
            {
                this.disconnectTaskSource?.TrySetResult(null);
            }
        }

        private void OnError(object source, ErrorEventArgs errorEvent)
        {
            Common.Logger.Write(errorEvent.ToString());
        }

        private void OnMessage(object source, MessageEventArgs messageEvent)
        {
            Console.Out.WriteLine("Recieve: " + messageEvent.Data);
            dynamic message = JsonConvert.DeserializeObject<ExpandoObject>(messageEvent.Data);
            if (message.type == "@WS/USER/SET_STATUS")
            {
                Statuses newStatus = Statuses.invisible;
                switch (message.payload)
                {
                    case "ingame":
                        newStatus = Statuses.ingame;
                        break;
                    case "online":
                        newStatus = Statuses.online;
                        break;
                    case "offline":
                    case "invisible":
                        newStatus = Statuses.invisible;
                        break;
                }

                var previousStatus = this.status;
                this.status = newStatus;
                this.targetStatus = newStatus;
                if (previousStatus != newStatus)
                {
                    this.UserStatusChanged?.Invoke(this, new UserStatusChangedEventArgs(newStatus));
                }
            }
        }
    }
}