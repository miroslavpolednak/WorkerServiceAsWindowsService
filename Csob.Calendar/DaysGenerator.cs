using System;
using System.Collections.Generic;
using System.Linq;

namespace Csob.Calendar
{
    public class DaysGenerator : IDaysGenerator
    {
        /// <summary>
        /// Calculate all CZ not working day (weekend + holiday) 
        /// </summary>
        public HashSet<DateTime> GetAllCZNotWorkingDay(int year)
        {
            HashSet<DateTime> holidaysWithNoWorkingDays = new HashSet<DateTime>();
            if (year <= 1900)
            {
                throw new NotSupportedException("Year have to be greater than 1900");
            }

            IEnumerable<DateTime> weekends = GetAllweekendsDays(year);

            holidaysWithNoWorkingDays.UnionWith(weekends);
            holidaysWithNoWorkingDays.UnionWith(CalculateCZHoliday(year));
            return holidaysWithNoWorkingDays;
        }

        public IEnumerable<DateTime> GetAllweekendsDays(int year)
        {
            DateTime yearStart = new DateTime(year, 1, 1);
            IEnumerable<DateTime> weekends = GetDaysBetween(yearStart, yearStart.AddDays(365))
                                         .Where(d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday);
            return weekends;
        }

        public IEnumerable<DateTime> GetDaysBetween(DateTime start, DateTime end)
        {
            for (DateTime i = start; i < end; i = i.AddDays(1))
            {
                yield return i;
            }
        }

        /// <summary>
        /// Calculate CZ holiday 
        /// </summary>
        public HashSet<DateTime> CalculateCZHoliday(int year)
        {
            if (year <= 1900)
            {
                throw new NotSupportedException("Year have to be greater than 1900");
            }

            //1. 1.Den obnovy samostatného českého státu
            //1. 1.Nový rok
            //2. 4.Velký pátek (pohyblivý)
            //5. 4.Velikonoční pondělí (pohyblivý)
            //1. 5.Svátek práce
            //8. 5.Den vítězství
            //5. 7.Den slovanských věrozvěstů Cyrila a Metoděje
            //6. 7.Den upálení mistra Jana Husa 
            //28. 9.Den české státnosti
            //28. 10.Den vzniku samostatného československého státu
            //17. 11.Den boje za svobodu a demokracii
            //24. 12.Štědrý den -
            //25. 12. 1.svátek vánoční
            //26. 12. 2.svátek vánoční
            HashSet<DateTime> holidays = new HashSet<DateTime>
            {
                //Add fixed holidays
                new DateTime(year, 1, 1),
                new DateTime(year, 5, 1),
                new DateTime(year, 5, 8),
                new DateTime(year, 7, 5),
                new DateTime(year, 7, 6),
                new DateTime(year, 9, 28),
                new DateTime(year, 10, 28),
                new DateTime(year, 11, 17),
                new DateTime(year, 12, 24),
                new DateTime(year, 12, 25),
                new DateTime(year, 12, 26)
            };

            //Add not fixed holidays
            DateTime easterdate = EasterDay(year);
            holidays.Add(easterdate);
            //Big friday
            holidays.Add(easterdate.AddDays(-2));
            //Easter monday
            holidays.Add(easterdate.AddDays(1));
            return holidays;
        }


        public DateTime EasterDay(int EYear)
        {
            int G, C, H, i, j, L, EMonth, EDay;
            G = EYear % 19;
            C = EYear / 100;
            H = ((C - (C / 4) - ((8 * C + 13) / 25) + (19 * G) + 15) % 30);
            i = H - ((H / 28) * (1 - (H / 28) * (29 / (H + 1)) * ((21 - G) / 11)));
            j = ((EYear + (EYear / 4) + i + 2 - C + (C / 4)) % 7);
            L = i - j;
            EMonth = 3 + ((L + 40) / 44);
            EDay = L + 28 - (31 * (EMonth / 4));
            System.DateTime EDateTime = new System.DateTime(EYear, EMonth, EDay, 0, 0, 0, 0);
            return EDateTime;
        }

    }
}
