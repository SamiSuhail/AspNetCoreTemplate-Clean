using MyApp.Server.Domain.Auth.User;

namespace MyApp.Server.Domain.Auth.EmailConfirmation.Failures;

public static class ResendConfirmationInvalidConstants
{
    public const string Message = "No user with this email is awaiting confirmation.";
}

public class ResendConfirmationInvalidFailure : DomainFailure
{
    private ResendConfirmationInvalidFailure() : base() { }
    public static DomainException Exception()
        => new ResendConfirmationInvalidFailure()
            .AddError(nameof(UserEntity.Email), ResendConfirmationInvalidConstants.Message)
            .ToException();
}
