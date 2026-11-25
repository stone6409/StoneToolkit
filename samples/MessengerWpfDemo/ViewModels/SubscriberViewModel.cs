using MessengerWpfDemo.Messages;
using StoneToolkit.Common.ComponentModel;
using StoneToolkit.Common.Messenger;

namespace MessengerWpfDemo.ViewModels
{
    /// <summary>
    /// 订阅方 ViewModel，接收消息并更新自身属性，界面随之刷新。
    /// </summary>
    public sealed class SubscriberViewModel : ObservableObject
    {
        private string _receivedMessage = string.Empty;

        public SubscriberViewModel(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 订阅者名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 最近收到的消息文本。
        /// </summary>
        public string ReceivedMessage
        {
            get => _receivedMessage;
            private set
            {
                _receivedMessage = value;
                RaisePropertyChanged(nameof(ReceivedMessage));
            }
        }

        /// <summary>
        /// 注册接收无 token 通道的消息。
        /// </summary>
        public void Subscribe(IMessenger messenger)
        {
            messenger.Register<StatusChangedMessage>(this, (subscriber, message) =>
                ((SubscriberViewModel)subscriber).Handle(message));
        }

        /// <summary>
        /// 注册接收指定 token 通道的消息。
        /// </summary>
        public void SubscribeToToken(IMessenger messenger, object token)
        {
            messenger.Register<StatusChangedMessage>(this, token, (subscriber, message) =>
                ((SubscriberViewModel)subscriber).Handle(message));
        }

        /// <summary>
        /// 退订该订阅者的全部消息。
        /// </summary>
        public void Unsubscribe(IMessenger messenger)
        {
            messenger.UnregisterAll(this);
        }

        /// <summary>
        /// 处理收到的消息并更新自身属性。
        /// </summary>
        private void Handle(StatusChangedMessage message)
        {
            ReceivedMessage = $"[{Name}] 收到 {message.Source}.{message.PropertyName} = {message.NewValue}";
        }
    }
}
