using MyApp.Server.Utilities;

namespace MyApp.Server.Modules.Commands.Auth.Registration.ResendConfirmation;

public class ResendConfirmationTransformer : IRequestTransformer<ResendConfirmationRequest>
{
    public ResendConfirmationRequest Transform(ResendConfirmationRequest request)
        => new(request.Email.Trim());
}
