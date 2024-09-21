using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.UserManagement.EmailUpdate.ChangeEmail;

public class ChageEmailTransformer : IRequestTransformer<ChangeEmailRequest>
{
    public ChangeEmailRequest Transform(ChangeEmailRequest request)
        => request with
        {
            Email = request.Email.Trim(),
        };
}
