namespace MyApp.Server.Utilities;

public interface IRequestTransformer<TRequest>
{
    TRequest Transform(TRequest request);
}
