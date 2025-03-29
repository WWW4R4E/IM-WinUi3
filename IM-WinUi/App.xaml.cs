using System;
using System.Net.Security;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;
using IMWinUi.Services;
using IMWinUi.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace IMWinUi
{
    public partial class App : Application
    {
        public NavigationService NavigationService { get; private set; }
        public ServiceCollection Services = new();

        public App()
        {

            // 配置 DbContext
            Services.AddDbContext<LocalDbcontext>(options =>
            {
                var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var dbPath = System.IO.Path.Combine(projectDirectory, "localMIContext.db");
                options.UseSqlite($"Data Source={dbPath}");
            }, ServiceLifetime.Transient);
            Services.AddSingleton<ChatClientService>();

            // 配置 Ioc.Default
            Ioc.Default.ConfigureServices(Services.BuildServiceProvider());

            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var context = Ioc.Default.GetRequiredService<LocalDbcontext>();
            context.Database.EnsureCreated();

            // 创建窗口实例后操作
            m_window = new LoginWindow();
            var windowAppWindow = m_window.AppWindow; 

            // 配置窗口属性
            var presenter = OverlappedPresenter.Create();
            windowAppWindow.SetPresenter(presenter);
            windowAppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 470, Height = 680 });

            m_window.Activate(); 
        }


        public Window? m_window;
    }
}