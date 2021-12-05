using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MunicipalityApp
{
    public static class Extensions
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ToImageSource(this Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumeration) => new ObservableCollection<T>(enumeration);
    }

    public static class CustomWindowExtensions
    {
        public static void SetupCustomWindow(this Window window)
        {
            window.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (sender, e) => window.Close()));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand,
                (sender, e) =>
                {
                    SystemCommands.MaximizeWindow(window);
                },
                (sender, e) =>
                {
                    e.CanExecute = window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip;
                }));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
                (sender, e) =>
                {
                    SystemCommands.MinimizeWindow(window);
                },
                (sender, e) =>
                {
                    e.CanExecute = window.ResizeMode != ResizeMode.NoResize;
                }));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand,
                (sender, e) =>
                {
                    SystemCommands.RestoreWindow(window);
                },
                (sender, e) =>
                {
                    e.CanExecute = window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip;
                }));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, (sender, e) =>
            {
                var element = e.OriginalSource as FrameworkElement;
                if (element == null)
                    return;

                var point = window.WindowState == WindowState.Maximized ? new System.Windows.Point(0, element.ActualHeight)
                    : new System.Windows.Point(window.Left + window.BorderThickness.Left, element.ActualHeight + window.Top + window.BorderThickness.Top);
                point = element.TransformToAncestor(window).Transform(point);
                SystemCommands.ShowSystemMenu(window, point);
            }));
        }
    }
}
