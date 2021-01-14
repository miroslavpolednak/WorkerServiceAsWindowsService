using System;
using System.Collections.Generic;
using System.Text;

namespace Csob.Project.Common
{
    public class QuartzJobsConfig
    {
        public List<QuartzJob> Jobs { get; set; }
    }

    public class QuartzJob
    {
        public string JobName { get; set; }
        public string CronTrigger { get; set; }
        public string CalendarName { get; set; }
        public bool CallAfterStart { get; set; }
        public Dictionary<string, string> CustomValues { get; set; }
    }
}
