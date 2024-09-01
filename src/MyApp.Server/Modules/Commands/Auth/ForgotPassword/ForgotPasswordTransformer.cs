﻿using MyApp.Server.Utilities;

namespace MyApp.Server.Modules.Commands.Auth.ForgotPassword;

public class ForgotPasswordTransformer : IRequestTransformer<ForgotPasswordRequest>
{
    public ForgotPasswordRequest Transform(ForgotPasswordRequest request)
        => new(request.Email.Trim(), request.Username);
}
