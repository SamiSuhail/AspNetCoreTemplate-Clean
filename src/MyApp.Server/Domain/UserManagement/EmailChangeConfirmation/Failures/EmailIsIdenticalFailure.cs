using MyApp.Server.Domain.Auth.User;

namespace MyApp.Server.Domain.UserManagement.EmailChangeConfirmation.Failures;

public class EmailIsIdenticalFailure : DomainFailure
{
    public const string Key = nameof(UserEntity.Email);
    public const string Message = "The provided email is identical to the current one.";

    private EmailIsIdenticalFailure() { }

    public static DomainException Exception()
        => new EmailIsIdenticalFailure()
        .AddError(Key, Message)
        .ToException();
}
