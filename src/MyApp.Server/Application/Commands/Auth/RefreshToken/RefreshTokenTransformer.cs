using MyApp.Server.Application.Utilities;

namespace MyApp.Server.Application.Commands.Auth.RefreshToken;

public class RefreshTokenTransformer : IRequestTransformer<RefreshTokenRequest>
{
    public RefreshTokenRequest Transform(RefreshTokenRequest request)
        => request with
        {
            RefreshToken = request.RefreshToken.Trim(),
        };
}
