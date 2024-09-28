namespace MyApp.Domain.Auth.PasswordResetConfirmation.Failures;

public class PasswordResetConfirmationCodeInvalidFailure : DomainFailure
{
    public const string Key = nameof(PasswordResetConfirmationEntity.Code);
    public const string Message = "The confirmation code is invalid or has expired.";
    private PasswordResetConfirmationCodeInvalidFailure() : base() { }
    public static DomainException Exception()
        => new PasswordResetConfirmationCodeInvalidFailure()
            .AddError(Key, Message)
            .ToException();
}
