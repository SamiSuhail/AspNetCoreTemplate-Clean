namespace MyApp.Domain.UserManagement.EmailChangeConfirmation.Failures;

public class EmailChangeNotRequestedFailure : DomainFailure
{
    public const string Key = "User";
    public const string Message = "The user has not requested email changing.";

    private EmailChangeNotRequestedFailure() { }

    public static DomainException Exception()
        => new EmailChangeNotRequestedFailure()
        .AddError(Key, Message)
        .ToException();
}
