using System.Collections.Generic;
using System.IO;
using IMWinUi.Helper;
using Microsoft.UI.Xaml;

namespace IMWinUi.Views
{
    public sealed partial class NotificationWindow : Window
    {
        public bool IsPlaying;
        public string _fileName;

        public NotificationWindow(string file)
        {
            _fileName = Path.GetFileName(file);
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);
            WinHelper.SetWindowSizeAndCenter(this, 640, 220);
        }

        private void PlayPause_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
            {
                PlayPauseIcon.Glyph = "\uE769";
                IsPlaying = false;
            }
            else
            {
                PlayPauseIcon.Glyph = "\uEDB5";
                IsPlaying = true;
            }
        }
        
        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}