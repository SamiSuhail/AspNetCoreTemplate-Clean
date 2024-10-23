namespace MyApp.Domain.Auth.PasswordResetConfirmation.Failures;

public class PasswordResetInvalidCodeFailure : DomainFailure
{
    public const string Key = "Code";
    public const string Message = "The confirmation code is invalid.";

    private PasswordResetInvalidCodeFailure() { }

    public static DomainException Exception()
        => new PasswordResetInvalidCodeFailure()
        .AddError(Key, Message)
        .ToException();
}
