namespace Common
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Storage
    {
        public static async Task<string> LoadToken()
        {
            var tokenFilePath = ObtainTokenFilePath();
            if (!File.Exists(tokenFilePath))
            {
                return null;
            }

            while (true)
            {
                try
                {
                    using (var file = File.Open(tokenFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        var length = file.Length;
                        byte[] entropy = new byte[16];
                        byte[] encryptedToken = new byte[length - 16];
                        file.Read(entropy, 0, entropy.Length);
                        file.Read(encryptedToken, 0, encryptedToken.Length);
                        byte[] tokenBytes = ProtectedData.Unprotect(encryptedToken, entropy, DataProtectionScope.CurrentUser);
                        return Encoding.ASCII.GetString(tokenBytes);
                    }
                }
                catch (IOException)
                {
                    await Task.Delay(100);
                }
            }
        }

        public static void SaveToken(string token)
        {
            byte[] entropy = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(entropy);
            byte[] tokenBytes = Encoding.ASCII.GetBytes(token);
            byte[] encryptedToken = ProtectedData.Protect(tokenBytes, entropy, DataProtectionScope.CurrentUser);
            var tokenFilePath = ObtainTokenFilePath();
            while (true)
            {
                try
                {
                    using (var file = File.Open(tokenFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        file.Write(entropy, 0, entropy.Length);
                        file.Write(encryptedToken, 0, encryptedToken.Length);
                        return;
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static async Task DeleteToken()
        {
            var tokenFilePath = ObtainTokenFilePath();
            while (true)
            {
                try
                {
                    File.Delete(tokenFilePath);
                }
                catch (IOException)
                {
                    await Task.Delay(100);
                }
            }
        }

        public static string ObtainConfigurationFilesDirectory()
        {
            var configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WarframeTradeAssistant");
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            return configDirectory;
        }

        private static string ObtainTokenFilePath()
        {
            var tokenFilePath = Path.Combine(ObtainConfigurationFilesDirectory(), "market.token");
            return tokenFilePath;
        }
    }
}