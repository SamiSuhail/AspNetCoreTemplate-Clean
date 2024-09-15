using MyApp.Server.Application.Utilities;

namespace MyApp.Server.Application.Commands.Auth.Registration.ResendConfirmation;

public class ResendConfirmationTransformer : IRequestTransformer<ResendConfirmationRequest>
{
    public ResendConfirmationRequest Transform(ResendConfirmationRequest request)
        => new(request.Email.Trim());
}
