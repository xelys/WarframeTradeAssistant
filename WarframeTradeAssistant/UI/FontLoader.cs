namespace WarframeTradeAssistant.UI
{
    using System;
    using System.Drawing;
    using System.Drawing.Text;
    using WarframeTradeAssistant.GameWindow;

    internal static class FontLoader
    {
        private static PrivateFontCollection fonts = new PrivateFontCollection();

        static FontLoader()
        {
            byte[] fontData = Properties.Resources.veramono;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.veramono.Length);
            NativeMethods.AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.veramono.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
        }

        public static Font CreateFont(float fontEmSize)
        {
            return new Font(fonts.Families[0], fontEmSize);
        }
    }
}
