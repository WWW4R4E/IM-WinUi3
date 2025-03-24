using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IMWinUi.Services;
using IMWinUi.Properties;
using System.Diagnostics;

namespace IMWinUi.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public readonly NavigationService NavigationService;
    public MainWindow()
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        NavigationService = new NavigationService(ContentFrame, Imnv);
        NavigationService.NavigateTo("CommentPage", null);
    }


    private void IMNV_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var selectedItem = args.SelectedItem as NavigationViewItem;
        if (selectedItem != null)
        {
            var pageTag = selectedItem.Tag as string;
            if (!string.IsNullOrEmpty(pageTag))
            {
                Debug.WriteLine(pageTag);
                if (pageTag == "Settings")
                {
                    pageTag = "SettingPage";
                }
                NavigationService.NavigateTo(pageTag, null);

            }

        }
    }
}
