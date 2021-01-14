using System;
using System.Collections.Generic;

namespace Csob.Calendar
{
    public interface IDaysGenerator
    {
        HashSet<DateTime> CalculateCZHoliday(int year);
        DateTime EasterDay(int EYear);
        HashSet<DateTime> GetAllCZNotWorkingDay(int year);
        IEnumerable<DateTime> GetDaysBetween(DateTime start, DateTime end);
        IEnumerable<DateTime> GetAllweekendsDays(int year);
    }
}