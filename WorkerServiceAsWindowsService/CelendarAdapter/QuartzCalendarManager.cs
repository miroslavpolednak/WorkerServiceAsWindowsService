using Csob.Calendar;
using Quartz;
using Quartz.Impl.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService.CelendarAdapter
{
    public class QuartzCalendarManager : IQuartzCalendarManager
    {
        private Dictionary<string, NamedCalendar> Calendars;
        private readonly IDaysGenerator _daysGenerator;

        public QuartzCalendarManager(IDaysGenerator daysGenerator)
        {
            Calendars = new Dictionary<string, NamedCalendar>();
            _daysGenerator = daysGenerator;
        }

        public void AddDefaultCalendars()
        {
            CreateCzNOtworkingDaysCalendar();

        }

        private void CreateCzNOtworkingDaysCalendar()
        {
            DateTime currentTime = DateTime.Today;
            List<int> years = Enumerable.Range(currentTime.Year, currentTime.Year + 10).ToList();
            List<DateTime> excludedDays = new List<DateTime>();
            years.ForEach(r => excludedDays.AddRange(_daysGenerator.GetAllCZNotWorkingDay(r)));
            string description = @"This calendar contains all weekend and holidays days from current year to current year + 10";
            AddNewCalendar("CZNotWorkingDaysCalendar", excludedDays, description);
        }

        public NamedCalendar GetSpecificCalendar(string calendarName)
        {
            lock (this)
            {
                return Calendars[calendarName];
            }

        }

        public void AddNewCalendar(string calendarName, IEnumerable<DateTime> excludedDays, string calendarDescription = null)
        {
            if (string.IsNullOrWhiteSpace(calendarName) || !excludedDays.Any())
            {
                throw new NotSupportedException("calendarName and excludedDays are required");
            }

            NamedCalendar namedCalendar = new NamedCalendar();
            namedCalendar.CalendarName = calendarName;
            if (calendarDescription != null)
            {
                namedCalendar.Description = calendarDescription;
            }
            foreach (DateTime day in excludedDays)
            {
                namedCalendar.AddExcludedDate(day);
            }

            lock (this)
            {
                Calendars.Add(calendarName, namedCalendar);
            }
        }
    }
}
