using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Models;
using IMWinUi.Services;
using IMWinUi.ViewModels;
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

        public App()
        {
            // 初始化服务容器
            var services = new ServiceCollection();

            // 配置 DbContext
            services.AddDbContext<LocalDbcontext>(options =>
            {
                var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var dbPath = System.IO.Path.Combine(projectDirectory, "localMIContext.db");
                options.UseSqlite($"Data Source={dbPath}");
            }, ServiceLifetime.Transient);

            // 配置 Ioc.Default
            Ioc.Default.ConfigureServices(services.BuildServiceProvider());

            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var context = Ioc.Default.GetRequiredService<LocalDbcontext>();
            context.Database.EnsureCreated();

            // 创建窗口实例后操作
            m_window = new LoginWindow();
            var windowAppWindow = m_window.AppWindow; // 获取Window的AppWindow属性

            // 配置窗口属性
            var presenter = OverlappedPresenter.Create();
            windowAppWindow.SetPresenter(presenter);
            windowAppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 470, Height = 680 });

            m_window.Activate(); // 移动到配置完成后激活窗口
        }


        public Window? m_window;
    }
}