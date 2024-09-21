using MyApp.Domain.Auth.User;

namespace MyApp.Domain.Auth.UserConfirmation.Failures;

public class ResendConfirmationInvalidFailure : DomainFailure
{
    public const string Key = nameof(UserEntity.Email);
    public const string Message = "No user with this email is awaiting confirmation.";

    private ResendConfirmationInvalidFailure() : base() { }
    public static DomainException Exception()
        => new ResendConfirmationInvalidFailure()
            .AddError(Key, Message)
            .ToException();
}
