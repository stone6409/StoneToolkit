using System.Collections.ObjectModel;
using StoneToolkit.Common.ComponentModel;
using StoneToolkit.Common.Messenger;

namespace MessengerWpfDemo.ViewModels
{
    /// <summary>
    /// 聚合 ViewModel，负责创建发布方与订阅方，并编排消息总线。
    /// </summary>
    public sealed class MainViewModel : ObservableObject
    {
        private readonly IMessenger _messenger;

        public MainViewModel(IMessenger messenger)
        {
            _messenger = messenger;

            Publisher = new PublisherViewModel(messenger);
            Subscribers = new ObservableCollection<SubscriberViewModel>();

            var broadcastSubscriber = new SubscriberViewModel("广播订阅者(无token)");
            broadcastSubscriber.Subscribe(messenger);
            Subscribers.Add(broadcastSubscriber);

            var channelASubscriber = new SubscriberViewModel("通道A订阅者");
            channelASubscriber.SubscribeToToken(messenger, "A");
            Subscribers.Add(channelASubscriber);

            var channelBSubscriber = new SubscriberViewModel("通道B订阅者");
            channelBSubscriber.SubscribeToToken(messenger, "B");
            Subscribers.Add(channelBSubscriber);
        }

        /// <summary>
        /// 发布方 ViewModel。
        /// </summary>
        public PublisherViewModel Publisher { get; }

        /// <summary>
        /// 订阅者集合，绑定到界面。
        /// </summary>
        public ObservableCollection<SubscriberViewModel> Subscribers { get; }
    }
}
