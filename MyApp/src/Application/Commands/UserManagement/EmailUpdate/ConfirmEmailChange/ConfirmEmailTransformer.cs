using MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Application.Utilities;
using MyApp.Utilities.Strings;

namespace MyApp.Application.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public class ConfirmEmailTransformer : IRequestTransformer<ConfirmEmailChangeRequest>
{
    public ConfirmEmailChangeRequest Transform(ConfirmEmailChangeRequest request)
        => request with
        {
            OldEmailCode = request.OldEmailCode.RemoveWhitespace(),
            NewEmailCode = request.NewEmailCode.RemoveWhitespace(),
        };
}
