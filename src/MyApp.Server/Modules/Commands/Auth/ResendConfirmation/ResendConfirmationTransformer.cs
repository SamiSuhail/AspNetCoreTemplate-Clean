using MyApp.Server.Utilities;

namespace MyApp.Server.Modules.Commands.Auth.ResendConfirmation;

public class ResendConfirmationTransformer : IRequestTransformer<ResendConfirmationRequest>
{
    public ResendConfirmationRequest Transform(ResendConfirmationRequest request)
        => new(request.Email.Trim());
}
