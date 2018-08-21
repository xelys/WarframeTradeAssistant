namespace WarframeTradeAssistant.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;
    using Newtonsoft.Json;

    internal sealed class NotificationsSender : IDisposable
    {
        private readonly Image.ChatMessage testMessage = new Image.ChatMessage { Name = "Warframe Trade Assistant", Text = "This is a test message." };

        private HttpClient client = new HttpClient();

        private DateTime lastNotificationTime;

        public void Dispose()
        {
            if (this.client != null)
            {
                this.client.Dispose();
            }

            this.client = null;
        }

        public async Task TestSlack(string url)
        {
            await this.SendToWebhook(url, "text", this.testMessage);
        }

        public async Task TestDiscord(string url)
        {
            await this.SendToWebhook(url, "content", this.testMessage);
        }

        public async Task<bool> Send(Image.ChatMessage message)
        {
            var throttle = this.Throttle();
            if (!throttle)
            {
                if (Properties.Settings.Default.SendNotificationsToSlack)
                {
                    await this.SendToWebhook(Properties.Settings.Default.SlackWebhookURL, "text", message);
                }

                if (Properties.Settings.Default.SendNotificationsToDiscord)
                {
                    await this.SendToWebhook(Properties.Settings.Default.DiscordWebhookURL, "content", message);
                }
            }

            return !throttle;
        }

        private bool Throttle()
        {
            var now = DateTime.Now;
            Console.Out.WriteLine((now - this.lastNotificationTime).TotalSeconds + " seconds since last notification");
            if ((now - this.lastNotificationTime).TotalSeconds > 10)
            {
                this.lastNotificationTime = DateTime.Now;
                return false;
            }

            return true;
        }

        private async Task SendToWebhook(string url, string textFieldName, Image.ChatMessage message)
        {
            dynamic jsonMessage = new ExpandoObject();
            ((IDictionary<string, object>)jsonMessage)[textFieldName] = message.Name + ": " + message.Text;
            StringContent content = new StringContent(JsonConvert.SerializeObject(jsonMessage), System.Text.Encoding.UTF8, "application/json");
            var response = await this.client.PostAsync(url, content);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Logger.Write("Notification error: " + response.ToString());
            }
        }
    }
}
