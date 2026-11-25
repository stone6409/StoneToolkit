using System;
using System.Collections.Generic;

namespace StoneToolkit.Common.Messenger
{
    /// <summary>
    /// 弱引用消息总线实现，基于 CommunityToolkit.Mvvm 的 WeakReferenceMessenger 设计理念。
    /// 通过弱引用跟踪订阅者，在订阅者被回收后自动清理，避免内存泄漏。
    /// </summary>
    public class WeakReferenceMessenger : IMessenger
    {
        /// <summary>
        /// 获取默认的消息总线实例。
        /// </summary>
        public static WeakReferenceMessenger Default { get; } = new WeakReferenceMessenger();

        private readonly object _lock = new object();
        private readonly Dictionary<Type, object> _map = new Dictionary<Type, object>();

        /// <summary>
        /// 初始化一个新的消息总线实例。
        /// </summary>
        public WeakReferenceMessenger()
        {
        }

        /// <inheritdoc />
        public void Register<TMessage>(object recipient, MessageHandler<object, TMessage> handler)
            where TMessage : class
        {
            Register(recipient, null, handler);
        }

        /// <inheritdoc />
        public void Register<TMessage>(object recipient, object token, MessageHandler<object, TMessage> handler)
            where TMessage : class
        {
            if (recipient == null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (_lock)
            {
                List<Subscription<TMessage>> list = GetOrCreateList<TMessage>();
                list.RemoveAll(subscription => subscription.Matches(recipient) && Equals(subscription.Token, token));
                list.Add(new Subscription<TMessage>(recipient, token, handler));
            }
        }

        /// <inheritdoc />
        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            Send<TMessage, object>(message, null);
        }

        /// <inheritdoc />
        public void Send<TMessage, TToken>(TMessage message, TToken token)
            where TMessage : class
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            MessageHandler<object, TMessage>[] handlers;
            object[] recipients;

            lock (_lock)
            {
                if (!_map.TryGetValue(typeof(TMessage), out object rawList))
                {
                    return;
                }

                SubscriptionList<TMessage> list = (SubscriptionList<TMessage>)rawList;
                list.Items.RemoveAll(subscription => !subscription.IsAlive);

                var handlerBuffer = new List<MessageHandler<object, TMessage>>();
                var recipientBuffer = new List<object>();
                foreach (Subscription<TMessage> subscription in list.Items)
                {
                    if (Equals(subscription.Token, token) && subscription.Recipient.TryGetTarget(out object recipient))
                    {
                        handlerBuffer.Add(subscription.Handler);
                        recipientBuffer.Add(recipient);
                    }
                }

                handlers = handlerBuffer.ToArray();
                recipients = recipientBuffer.ToArray();
            }

            for (int i = 0; i < handlers.Length; i++)
            {
                handlers[i](recipients[i], message);
            }
        }

        /// <inheritdoc />
        public void Unregister<TMessage>(object recipient)
            where TMessage : class
        {
            if (recipient == null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            lock (_lock)
            {
                if (_map.TryGetValue(typeof(TMessage), out object rawList))
                {
                    SubscriptionList<TMessage> list = (SubscriptionList<TMessage>)rawList;
                    list.Items.RemoveAll(subscription => subscription.Matches(recipient));
                }
            }
        }

        /// <inheritdoc />
        public void Unregister<TMessage>(object recipient, object token)
            where TMessage : class
        {
            if (recipient == null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            lock (_lock)
            {
                if (_map.TryGetValue(typeof(TMessage), out object rawList))
                {
                    SubscriptionList<TMessage> list = (SubscriptionList<TMessage>)rawList;
                    list.Items.RemoveAll(subscription => subscription.Matches(recipient) && Equals(subscription.Token, token));
                }
            }
        }

        /// <inheritdoc />
        public void UnregisterAll(object recipient)
        {
            if (recipient == null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            lock (_lock)
            {
                foreach (KeyValuePair<Type, object> entry in _map)
                {
                    ((IRecipientSubscriptions)entry.Value).RemoveByRecipient(recipient);
                }
            }
        }

        /// <summary>
        /// 获取或创建指定消息类型的订阅集合。
        /// 必须在锁内调用。
        /// </summary>
        private List<Subscription<TMessage>> GetOrCreateList<TMessage>()
            where TMessage : class
        {
            if (_map.TryGetValue(typeof(TMessage), out object rawList))
            {
                return ((SubscriptionList<TMessage>)rawList).Items;
            }

            var list = new SubscriptionList<TMessage>();
            _map[typeof(TMessage)] = list;
            return list.Items;
        }

        /// <summary>
        /// 非泛型订阅集合接口，用于跨类型统一退订。
        /// </summary>
        private interface IRecipientSubscriptions
        {
            void RemoveByRecipient(object recipient);
        }

        /// <summary>
        /// 指定消息类型的订阅集合容器。
        /// </summary>
        private sealed class SubscriptionList<TMessage> : IRecipientSubscriptions
            where TMessage : class
        {
            public List<Subscription<TMessage>> Items { get; } = new List<Subscription<TMessage>>();

            public void RemoveByRecipient(object recipient)
            {
                Items.RemoveAll(subscription => subscription.Matches(recipient));
            }
        }

        /// <summary>
        /// 单个订阅项，弱引用持有订阅者。
        /// </summary>
        private sealed class Subscription<TMessage>
            where TMessage : class
        {
            public WeakReference<object> Recipient { get; }
            public object Token { get; }
            public MessageHandler<object, TMessage> Handler { get; }

            public Subscription(object recipient, object token, MessageHandler<object, TMessage> handler)
            {
                Recipient = new WeakReference<object>(recipient);
                Token = token;
                Handler = handler;
            }

            public bool IsAlive => Recipient.TryGetTarget(out object target) && target != null;

            public bool Matches(object recipient)
            {
                return Recipient.TryGetTarget(out object target) && ReferenceEquals(target, recipient);
            }
        }
    }
}
