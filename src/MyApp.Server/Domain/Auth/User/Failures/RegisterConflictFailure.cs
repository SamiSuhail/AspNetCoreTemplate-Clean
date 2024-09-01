namespace MyApp.Server.Domain.Auth.User.Failures;

public class RegisterConflictFailure : DomainFailure
{
    public const string UsernameTakenMessage = "Username is already taken.";
    public const string EmailTakenMessage = "Email is already taken.";
    public static RegisterConflictFailure Create(bool usernameTaken, bool emailTaken)
    {
        var failure = new RegisterConflictFailure();

        if (usernameTaken)
            failure.AddError(nameof(UserEntity.Username), UsernameTakenMessage);

        if (emailTaken)
            failure.AddError(nameof(UserEntity.Email), EmailTakenMessage);

        return failure;
    }
}
