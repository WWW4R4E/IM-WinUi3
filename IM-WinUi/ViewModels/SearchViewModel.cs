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
        internal readonly SearchService SearchService;
        [ObservableProperty] private List<ResultInformation>? _resultUsers;
        [ObservableProperty] private List<LocalMessage>? _resultMessages;
        private readonly DispatcherQueue _dispatcherQueue;

        public SearchViewModel()
        {
            SearchService = Ioc.Default.GetRequiredService<SearchService>();
            SearchService.OnSearchResultReceived += HandleSearchResult;
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        ~SearchViewModel()
        {
            SearchService.OnSearchResultReceived -= HandleSearchResult;
        }

        [RelayCommand]
        internal void ExecuteSearch(string argsQueryText)
        {
            SearchService.SearchUser(argsQueryText);
        }

        internal void HandleSearchResult(SearchResult result)
        {
            // 序列化输出result
            
            Debug.WriteLine("查询结果为:"+ JsonSerializer.Serialize(result.Result));
            // switch (result.Type)
            // {
            //     case "IMUserList":

                    _dispatcherQueue.TryEnqueue(() => { ResultUsers = result.Result; });
                    // break;
            // }
        }
    }
}