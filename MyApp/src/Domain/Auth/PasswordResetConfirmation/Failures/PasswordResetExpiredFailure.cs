namespace MyApp.Domain.Auth.PasswordResetConfirmation.Failures;

public class PasswordResetExpiredFailure : DomainFailure
{
    public const string Key = "User";
    public const string Message = "The request for changing password has expired.";

    private PasswordResetExpiredFailure() { }

    public static DomainException Exception()
        => new PasswordResetExpiredFailure()
        .AddError(Key, Message)
        .ToException();
}
