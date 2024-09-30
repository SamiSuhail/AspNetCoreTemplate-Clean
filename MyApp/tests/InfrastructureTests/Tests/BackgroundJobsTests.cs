using Microsoft.Extensions.Configuration;
using MyApp.Application.BackgroundJobs.CleanupConfirmations;
using MyApp.Application.Utilities;
using MyApp.Infrastructure.BackgroundJobs;
using Quartz;

namespace MyApp.InfrastructureTests.Tests;

public class BackgroundJobsTests(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task ScheduledBackgroundJob_RunsOnSchedule()
    {
        var sectionName = BackgroundJobsSettings.SectionName;

        var configuration = new ConfigurationBuilder()
            .AddConfiguration(Configuration)
            .AddInMemoryCollection([
                new($"{sectionName}:{nameof(BackgroundJobsSettings.Enabled)}", "true"),
                new($"{sectionName}:{CounterBackgroundJob.Name}:{nameof(BackgroundJobSettings.Enabled)}", "true"),
                ]);

        await RunJobs(CounterBackgroundJob.Start, waitTimeSeconds: 10);

        CounterBackgroundJob.Counter.Should().BeGreaterThan(0);
    }

    class CounterBackgroundJob : BaseBackgroundJob<CounterBackgroundJob>, IJob
    {
        public static int Counter { get; private set; } = 0;
        public Task Execute(IJobExecutionContext context)
        {
            Counter++;
            return Task.CompletedTask;
        }

        public static void Start(IServiceCollectionQuartzConfigurator options)
        {
            var randomName = Guid.NewGuid().ToString();
            var jobKey = JobKey.Create(randomName);

            options.AddJob<CounterBackgroundJob>(jb =>
                jb.WithIdentity(jobKey)
                    .DisallowConcurrentExecution()
                    .Build());

            options.AddTrigger(tb =>
                tb.WithIdentity(randomName)
                    .ForJob(jobKey)
                    .WithCronSchedule("* * * * * ?"));
        }
    }

    private async Task RunJobs(Action<IServiceCollectionQuartzConfigurator> setupJobs, int waitTimeSeconds = 3)
    {
        using var sp = new ServiceCollection()
            .AddCustomBackgroundJobs(Configuration, (c, _) =>
            {
                setupJobs(c);
            })
            .BuildServiceProvider();
        using var scope = sp.CreateScope();
        await Task.Delay(TimeSpan.FromSeconds(waitTimeSeconds));
    }
}
