using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IMWinUi.Controls
{
    public sealed partial class SettingExpander : UserControl
    {
        public SettingExpander()
        {
            this.InitializeComponent();
        }

        // IsExpanded 属性
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SettingExpander), new PropertyMetadata(false));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // 新增SettingGlyph属性
        public static readonly DependencyProperty SettingGlyphProperty =
            DependencyProperty.Register("SettingGlyph", typeof(string), typeof(SettingExpander), new PropertyMetadata("")); // 默认值保持原示例Glyph

        public string SettingGlyph
        {
            get { return (string)GetValue(SettingGlyphProperty); }
            set { SetValue(SettingGlyphProperty, value); }
        }

        // SettingTitle 属性
        public static readonly DependencyProperty SettingTitleProperty =
            DependencyProperty.Register("SettingTitle", typeof(string), typeof(SettingExpander), new PropertyMetadata(string.Empty));

        public string SettingTitle
        {
            get { return (string)GetValue(SettingTitleProperty); }
            set { SetValue(SettingTitleProperty, value); }
        }

        // SettingDescription 属性
        public static readonly DependencyProperty SettingDescriptionProperty =
            DependencyProperty.Register("SettingDescription", typeof(string), typeof(SettingExpander), new PropertyMetadata(string.Empty));

        public string SettingDescription
        {
            get { return (string)GetValue(SettingDescriptionProperty); }
            set { SetValue(SettingDescriptionProperty, value); }
        }

        // SettingContent 属性
        public static readonly DependencyProperty SettingContentProperty =
            DependencyProperty.Register("SettingContent", typeof(object), typeof(SettingExpander), new PropertyMetadata(null));

        public object SettingContent
        {
            get { return GetValue(SettingContentProperty); }
            set { SetValue(SettingContentProperty, value); }
        }
    }
}
