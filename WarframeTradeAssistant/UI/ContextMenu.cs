namespace WarframeTradeAssistant.UI
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using MaterialSkin.Controls;
    using WarframeTradeAssistant.Utils;

    internal class ContextMenu
    {
        private ContextMenuStrip contextMenuStrip;

        public bool Enabled
        {
            get => this.contextMenuStrip.Enabled;
            set => this.contextMenuStrip.Enabled = value;
        }

        public ContextMenuStrip Create(Action showSettings, Func<Market.Statuses, Task> updateStatus, Func<Task> disconnect)
        {
            MaterialContextMenuStrip menu = new MaterialContextMenuStrip();
            menu
                .AddItem("settings", "Settings", showSettings)
                .AddSeparator()
                .AddItem("#online", "Online", () => updateStatus(Market.Statuses.online))
                .AddItem("#ingame", "Online In Game", () => updateStatus(Market.Statuses.ingame))
                .AddItem("#invisible", "Invisible", () => updateStatus(Market.Statuses.invisible))
                .AddItem("#disconnected", "Offline", () => disconnect(), true, true)
                .AddSeparator()
                .AddItem("exit", "Exit", () => Application.Exit());
            this.contextMenuStrip = menu;
            return this.contextMenuStrip;
        }

        public void UnsetCheckbox()
        {
            this.SetCheckbox(string.Empty);
        }

        public void SetCheckbox(string checkboxName)
        {
            if (this.contextMenuStrip == null)
            {
                return;
            }

            if (this.contextMenuStrip.InvokeRequired)
            {
                this.contextMenuStrip.Invoke(new ActionDelegate<string>(this.SetCheckbox), checkboxName);
            }

            for (var i = 0; i < this.contextMenuStrip.Items.Count; i++)
            {
                var item = this.contextMenuStrip.Items[i] as ToolStripMenuItem;
                if (item != null && item.Name.StartsWith("#"))
                {
                    item.Checked = checkboxName == item.Name.Substring(1);
                }
            }
        }
    }
}
