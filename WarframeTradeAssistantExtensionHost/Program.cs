namespace WarframeTradeAssistantExtensionHost
{
    using Common;

    internal static class Program
    {
        internal static void Main(string[] args)
        {
#if DEBUG
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
#endif
            Logger.Initialize(Logger.ApplicationMode.NativeMessagingHost);
            Host.ProcessMessages();
        }
    }
}
