namespace MyApp.Server.Domain.Auth.UserConfirmation.Failures;

public class UserConfirmationCodeInvalidFailure : DomainFailure
{
    public const string Key = nameof(UserConfirmationEntity.Code);
    public const string Message = "The confirmation code is invalid or has expired.";
    private UserConfirmationCodeInvalidFailure() : base() { }
    public static DomainException Exception()
        => new UserConfirmationCodeInvalidFailure()
            .AddError(Key, Message)
            .ToException();
}
