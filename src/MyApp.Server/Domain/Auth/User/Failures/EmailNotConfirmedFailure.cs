namespace MyApp.Server.Domain.Auth.User.Failures;

public class EmailNotConfirmedFailure : DomainFailure
{
    public const string Key = nameof(UserEntity.Email);
    public const string Message = "User has not confirmed email address.";
    private EmailNotConfirmedFailure() : base() { }
    public static DomainException Exception()
        => new EmailNotConfirmedFailure()
            .AddError(Key, Message)
            .ToException();
}
