using System;
using System.Net.Security;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;
using IMWinUi.Services;
using IMWinUi.ViewModels;
using IMWinUi.Views;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace IMWinUi
{
    public partial class App : Application
    {
        public ServiceCollection Services = new();

        public App()
        {
            Services.AddSingleton<LocalDbContext>();
            Services.AddSingleton<ChatClientService>();
            Services.AddSingleton<AccountService>();
            Services.AddSingleton<SearchService>();
            Ioc.Default.ConfigureServices(Services.BuildServiceProvider());

            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {

            // 创建窗口实例后操作
            // m_window = new LoginWindow();
            m_window = new AddWindow( );
            m_window.Content = new AddPage(new byte[] {}, null);
            var windowAppWindow = m_window.AppWindow; 

            // 配置窗口属性
            var presenter = OverlappedPresenter.Create();
            windowAppWindow.SetPresenter(presenter);
            windowAppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 680, Height = 880 });

            m_window.Activate(); 
        }


        public Window? m_window;
    }
}