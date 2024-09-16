using MyApp.Server.Domain.Shared;

namespace MyApp.Server.Domain.Auth.EmailChangeConfirmation;

public class EmailChangeConfirmationEntity : BaseConfirmationEntity
{
    public static EmailChangeConfirmationEntity Create(int userId)
        => Create<EmailChangeConfirmationEntity>(userId);
}
