namespace MyApp.Server.Domain.Auth.User.Failures;

public class UserSessionCouldNotBeRefreshedFailure : DomainFailure
{
    public const string Key = "User";
    public const string Message = "The user's session could not be refreshed.";
    private UserSessionCouldNotBeRefreshedFailure() : base() { }
    public static DomainException Exception()
        => new UserSessionCouldNotBeRefreshedFailure()
            .AddError(Key, Message)
            .ToException();
}
