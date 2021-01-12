How to add new Job:

1)Add new class, this class have to inherit from BaseJob
2)Add job configuration to appsettings.json 
	- find section QuartzJobsConfig and add new object to Jobs array (JobName have to has same name as job class)
3)If new job has some external dependencies, those dependencies have to be register to IOCDI container (Program.cs ConfigureServices)