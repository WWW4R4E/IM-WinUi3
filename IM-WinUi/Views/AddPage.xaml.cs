using Microsoft.UI.Xaml.Controls;

namespace IMWinUi.Views
{
    public sealed partial class AddPage : Page
    {
        public byte[] ProfilePicture { get; }
        public string Question { get; }
        public string Name { get; } = "群名";

        public AddPage(byte[] profilePicture, string? question)
        {
            ProfilePicture = profilePicture;
            Question = question ?? "和大家打个招呼吧!";
            InitializeComponent();
        }
    }
}
