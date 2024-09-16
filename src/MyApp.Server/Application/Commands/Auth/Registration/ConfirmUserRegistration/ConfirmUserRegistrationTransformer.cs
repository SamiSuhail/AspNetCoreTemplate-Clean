using MyApp.Server.Application.Utilities;
using MyApp.Server.Shared;

namespace MyApp.Server.Application.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationTransformer : IRequestTransformer<ConfirmUserRegistrationRequest>
{
    public ConfirmUserRegistrationRequest Transform(ConfirmUserRegistrationRequest request)
        => new(request.Code.RemoveWhitespace());
}
