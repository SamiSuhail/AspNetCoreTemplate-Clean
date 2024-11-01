using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Application.Startup.Behaviours;
using MyApp.Utilities.Strings;

namespace MyApp.Application.Modules.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public class ConfirmPasswordChangeTransformer : IRequestTransformer<ConfirmPasswordChangeRequest>
{
    public ConfirmPasswordChangeRequest Transform(ConfirmPasswordChangeRequest request)
        => request with
        {
            Code = request.Code.RemoveWhitespace(),
        };
}
