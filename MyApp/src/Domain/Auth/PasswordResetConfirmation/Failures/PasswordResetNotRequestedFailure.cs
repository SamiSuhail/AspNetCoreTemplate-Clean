namespace MyApp.Domain.Auth.PasswordResetConfirmation.Failures;

public class PasswordResetNotRequestedFailure : DomainFailure
{
    public const string Key = "User";
    public const string Message = "The user has not requested password changing.";

    private PasswordResetNotRequestedFailure() { }

    public static DomainException Exception()
        => new PasswordResetNotRequestedFailure()
        .AddError(Key, Message)
        .ToException();
}
