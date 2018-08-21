namespace Common
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.Win32;

    public static class Registration
    {
        private const string AppName = "space.xelys.warframetradeassistanthost";

        private const string FirefoxExtensionID = "warframetradeassistant@xelys.space";

        private const string ChromeExtensionID = "pohndjikhbffbnllabhedkeocnojjgnl";

        private const string ChromeExtensionURL = "https://clients2.google.com/service/update2/crx";

        public static bool IsFirefoxInstalled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Mozilla\Firefox", false))
            {
                return key != null;
            }
        }

        public static bool IsFirefoxExtensionInstalled()
        {
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var extensionFilePath = Path.Combine(Path.GetDirectoryName(exePath), FirefoxExtensionID + ".xpi");
            var hostManifestFileName = Path.Combine(Path.GetDirectoryName(exePath), "firefox.json");
            var isExtensionInstalled = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Mozilla\Firefox\Extensions", FirefoxExtensionID, null) as string == extensionFilePath;
            var isHostRegistered = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Mozilla\NativeMessagingHosts\" + AppName, null, null) as string == hostManifestFileName;
            return isExtensionInstalled && isHostRegistered;
        }

        public static bool IsChromeInstalled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Google\Chrome", false))
            {
                return key != null;
            }
        }

        public static bool IsChromeExtensionInstalled()
        {
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var hostManifestFileName = Path.Combine(Path.GetDirectoryName(exePath), "chrome.json");
            var isExtensionInstalled = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Google\Chrome\Extensions\" + ChromeExtensionID, "update_url", null) as string == ChromeExtensionURL;
            var isHostRegistered = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Google\Chrome\NativeMessagingHosts\" + AppName, null, null) as string == hostManifestFileName;
            return isExtensionInstalled && isHostRegistered;
        }

        public static void UpdateRegistry(bool firefox, bool chrome)
        {
            if (IsFirefoxInstalled())
            {
                if (firefox)
                {
                    RegisterFirefox();
                }
                else
                {
                    UnregisterFirefox();
                }
            }

            if (IsChromeInstalled())
            {
                if (chrome)
                {
                    RegisterChrome();
                }
                else
                {
                    UnregisterChrome();
                }
            }
        }

        private static void RegisterFirefox()
        {
            RegisterHost(@"Software\Mozilla\NativeMessagingHosts", "firefox.json");
            InstallFirefoxExtension();
        }

        private static void UnregisterFirefox()
        {
            UnregisterHost(@"Software\Mozilla\NativeMessagingHosts");
            UninstallFrefoxExtension();
        }

        private static void RegisterChrome()
        {
            RegisterHost(@"Software\Google\Chrome\NativeMessagingHosts", "chrome.json");
            InstallChromeExtension();
        }

        private static void UnregisterChrome()
        {
            UnregisterHost(@"Software\Google\Chrome\NativeMessagingHosts");
            UninstallChromeExtension();
        }

        private static void InstallFirefoxExtension()
        {
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var extensionFileName = Path.Combine(Path.GetDirectoryName(exePath), FirefoxExtensionID + ".xpi");

            RegistryKey extensionsKey = null;
            try
            {
                extensionsKey = CreateSubkeyIfDoesntExist(@"Software\Mozilla\Firefox", "Extensions");
                if (extensionsKey != null)
                {
                    extensionsKey.SetValue(FirefoxExtensionID, extensionFileName);
                }
            }
            finally
            {
                if (extensionsKey != null)
                {
                    extensionsKey.Close();
                }
            }
        }

        private static void UninstallFrefoxExtension()
        {
            using (RegistryKey extensionsKey = Registry.CurrentUser.OpenSubKey(@"Software\Mozilla\Firefox\Extensions", true))
            {
                if (extensionsKey != null && extensionsKey.GetValue(FirefoxExtensionID) != null)
                {
                    extensionsKey.DeleteValue(FirefoxExtensionID);
                }
            }
        }

        private static void InstallChromeExtension()
        {
            using (var extensionsKey = CreateSubkeyIfDoesntExist(@"Software\Google\Chrome", "Extensions"))
            {
                if (extensionsKey != null)
                {
                    RegistryKey extensionKey = null;
                    try
                    {
                        extensionKey = CreateSubkeyIfDoesntExist(@"Software\Google\Chrome\Extensions", ChromeExtensionID);
                        if (extensionKey != null)
                        {
                            extensionKey.SetValue("update_url", ChromeExtensionURL);
                        }
                    }
                    finally
                    {
                        if (extensionKey != null)
                        {
                            extensionKey.Close();
                        }
                    }
                }
            }
        }

        private static void UninstallChromeExtension()
        {
            using (RegistryKey extensionsKey = Registry.CurrentUser.OpenSubKey(@"Software\Google\Chrome\Extensions", true))
            {
                if (extensionsKey != null && extensionsKey.GetSubKeyNames().Any(n => n == ChromeExtensionID))
                {
                    extensionsKey.DeleteSubKey(ChromeExtensionID);
                }
            }
        }

        private static void RegisterHost(string hostsPath, string manifestFileName)
        {
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var hostManifestFileName = Path.Combine(Path.GetDirectoryName(exePath), manifestFileName);
            RegistryKey hostKey = null;
            try
            {
                hostKey = CreateSubkeyIfDoesntExist(hostsPath, AppName);
                if (hostKey != null)
                {
                    hostKey.SetValue(null, hostManifestFileName);
                }
            }
            finally
            {
                if (hostKey != null)
                {
                    hostKey.Close();
                }
            }
        }

        private static void UnregisterHost(string hostsPath)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(hostsPath, true))
            {
                if (key != null && key.GetSubKeyNames().Any(k => k == AppName))
                {
                    key.DeleteSubKey(AppName);
                }
            }
        }

        private static RegistryKey CreateSubkeyIfDoesntExist(string parent, string subkey)
        {
            using (RegistryKey parentKey = Registry.CurrentUser.OpenSubKey(parent, true))
            {
                if (parentKey != null)
                {
                    var subKeyExists = parentKey.GetSubKeyNames().Any(k => k == subkey);
                    if (!subKeyExists)
                    {
                        return parentKey.CreateSubKey(subkey);
                    }
                    else
                    {
                        return parentKey.OpenSubKey(subkey, true);
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
