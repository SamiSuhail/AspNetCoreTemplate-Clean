namespace MyApp.Server.Domain.Auth.PasswordResetConfirmation.Failures;

public class PasswordResetConfirmationCodeInvalidFailure : DomainFailure
{
    public const string Message = "The confirmation code is invalid or has expired.";
    private PasswordResetConfirmationCodeInvalidFailure() : base() { }
    public static DomainException Exception()
        => new PasswordResetConfirmationCodeInvalidFailure()
            .AddError(nameof(PasswordResetConfirmationEntity.Code), Message)
            .ToException();
}
