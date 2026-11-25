using MessengerDemo.Messages;
using StoneToolkit.Common.Messenger;

namespace MessengerDemo.Models
{
    /// <summary>
    /// 订阅者，通过消息总线注册接收消息。
    /// 注意：回调采用"不捕获 this"的写法，以保证弱引用机制生效。
    /// </summary>
    public sealed class Subscriber
    {
        private readonly string _name;

        public Subscriber(string name)
        {
            _name = name;
        }

        public string Name => _name;

        /// <summary>
        /// 注册接收指定类型的消息（无 token 通道）。
        /// </summary>
        public void Subscribe(IMessenger messenger)
        {
            messenger.Register<StatusChangedMessage>(this, (subscriber, message) =>
                ((Subscriber)subscriber).Handle(message));
        }

        /// <summary>
        /// 注册接收指定类型的消息，并通过 token 细分通道。
        /// </summary>
        public void SubscribeToToken(IMessenger messenger, object token)
        {
            messenger.Register<StatusChangedMessage>(this, token, (subscriber, message) =>
                ((Subscriber)subscriber).Handle(message));
        }

        /// <summary>
        /// 退订该订阅者的全部消息。
        /// </summary>
        public void Unsubscribe(IMessenger messenger)
        {
            messenger.UnregisterAll(this);
        }

        /// <summary>
        /// 处理收到的消息并输出到控制台。
        /// </summary>
        private void Handle(StatusChangedMessage message)
        {
            Console.WriteLine($"[{Name}] 收到 {message.Source}.{message.PropertyName} = {message.NewValue}");
        }
    }
}
