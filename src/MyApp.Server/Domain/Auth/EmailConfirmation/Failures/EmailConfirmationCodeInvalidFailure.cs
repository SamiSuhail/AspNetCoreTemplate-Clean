namespace MyApp.Server.Domain.Auth.EmailConfirmation.Failures;

public class EmailConfirmationCodeInvalidFailure : DomainFailure
{
    public const string Message = "The confirmation code is invalid or has expired.";
    private EmailConfirmationCodeInvalidFailure() : base() { }
    public static DomainException Exception()
        => new EmailConfirmationCodeInvalidFailure()
            .AddError(nameof(EmailConfirmationEntity.Code), Message)
            .ToException();
}
