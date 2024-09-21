namespace MyApp.Domain.Auth.User.Failures;

public class UserConflictFailure : DomainFailure
{
    public const string UsernameTakenKey = nameof(UserEntity.Username);
    public const string UsernameTakenMessage = "Username is already taken.";
    public const string EmailTakenKey = nameof(UserEntity.Email);
    public const string EmailTakenMessage = "Email is already taken.";

    public static UserConflictFailure Create(bool usernameTaken, bool emailTaken)
    {
        var failure = new UserConflictFailure();

        if (usernameTaken)
            failure.AddError(UsernameTakenKey, UsernameTakenMessage);

        if (emailTaken)
            failure.AddError(EmailTakenKey, EmailTakenMessage);

        return failure;
    }

    public static DomainException EmailException()
        => Create(usernameTaken: false, emailTaken: true)
            .ToException();
}
