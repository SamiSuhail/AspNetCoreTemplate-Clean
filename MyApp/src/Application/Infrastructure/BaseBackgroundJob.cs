using Quartz;

namespace MyApp.Application.Infrastructure;
public abstract class BaseBackgroundJob<TActual> : IJob
{
    public static string Name { get; } = typeof(TActual).Name;

    public abstract Task Execute(IJobExecutionContext context);
}
