using System;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BulkFileRenamer
{
    public static class IconHelper
    {
        const uint SHGFI_USEFILEATTRIBUTES = 0x10; // Treat the path as a file or folder
        const uint FILE_ATTRIBUTE_DIRECTORY = 0x10; // Specify that it's a folder

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0; // Large icon
        private const uint SHGFI_SMALLICON = 0x1; // Small icon

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("Shell32.dll")]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public static ImageSource GetFolderIcon()
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImg = SHGetFileInfo(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                FILE_ATTRIBUTE_DIRECTORY,
                ref shinfo,
                (uint)Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);

            if (hImg == IntPtr.Zero)
                return null;

            try
            {
                // Convert the icon to a WPF ImageSource
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    shinfo.hIcon,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                // Clean up the icon handle
                DestroyIcon(shinfo.hIcon);
            }
        }
    }
}
