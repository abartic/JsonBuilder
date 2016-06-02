using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JsonGenerator
{
    class WindowManagerEx : WindowManager
    {

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            Window window = base.EnsureWindow(model, view, isDialog);

            if (isDialog)
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;
            }
            else
            {
                window.SizeToContent = SizeToContent.Manual;
                window.WindowState = WindowState.Maximized;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            return window;
        }
    }
}
