namespace MyApp.Server.Infrastructure.Messaging;

public class MessagingSettings
{
    public const string SectionName = "Messaging";

    public required string Host { get; set; }
    public required int Port { get; set; }
    public required int ManagementPort { get; set; }
    public required string User { get; set; }
    public required string Pass { get; set; }
    public required bool UseSsl { get; set; }

    public static MessagingSettings Get(IConfiguration configuration)
        => configuration.GetSection(SectionName)
            .Get<MessagingSettings>()!;
}
