using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;

namespace IMWinUi.ViewModels
{
    internal class AccountService
    {
        private HubConnection _hubConnection;
        private TaskCompletionSource<bool> _loginTaskCompletionSource;
        private LocalDbContext db = Ioc.Default.GetRequiredService<LocalDbContext>();
        public event EventHandler<UpdateDbEventArgs> OnUpdateDb;
        public string JwtToken { get; private set; } // 添加JWT令牌属性

        public AccountService()
        {
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            Debug.WriteLine("开始初始化Account HubConnection...");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5287/AccountHub") // 不传递 JWT 令牌
                .Build();

            _hubConnection.On<List<LocalUser>?, List<LocalMessage>?, List<LocalGroup>?>("HandleDatabaseUpdates",
                HandleDatabaseUpdates);

            // 注册登录结果回调
            _hubConnection.On<bool, string, string>("LoginResult", (success, message, jwtToken) =>
            {
                if (!_loginTaskCompletionSource.Task.IsCompleted)
                {
                    if (success)
                    {
                        JwtToken = jwtToken; // 存储JWT令牌
                        _loginTaskCompletionSource.SetResult(true);
                        _ = ReconnectWithTokenAsync(jwtToken); // 登录成功后重新连接并传递 JWT 令牌
                    }
                    else
                    {
                        _loginTaskCompletionSource.SetResult(false);
                    }
                }
            });

            try
            {
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
                // 新增：初始化失败时触发重连
                await StartConnectionAsync();
            }
        }

        private async Task ReconnectWithTokenAsync(string jwtToken)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync(); // 断开当前连接
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5287/AccountHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(jwtToken); // 传递 JWT 令牌
                })
                .Build();

            _hubConnection.On<List<LocalUser>?, List<LocalMessage>?, List<LocalGroup>?>("HandleDatabaseUpdates",
                HandleDatabaseUpdates);

            try
            {
                await _hubConnection.StartAsync();
                Debug.WriteLine("使用 JWT 令牌重新连接成功。");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"使用 JWT 令牌重新连接失败: {ex.Message}");
            }
        }


        internal async Task<bool> LoginAsync(string userIdText, string passwordText)
        {
            try
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    Debug.WriteLine("HubConnection 未连接，无法进行登录操作。");
                    return false;
                }

                _loginTaskCompletionSource = new TaskCompletionSource<bool>();
                await _hubConnection.InvokeAsync("Login", userIdText, passwordText);
                Debug.WriteLine("用户登录请求已发送。");
                return await _loginTaskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"用户登录失败: {ex.Message}");
                return false;
            }
        }


        // 重连方法
        internal async Task StartConnectionAsync()
        {
            int retryCount = 0;
            const int maxRetryCount = 5;
            const int initialDelay = 2000; // 初始延迟2秒
            int retryDelay = initialDelay;

            while (_hubConnection?.State != HubConnectionState.Connected && retryCount < maxRetryCount)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    retryCount = 0;
                    Debug.WriteLine("AccountHub重新连接成功");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    retryDelay = initialDelay * (int)Math.Pow(2, retryCount); // 指数退避
                    Debug.WriteLine($"AccountHub重连失败，第{retryCount}次尝试，将在{retryDelay}ms后重试: {ex.Message}");
                    await Task.Delay(retryDelay);
                }
            }
        }

        public async Task GetDatabaseUpdatesAsync(DateTime defaultLastSyncTime)
        {
            await _hubConnection.InvokeAsync("GetDatabaseUpdates", defaultLastSyncTime);
        }

        private async Task HandleDatabaseUpdates(List<LocalUser>? updateUsers, List<LocalMessage>? updateMessages,
            List<LocalGroup>? updateGroups)
        {
            try
            {
                db.SyncUsers(updateUsers);
                db.SyncMessages(updateMessages);
                db.SyncGroups(updateGroups);
                OnUpdateDb?.Invoke(this, new UpdateDbEventArgs { Success = true });
            }
            catch
            {
                Debug.WriteLine("同步数据库时出错");
            }
        }
    }

    internal class UpdateDbEventArgs : EventArgs
    {
        public bool Success { get; set; }
    }
}