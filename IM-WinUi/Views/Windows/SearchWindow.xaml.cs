using IMWinUi.Models;
using IMWinUi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Graphics;
using IMWinUi.Helper;
using WinRT.Interop;

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

            WinHelper.SetWindowSizeAndCenter(this, 800, 1080);
        }

        private void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            // _searchViewModel.ExecuteSearch(args.QueryText);
            _searchViewModel.ResultUsers = new List<ResultInformation>
            {
                new ResultInformation
                {
                    Id = 1,
                    Name = "test",
                    ProfilePicture = new byte[0]
                },
            };
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
