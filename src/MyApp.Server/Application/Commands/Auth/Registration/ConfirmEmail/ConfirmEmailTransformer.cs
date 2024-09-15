using MyApp.Server.Application.Utilities;
using MyApp.Server.Shared;

namespace MyApp.Server.Application.Commands.Auth.Registration.ConfirmEmail;

public class ConfirmEmailTransformer : IRequestTransformer<ConfirmEmailRequest>
{
    public ConfirmEmailRequest Transform(ConfirmEmailRequest request)
        => new(request.Code.RemoveWhitespace());
}
