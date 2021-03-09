using Csob.Project.Common;
using Csob.Project.WindowsService.CelendarAdapter;
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
        private readonly IQuartzCalendarManager _quartzCalendarManager;
        private static StdSchedulerFactory factory;
        private static IScheduler scheduler;

        public DefaultWorker(ILogger<DefaultWorker> logger,
                             IServiceProvider services,
                             IOptions<QuartzJobsConfig> quartzConfig,
                             IQuartzCalendarManager quartzCalendarManager)
        {
            _logger = logger;
            _services = services;
            _quartzConfig = quartzConfig;
            _quartzCalendarManager = quartzCalendarManager;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            //Add default calendars
            _quartzCalendarManager.AddDefaultCalendars();

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
                Thread thread = Thread.CurrentThread;
                string threadName = thread.Name;
                _logger.LogInformation("Service {time} {threadName}", DateTimeOffset.Now, threadName);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task RegisterAllQuartzJobs()
        {
            //Get all jobs type (those jobs are registered in IOC automatically)
            var jobs = DiHelper.GetAllTypesThatImplement(typeof(IJob));
            foreach (Type job in jobs)
            {
                //ToDo add calendar logic

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

                    if (!string.IsNullOrWhiteSpace(jobConfiguration.CalendarName))
                    {
                        NamedCalendar calendar = _quartzCalendarManager.GetSpecificCalendar(jobConfiguration.CalendarName);
                        await scheduler.AddCalendar(jobConfiguration.CalendarName, calendar, false, true);
                        cronTriggerConf.ModifiedByCalendar(jobConfiguration.CalendarName);
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
