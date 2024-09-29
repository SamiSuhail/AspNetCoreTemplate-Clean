namespace MyApp.Domain.Auth.User.Failures;

public class UserRegistrationNotConfirmedFailure : DomainFailure
{
    public const string Key = "User";
    public const string Message = "User has not confirmed registration.";
    private UserRegistrationNotConfirmedFailure() : base() { }
    public static DomainException Exception()
        => new UserRegistrationNotConfirmedFailure()
            .AddError(Key, Message)
            .ToException();
}
