using System;

namespace StoneToolkit.Common.Messenger
{
    /// <summary>
    /// 定义消息总线接口，用于在不同对象之间进行松耦合的消息发布与订阅。
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// 注册一个订阅者，接收指定类型的消息。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="recipient">订阅者受体对象。</param>
        /// <param name="handler">消息回调。</param>
        void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler)
            where TMessage : class;

        /// <summary>
        /// 注册一个订阅者，通过 token 区分同一类型消息的不同通道。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="recipient">订阅者受体对象。</param>
        /// <param name="token">通道标记。</param>
        /// <param name="handler">消息回调。</param>
        void Register<TMessage>(object recipient, object token, MessageHandler<object, TMessage> handler)
            where TMessage : class;

        /// <summary>
        /// 发送消息，通知所有匹配的订阅者。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="message">消息实例。</param>
        void Send<TMessage>(TMessage message)
            where TMessage : class;

        /// <summary>
        /// 发送消息，通过 token 区分通道，通知匹配的订阅者。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <typeparam name="TToken">token 类型。</typeparam>
        /// <param name="message">消息实例。</param>
        /// <param name="token">通道标记。</param>
        void Send<TMessage, TToken>(TMessage message, TToken token)
            where TMessage : class;

        /// <summary>
        /// 退订一个订阅者对该类型消息的所有注册。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="recipient">订阅者受体对象。</param>
        void Unregister<TMessage>(object recipient)
            where TMessage : class;

        /// <summary>
        /// 退订一个订阅者对该类型且对应 token 的注册。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="recipient">订阅者受体对象。</param>
        /// <param name="token">通道标记。</param>
        void Unregister<TMessage>(object recipient, object token)
            where TMessage : class;

        /// <summary>
        /// 退订该订阅者的全部消息注册。
        /// </summary>
        /// <param name="recipient">订阅者受体对象。</param>
        void UnregisterAll(object recipient);
    }
}
