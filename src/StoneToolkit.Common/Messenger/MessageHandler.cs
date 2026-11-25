namespace StoneToolkit.Common.Messenger
{
    /// <summary>
    /// 处理消息的委托，定义消息回调的签名。
    /// </summary>
    /// <typeparam name="TRecipient">接收消息的受体类型。</typeparam>
    /// <typeparam name="TMessage">消息类型。</typeparam>
    /// <param name="recipient">收到消息的受体对象。</param>
    /// <param name="message">消息实例。</param>
    public delegate void MessageHandler<TRecipient, TMessage>(TRecipient recipient, TMessage message)
        where TRecipient : class
        where TMessage : class;
}
