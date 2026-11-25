using System.Windows.Input;
using MessengerWpfDemo.Messages;
using StoneToolkit.Common.ComponentModel;
using StoneToolkit.Common.Messenger;
using StoneToolkit.WpfCommon.Input;

namespace MessengerWpfDemo.ViewModels
{
    /// <summary>
    /// 发布方 ViewModel，发送消息到消息总线。
    /// </summary>
    public sealed class PublisherViewModel : ObservableObject
    {
        private readonly IMessenger _messenger;
        private string _messageText = string.Empty;

        public PublisherViewModel(IMessenger messenger)
        {
            _messenger = messenger;
            SendCommand = new RelayCommand(Send);
        }

        /// <summary>
        /// 用户输入的消息文本。
        /// </summary>
        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                RaisePropertyChanged(nameof(MessageText));
            }
        }

        /// <summary>
        /// 发送消息的命令。
        /// </summary>
        public ICommand SendCommand { get; }

        private void Send()
        {
            if (string.IsNullOrWhiteSpace(MessageText))
            {
                return;
            }

            string text = MessageText.Trim();

            // 以 "X:" 开头则由 token 通道定向分发，否则无 token 广播
            if (text.Length >= 2 && text[1] == ':')
            {
                string token = text.Substring(0, 1);
                string value = text.Substring(2);
                _messenger.Send(new StatusChangedMessage(nameof(PublisherViewModel), "Message", value), token);
            }
            else
            {
                _messenger.Send(new StatusChangedMessage(nameof(PublisherViewModel), "Message", text));
            }

            MessageText = string.Empty;
        }
    }
}
