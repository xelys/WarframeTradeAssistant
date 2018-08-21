namespace WarframeTradeAssistant
{
    using System;
    using System.Windows.Forms;
    using Common;
    using WarframeTradeAssistant.UI;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main(string[] args)
        {
#if DEBUG
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
#endif
            Logger.Initialize(Logger.ApplicationMode.Main);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Main pi = new Main())
            {
                Application.Run();
            }
        }
    }
}
