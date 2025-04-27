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
        private bool _isSettingSelectedItem;

        // 注册 Frame 和 NavigationView
        public NavigationService(Frame contentFrame, NavigationView navigationView)
        {
            _contentFrame = contentFrame;
            _navigationView = navigationView;
        }

        // 导航到指定页面
        public void NavigateTo(string pageTag, object? data, bool isNavigate = false)
        {
            var selectedItem = _navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == pageTag);

            Type? pageType = Type.GetType($"IMWinUi.Views.{pageTag}");
            if (pageType == null)
            {
                throw new InvalidOperationException($"无法找到页面类型: IMWinUi.Views.{pageTag}");
            }

            // 如果是内部设置选中项触发的，则跳过导航
            if (_isSettingSelectedItem)
            {
                return;
            }

            // 直接调用时（isNavigate为true）先执行导航
            _contentFrame.Navigate(pageType, data);

            if (isNavigate)
            {
                _isSettingSelectedItem = true;
                _navigationView.SelectedItem = selectedItem;
                _isSettingSelectedItem = false;
            }
        }
    }
}