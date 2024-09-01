using MyApp.Server.Utilities;

namespace MyApp.Server.Modules.Commands.Auth.ConfirmEmail;

public class ConfirmEmailTransformer : IRequestTransformer<ConfirmEmailRequest>
{
    public ConfirmEmailRequest Transform(ConfirmEmailRequest request)
        => new(request.Code.RemoveWhitespace());
}
