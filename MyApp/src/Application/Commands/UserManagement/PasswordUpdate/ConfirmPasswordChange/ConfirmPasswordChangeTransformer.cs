using MyApp.Application.Interfaces.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Application.Interfaces.Utilities;
using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public class ConfirmPasswordChangeTransformer : IRequestTransformer<ConfirmPasswordChangeRequest>
{
    public ConfirmPasswordChangeRequest Transform(ConfirmPasswordChangeRequest request)
        => request with
        {
            Code = request.Code.RemoveWhitespace(),
        };
}
