using StoneToolkit.Common.Messenger;

namespace MessengerDemo.Messages
{
    /// <summary>
    /// 表示某个来源对象的某个属性发生变化的消息。
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
        /// 发生变化的属性名。
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// 新的属性值。
        /// </summary>
        public string NewValue { get; }
    }
}
