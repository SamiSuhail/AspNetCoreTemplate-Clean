using MyApp.Server.Utilities;

namespace MyApp.Server.Modules.Commands.Auth.Registration.Register;

public class RegisterTransformer : IRequestTransformer<RegisterRequest>
{
    public RegisterRequest Transform(RegisterRequest request)
        => new(request.Email.Trim(),
            request.Username,
            request.Password);
}
