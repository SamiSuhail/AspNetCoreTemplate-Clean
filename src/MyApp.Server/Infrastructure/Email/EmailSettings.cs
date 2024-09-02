namespace MyApp.Server.Infrastructure.Email;

public class EmailSettings
{
    public const string SectionName = "Email";

    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string SMTPHost { get; set; }
    public required int SMTPPort { get; set; }

    public static EmailSettings Get(IConfiguration configuration)
        => configuration.GetSection(SectionName)
            .Get<EmailSettings>()!;
}
