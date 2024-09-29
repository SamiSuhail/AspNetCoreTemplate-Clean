namespace MyApp.Application.Utilities;

public interface IRequestTransformer<TRequest>
{
    TRequest Transform(TRequest request);
}
