namespace MyApp.Server.Domain.UserManagement.EmailChangeConfirmation.Failures;

public class EmailChangeInvalidCodesFailure : DomainFailure
{
    public const string Key = "Code";
    public const string Message = "One or both of the provided codes are invalid.";

    private EmailChangeInvalidCodesFailure() { }

    public static DomainException Exception()
        => new EmailChangeInvalidCodesFailure()
        .AddError(Key, Message)
        .ToException();
}
