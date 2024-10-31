namespace MyApp.Domain.Auth.PasswordResetConfirmation.Failures;

public class ForgotPasswordUserNotFoundFailure : DomainFailure
{
    private ForgotPasswordUserNotFoundFailure() : base() { }

    public const string Key = "User";
    public const string Message = "No user was found with these email and username.";

    public static DomainException Exception()
        => new ForgotPasswordUserNotFoundFailure()
            .AddError(Key, Message)
            .ToException();
}
