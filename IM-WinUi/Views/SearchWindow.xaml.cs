using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using Windows.Graphics;
using WinRT.Interop;
using System.Text.Json;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Services;

namespace IMWinUi.Views
{
    public sealed partial class SearchWindow : Window
    {
        private SearchViewModel _searchViewModel = new();
        public SearchWindow(int select)
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(Title);
            InitializeComponent();
            Pt.SelectedIndex = select;

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

        private void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _searchViewModel.ExecuteSearch(args.QueryText);
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedData = e.ClickedItem; 
            if (clickedData is IMUser)
            {
                var currentApp = (App)Application.Current;
                var window = (MainWindow)currentApp.m_window!;
                var navigationService = window.NavigationService;
                navigationService.NavigateTo("ContactPage", clickedData as IMUser);
            }
            else if(clickedData is IMMessage)
            {
                Debug.WriteLine("IMMessage");
            }
        }

    }
}
