using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.Auth.PasswordManagement.ForgotPassword;

public class ForgotPasswordTransformer : IRequestTransformer<ForgotPasswordRequest>
{
    public ForgotPasswordRequest Transform(ForgotPasswordRequest request)
        => new(request.Email.Trim(), request.Username);
}
