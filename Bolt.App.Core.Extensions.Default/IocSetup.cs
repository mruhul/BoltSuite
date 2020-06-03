using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.App.Core.Extensions.Default
{
    public class AppCoreIocSetupOptions
    {
        /// <summary>
        /// Setup the static <see cref="Log"/> instance to use directly from class without
        /// injecting <see cref="ILogger{T}"/> in all places using bootstrapper task.
        /// In order to make this work you need to set this property true and also
        /// make sure SkipRunningBootstrapperTasks is false
        /// </summary>
        public bool SetupDefaultLogInstance { get; set; } = true;
        
        /// <summary>
        /// When set to false the system use a service host to run all registered impl of <see cref="IBootstrapperTask"/>
        /// on application start
        /// </summary>
        public bool SkipRunningBootstrapperTasks { get; set; } = false;
    }
    
    public static class IocSetup
    {
        /// <summary>
        /// Register all implementations in your ioc. You alos can n
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        public static void AddBoltCore(this IServiceCollection services, AppCoreIocSetupOptions options = null)
        {
            options ??= new AppCoreIocSetupOptions();
            
            services.TryAddSingleton<IAppInfoProvider, AppInfoProvider>();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IConfig<>), typeof(DefaultConfig<>)));
            services.TryAddScoped<IContextStore,ContextStore>();

            if (options.SkipRunningBootstrapperTasks) return;

            if (options.SetupDefaultLogInstance)
            {
                services.TryAddEnumerable(ServiceDescriptor.Transient<IBootstrapperTask, LogSetupTask>());
            }

            services.AddHostedService<BootstrapperTaskRunner>();
        }
    }
}