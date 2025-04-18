using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Linq;

namespace IMWinUi.Services
{
    public class NavigationService
    {
        private Frame _contentFrame;
        private NavigationView _navigationView;

        // 注册 Frame 和 NavigationView
        public NavigationService(Frame contentFrame, NavigationView navigationView)
        {
            _contentFrame = contentFrame;
            _navigationView = navigationView;
        }
        // 导航到指定页面
        public void NavigateTo(string pageTag, object? data)
        {
            var selectedItem = _navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == pageTag);

            // 导航到对应页面
            Type? pageType = Type.GetType($"IMWinUi.Views.{pageTag}");
            if (pageType == null)
            {
                throw new InvalidOperationException($"无法找到页面类型: IMWinUi.Views.{pageTag}");
            }
            _navigationView.SelectedItem = selectedItem;
            _contentFrame.Navigate(pageType, data);
        }

    }
}
