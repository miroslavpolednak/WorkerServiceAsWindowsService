using Csob.Calendar;
using Csob.Project.Common;
using Csob.Project.WindowsService.CelendarAdapter;
using Csob.Project.WindowsService.FileWatcher;
using Csob.Project.WindowsService.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService
{
    public class Program
    {

        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            SetWorkingDirectory();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new ConfigurationBuilder()
                                       .SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile("appsettings.json");

            Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            var quartzConfig = Configuration.GetSection("QuartzJobsConfig");
            List<Type> jobTypes = DiHelper.GetAllTypesThatImplement(typeof(IJob));
            List<Type> watcherTypes = DiHelper.GetAllTypesThatImplement(typeof(IWatcher));

            return Host.CreateDefaultBuilder(args)
                      .UseWindowsService()
                      .ConfigureServices((hostContext, services) =>
                      {
                          services.AddLogging(logging => logging.AddCsobLogging(Configuration));
                          services.AddHostedService<DefaultWorker>();
                          services.AddHostedService<TestWorker1>();
                          services.Configure<QuartzJobsConfig>(quartzConfig);
                          services.AddSingleton<IDaysGenerator, DaysGenerator>();
                          services.AddSingleton<IQuartzCalendarManager, QuartzCalendarManager>();
                          //All jobs registration
                          jobTypes.ForEach(s => services.AddScoped(s));
                          watcherTypes.ForEach(s => services.AddSingleton(s));
                      });

        }

        private static void SetWorkingDirectory()
        {
            // Get the current directory.
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            string directory = Path.GetDirectoryName(path);
            // Change the current directory.
            Environment.CurrentDirectory = directory;
        }
    }
}
