// App.xaml.cs
using IMWinUi.Models;
using IMWinUi.Services;
using IMWinUi.Views;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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
        public NavigationService NavigationService { get; private set; }

        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // 确保数据库和表被创建
            using (var context = new LocalDbcontext())
            {
                context.Database.EnsureCreated();
            }
            //if (string.IsNullOrWhiteSpace(Properties.Settings.Default.LastUserName) & string.IsNullOrWhiteSpace(Properties.Settings.Default.LastUserPassword))
            //{
            //m_window = new LoginWindow();
            m_window = new MainWindow();
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
