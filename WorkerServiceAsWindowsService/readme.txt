How to add new Job:

1)Add new class, this class have to inherit from BaseJob
2)Add job configuration to appsettings.json 
	- find section QuartzJobsConfig and add new object to Jobs array (JobName have to has same name as job class)
3)If new job has some external dependencies, those dependencies have to be register to IOCDI container (Program.cs ConfigureServices)


Calendar guide:

1) Only one calendar is here implemented, it is name is CZNotWorkingDaysCalendar and contain all weekends and CZ holiday days.   
2) If you want implement other calendars with excluded days (in those days specific job not be fired) use QuartzCalendarManager class and call method AddNewCalendar for this purpouse! 


