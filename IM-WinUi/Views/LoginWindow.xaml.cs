using IMWinUi.Helper;
using IMWinUi.Properties;
using IMWinUi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using Windows.Graphics;
using WinRT.Interop;


namespace IMWinUi.Views
{
    public sealed partial class LoginWindow : Window
    {
        bool RememberLogin = Properties.Settings.Default.Remember;
        AccountViewModel _accountViewModel = new AccountViewModel();
        public LoginWindow()
        {
            this.SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.BaseAlt };
            ExtendsContentIntoTitleBar = true;
            this.InitializeComponent();
            // 获取窗口句柄
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

            // 获取 AppWindow
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                // 设置窗口大小
                int width = 470;
                int height = 680;
                appWindow.Resize(new SizeInt32(width, height));

                // 获取显示器的工作区域
                DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                RectInt32 workArea = displayArea.WorkArea;

                // 计算居中位置
                int x = (workArea.Width - width) / 2 + workArea.X;
                int y = (workArea.Height - height) / 2 + workArea.Y;

                // 移动窗口到居中位置
                appWindow.MoveAndResize(new RectInt32(x, y, width, height));
                Debug.WriteLine(RememberLogin);
                if (RememberLogin)
                {
                    userNameTextBox.Text = Settings.Default.LastUserName;
                    passworBoxWithRevealmode.Password = Settings.Default.LastUserPassword;
                }
            }
        }
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userNameTextBox.Text) & string.IsNullOrWhiteSpace(passworBoxWithRevealmode.Password))
            {
                ShowDiaglog.ShowMessage("提示", "用户名和密码不能为空");
            }
            else
            {
                bool loginResult = await _accountViewModel.LoginAsync(userNameTextBox.Text, passworBoxWithRevealmode.Password);

                if (loginResult)
                {
                    Debug.WriteLine("登录" + RememberLogin);

                    if (RememberLogin)
                    {
                        // 保存登录用户名到设置
                        Settings.Default.LastUserName = userNameTextBox.Text;
                        Settings.Default.LastUserPassword = passworBoxWithRevealmode.Password;
                        Settings.Default.Remember = true;
                        // 保存JWT令牌到设置
                        Settings.Default.JwtToken = _accountViewModel.JwtToken;
                    }
                    else
                    {
                        Settings.Default.Remember = false;
                    }
                    Debug.WriteLine("最终"+ RememberLogin);
                    Settings.Default.Save();


                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Activate();
                    this.Close();
                }
                else
                {
                    ShowDiaglog.ShowMessage("提示", "登录失败，请检查用户名和密码或与服务器的网络连接。");
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
            if (revealModeCheckBox1.IsChecked == true)
            {
                passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }
    }
}
