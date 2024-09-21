using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationTransformer : IRequestTransformer<ConfirmUserRegistrationRequest>
{
    public ConfirmUserRegistrationRequest Transform(ConfirmUserRegistrationRequest request)
        => new(request.Code.RemoveWhitespace());
}
