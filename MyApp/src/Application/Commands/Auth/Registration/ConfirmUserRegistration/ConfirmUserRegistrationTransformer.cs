using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationTransformer : IRequestTransformer<ConfirmUserRegistrationRequest>
{
    public ConfirmUserRegistrationRequest Transform(ConfirmUserRegistrationRequest request)
        => request with
        {
            Code = request.Code.RemoveWhitespace(),
        };
}
