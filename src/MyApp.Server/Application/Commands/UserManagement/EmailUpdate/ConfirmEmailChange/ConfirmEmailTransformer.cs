using MyApp.Server.Application.Utilities;

namespace MyApp.Server.Application.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public class ConfirmEmailTransformer : IRequestTransformer<ConfirmEmailChangeRequest>
{
    public ConfirmEmailChangeRequest Transform(ConfirmEmailChangeRequest request)
        => request with
        {
            OldEmailCode = request.OldEmailCode.Trim(),
            NewEmailCode = request.NewEmailCode.Trim(),
        };
}
