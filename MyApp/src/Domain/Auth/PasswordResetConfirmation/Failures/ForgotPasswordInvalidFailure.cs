namespace MyApp.Domain.Auth.PasswordResetConfirmation.Failures;

public class ForgotPasswordInvalidFailure : DomainFailure
{
    private ForgotPasswordInvalidFailure() : base() { }

    public const string Key = "User";
    public const string Message = "No user was found with these email and username.";

    public static DomainException Exception()
        => new ForgotPasswordInvalidFailure()
            .AddError(Key, Message)
            .ToException();
}
