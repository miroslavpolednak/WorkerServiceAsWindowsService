using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService.FileWatcher
{
    public class Watcher2 : WatcherBase
    {
        private readonly ILogger<Watcher2> _logger;

        public Watcher2(ILogger<Watcher2> logger, ILogger<WatcherBase> baseLogger) : base(baseLogger)
        {
            _logger = logger;
        }

        internal override Task ExecuteInternal(FileSystemWatcher fileSystemWatcher)
        {
            // Example offexception
            throw new NotSupportedException("This action is not supported");
            _logger.LogInformation($"Information from {nameof(Watcher2)}");
            return Task.CompletedTask;
        }
    }
}
