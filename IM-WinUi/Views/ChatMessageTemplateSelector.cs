﻿using IMWinUi.Models;
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
        private readonly long _myId = Properties.Settings.Default.LastUserId;

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is LocalMessage message)
            {
                if (message.SenderId == _myId)
                {
                    return message.ContentType == MessageType.Text ? MyTextTemplate : MyImageTemplate;
                }
                else
                {
                    return message.ContentType == MessageType.Text ? OtherTextTemplate : OtherImageTemplate;
                }
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}