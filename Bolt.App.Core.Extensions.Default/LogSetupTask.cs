using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bolt.App.Core.Extensions.Default
{
    internal sealed class LogSetupTask : IBootstrapperTask
    {
        private readonly ILoggerFactory _loggerFactory;

        public LogSetupTask(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        
        public Task RunAsync()
        {
            Log.Init(_loggerFactory);
            
            return Task.CompletedTask;
        }
    }
}