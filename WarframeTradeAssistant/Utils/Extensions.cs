namespace WarframeTradeAssistant.Utils
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    internal static class Extensions
    {
        public static ContextMenuStrip AddItem(this ContextMenuStrip menu, string name, string text, Func<Task> handler, bool enabled = true, bool itemChecked = false)
        {
            ToolStripMenuItem item = new ToolStripMenuItem
            {
                Name = name,
                Text = text,
                Enabled = enabled,
                Checked = itemChecked
            };
            item.Click += async (object sender, EventArgs e) =>
            {
                await handler();
            };
            menu.Items.Add(item);
            return menu;
        }

        public static ContextMenuStrip AddItem(this ContextMenuStrip menu, string name, string text, Action handler, bool enabled = true, bool itemChecked = false)
        {
            ToolStripMenuItem item = new ToolStripMenuItem
            {
                Name = name,
                Text = text,
                Enabled = enabled,
                Checked = itemChecked
            };
            item.Click += (object sender, EventArgs e) =>
            {
                handler();
            };
            menu.Items.Add(item);
            return menu;
        }

        public static ContextMenuStrip AddSeparator(this ContextMenuStrip menu)
        {
            menu.Items.Add(new ToolStripSeparator());
            return menu;
        }

        public static void Forget(this Task task)
        {
            task.ContinueWith(
                t => { Common.Logger.Write(t.Exception); },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
