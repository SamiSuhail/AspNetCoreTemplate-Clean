using MyApp.Server.Infrastructure.Utilities;

namespace MyApp.Server.Infrastructure.Email;

public class EmailSettings : BaseSettings<EmailSettings>
{
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string SMTPHost { get; set; }
    public required int SMTPPort { get; set; }
}
