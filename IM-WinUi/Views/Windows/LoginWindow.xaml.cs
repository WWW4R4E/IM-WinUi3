using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Helper;
using IMWinUi.Models;
using IMWinUi.Properties;
using IMWinUi.ViewModels;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace IMWinUi.Views;

public sealed partial class LoginWindow : Window
{
    private readonly AccountService _accountService;
    private bool _rememberLogin = Settings.Default.Remember;
    LocalDbContext db = Ioc.Default.GetRequiredService<LocalDbContext>();

    public LoginWindow()
    {
        ExtendsContentIntoTitleBar = true;
        SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
        _accountService = Ioc.Default.GetRequiredService<AccountService>();
        _accountService.OnUpdateDb += ToCommentPage;
        InitializeComponent();
        WinHelper.SetWindowSizeAndCenter(this, 680, 880);
        if (_rememberLogin)
        {
            if (Settings.Default.LastUserId != 0)
            {
                UserNameTextBox.Text = Settings.Default.LastUserId.ToString();
                PasswordBoxWithRevealMode.Password = Settings.Default.LastUserPassword;
            }
        }
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserNameTextBox.Text) &
            string.IsNullOrWhiteSpace(PasswordBoxWithRevealMode.Password))
        {
            WinHelper.ShowMessage("提示", "账号和密码不能为空", this.Content.XamlRoot);
        }
        else
        {
            var loginResult =
                await _accountService.LoginAsync(UserNameTextBox.Text, PasswordBoxWithRevealMode.Password);

            if (loginResult)
            {
                Debug.WriteLine("登录" + _rememberLogin);

                if (_rememberLogin)
                {
                    // 初始化对应数据库
                    db.InitializeDatabase(UserNameTextBox.Text);

                    // 保存登录用户名到设置
                    if (long.TryParse(UserNameTextBox.Text, out var userId))
                    {
                        Settings.Default.LastUserId = userId;
                    }
                    else
                    {
                        throw new ArgumentException("用户名不是有效的整数", nameof(UserNameTextBox.Text));
                    }

                    Settings.Default.LastUserPassword = PasswordBoxWithRevealMode.Password;
                    Settings.Default.Remember = true;
                    // 保存JWT令牌到设置
                    Settings.Default.JwtToken = _accountService.JwtToken;
                }
                else
                {
                    Settings.Default.Remember = false;
                }

                Debug.WriteLine("最终" + _rememberLogin);
                Settings.Default.Save();

                // 同步数据库
                await SyncDatabase();

                var currentApp = (App)Application.Current;
                currentApp.m_window = new MainWindow();
                currentApp.m_window.Activate();
                Close();
            }
            else
            {
                WinHelper.ShowMessage("提示", "登录失败，请检查用户名和密码或与服务器的网络连接。", this.Content.XamlRoot);
                _accountService.StartConnectionAsync();
            }
        }
    }

    private void ToCommentPage(object? sender, UpdateDbEventArgs e)
    {
        var currentApp = (App)Application.Current;
        currentApp.m_window = new MainWindow();
        currentApp.m_window.Activate();
        Close();
    }


    private async Task SyncDatabase()
    {
        // 初始化 LastSyncTime
        if (Settings.Default.LastSyncTime == DateTime.MinValue)
        {
            Settings.Default.LastSyncTime = DateTime.UtcNow.AddDays(-30); // 默认设置为 30 天前
            Settings.Default.Save();
        }

        await _accountService.GetDatabaseUpdatesAsync(Settings.Default.LastSyncTime);
        // 更新最后同步时间
        Settings.Default.LastSyncTime = DateTime.UtcNow;
        Settings.Default.Save();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void RetrieveButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
    {
        if (RevealModeCheckBox1.IsChecked == true)
            PasswordBoxWithRevealMode.PasswordRevealMode = PasswordRevealMode.Visible;
        else
            PasswordBoxWithRevealMode.PasswordRevealMode = PasswordRevealMode.Hidden;
    }
}