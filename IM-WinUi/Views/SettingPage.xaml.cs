using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IMWinUi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingsViewModel ViewModel
        {
            get;
        }
        public SettingPage()
        {
            this.InitializeComponent();
        }
    }

    public class SettingsViewModel
    {
       public string UiThemeTitle;
        public string UiThemeDescription;
        public string UiThemeIcon;
        public string UiThemeSymbol;
        public string UiThemeSetting;
        public string Themes;
        public string ElementTheme;
        public object UpdateContentCommand { get; }
        public object RateCommand { get; }
        public object UpdateAvailable { get; }
        public object VersionDescription { get; }
        public object FixHorizontalPicture { get; set; }
        public object TimeAsHour { get; set; }
    }
}
