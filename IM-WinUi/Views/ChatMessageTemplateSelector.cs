using IMWinUi.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IMWinUi.Views
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MyTextTemplate { get; set; }
        public DataTemplate MyImageTemplate { get; set; }
        public DataTemplate OtherTextTemplate { get; set; }
        public DataTemplate OtherImageTemplate { get; set; }
        private readonly string _myName = Properties.Settings.Default.LastUserName;

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is IMMessage message)
            {
                if (message.SenderName == _myName)
                {
                    return message.Type == MessageType.Text ? MyTextTemplate : MyImageTemplate;
                }
                else
                {
                    return message.Type == MessageType.Text ? OtherTextTemplate : OtherImageTemplate;
                }
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}