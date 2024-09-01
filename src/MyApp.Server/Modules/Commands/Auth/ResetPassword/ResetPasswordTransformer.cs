using MyApp.Server.Utilities;

namespace MyApp.Server.Modules.Commands.Auth.ResetPassword;

public class ResetPasswordTransformer : IRequestTransformer<ResetPasswordRequest>
{
    public ResetPasswordRequest Transform(ResetPasswordRequest request)
        => new(request.Code.RemoveWhitespace(),
                request.Password);
}
