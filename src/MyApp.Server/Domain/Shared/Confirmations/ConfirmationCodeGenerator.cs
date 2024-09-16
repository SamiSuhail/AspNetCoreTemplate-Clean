namespace MyApp.Server.Domain.Shared.Confirmations;

public static class ConfirmationCodeGenerator
{
    public static string GenerateCode()
    {
        var codeMaxValueExclusive = (int)Math.Pow(10, BaseConfirmationConstants.CodeLength);
        return new Random().Next(codeMaxValueExclusive).ToString($"D{BaseConfirmationConstants.CodeLength}");
    }
}
