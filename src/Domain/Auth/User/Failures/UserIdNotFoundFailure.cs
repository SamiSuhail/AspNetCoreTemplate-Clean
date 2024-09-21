namespace MyApp.Domain.Auth.User.Failures;
public class UserIdNotFoundFailure : DomainFailure
{
    public const string Key = nameof(UserEntity.Id);
    public const string Message = "User not found.";
    private UserIdNotFoundFailure() : base() { }
    public static DomainException Exception()
        => new UserIdNotFoundFailure()
            .AddError(Key, Message)
            .ToException();
}
