using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.UI.Xaml.Controls;

namespace IMWinUi.Services;

public class SearchService
{
    private HubConnection _hubConnection;

    public SearchService()
    {
        _ = IntializeAsync();
    }

    private async Task IntializeAsync()
    {
        Debug.WriteLine("初始化搜索服务");
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5001/search")
            .Build();
        // 监听服务器返回的搜索结果
        _hubConnection.On<object>("SearchResult", result =>
        {
            if (result is string message)
            {
                Debug.WriteLine($"搜索失败: {message}");
            }
            else
            {
                Debug.WriteLine($"搜索成功: {result}");
            }
        });

        // 启动连接
        await _hubConnection.StartAsync();
    }

    // 搜索用户名
    public async Task SearchUserName(string name)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("SearchUserName", name);
        }
        else
        {
            Debug.WriteLine("SignalR 连接未建立，无法发送搜索请求。");
        }
    }

    // 搜索用户 ID
    public async Task SearchUserId(int id)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("SearchUserId", id);
        }
        else
        {
            Debug.WriteLine("SignalR 连接未建立，无法发送搜索请求。");
        }
    }
}