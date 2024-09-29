using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.Auth.Registration.ResendConfirmation;

public class ResendConfirmationTransformer : IRequestTransformer<ResendConfirmationRequest>
{
    public ResendConfirmationRequest Transform(ResendConfirmationRequest request)
        => new(request.Email.Trim());
}
