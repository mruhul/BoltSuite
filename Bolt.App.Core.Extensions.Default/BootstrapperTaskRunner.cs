using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bolt.App.Core.Extensions.Default
{
    internal sealed class BootstrapperTaskRunner : IHostedService
    {
        private readonly IEnumerable<IBootstrapperTask> _tasks;

        public BootstrapperTaskRunner(IEnumerable<IBootstrapperTask> tasks)
        {
            _tasks = tasks;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Info($"Start running hosted service {nameof(BootstrapperTaskRunner)}");

            if (_tasks == null) return;

            foreach (var task in _tasks)
            {
                try
                {
                    await task.RunAsync();
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Bootstrapper task {task.GetType()} failed with message {e.Message}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Info($"Stopping {nameof(BootstrapperTaskRunner)}...");
            return Task.CompletedTask;
        }
    }
}