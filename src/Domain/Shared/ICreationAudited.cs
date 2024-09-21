namespace MyApp.Domain.Shared;

public interface ICreationAudited
{
    public DateTime CreatedAt { get; }
}
