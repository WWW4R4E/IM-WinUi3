using CommunityToolkit.Mvvm.DependencyInjection;
using IMWinUi.Properties;
 using IMWinUi.Services;
 using IMWinUi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
 using Microsoft.UI.Xaml.Controls;
 using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Diagnostics;

 namespace IMWinUi.Views;

 public sealed partial class MainWindow : Window
 {
     public readonly NavigationService NavigationService;
     internal MainWindowVIewModel MainWindowVIewModel { get; }
     public MainWindow()
     {
         InitializeComponent();
         ExtendsContentIntoTitleBar = true;
         SetTitleBar(AppTitleBar);
        NavigationService = new NavigationService(ContentFrame, Imnv);
        Imnv.SelectedItem = Imnv.MenuItems[0];
     }


     private void IMNV_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
     {
         var selectedItem = args.SelectedItem as NavigationViewItem;
         if (selectedItem != null)
         {
             var pageTag = selectedItem.Tag as string;
             if (!string.IsNullOrEmpty(pageTag))
             {
                 if (pageTag == "Settings")
                 {
                     pageTag = "SettingPage";
                 }
                 NavigationService.NavigateTo(pageTag, null);    
             }

         }
     }
 }
