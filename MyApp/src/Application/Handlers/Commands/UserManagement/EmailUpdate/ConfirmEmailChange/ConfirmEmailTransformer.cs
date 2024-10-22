using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Application.Startup.Behaviours;
using MyApp.Utilities.Strings;

namespace MyApp.Application.Handlers.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public class ConfirmEmailTransformer : IRequestTransformer<ConfirmEmailChangeRequest>
{
    public ConfirmEmailChangeRequest Transform(ConfirmEmailChangeRequest request)
        => request with
        {
            OldEmailCode = request.OldEmailCode.RemoveWhitespace(),
            NewEmailCode = request.NewEmailCode.RemoveWhitespace(),
        };
}
