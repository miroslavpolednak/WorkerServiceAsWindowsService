using Quartz.Impl.Calendar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csob.Project.WindowsService.CelendarAdapter
{
    public sealed class NamedCalendar : HolidayCalendar
    {
        public string CalendarName { get; set; }
    }
}
