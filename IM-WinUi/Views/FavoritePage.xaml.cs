using IMWinUi.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IMWinUi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class FavoritePage : Page
    {
        ObservableCollection<IMMessage> Messages { get; set; } = new();
        public FavoritePage()
        {
            this.InitializeComponent();
            Messages.Add(new IMMessage(MessageType.Text, Properties.Settings.Default.LastUserName, "www", "Hello!"));
            Messages.Add(new IMMessage(MessageType.Image, Properties.Settings.Default.LastUserName, "张三", "C:\\Users\\123\\Desktop\\IM-WinUi\\Assets\\logo.jpg"));
            Messages.Add(new IMMessage(MessageType.Text, "www", "张三", "How are you?"));
        }
    }
}
