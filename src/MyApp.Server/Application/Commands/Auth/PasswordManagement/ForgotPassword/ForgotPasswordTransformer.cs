using MyApp.Server.Application.Utilities;

namespace MyApp.Server.Application.Commands.Auth.PasswordManagement.ForgotPassword;

public class ForgotPasswordTransformer : IRequestTransformer<ForgotPasswordRequest>
{
    public ForgotPasswordRequest Transform(ForgotPasswordRequest request)
        => new(request.Email.Trim(), request.Username);
}
