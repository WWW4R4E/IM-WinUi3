using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Linq;

namespace IMWinUi.Services
{
    public class NavigationService
    {
        private Frame _contentFrame;
        private NavigationView _navigationView;

        // 注册 Frame 和 NavigationView
        public void RegisterFrameAndNavigationView(Frame contentFrame, NavigationView navigationView)
        {
            _contentFrame = contentFrame;
            _navigationView = navigationView;
        }

        // 导航到指定页面
        public void NavigateTo(string pageTag)
        {
            if (_contentFrame == null || _navigationView == null)
            {
                Debug.WriteLine("Frame 或 NavigationView 未注册");
                return;
            }

            // 找到对应的 NavigationViewItem
            var selectedItem = _navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == pageTag);

            if (selectedItem != null)
            {
                // 设置 NavigationView 的选中项
                _navigationView.SelectedItem = selectedItem;

                // 导航到对应页面
                Type pageType = Type.GetType($"IMWinUi.Views.{pageTag}");
                if (pageType != null)
                {
                    _contentFrame.Navigate(pageType, null);
                }
                else
                {
                    Debug.WriteLine($"未找到对应页面: {pageTag}");
                }
            }
            else
            {
                Debug.WriteLine($"未找到对应 NavigationViewItem: {pageTag}");
            }
        }
    }
}
