using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Csob.Project.WindowsService.Jobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _services;

        public JobFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                if (bundle == null)
                    throw new ArgumentException("Bundle cannot by null");
                using IServiceScope scope = _services.CreateScope();
                return (IJob)scope.ServiceProvider.GetService(bundle.JobDetail.JobType);
            }
            catch (Exception e)
            {
                throw new SchedulerException(string.Format(CultureInfo.InvariantCulture, "Job of type {0} wasn´t created !!!", nameof(bundle.JobDetail.JobType)), e);
            }
        }

        public void ReturnJob(IJob job)
        {

        }
    }
}
