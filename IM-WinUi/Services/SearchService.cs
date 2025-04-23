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

        // 将On方法的参数类型改为object以接收不同类型结果
        _hubConnection.On<string>("SearchResult", result =>
        {
            try
            {
                // 尝试将 result 反序列化为 List<IMUser> 对象
                var users = JsonSerializer.Deserialize<List<IMUser>>(result);
                OnSearchResultReceived?.Invoke(new SearchResult
                {
                    Success = true,
                    Type = "IMUserList",
                    Result = users
                });
            }
            catch (JsonException)
            {
                // 反序列化失败，说明 result 是错误消息
                OnSearchResultReceived?.Invoke(new SearchResult
                {
                    Success = false,
                    Type = "Error",
                    Result = result
                });
            }
        });

        // 启动连接
        await _hubConnection.StartAsync();
    }

    // 搜索用户
    public async Task SearchUser(string searchTerm)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("SearchUser", searchTerm);
            Debug.WriteLine("发送搜索请求：" + searchTerm);
        }
        else
        {
            Debug.WriteLine("SignalR 连接未建立，无法发送搜索请求。");
        }
    }
}