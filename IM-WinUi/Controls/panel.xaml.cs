using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IMWinUi.Controls
{
    public sealed partial class Panel : UserControl
    {
        public Panel()
        {
            InitializeComponent(); // 确保命名空间和类名与 XAML 文件中的 x:Class 一致
        }

        public static readonly new DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(UIElement),
            typeof(Panel),
            new PropertyMetadata(null, OnContentChanged));

        public new UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Panel panel)
            {
                panel.ContentArea.Content = e.NewValue; // 确保 ContentArea 的名称与 XAML 文件中的 x:Name 一致
            }
        }
    }
}