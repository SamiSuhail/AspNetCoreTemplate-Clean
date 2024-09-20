using MyApp.Server.Application.Utilities;
using MyApp.Server.Shared;

namespace MyApp.Server.Application.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public class ConfirmPasswordChangeTransformer : IRequestTransformer<ConfirmPasswordChangeRequest>
{
    public ConfirmPasswordChangeRequest Transform(ConfirmPasswordChangeRequest request)
        => request with
        {
            Code = request.Code.RemoveWhitespace(),
        };
}
