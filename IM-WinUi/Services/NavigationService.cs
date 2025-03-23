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
        public void NavigateTo(string pageTag, object data)
        {
            if (_contentFrame == null || _navigationView == null)
            {
                Debug.WriteLine("Frame 或 NavigationView 未注册");
                return;
            }
            var selectedItem = _navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == pageTag);
            _navigationView.SelectedItem = selectedItem;

            // 导航到对应页面
            Type pageType = Type.GetType($"IMWinUi.Views.{pageTag}");
            _contentFrame.Navigate(pageType, data);
        }
    }
}
