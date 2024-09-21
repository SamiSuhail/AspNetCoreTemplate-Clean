namespace MyApp.Domain.UserManagement.PasswordChangeConfirmation.Failures;

public class PasswordChangeNotRequestedFailure : DomainFailure
{
    public const string Key = "User";
    public const string Message = "The user has not requested password changing.";

    private PasswordChangeNotRequestedFailure() { }

    public static DomainException Exception()
        => new PasswordChangeNotRequestedFailure()
        .AddError(Key, Message)
        .ToException();
}
