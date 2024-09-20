namespace MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation.Failures;

public class PasswordChangeInvalidCodeFailure : DomainFailure
{
    public const string Key = "Code";
    public const string Message = "The confirmation code is invalid.";

    private PasswordChangeInvalidCodeFailure() { }

    public static DomainException Exception()
        => new PasswordChangeInvalidCodeFailure()
        .AddError(Key, Message)
        .ToException();
}
