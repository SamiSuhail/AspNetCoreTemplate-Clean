using MyApp.Application.Interfaces.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Application.Startup.Behaviours;
using MyApp.Utilities.Strings;

namespace MyApp.Application.Handlers.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationTransformer : IRequestTransformer<ConfirmUserRegistrationRequest>
{
    public ConfirmUserRegistrationRequest Transform(ConfirmUserRegistrationRequest request)
        => request with
        {
            Code = request.Code.RemoveWhitespace(),
        };
}
