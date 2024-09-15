using MyApp.Server.Application.Utilities;
using MyApp.Server.Shared;

namespace MyApp.Server.Application.Commands.Auth.PasswordManagement.ResetPassword;

public class ResetPasswordTransformer : IRequestTransformer<ResetPasswordRequest>
{
    public ResetPasswordRequest Transform(ResetPasswordRequest request)
        => new(request.Code.RemoveWhitespace(),
                request.Password);
}
