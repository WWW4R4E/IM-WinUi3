using System;
using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using System.Linq;
using Microsoft.UI.Xaml.Navigation;

namespace IMWinUi.Views
{
    public sealed partial class ContactPage
    {
        internal ContactPageViewModel ContactPageViewModel;
        public ContactPage()
        {
            InitializeComponent();
            ContactsCvs.Source = ContactPageViewModel.GetContactsGroupedAsync();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var data = e.Parameter;
            if (data != null && data is IMUser user)
            {
                if (ContactPageViewModel.Friends.All(x => x.UserName != user.UserName))
                {
                    ContactPageViewModel.Friends.Add(user);
                }

                ContactPageViewModel.User =
                    ContactPageViewModel.Friends.FirstOrDefault(x => x.UserName == user.UserName);
            }
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
            if (listbox!.SelectedIndex != -1)
            {
                var searchWindow = new SearchWindow(listbox.SelectedIndex);
                searchWindow.Activate();
                listbox.SelectedIndex = -1;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentApp = (App)Application.Current;
            var window = (MainWindow)currentApp.m_window!;
            var navigationService = window.NavigationService;
            navigationService.NavigateTo("CommentPage", ContactPageViewModel.User);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = (sender as ListView)!;
            ContactPageViewModel.User = (listView.SelectedItem as IMUser)!;
        }
    }

}
