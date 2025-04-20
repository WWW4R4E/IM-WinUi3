using System.Diagnostics;
using Windows.Graphics;
using IMWinUi.Helper;
using IMWinUi.Properties;
using IMWinUi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinRT.Interop;

namespace IMWinUi.Views;

public sealed partial class LoginWindow : Window
{
    private readonly AccountService _accountService = new();
    private bool _rememberLogin = Settings.Default.Remember;

    public LoginWindow()
    {
        SystemBackdrop = new MicaBackdrop { Kind = MicaKind.BaseAlt };
        ExtendsContentIntoTitleBar = true;
        InitializeComponent();
        if (_rememberLogin)
        {
            UserNameTextBox.Text = Settings.Default.LastUserName;
            PasswordBoxWithRevealMode.Password = Settings.Default.LastUserPassword;
        }
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserNameTextBox.Text) &
            string.IsNullOrWhiteSpace(PasswordBoxWithRevealMode.Password))
        {
            ShowDiaglog.ShowMessage("提示", "用户名和密码不能为空",this.Content.XamlRoot);
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
                    // 保存登录用户名到设置
                    Settings.Default.LastUserName = UserNameTextBox.Text;
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

                var currentApp = (App)Application.Current;
                currentApp.m_window = new MainWindow();
                currentApp.m_window.Activate();
                Close();
            }
            else
            {
                ShowDiaglog.ShowMessage("提示", "登录失败，请检查用户名和密码或与服务器的网络连接。",this.Content.XamlRoot);
            }
        }
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