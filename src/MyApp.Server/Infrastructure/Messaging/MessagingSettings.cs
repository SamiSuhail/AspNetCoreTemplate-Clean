using MyApp.Server.Infrastructure.Utilities;

namespace MyApp.Server.Infrastructure.Messaging;

public class MessagingSettings : BaseSettings<MessagingSettings>
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required int ManagementPort { get; set; }
    public required string User { get; set; }
    public required string Pass { get; set; }
    public required bool UseSsl { get; set; }
}
