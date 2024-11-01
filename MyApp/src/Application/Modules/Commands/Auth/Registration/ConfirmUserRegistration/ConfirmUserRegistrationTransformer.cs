using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Application.Startup.Behaviours;
using MyApp.Utilities.Strings;

namespace MyApp.Application.Modules.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationTransformer : IRequestTransformer<ConfirmUserRegistrationRequest>
{
    public ConfirmUserRegistrationRequest Transform(ConfirmUserRegistrationRequest request)
        => request with
        {
            Code = request.Code.RemoveWhitespace(),
        };
}
