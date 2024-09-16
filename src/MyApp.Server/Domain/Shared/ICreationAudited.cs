namespace MyApp.Server.Domain.Shared;

public interface ICreationAudited
{
    public DateTime CreatedAt { get; }
}
