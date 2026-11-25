
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace StoneToolkit.Web.Utilities
{
    public static class ApplicationLifecycleExtensions
    {
        public static IHost RegisterApplicationLifecycleEvents(
            this IHost host, 
            Func<Task> cleanupAction = null)
        {
            var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            var lifecycleManager = new ApplicationLifecycleManager(lifetime);

            if (cleanupAction != null)
            {
                lifecycleManager.OnCleanup += async () =>
                {
                    Console.WriteLine("执行主程序的清理逻辑...");
                    await cleanupAction();
                };
            }

            lifecycleManager.RegisterApplicationLifecycleEvents();
            return host;
        }
    }
}
