namespace MyApp.Domain.Infra.Instance.Failures;

public class InstanceNameTakenFailure : DomainFailure
{
    public const string Key = nameof(InstanceEntity.Name);
    public const string Message = "Instance name is taken.";

    private InstanceNameTakenFailure() { }

    public static DomainException Exception()
        => new InstanceNameTakenFailure()
            .AddError(Key, Message)
            .ToException();
}
