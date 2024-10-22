using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Application.Startup.Behaviours;
using MyApp.Utilities.Strings;

namespace MyApp.Application.Handlers.Commands.Auth.PasswordManagement.ResetPassword;

public class ResetPasswordTransformer : IRequestTransformer<ResetPasswordRequest>
{
    public ResetPasswordRequest Transform(ResetPasswordRequest request)
        => request with
        {
            Code = request.Code.RemoveWhitespace(),
        };
}
