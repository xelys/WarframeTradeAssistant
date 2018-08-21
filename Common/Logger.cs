namespace Common
{
    using System;
    using System.IO;

    public class Logger
    {
        private static readonly object LockObject = new object();

        private static string logFileName;

        public enum ApplicationMode
        {
            Main,
            NativeMessagingHost
        }

        public static void Initialize(ApplicationMode mode)
        {
            var configDirectory = Storage.ObtainConfigurationFilesDirectory();
            logFileName = Path.Combine(configDirectory, mode == ApplicationMode.Main ? "log.txt" : "host.log.txt");
        }

        public static void Write(object ex)
        {
            Write(ex.ToString());
        }

        public static void Write(string message)
        {
            var now = DateTime.Now;
            message = now.ToShortDateString() + " " + now.ToLongTimeString() + " " + message;
            lock (LockObject)
            {
                using (var file = new StreamWriter(logFileName, true))
                {
                    file.WriteLine(message);
                }
            }
        }
    }
}
