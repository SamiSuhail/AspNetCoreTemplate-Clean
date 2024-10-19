namespace MyApp.Domain.Infra.Instance.Failures;

public class InstanceNotFoundFailure : DomainFailure
{
    public const string Key = "Instance";
    public const string Message = "Instance not found.";

    private InstanceNotFoundFailure() { }

    public static DomainException Exception()
        => new InstanceNotFoundFailure()
            .AddError(Key, Message)
            .ToException();
}
