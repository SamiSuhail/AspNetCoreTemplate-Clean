using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public class ConfirmEmailTransformer : IRequestTransformer<ConfirmEmailChangeRequest>
{
    public ConfirmEmailChangeRequest Transform(ConfirmEmailChangeRequest request)
        => request with
        {
            OldEmailCode = request.OldEmailCode.Trim(),
            NewEmailCode = request.NewEmailCode.Trim(),
        };
}
