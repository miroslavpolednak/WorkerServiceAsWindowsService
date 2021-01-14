using System;
using System.Collections.Generic;

namespace Csob.Project.WindowsService.CelendarAdapter
{
    public interface IQuartzCalendarManager
    {
        void AddDefaultCalendars();
        void AddNewCalendar(string calendarName, IEnumerable<DateTime> excludedDays, string calendarDescription = null);
        NamedCalendar GetSpecificCalendar(string calendarName);
    }
}