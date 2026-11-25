using MessengerDemo.Messages;
using StoneToolkit.Common.Messenger;

namespace MessengerDemo.Models
{
    /// <summary>
    /// 发布者，持有消息总线，负责发送消息。
    /// </summary>
    public sealed class Publisher
    {
        private readonly IMessenger _messenger;

        public Publisher(IMessenger messenger)
        {
            _messenger = messenger;
        }

        /// <summary>
        /// 通过指定 token 通道发布一次消息。
        /// </summary>
        public void Publish(string token, string propertyName, string newValue)
        {
            _messenger.Send(
                new StatusChangedMessage(GetType().Name, propertyName, newValue),
                token);
        }

        /// <summary>
        /// 通过无 token 通道广播一条消息。
        /// </summary>
        public void Broadcast(string propertyName, string newValue)
        {
            _messenger.Send(new StatusChangedMessage(GetType().Name, propertyName, newValue));
        }
    }
}
