using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IMWinUi.Helper
{
    public class ShowDiaglog
    {
        internal static async Task ShowMessage(string title, string content, XamlRoot xamlRoot)
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = xamlRoot 
            };
            ContentDialogResult result = await noWifiDialog.ShowAsync();
        }
    }
}