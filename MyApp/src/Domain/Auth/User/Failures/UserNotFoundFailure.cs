namespace MyApp.Domain.Auth.User.Failures;
public class UserNotFoundFailure : DomainFailure
{
    public const string Key = nameof(UserEntity.Id);
    public const string Message = "User not found.";
    private UserNotFoundFailure() : base() { }
    public static DomainException Exception()
        => new UserNotFoundFailure()
            .AddError(Key, Message)
            .ToException();
}
