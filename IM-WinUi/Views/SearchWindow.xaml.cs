using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WinRT.Interop;


namespace IMWinUi.Views
{
    public sealed partial class SearchWindow : Window
    {
        public SearchWindow(int select)
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(Title);
            InitializeComponent();

            NvSample.SelectedItem = NvSample.MenuItems[select];
            
            // 获取窗口句柄
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            // 获取 AppWindow
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            
            if (appWindow != null)
            {
                // 设置窗口大小
                int width = 800;
                int height = 1080;
                appWindow.Resize(new SizeInt32(width, height));

                // 获取显示器的工作区域
                DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                RectInt32 workArea = displayArea.WorkArea;

                // 计算居中位置
                int x = (workArea.Width - width) / 2 + workArea.X;
                int y = (workArea.Height - height) / 2 + workArea.Y;

                // 移动窗口到居中位置
                appWindow.MoveAndResize(new RectInt32(x, y, width, height));
            }
        }
    }
}
