using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService.Jobs
{
    public class Job2 : BaseJob
    {
        private readonly ILogger<Job2> _logger;

        public Job2(ILogger<Job2> logger, ILogger<BaseJob> baselogger) : base(baselogger)
        {
            _logger = logger;
        }
        internal override async Task ExecuteInternal(IJobExecutionContext context)
        {
            _logger.LogInformation("Job2 started");
            await Task.CompletedTask;
        }
    }
}
