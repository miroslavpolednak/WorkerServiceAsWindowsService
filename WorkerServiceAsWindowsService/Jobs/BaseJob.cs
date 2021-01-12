using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService.Jobs
{
    public abstract class BaseJob : IJob
    {
        private readonly ILogger<BaseJob> _logger;
        
        public BaseJob(ILogger<BaseJob> logger)
        {
            _logger = logger;
        }

        internal abstract Task ExecuteInternal(IJobExecutionContext context);

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await ExecuteInternal(context);
            }
            catch (Exception exp)
            {

                _logger.LogError(exp, "Unhandled exception occur");
            }
        }


    }
}
