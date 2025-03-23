using IMWinUi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IMWinUi.ViewModels
{
    internal class CommentPageViewModel : INotifyPropertyChanged
    {        
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<IMMessage> _messages;
        private ObservableCollection<IMUser> _users;
        private IMUser _selectUser;
        private string _chatInput;

        public ObservableCollection<IMUser> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<IMMessage> Messages
        {
            get => _messages;
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ChatInput
        {
            get => _chatInput;
            set
            {
                if (_chatInput != value)
                {
                    _chatInput = value;
                    OnPropertyChanged();
                }
            }
        }

        
        public CommentPageViewModel()
        {
            Users = new ObservableCollection<IMUser>
            {
                new IMUser(1, "张三"),
                new IMUser(2, "王五")
            };
            Messages = new ObservableCollection<IMMessage>();
        }

        public IMUser SelectUser
        {
            get => _selectUser;
            set
            {
                if (_selectUser != value)
                {
                    _selectUser = value;
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}