// App.xaml.cs

using System;
using IMWinUi.Models;
using IMWinUi.Services;
using IMWinUi.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace IMWinUi
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
 
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }
        
        
        public NavigationService NavigationService { get; private set; }

        public App()
        {
            // 初始化服务容器
            var services = new ServiceCollection();
        
            // 配置DbContext
            services.AddDbContext<LocalDbcontext>(options => 
            {
                var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var dbPath = System.IO.Path.Combine(projectDirectory, "localMIContext.db");
                options.UseSqlite($"Data Source={dbPath}");
            });
            
            // 构建服务提供者
            ServiceProvider = services.BuildServiceProvider();
            
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // 通过 DI 获取实例（假设已通过 App.ServiceProvider 获取）
            // using (var context = App.ServiceProvider.GetRequiredService<LocalDbcontext>())
            {
                var context = App.ServiceProvider.GetRequiredService<LocalDbcontext>();
                context.Database.EnsureCreated();
            }
            //if (string.IsNullOrWhiteSpace(Properties.Settings.Default.LastUserName) & string.IsNullOrWhiteSpace(Properties.Settings.Default.LastUserPassword))
            //{
            m_window = new LoginWindow();
            //m_window = new MainWindow();
            m_window.Activate();
            //}
            //else
            //{
            //    m_window = new MainWindow();
            //    m_window.Activate();
            //}
        }

        public Window? m_window;
    }
}
