using MyApp.Application.Utilities;

namespace MyApp.Application.Commands.Auth.RefreshToken;

public class RefreshTokenTransformer : IRequestTransformer<RefreshTokenRequest>
{
    public RefreshTokenRequest Transform(RefreshTokenRequest request)
        => request with
        {
            RefreshToken = request.RefreshToken.Trim(),
        };
}
