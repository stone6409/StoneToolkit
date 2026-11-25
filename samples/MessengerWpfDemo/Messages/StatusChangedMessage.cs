using StoneToolkit.Common.Messenger;

namespace MessengerWpfDemo.Messages
{
    /// <summary>
    /// 表示某个来源对象的某个属性/消息发生变化的消息。
    /// </summary>
    public sealed class StatusChangedMessage : IMessage
    {
        public StatusChangedMessage(string source, string propertyName, string newValue)
        {
            Source = source;
            PropertyName = propertyName;
            NewValue = newValue;
        }

        /// <summary>
        /// 来源对象名称。
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// 属性名。
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// 新值。
        /// </summary>
        public string NewValue { get; }
    }
}
