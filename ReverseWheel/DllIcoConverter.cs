using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ReverseWheel {
    [ValueConversion(typeof(int), typeof(ImageSource))]
    public class DllIcoConverter : IValueConverter {
        [DllImport("shell32.dll")]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (targetType != typeof(ImageSource))
                return Binding.DoNothing;
            IntPtr iconHandle = ExtractIcon(Process.GetCurrentProcess().Handle, @"C:\Windows\System32\DDORes.dll", (int)value);
            if (iconHandle == IntPtr.Zero)
                return null;
            ImageSource icon = Imaging.CreateBitmapSourceFromHIcon(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
