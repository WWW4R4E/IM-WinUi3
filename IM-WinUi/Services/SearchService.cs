using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Threading.Tasks;
using IMWinUi.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.UI.Xaml.Controls;

namespace IMWinUi.Services;

public class SearchService
{
    public event Action<SearchResult> OnSearchResultReceived;
    private HubConnection _hubConnection;

    public SearchService()
    {
        _ = IntializeAsync();
    }

    private async Task IntializeAsync()
    {
        Debug.WriteLine("初始化搜索服务");
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5287/Searchhub")
            .Build();
        
        // 注册不同类型的搜索结果处理
        RegisterSearchHandler("SearchUserResult", "UserList");
        RegisterSearchHandler("SearchGroupResult", "GroupList");
        
        // 启动连接
        await _hubConnection.StartAsync();
    }

    private void RegisterSearchHandler(string eventName, string resultType)
    {
        _hubConnection.On<string>(eventName, result =>
        {
            try
            {
                Debug.WriteLine($"[{eventName}] 接收到搜索结果：" + result);
                var users = JsonSerializer.Deserialize<List<ResultInformation>>(result);
                OnSearchResultReceived?.Invoke(new SearchResult
                {
                    Success = true,
                    Type = resultType,
                    Result = users
                });
            }
            catch (JsonException)
            {
                OnSearchResultReceived?.Invoke(new SearchResult
                {
                    Success = false,
                    Type = "Error"
                });
            }
        });
    }
    // 搜索用户
    public void SearchUser(string searchTerm)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
             _hubConnection.SendAsync("SearchUser", searchTerm);
            Debug.WriteLine("发送搜索请求：" + searchTerm);
        }
        else
        {
            Debug.WriteLine("SignalR 连接未建立，无法发送搜索请求。");
        }
    }
}