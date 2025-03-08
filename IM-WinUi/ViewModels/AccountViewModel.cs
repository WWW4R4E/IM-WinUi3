using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IMWinUi.ViewModels
{
    internal class AccountViewModel
    {
        private HubConnection _hubConnection;
        private TaskCompletionSource<bool> _loginTaskCompletionSource;
        public string JwtToken { get; private set; } // 添加JWT令牌属性

        public AccountViewModel()
        {
            Debug.WriteLine("AccountViewModel 构造函数被调用。");
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("开始初始化 HubConnection...");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5287/AccountHub") // 修复 URL 路径
                .Build();

            // 注册登录结果回调
            _hubConnection.On<bool, string, string>("LoginResult", (success, message, jwtToken) =>
            {
                if (_loginTaskCompletionSource != null && !_loginTaskCompletionSource.Task.IsCompleted)
                {
                    if (success)
                    {
                        Debug.WriteLine("用户登录成功。");
                        JwtToken = jwtToken; // 存储JWT令牌
                        _loginTaskCompletionSource.SetResult(true);
                    }
                    else
                    {
                        Debug.WriteLine($"用户登录失败: {message}");
                        _loginTaskCompletionSource.SetResult(false);
                    }
                }
            });

            try
            {
                Debug.WriteLine("尝试启动 HubConnection...");
                await _hubConnection.StartAsync();

                // 检查连接状态以确认是否成功连接
                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    Debug.WriteLine("HubConnection 已成功启动。");
                }
                else
                {
                    Debug.WriteLine($"HubConnection 启动失败，当前状态: {_hubConnection.State}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"启动 HubConnection 失败: {ex.Message}");
            }
        }

        internal async Task<bool> LoginAsync(string userNameText, string passwordText)
        {
            try
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    Debug.WriteLine("HubConnection 未连接，无法进行登录操作。");
                    return false;
                }

                _loginTaskCompletionSource = new TaskCompletionSource<bool>();
                await _hubConnection.InvokeAsync("Login", userNameText, passwordText);
                Debug.WriteLine("用户登录请求已发送。");
                return await _loginTaskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"用户登录失败: {ex.Message}");
                return false;
            }
        }
    }
}
