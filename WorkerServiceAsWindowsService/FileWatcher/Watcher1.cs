using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService.FileWatcher
{
    public class Watcher1 : WatcherBase
    {
        private readonly ILogger<Watcher1> _logger;

        public Watcher1(ILogger<Watcher1> logger, ILogger<WatcherBase> baseLogger) : base(baseLogger)
        {
            _logger = logger;
        }
        internal override Task ExecuteInternal(FileSystemWatcher fileSystemWatcher)
        {
            fileSystemWatcher.Path = @"C:\Test";
            fileSystemWatcher.Created += OnCreated;
            //fileSystemWatcher.Changed += OnCreated;
            fileSystemWatcher.Filter = "*.txt";
            fileSystemWatcher.EnableRaisingEvents = true;
            _logger.LogInformation($"Hello world from {nameof(Watcher1)}");
            return Task.CompletedTask;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            RetryPolicy policy = Policy.Handle<Exception>()
           .WaitAndRetry(5,
                         retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                         (exception, timeSpan, context) =>
          {
              _logger.LogError(exception, $"Polly time:{timeSpan}");
          });

            policy.Execute(() =>
             {
                 IEnumerable<string> lines = File.ReadLines(e.FullPath);
                 string message = string.Join(Environment.NewLine, lines);
                 _logger.LogInformation(message);

             });
        }
    }
}
