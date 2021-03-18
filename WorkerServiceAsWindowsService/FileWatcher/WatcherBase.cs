using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService.FileWatcher
{
    public abstract class WatcherBase : IWatcher
    {
        private readonly FileSystemWatcher _watcher;
        private readonly ILogger<WatcherBase> _logger;

        public WatcherBase(ILogger<WatcherBase> baseLogger)
        {
            _watcher = new FileSystemWatcher();
            _logger = baseLogger;
        }

        internal abstract Task ExecuteInternal(FileSystemWatcher fileSystemWatcher);

        public async Task StartWatching()
        {
            try
            {
                await ExecuteInternal(_watcher);
                //ToDo move file to history
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"{nameof(WatcherBase)}");
                //ToDo move file to error folder
            }
        }
    }
}
