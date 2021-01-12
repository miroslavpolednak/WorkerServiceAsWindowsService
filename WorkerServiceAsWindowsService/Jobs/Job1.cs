using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService.Jobs
{
    public class Job1 : BaseJob
    {
        private readonly ILogger<Job1> _logger;

        public Job1(ILogger<Job1> logger, ILogger<BaseJob> baselogger) : base(baselogger)
        {
            _logger = logger;
        }

        internal override async Task ExecuteInternal(IJobExecutionContext context)
        {
            _logger.LogInformation("Job1 started");
            await Task.CompletedTask;
        }
    }
}
