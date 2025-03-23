using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace IMWinUi.Views
{
    public sealed partial class ContactPage : Page
    {
        internal ContactPageViewModel ContactPageViewModel = new ContactPageViewModel();
        public ContactPage()
        {
            this.InitializeComponent();
            ContactsCVS.Source = ContactPageViewModel.GetContactsGroupedAsync();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentApp = (App)Application.Current;
            var window = (MainWindow)currentApp.m_window;
            var navigationService = window.NavigationService;
            navigationService.NavigateTo("CommentPage", ContactPageViewModel.User);
            //var frame = (CommentPage)window.contentFrame.Content;
            //frame.CommentPageViewModel.Users.Add(ContactPageViewModel.User);
            //frame.CommentPageViewModel.SelectUser = ContactPageViewModel.User;  
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            ContactPageViewModel.User = listView.SelectedItem as IMUser;
        }
    }

}
