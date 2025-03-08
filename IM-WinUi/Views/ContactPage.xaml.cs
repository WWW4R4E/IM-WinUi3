using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IMWinUi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactPage : Page
    {
        List<IMUser> users;
        public ContactPage()
        {
            this.InitializeComponent();
            ContactsCVS.Source = UserGroupViewModel.GetContactsGroupedAsync();
        }
        private void LineElement_Loaded(object sender, RoutedEventArgs e)
        {
            var line = sender as Line;
            if (line != null)
            {
                line.X2 = line.ActualWidth;
            }
        }

        private void AutoSuggestBox_GettingFocus(UIElement sender, Microsoft.UI.Xaml.Input.GettingFocusEventArgs args)
        {
            // 清空 ContactGrid 的内容
            ContactGrid.Visibility = Visibility.Collapsed;

            NewContactGrid.Visibility = Visibility.Visible;
        }

        private void AutoSuggestBox_LosingFocus(UIElement sender, Microsoft.UI.Xaml.Input.LosingFocusEventArgs args)
        {
            // 清空 ContactGrid 的内容
            ContactGrid.Visibility = Visibility.Visible;

            NewContactGrid.Visibility = Visibility.Collapsed;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = sender as ListBox;
            if (listbox.SelectedIndex != -1)
            {
                var searchWindow = new SearchWindow();
                searchWindow.Activate();
                listbox.SelectedIndex = -1;
            }
        }
    }

}
