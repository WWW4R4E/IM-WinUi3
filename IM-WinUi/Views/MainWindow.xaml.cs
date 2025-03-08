using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace IMWinUi.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{

    public MainWindow()
    {   
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        contentFrame.Navigate(typeof(CommentPage), null);
    }

    private void IMNV_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var selectedItem = args.SelectedItem as NavigationViewItem;
        if (selectedItem != null)
        {
            var pageTag = selectedItem.Tag as string;

            if (!string.IsNullOrEmpty(pageTag))
            {
                Type pageType = Type.GetType($"IMWinUi.Views.{pageTag}");
                if (pageType != null)
                {
                    contentFrame.Navigate(pageType, null);
                }
                else
                {
                    Debug.WriteLine("没有找到对应页面");
                }
            }
        }
    }
}
