namespace MyApp.Domain.UserManagement.EmailChangeConfirmation.Failures;

public class EmailChangeExpiredFailure : DomainFailure
{
    public const string Key = "User";
    public const string Message = "The request for changing email has expired.";

    private EmailChangeExpiredFailure() { }

    public static DomainException Exception()
        => new EmailChangeExpiredFailure()
        .AddError(Key, Message)
        .ToException();
}
