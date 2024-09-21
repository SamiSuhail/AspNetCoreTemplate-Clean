using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.Auth.PasswordManagement.ResetPassword;

public class ResetPasswordTransformer : IRequestTransformer<ResetPasswordRequest>
{
    public ResetPasswordRequest Transform(ResetPasswordRequest request)
        => new(request.Code.RemoveWhitespace(),
                request.Password);
}
