namespace MyApp.Domain.Auth.User.Failures;
public class LoginInvalidFailure : DomainFailure
{
    public const string Key = "LoginInvalid";
    public const string Message = "Invalid username or password.";
    private LoginInvalidFailure() : base() { }
    public static DomainException Exception()
        => new LoginInvalidFailure()
            .AddError(Key, Message)
            .ToException();
}
