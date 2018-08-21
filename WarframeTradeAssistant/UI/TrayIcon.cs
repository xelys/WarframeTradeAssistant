namespace WarframeTradeAssistant.UI
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using WarframeTradeAssistant.Properties;
    using WarframeTradeAssistant.Utils;

    internal sealed class TrayIcon : IDisposable
    {
        private Icon[] animatedIconFrames;

        private NotifyIcon notifyIcon;

        public TrayIcon()
        {
            this.notifyIcon = new NotifyIcon
            {
                Visible = true
            };
            this.DisplayOfflineIcon();
        }

        public void Dispose()
        {
            if (this.notifyIcon != null)
            {
                this.notifyIcon.Dispose();
            }

            this.notifyIcon = null;
        }

        public void SetContextMenuStrip(ContextMenuStrip contextMenuStrip)
        {
            this.notifyIcon.ContextMenuStrip = contextMenuStrip;
        }

        public void DisplayOfflineIcon()
        {
            this.SetIcon(Resources.offline);
            this.notifyIcon.Text = "Offline";
        }

        public void DisplaySpinner(string message)
        {
            this.SetIcon(new Icon[] { Resources._0, Resources._1, Resources._2, Resources._3, Resources._4, Resources._5, Resources._6, Resources._7 });
            this.notifyIcon.Text = message;
        }

        public void DisplayStatusIcon(Market.Statuses status)
        {
            switch (status)
            {
                case Market.Statuses.invisible:
                    this.SetIcon(Resources.invisible);
                    this.notifyIcon.Text = "Invisible";
                    break;
                case Market.Statuses.ingame:
                    this.SetIcon(Resources.ingame);
                    this.notifyIcon.Text = "Online In Game";
                    break;
                case Market.Statuses.online:
                    this.SetIcon(Resources.online);
                    this.notifyIcon.Text = "Online";
                    break;
                default:
                    this.SetIcon(Resources.error);
                    this.notifyIcon.Text = "Error connecting to market";
                    break;
            }
        }

        private async Task Animate()
        {
            int i = 0;
            while (this.animatedIconFrames != null)
            {
                this.notifyIcon.Icon = this.animatedIconFrames[i];
                i++;
                if (i >= this.animatedIconFrames.Length)
                {
                    i = 0;
                }

                await Task.Delay(1000 / this.animatedIconFrames.Length);
            }
        }

        private void SetIcon(Icon icon)
        {
            this.animatedIconFrames = null;
            this.notifyIcon.Icon = icon;
        }

        private void SetIcon(Icon[] iconFrames)
        {
            this.animatedIconFrames = iconFrames;
            this.Animate().Forget();
        }
    }
}
