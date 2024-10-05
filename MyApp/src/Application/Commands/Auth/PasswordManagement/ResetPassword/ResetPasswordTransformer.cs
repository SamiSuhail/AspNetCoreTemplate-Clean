using MyApp.Application.Interfaces.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Application.Interfaces.Utilities;
using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.Auth.PasswordManagement.ResetPassword;

public class ResetPasswordTransformer : IRequestTransformer<ResetPasswordRequest>
{
    public ResetPasswordRequest Transform(ResetPasswordRequest request)
        => request with
        {
            Code = request.Code.RemoveWhitespace(),
        };
}
