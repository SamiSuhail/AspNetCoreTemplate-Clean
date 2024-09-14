namespace MyApp.Server.Domain.Auth.User.Failures;

public class RegisterConflictFailure : DomainFailure
{
    public const string UsernameTakenKey = nameof(UserEntity.Username);
    public const string UsernameTakenMessage = "Username is already taken.";
    public const string EmailTakenKey = nameof(UserEntity.Email);
    public const string EmailTakenMessage = "Email is already taken.";
    public static RegisterConflictFailure Create(bool usernameTaken, bool emailTaken)
    {
        var failure = new RegisterConflictFailure();

        if (usernameTaken)
            failure.AddError(UsernameTakenKey, UsernameTakenMessage);

        if (emailTaken)
            failure.AddError(EmailTakenKey, EmailTakenMessage);

        return failure;
    }
}
