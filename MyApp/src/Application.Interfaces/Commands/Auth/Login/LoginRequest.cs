﻿namespace MyApp.Application.Interfaces.Commands.Auth.Login;

public record LoginRequest(
    string Username,
    string Password,
    string[] Scopes);
