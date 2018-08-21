namespace WarframeTradeAssistant.UI
{
    using System;
    using Common;
    using MaterialSkin;
    using MaterialSkin.Controls;
    using WarframeTradeAssistant.Notifications;

    internal partial class Settings : MaterialForm
    {
        private readonly NotificationsSender notificationsSender;

        public Settings(NotificationsSender notificationsSender)
        {
            this.InitializeComponent();
            MaterialSkinManager.Instance.AddFormToManage(this);
            this.LoadSettings();
            this.notificationsSender = notificationsSender;
        }

        private void Cancel(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Accept(object sender, EventArgs e)
        {
            this.SaveSettings();
            this.Close();
        }

        private async void TestSlackWebhook(object sender, EventArgs e)
        {
            await this.notificationsSender.TestSlack(this.slackWebhookURL.Text);
        }

        private async void TestDiscordWebhook(object sender, EventArgs e)
        {
            await this.notificationsSender.TestDiscord(this.discordWebhookURL.Text);
        }

        private void LoadSettings()
        {
            Properties.Settings.Default.Reload();
            this.showNotificationsOnlyOnBackgroundMessages.Checked = Properties.Settings.Default.ShowNotificationsOnlyOnBackgroundMessages;
            this.showInvisibleChatWarning.Checked = Properties.Settings.Default.ShowInvisibleChatWarning;
            this.sendNotificationsToSlack.Checked = Properties.Settings.Default.SendNotificationsToSlack;
            this.sendNotificationsToDiscord.Checked = Properties.Settings.Default.SendNotificationsToDiscord;

            if (Registration.IsFirefoxInstalled())
            {
                this.installFirefoxExtension.Enabled = true;
                this.installFirefoxExtension.Checked = Registration.IsFirefoxExtensionInstalled();
            }
            else
            {
                this.installFirefoxExtension.Enabled = false;
            }

            if (Registration.IsChromeInstalled())
            {
                this.installChromeExtension.Enabled = true;
                this.installChromeExtension.Checked = Registration.IsChromeExtensionInstalled();
            }
            else
            {
                this.installChromeExtension.Enabled = false;
            }

            this.connectToMarketOnStartup.Checked = Properties.Settings.Default.ConnectToMarketOnStartup;
            this.automaticallyChangeStatus.Checked = Properties.Settings.Default.AutomaticallyChangeStatus;
            this.slackWebhookURL.Text = Properties.Settings.Default.SlackWebhookURL;
            this.discordWebhookURL.Text = Properties.Settings.Default.DiscordWebhookURL;
            this.forceOrderStatusUpdates.Checked = Properties.Settings.Default.ForceOrderStatusUpdates;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.ShowNotificationsOnlyOnBackgroundMessages = this.showNotificationsOnlyOnBackgroundMessages.Checked;
            Properties.Settings.Default.ShowInvisibleChatWarning = this.showInvisibleChatWarning.Checked;
            Properties.Settings.Default.SendNotificationsToSlack = this.sendNotificationsToSlack.Checked;
            Properties.Settings.Default.SendNotificationsToDiscord = this.sendNotificationsToDiscord.Checked;
            Properties.Settings.Default.SlackWebhookURL = this.slackWebhookURL.Text;
            Properties.Settings.Default.DiscordWebhookURL = this.discordWebhookURL.Text;
            Properties.Settings.Default.ConnectToMarketOnStartup = this.connectToMarketOnStartup.Checked;
            Properties.Settings.Default.AutomaticallyChangeStatus = this.automaticallyChangeStatus.Checked;
            Properties.Settings.Default.ForceOrderStatusUpdates = this.forceOrderStatusUpdates.Checked;
            Properties.Settings.Default.Save();
            Registration.UpdateRegistry(this.installFirefoxExtension.Checked, this.installChromeExtension.Checked);
        }

        private void SlackWebhookURL_TextChanged(object sender, EventArgs e)
        {
            this.testSlackWebhook.Enabled = this.IsURL(this.slackWebhookURL.Text);
        }

        private void DiscordWebhookURL_TextChanged(object sender, EventArgs e)
        {
            this.testDiscordWebhook.Enabled = this.IsURL(this.discordWebhookURL.Text);
        }

        private bool IsURL(string url)
        {
            if (url == null)
            {
                return false;
            }

            try
            {
                var host = new Uri(url).Host;
                return host == "hooks.slack.com" || host == "discordapp.com";
            }
            catch (UriFormatException)
            {
                return false;
            }
        }
    }
}
