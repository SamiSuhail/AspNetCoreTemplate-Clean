namespace MyApp.Domain.UserManagement.PasswordChangeConfirmation.Failures;

public class PasswordIsIdenticalFailure : DomainFailure
{
    public const string Key = "NewPassword";
    public const string Message = "New password is identical to old one.";
    private PasswordIsIdenticalFailure() { }

    public static DomainException Exception()
        => new PasswordIsIdenticalFailure()
            .AddError(Key, Message)
            .ToException();
}
