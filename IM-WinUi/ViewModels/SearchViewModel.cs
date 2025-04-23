using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMWinUi.Models;
using IMWinUi.Services;
using System.Threading.Tasks;
using Windows.System;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace IMWinUi.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        internal readonly SearchService _searchService;
        [ObservableProperty] private List<IMUser> _resultUsers;
        [ObservableProperty] private List<IMMessage> _resultMessages;
        private readonly DispatcherQueue _dispatcherQueue;

        public SearchViewModel()
        {
            _searchService = Ioc.Default.GetRequiredService<SearchService>();
            _searchService.OnSearchResultReceived += HandleSearchResult;
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        ~SearchViewModel()
        {
            _searchService.OnSearchResultReceived -= HandleSearchResult;
        }

        [RelayCommand]
        internal async Task ExecuteSearch(string argsQueryText)
        {
            _searchService.SearchUser(argsQueryText);
        }

        internal void HandleSearchResult(SearchResult result)
        {
            Debug.WriteLine("搜索结果：" + result);
            switch (result.Type)
            {
                case "IMUserList":

                    _dispatcherQueue.TryEnqueue(() => { ResultUsers = result.Result as List<IMUser>; });
                    break;
            }
        }
    }
}