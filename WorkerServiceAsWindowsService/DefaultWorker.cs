using Csob.Project.Common;
using Csob.Project.WindowsService.Helpers;
using Csob.Project.WindowsService.Jobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService
{
    public class DefaultWorker : BackgroundService
    {
        private readonly ILogger<DefaultWorker> _logger;
        private readonly IServiceProvider _services;
        private readonly IOptions<QuartzJobsConfig> _quartzConfig;
        private static StdSchedulerFactory factory;
        private static IScheduler scheduler;

        public DefaultWorker(ILogger<DefaultWorker> logger, IServiceProvider services, IOptions<QuartzJobsConfig> quartzConfig)
        {
            _logger = logger;
            _services = services;
            _quartzConfig = quartzConfig;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            IJobFactory jobFactory = new JobFactory(_services);
            // Grab the Scheduler instance from the Factory
            NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
            factory = new StdSchedulerFactory(props);
            scheduler = await factory.GetScheduler();
            scheduler.JobFactory = jobFactory;

            //Register all jobs to quartz
            await RegisterAllQuartzJobs();

            await scheduler.Start();

            _logger.LogInformation("Services started");


            await base.StartAsync(cancellationToken);
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await scheduler.Shutdown();
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Service is live at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task RegisterAllQuartzJobs()
        {
            //Get all jobs type (those jobs are registered in IOC automatically)
            var jobs = DiHelper.GetAllTypesThatImplement(typeof(IJob));
            foreach (Type job in jobs)
            {
                QuartzJob jobConfiguration = _quartzConfig.Value?.Jobs?.FirstOrDefault(r => r.JobName == job.Name);
                if (jobConfiguration == null)
                {
                    throw new ArgumentNullException($"There is no configuration for Job {job.Name}, pleas add configuration to appsettings.json");
                }
                if (jobConfiguration.CallAfterStart)
                {
                    IJobDetail jobAfterStart = JobBuilder.Create(job)
                    .WithIdentity($"{job.Name}AfterStart")
                    .Build();
                    ITrigger triggerAfterStart = TriggerBuilder.Create()
                         .WithIdentity($"{job.Name}triggerAfterStart")
                         .StartNow()
                         .Build();
                    //Run only after service start
                    await scheduler.ScheduleJob(jobAfterStart, triggerAfterStart);
                }
                if (!string.IsNullOrWhiteSpace(jobConfiguration.CronTrigger))
                {
                    IJobDetail cronJob = JobBuilder.Create(job)
                    .WithIdentity(job.Name)
                    .Build();
                    var cronTriggerConf = TriggerBuilder.Create()
                    .WithIdentity($"{job.Name}trigger")
                    .WithCronSchedule(jobConfiguration.CronTrigger)
                    .ForJob(job.Name);

                    bool existCalendar = false;
                    //ToDo implenet calendar logic
                    if (existCalendar)
                    {
                        cronTriggerConf.ModifiedByCalendar("CalendarName");
                    }

                    var cronTrigger = cronTriggerConf.Build();
                    await scheduler.ScheduleJob(cronJob, cronTrigger);
                }
            }
        }
        public override void Dispose()
        {

        }
    }
}
