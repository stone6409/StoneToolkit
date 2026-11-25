using MessengerDemo.Models;
using StoneToolkit.Common.Messenger;

namespace MessengerDemo
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("========== Messenger 示例 ==========");

            var messenger = new WeakReferenceMessenger();
            var publisher = new Publisher(messenger);

            // 步骤1：基础订阅/发布（无 token 通道）
            Console.WriteLine();
            Console.WriteLine("--步骤1）基础订阅/发布--");
            var subscriberA = new Subscriber("A");
            subscriberA.Subscribe(messenger);
            publisher.Broadcast("Status", "Running");

            // 步骤2：token 通道细分
            Console.WriteLine();
            Console.WriteLine("--步骤2）token 通道细分--");
            var subscriberB = new Subscriber("B");
            var subscriberC = new Subscriber("C");
            subscriberB.SubscribeToToken(messenger, "ch-A");
            subscriberC.SubscribeToToken(messenger, "ch-B");
            publisher.Publish("ch-A", "Speed", "100");
            publisher.Publish("ch-B", "Speed", "200");

            // 步骤3：退订
            Console.WriteLine();
            Console.WriteLine("--步骤3）退订--");
            subscriberA.Unsubscribe(messenger);
            publisher.Broadcast("Status", "Stopped");

            // 步骤4：弱引用清理（订阅者被 GC 后不再收到消息）
            Console.WriteLine();
            Console.WriteLine("--步骤4）弱引用清理--");
            CreateAndSubscribe(messenger, "D");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            publisher.Broadcast("Status", "AfterGC");

            Console.WriteLine();
            Console.WriteLine("演示结束。按任意键退出。");
            if (Console.IsInputRedirected == false)
            {
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 创建并注册一个临时订阅者，方法返回后不再持有其强引用。
        /// </summary>
        private static void CreateAndSubscribe(IMessenger messenger, string name)
        {
            var temporary = new Subscriber(name);
            temporary.Subscribe(messenger);
            Console.WriteLine($"(已创建临时订阅者 {name}，注册后立即失去强引用)");
        }
    }
}
